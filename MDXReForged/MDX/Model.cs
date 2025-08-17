using MDXReForged.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MDXReForged.MDX
{
    public class Model
    {
        private const string ExpectedMagic = "MDLX";
        private readonly static uint[] KnownVersions = [800, 900, 1000, 1100, 1200];
        public string BaseFile { get; }
        public string Magic { get; private set; }
        public IReadOnlyList<BaseChunk> Chunks { get; private set; }
        public IReadOnlyList<GenObject> Hierarchy { get; private set; }

        public string Name => Get<MODL>().Name;
        public string AnimationFile => Get<MODL>().AnimationFile;
        public CExtent Bounds => Get<MODL>().Bounds;
        public uint BlendTime => Get<MODL>().BlendTime;
        public uint Version { get; private set; } = 0;
        public Action<string> UnknownChunkLogger { get; set; } = s => Console.WriteLine(s);

        public Model(string file)
        {
            BaseFile = file;
            using var fs = new FileInfo(file).OpenRead();
            using var br = new BinaryReader(fs);
            Initialize(br);
        }

        public Model(Stream stream)
        {
            BaseFile = (stream as FileStream)?.Name;
            using var br = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
            Initialize(br);
        }

        public Model(BinaryReader reader)
        {
            BaseFile = (reader.BaseStream as FileStream)?.Name;
            Initialize(reader);
        }

        private void Initialize(BinaryReader br)
        {
            var chunks = new List<BaseChunk>();
            Magic = br.ReadString(4);

            if (Magic != ExpectedMagic)
                throw new InvalidDataException($"Invalid magic '{Magic}', expected '{ExpectedMagic}'.");

            while (!br.AtEnd())
                ReadChunk(br, chunks);

            Chunks = chunks;
            PopulateHierarchy();
        }

        private void ReadChunk(BinaryReader br, List<BaseChunk> chunks)
        {
            // no point parsing last 8 bytes as it's either padding or an empty chunk
            if (br.BaseStream.Length - br.BaseStream.Position <= 8)
                return;

            long start = br.BaseStream.Position;
            uint tag = br.ReadUInt32Tag();
            if (ChunkFactories.TryGetValue(tag, out var factory))
            {
                br.BaseStream.Position = start;
                var chunk = factory(br, Version);

                if (Version == 0 && chunk is VERS vers)
                {
                    Version = vers.FormatVersion;
                    if (!KnownVersions.Contains(Version))
                        Console.WriteLine($"[WARNING] Unknown format version {Version}");
                }    

                long expectedEnd = start + 8 + chunk.Size;

                if (br.BaseStream.Position != expectedEnd)
                {
                    UnknownChunkLogger?.Invoke($"[WARNING] Chunk {chunk.Type} at {start} size mismatch: declared {chunk.Size}, read {br.BaseStream.Position - start - 8}");
                    br.BaseStream.Position = expectedEnd;
                }

                chunks.Add(chunk);
            }
            else
            {
                uint size = br.ReadUInt32();
                UnknownChunkLogger?.Invoke($"[WARNING] Unknown type {Extensions.TagToString(tag)} at {start}, skipping {size} bytes");
                br.BaseStream.Seek(size, SeekOrigin.Current);
            }
        }

        private void PopulateHierarchy()
        {
            var hierachy = new List<GenObject>();

            // inherits MDLGENOBJECT
            foreach (var chunk in Chunks)
                if (chunk is IReadOnlyList<GenObject> collection)
                    hierachy.AddRange(collection);

            hierachy.Sort((x, y) => x.ObjectId.CompareTo(y.ObjectId));
            Hierarchy = hierachy;
        }

        private static readonly Dictionary<uint, Func<BinaryReader, uint, BaseChunk>> ChunkFactories = new Dictionary<uint, Func<BinaryReader, uint, BaseChunk>>
        {
            { Tags.VERS, (br, version) => new VERS(br, version) },
            { Tags.MODL, (br, version) => new MODL(br, version) },
            { Tags.SEQS, (br, version) => new SEQS(br, version) },
            { Tags.MTLS, (br, version) => new MTLS(br, version) },
            { Tags.TEXS, (br, version) => new TEXS(br, version) },
            { Tags.GEOS, (br, version) => new GEOS(br, version) },
            { Tags.BONE, (br, version) => new BONE(br, version) },
            { Tags.HELP, (br, version) => new HELP(br, version) },
            { Tags.ATCH, (br, version) => new ATCH(br, version) },
            { Tags.PIVT, (br, version) => new PIVT(br, version) },
            { Tags.CAMS, (br, version) => new CAMS(br, version) },
            { Tags.EVTS, (br, version) => new EVTS(br, version) },
            { Tags.CLID, (br, version) => new CLID(br, version) },
            { Tags.GLBS, (br, version) => new GLBS(br, version) },
            { Tags.GEOA, (br, version) => new GEOA(br, version) },
            { Tags.PRE2, (br, version) => new PRE2(br, version) },
            { Tags.RIBB, (br, version) => new RIBB(br, version) },
            { Tags.LITE, (br, version) => new LITE(br, version) },
            { Tags.TXAN, (br, version) => new TXAN(br, version) },
            { Tags.BPOS, (br, version) => new BPOS(br, version) },
            { Tags.FAFX, (br, version) => new FAFX(br, version) },
            { Tags.PREM, (br, version) => new PREM(br, version) },
            { Tags.CORN, (br, version) => new CORN(br, version) },
        };
        public bool Has<T>() where T : BaseChunk => Chunks.Any(x => x is T);

        /// <summary>
        /// Retrieves the first chunk of the specified type from the model.
        /// </summary>
        /// <typeparam name="T">The chunk type to retrieve. Must inherit from <see cref="BaseChunk"/>.</typeparam>
        public T Get<T>() where T : BaseChunk => (T)Chunks.FirstOrDefault(x => x is T);

        public VERS GetVersionChunk() => Get<VERS>();

        public MODL GetModelChunk() => Get<MODL>();

        /// <summary>
        /// Retrieves a read-only list of items from a chunk of the specified type.
        /// </summary>
        /// /// <returns>
        /// A read-only list of items contained in the specified chunk.  
        /// If the chunk is not present in the model, returns an empty list instead of null.
        /// </returns>
        public IReadOnlyList<TItem> GetItems<TChunk, TItem>()
            where TChunk : EnumerableBaseChunk<TItem>
        {
            var chunk = Get<TChunk>();
            return chunk != null ? chunk : Array.Empty<TItem>();
        }

        /// <summary>
        /// Returns a list of animation sequences from the SEQS chunk.
        /// </summary>
        public IReadOnlyList<Sequence> GetSequences() => GetItems<SEQS, Sequence>();

        /// <summary>
        /// Returns a list of materials from the MTLS chunk.
        /// </summary>
        public IReadOnlyList<Material> GetMaterials() => GetItems<MTLS, Material>();

        /// <summary>
        /// Returns a list of textures from the TEXS chunk.
        /// </summary>
        public IReadOnlyList<Texture> GetTextures() => GetItems<TEXS, Texture>();

        /// <summary>
        /// Returns a list of geosets from the GEOS chunk.
        /// </summary>
        public IReadOnlyList<Geoset> GetGeosets() => GetItems<GEOS, Geoset>();

        /// <summary>
        /// Returns a list of bones from the BONE chunk.
        /// </summary>
        public IReadOnlyList<Bone> GetBones() => GetItems<BONE, Bone>();

        /// <summary>
        /// Returns a list of helpers from the HELP chunk.
        /// </summary>
        public IReadOnlyList<Helper> GetHelpers() => GetItems<HELP, Helper>();

        /// <summary>
        /// Returns a list of attachments from the ATCH chunk.
        /// </summary>
        public IReadOnlyList<Attachment> GetAttachments() => GetItems<ATCH, Attachment>();

        /// <summary>
        /// Returns a list of pivot points from the PIVT chunk.
        /// </summary>
        public IReadOnlyList<CVector3> GetPivots() => GetItems<PIVT, CVector3>();

        /// <summary>
        /// Returns a list of cameras from the CAMS chunk.
        /// </summary>
        public IReadOnlyList<Camera> GetCameras() => GetItems<CAMS, Camera>();

        /// <summary>
        /// Returns a list of events from the EVTS chunk.
        /// </summary>
        public IReadOnlyList<Event> GetEvents() => GetItems<EVTS, Event>();

        /// <summary>
        /// Returns a list of collision shapes from the CLID chunk.
        /// </summary>
        public IReadOnlyList<CollisionShape> GetCollisionShapes() => GetItems<CLID, CollisionShape>();

        /// <summary>
        /// Returns a list of global sequence durations from the GLBS chunk.
        /// </summary>
        public IReadOnlyList<int> GetGlobalSequences() => GetItems<GLBS, int>();

        /// <summary>
        /// Returns a list of geoset animations from the GEOA chunk.
        /// </summary>
        public IReadOnlyList<GeosetAnimation> GetGeosetAnimations() => GetItems<GEOA, GeosetAnimation>();

        /// <summary>
        /// Returns a list of ParticleEmitter2 objects from the PRE2 chunk.
        /// </summary>
        public IReadOnlyList<ParticleEmitter2> GetParticleEmitters2() => GetItems<PRE2, ParticleEmitter2>();

        /// <summary>
        /// Returns a list of ribbon emitters from the RIBB chunk.
        /// </summary>
        public IReadOnlyList<RibbonEmitter> GetRibbonEmitters() => GetItems<RIBB, RibbonEmitter>();

        /// <summary>
        /// Returns a list of lights from the LITE chunk.
        /// </summary>
        public IReadOnlyList<Light> GetLights() => GetItems<LITE, Light>();

        /// <summary>
        /// Returns a list of texture animations from the TXAN chunk.
        /// </summary>
        public IReadOnlyList<TextureAnimation> GetTextureAnims() => GetItems<TXAN, TextureAnimation>();

        /// <summary>
        /// Returns a list of bind pose matrices from the BPOS chunk.
        /// </summary>
        public IReadOnlyList<C34Matrix> GetBindPoses() => GetItems<BPOS, C34Matrix>();

        /// <summary>
        /// Returns a list of FaceFX nodes from the FAFX chunk.
        /// </summary>
        public IReadOnlyList<FaceFX> GetFaceFX() => GetItems<FAFX, FaceFX>();

        /// <summary>
        /// Returns a list of legacy particle emitters from the PREM chunk.
        /// </summary>
        public IReadOnlyList<ParticleEmitter> GetParticleEmitters() => GetItems<PREM, ParticleEmitter>();

        /// <summary>
        /// Returns a list of PopcornFX particle emitters from the CORN chunk.
        /// </summary>
        public IReadOnlyList<ParticleEmitterPopcorn> GetCornEmitters() => GetItems<CORN, ParticleEmitterPopcorn>();

        /// <summary>
        /// Returns a multiline string containing detailed information about the model.
        /// </summary>
        public string GetDetailedInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine(ToString());

            void Append<TChunk, TItem>(string label) where TChunk : EnumerableBaseChunk<TItem>
            {
                sb.AppendLine($"\t{label}: {GetItems<TChunk, TItem>().Count}");
            }

            Append<SEQS, Sequence>("Sequences");
            Append<MTLS, Material>("Materials");
            Append<TEXS, Texture>("Textures");
            Append<GEOS, Geoset>("Geosets");
            Append<GEOA, GeosetAnimation>("GeosetAnimations");
            Append<BONE, Bone>("Bones");
            Append<HELP, Helper>("Helpers");
            Append<ATCH, Attachment>("Attachments");
            Append<PIVT, CVector3>("Pivots");
            Append<CAMS, Camera>("Cameras");
            Append<EVTS, Event>("Events");
            Append<CLID, CollisionShape>("CollisionShapes");
            Append<GLBS, int>("GlobalSequences");
            Append<PRE2, ParticleEmitter2>("ParticleEmitters2");
            Append<PREM, ParticleEmitter>("ParticleEmitters");
            Append<CORN, ParticleEmitterPopcorn>("PopcornEmitters");
            Append<RIBB, RibbonEmitter>("RibbonEmitters");
            Append<LITE, Light>("Lights");
            Append<TXAN, TextureAnimation>("TextureAnimations");
            Append<BPOS, C34Matrix>("BindPoses");
            Append<FAFX, FaceFX>("FaceFXNodes");

            return sb.ToString();
        }

        public override string ToString() => $"Model \"{Name}\" (Version: MDX{Version})";
    }
}
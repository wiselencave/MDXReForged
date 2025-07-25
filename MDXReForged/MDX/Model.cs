using MDXReForged.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class Model
    {
        public readonly string BaseFile;
        public readonly string Magic;
        public IReadOnlyList<BaseChunk> Chunks;
        public IReadOnlyList<GenObject> Hierachy;

        public string Name => Get<MODL>().Name;
        public string AnimationFile => Get<MODL>().AnimationFile;
        public CExtent Bounds => Get<MODL>().Bounds;
        public uint BlendTime => Get<MODL>().BlendTime;
        public uint Version { get; private set; } = 0;

        public Model(string file)
        {
            BaseFile = file;

            var chunks = new List<BaseChunk>();
            using (var fs = new FileInfo(file).OpenRead())
            using (var br = new BinaryReader(fs))
            {
                Magic = br.ReadString(4);
                while (!br.AtEnd())
                    ReadChunk(br, chunks);
            }

            Chunks = chunks;
            PopulateHierachy();
        }

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


        private void ReadChunk(BinaryReader br, List<BaseChunk> chunks)
        {
            // no point parsing last 8 bytes as it's either padding or an empty chunk
            if (br.BaseStream.Length - br.BaseStream.Position <= 8)
                return;

            uint tag = br.ReadUInt32Tag();
            if (ChunkFactories.TryGetValue(tag, out var factory))
            {
                br.BaseStream.Position -= 4;
                var chunk = factory(br, Version);

                if (Version == 0 && chunk is VERS vers)
                    Version = vers.FormatVersion;

                chunks.Add(chunk);
            }
            else
            {
                throw new Exception($"Unknown type {Extensions.TagToString(tag)}");
            }
        }

        private void PopulateHierachy()
        {
            var hierachy = new List<GenObject>();

            // inherits MDLGENOBJECT
            foreach (var chunk in Chunks)
                if (chunk is IReadOnlyList<GenObject> collection)
                    hierachy.AddRange(collection);

            hierachy.Sort((x, y) => x.ObjectId.CompareTo(y.ObjectId));
            Hierachy = hierachy;
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
    }
}
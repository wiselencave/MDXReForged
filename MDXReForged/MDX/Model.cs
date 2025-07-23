using MDXReForged.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// <returns>
        /// The first instance of the chunk type <typeparamref name="T"/> if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method is intended for optional chunks. If the chunk is required for further logic,
        /// ensure null-checking or use <see cref="GetItems{TChunk, TItem}"/> if applicable.
        /// </remarks>
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

            string tag = br.ReadString(4);
            if (TypeLookup.TryGetValue(tag, out Type value))
            {
                br.BaseStream.Position -= 4;
                var chunk = (BaseChunk)Activator.CreateInstance(value, br, Version);

                if (Version == 0 && chunk is VERS vers)
                    Version = vers.FormatVersion;

                chunks.Add(chunk);
            }
            else
            {
                throw new Exception($"Unknown type {tag}");
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

        private static readonly Dictionary<string, Type> TypeLookup = new Dictionary<string, Type>
        {
            { "VERS", typeof(VERS) },
            { "MODL", typeof(MODL) },
            { "SEQS", typeof(SEQS) },
            { "MTLS", typeof(MTLS) },
            { "TEXS", typeof(TEXS) },
            { "GEOS", typeof(GEOS) },
            { "BONE", typeof(BONE) },
            { "HELP", typeof(HELP) },
            { "ATCH", typeof(ATCH) },
            { "PIVT", typeof(PIVT) },
            { "CAMS", typeof(CAMS) },
            { "EVTS", typeof(EVTS) },
            { "CLID", typeof(CLID) },
            { "GLBS", typeof(GLBS) },
            { "GEOA", typeof(GEOA) },
            { "PRE2", typeof(PRE2) },
            { "RIBB", typeof(RIBB) },
            { "LITE", typeof(LITE) },
            { "TXAN", typeof(TXAN) },
            { "BPOS", typeof(BPOS) },
            { "FAFX", typeof(FAFX) },
            { "PREM", typeof(PREM) },
            { "CORN", typeof(CORN) },
        };
    }
}
using MDXReForged.Structs;
using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class PRE2 : EnumerableBaseChunk<ParticleEmitter2>
    {
        public PRE2(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new ParticleEmitter2(br));
        }
    }

    public class ParticleEmitter2 : GenObject
    {
        public float Speed { get; }
        public float Variation { get; }
        public float Latitude { get; }
        public float Gravity { get; }
        public float Lifespan { get; }
        public float EmissionRate { get; }
        public float Length { get; }
        public float Width { get; }
        public PARTICLE_BLEND_MODE BlendMode { get; }
        public int Rows { get; }
        public int Cols { get; }
        public PARTICLE_TYPE ParticleType { get; }
        public float TailLength { get; }
        public float MiddleTime { get; }
        public CVector3 StartColor { get; }
        public CVector3 MiddleColor { get; }
        public CVector3 EndColor { get; }
        public float StartAlpha { get; }
        public float MiddleAlpha { get; }
        public float EndAlpha { get; }
        public float StartScale { get; }
        public float MiddleScale { get; }
        public float EndScale { get; }
        public uint LifespanUVAnimStart { get; }
        public uint LifespanUVAnimEnd { get; }
        public uint LifespanUVAnimRepeat { get; }
        public uint DecayUVAnimStart { get; }
        public uint DecayUVAnimEnd { get; }
        public uint DecayUVAnimRepeat { get; }
        public uint TailUVAnimStart { get; }
        public uint TailUVAnimEnd { get; }
        public uint TailUVAnimRepeat { get; }
        public uint TailDecayUVAnimStart { get; }
        public uint TailDecayUVAnimEnd { get; }
        public uint TailDecayUVAnimRepeat { get; }
        public uint Squirts { get; }
        public uint TextureId { get; }
        public int PriorityPlane { get; }
        public uint ReplaceableId { get; }

        public Track<float> SpeedKeys { get; } = Track<float>.Empty;
        public Track<float> VariationKeys { get; } = Track<float>.Empty;
        public Track<float> LatitudeKeys { get; } = Track<float>.Empty;
        public Track<float> GravityKeys { get; } = Track<float>.Empty;
        public Track<float> EmissionRateKeys { get; } = Track<float>.Empty;
        public Track<float> WidthKeys { get; } = Track<float>.Empty;
        public Track<float> LengthKeys { get; } = Track<float>.Empty;
        public Track<float> VisibilityKeys { get; } = Track<float>.Empty;

        public ParticleEmitter2(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            Speed = br.ReadSingle();
            Variation = br.ReadSingle();
            Latitude = br.ReadSingle();
            Gravity = br.ReadSingle();
            Lifespan = br.ReadSingle();
            EmissionRate = br.ReadSingle();
            Length = br.ReadSingle();
            Width = br.ReadSingle();
            BlendMode = (PARTICLE_BLEND_MODE)br.ReadUInt32();
            Rows = br.ReadInt32();
            Cols = br.ReadInt32();
            ParticleType = (PARTICLE_TYPE)br.ReadInt32();
            TailLength = br.ReadSingle();
            MiddleTime = br.ReadSingle();

            StartColor = new CVector3(br);
            MiddleColor = new CVector3(br);
            EndColor = new CVector3(br);
            StartAlpha = br.ReadByte() / 255f;
            MiddleAlpha = br.ReadByte() / 255f;
            EndAlpha = br.ReadByte() / 255f;
            StartScale = br.ReadSingle();
            MiddleScale = br.ReadSingle();
            EndScale = br.ReadSingle();

            LifespanUVAnimStart = br.ReadUInt32();
            LifespanUVAnimEnd = br.ReadUInt32();
            LifespanUVAnimRepeat = br.ReadUInt32();

            DecayUVAnimStart = br.ReadUInt32();
            DecayUVAnimEnd = br.ReadUInt32();
            DecayUVAnimRepeat = br.ReadUInt32();

            TailUVAnimStart = br.ReadUInt32();
            TailUVAnimEnd = br.ReadUInt32();
            TailUVAnimRepeat = br.ReadUInt32();

            TailDecayUVAnimStart = br.ReadUInt32();
            TailDecayUVAnimEnd = br.ReadUInt32();
            TailDecayUVAnimRepeat = br.ReadUInt32();

            TextureId = br.ReadUInt32();
            Squirts = br.ReadUInt32();  // 1 for footsteps and impact spell effects
            PriorityPlane = br.ReadInt32();
            ReplaceableId = br.ReadUInt32();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KP2S: SpeedKeys = new Track<float>(tagname, br); break;
                    case KP2R: VariationKeys = new Track<float>(tagname, br); break;
                    case KP2G: GravityKeys = new Track<float>(tagname, br); break;
                    case KP2W: WidthKeys = new Track<float>(tagname, br); break;
                    case KP2N: LengthKeys = new Track<float>(tagname, br); break;
                    case KP2V: VisibilityKeys = new Track<float>(tagname, br); break;
                    case KP2E: EmissionRateKeys = new Track<float>(tagname, br); break;
                    case KP2L: LatitudeKeys = new Track<float>(tagname, br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
using MDXReForged.Structs;
using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class CORN : EnumerableBaseChunk<ParticleEmitterPopcorn>
    {
        public CORN(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new ParticleEmitterPopcorn(br));
        }
    }

    public class ParticleEmitterPopcorn : GenObject
    {
        public float LifeSpan { get;  }
        public float EmissionRate { get;  }
        public float Speed { get; }
        public CVector3 Color { get; }
        public float Alpha { get; }
        public uint ReplaceableId { get; }
        public string FilePath { get; }
        public string AnimVisibilityGuide { get; }

        public Track<float> SpeedKeys { get; } = Track<float>.Empty;
        public Track<float> VisibilityKeys { get; } = Track<float>.Empty;
        public Track<float> LifespanKeys { get; } = Track<float>.Empty;
        public Track<float> AlphaKeys { get; } = Track<float>.Empty;
        public Track<float> EmissionRateKeys { get; } = Track<float>.Empty;
        public Track<CVector3> ColorKeys { get; } = Track<CVector3>.Empty;

        public ParticleEmitterPopcorn(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GenObjectFlags)br.ReadUInt32();

            LoadTracks(br);

            LifeSpan = br.ReadSingle();
            EmissionRate = br.ReadSingle();
            Speed = br.ReadSingle();
            Color = new CVector3(br);
            Alpha = br.ReadSingle();
            ReplaceableId = br.ReadUInt32(); 
            FilePath = br.ReadCString(Constants.SizeFileName);
            AnimVisibilityGuide = br.ReadCString(Constants.SizeFileName);

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KPPS: SpeedKeys = new Track<float>(tagname, br); break;
                    case KPPV: VisibilityKeys = new Track<float>(tagname, br); break;
                    case KPPL: LifespanKeys = new Track<float>(tagname, br); break;
                    case KPPA: AlphaKeys = new Track<float>(tagname, br); break;
                    case KPPE: EmissionRateKeys = new Track<float>(tagname, br); break;
                    case KPPC: ColorKeys = new Track<CVector3>(tagname, br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
        public override string ToString() =>
            $"PopcornFX \"{Name}\" (ObjectId: {ObjectId}, Parent: {ParentId}) — FilePath: \"{FilePath}\"";
    }
}
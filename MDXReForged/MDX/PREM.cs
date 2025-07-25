using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class PREM : EnumerableBaseChunk<ParticleEmitter>
    {
        public PREM(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new ParticleEmitter(br));
        }
    }

    public class ParticleEmitter : GenObject
    {
        public float EmissionRate { get; }
        public float Gravity { get; }
        public float Longitude { get; }
        public float Latitude { get; }
        public string Path { get; }
        public float Lifespan { get; }
        public float Speed { get; }

        public Track<float> EmissionKeys { get; } = Track<float>.Empty;
        public Track<float> GravityKeys { get; } = Track<float>.Empty;
        public Track<float> LongitudeKeys { get; } = Track<float>.Empty;
        public Track<float> LatitudeKeys { get; } = Track<float>.Empty;
        public Track<float> LifespanKeys { get; } = Track<float>.Empty;
        public Track<float> SpeedKeys { get; } = Track<float>.Empty;
        public Track<float> VisibilityKeys { get; } = Track<float>.Empty;

        public ParticleEmitter(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            EmissionRate = br.ReadSingle();
            Gravity = br.ReadSingle();
            Longitude = br.ReadSingle();
            Latitude = br.ReadSingle();
            Path = br.ReadCString(Constants.SizeFileName);
            Lifespan = br.ReadSingle();
            Speed = br.ReadSingle();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KPEE: EmissionKeys = new Track<float>(br); break;
                    case KPEG: GravityKeys = new Track<float>(br); break;
                    case KPLN: LongitudeKeys = new Track<float>(br); break;
                    case KPLT: LatitudeKeys = new Track<float>(br); break;
                    case KPEL: LifespanKeys = new Track<float>(br); break;
                    case KPES: SpeedKeys = new Track<float>(br); break;
                    case KPEV: VisibilityKeys = new Track<float>(br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
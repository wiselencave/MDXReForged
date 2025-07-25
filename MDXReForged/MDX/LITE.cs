using MDXReForged.Structs;
using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class LITE : EnumerableBaseChunk<Light>
    {
        public LITE(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Light(br, version));
        }
    }

    public class Light : GenObject
    {
        public LIGHT_TYPE Type { get; }
        public float AttenuationStart { get; }
        public float AttenuationEnd { get;  }
        public CVector3 Color { get; }
        public float Intensity { get; }
        public CVector3 AmbientColor { get; }
        public float AmbientIntensity {  get; }
        public float? ShadowIntensity { get; } = null;

        public Track<float> AttenStartKeys { get; } = Track<float>.Empty;
        public Track<float> AttenEndKeys { get; } = Track<float>.Empty;
        public Track<CVector3> ColorKeys { get; } = Track<CVector3>.Empty;
        public Track<float> IntensityKeys { get; } = Track<float>.Empty;
        public Track<CVector3> AmbColorKeys { get; } = Track<CVector3>.Empty;
        public Track<float> AmbIntensityKeys { get; } = Track<float>.Empty;
        public Track<float> VisibilityKeys { get; } = Track<float>.Empty;

        public Light(BinaryReader br, uint version)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            Type = (LIGHT_TYPE)br.ReadInt32();
            AttenuationStart = br.ReadSingle();
            AttenuationEnd = br.ReadSingle();
            Color = new CVector3(br);
            Intensity = br.ReadSingle();
            AmbientColor = new CVector3(br);
            AmbientIntensity = br.ReadSingle();

            if (version >= 1200)
                ShadowIntensity = br.ReadSingle();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KLAI: IntensityKeys = new Track<float>(tagname, br); break;
                    case KLBI: AmbIntensityKeys = new Track<float>(tagname, br); break;
                    case KLAV: VisibilityKeys = new Track<float>(tagname, br); break;
                    case KLAC: ColorKeys = new Track<CVector3>(tagname, br); break;
                    case KLBC: AmbColorKeys = new Track<CVector3>(tagname, br); break;
                    case KLAS: AttenStartKeys = new Track<float>(tagname, br); break;
                    case KLAE: AttenEndKeys = new Track<float>(tagname, br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
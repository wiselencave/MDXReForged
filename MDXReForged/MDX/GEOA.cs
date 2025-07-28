using MDXReForged.Structs;
using System.IO;
using System.Linq;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class GEOA : EnumerableBaseChunk<GeosetAnimation>
    {
        public GEOA(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new GeosetAnimation(br));
        }
    }

    public class GeosetAnimation
    {
        public float Alpha { get; }
        public bool HasColorKeys { get; }
        public CVector3 Color { get; }
        public int GeosetId { get; }

        public Track<float> AlphaKeys { get; } = Track<float>.Empty;
        public Track<CVector3> ColorKeys { get; } = Track<CVector3>.Empty;

        public GeosetAnimation(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadInt32();

            Alpha = br.ReadSingle();
            HasColorKeys = br.ReadInt32() == 1;
            Color = new CVector3(br);
            GeosetId = br.ReadInt32();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KGAO: AlphaKeys = new Track<float>(tagname, br); break;
                    case KGAC: ColorKeys = new Track<CVector3>(tagname, br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }

        public override string ToString() => $"GeosetAnimation — GeosetId: {GeosetId}, Color: {Color}, Alpha: {Alpha},\r\n\t Color Track: {ColorKeys}, Alpha Track: {AlphaKeys}";
    }
}
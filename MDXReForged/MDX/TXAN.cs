using MDXReForged.Structs;
using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class TXAN : EnumerableBaseChunk<TextureAnimation>
    {
        public TXAN(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new TextureAnimation(br));
        }
    }

    public class TextureAnimation
    {
        public Track<CVector3> TranslationKeys { get; } = Track<CVector3>.Empty;
        public Track<CVector4> RotationKeys { get; } = Track<CVector4>.Empty;
        public Track<CVector3> ScaleKeys { get; } = Track<CVector3>.Empty; 

        public TextureAnimation(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KTAT: TranslationKeys = new Track<CVector3>(tagname, br); break;
                    case KTAR: RotationKeys = new Track<CVector4>(tagname, br); break;
                    case KTAS: ScaleKeys = new Track<CVector3>(tagname, br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
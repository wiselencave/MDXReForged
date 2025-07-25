using MDXReForged.Structs;
using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class CAMS : EnumerableBaseChunk<Camera>
    {
        public CAMS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Camera(br));
        }
    }

    public class Camera
    {
        public string Name { get; }
        public CVector3 Pivot { get; }
        public float FieldOfView { get; }
        public float FarClip { get; }
        public float NearClip { get; }
        public CVector3 TargetPosition { get; }

        public Track<CVector3> TranslationKeys { get; private set; } = Track<CVector3>.Empty;
        public Track<CVector3> TargetTranslationKeys { get; private set; } = Track<CVector3>.Empty;
        public Track<float> RotationKeys { get; private set; } = Track<float>.Empty;

        public Camera(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            Name = br.ReadCString(Constants.SizeName);
            Pivot = new CVector3(br);
            FieldOfView = br.ReadSingle();
            FarClip = br.ReadSingle();
            NearClip = br.ReadSingle();
            TargetPosition = new CVector3(br);

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tag = br.ReadUInt32Tag();
                switch (tag)
                {
                    case KCTR: TranslationKeys = new Track<CVector3>(br); break;
                    case KTTR: TargetTranslationKeys = new Track<CVector3>(br); break;
                    case KCRL: RotationKeys = new Track<float>(br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
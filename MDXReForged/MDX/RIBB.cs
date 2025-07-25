using MDXReForged.Structs;
using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class RIBB : EnumerableBaseChunk<RibbonEmitter>
    {
        public RIBB(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new RibbonEmitter(br));
        }
    }

    public class RibbonEmitter : GenObject
    {
        public uint TotalSize { get; }
        public float HeightAbove { get; }
        public float HeightBelow { get; }
        public float Alpha { get; }
        public CVector3 Color { get; }
        public uint EdgesPerSecond { get; }
        public float Lifetime { get; }
        public uint TextureSlot { get; }
        public uint TextureRows { get; }
        public uint TextureColumns { get; }
        public uint MaterialId { get; }
        public float Gravity { get; }

        public Track<float> HeightAboveKeys { get; } = Track<float>.Empty;
        public Track<float> HeightBelowKeys { get; } = Track<float>.Empty;
        public Track<float> AlphaKeys { get; } = Track<float>.Empty;
        public Track<float> VisibilityKeys { get; } = Track<float>.Empty;
        public Track<CVector3> ColorKeys { get; } = Track<CVector3>.Empty;
        public Track<uint> TextureKeys { get; } = Track<uint>.Empty;

        public RibbonEmitter(BinaryReader br)
        {
            long end = br.BaseStream.Position + (TotalSize = br.ReadUInt32());

            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            HeightAbove = br.ReadSingle();
            HeightBelow = br.ReadSingle();
            Alpha = br.ReadSingle();
            Color = new CVector3(br);
            Lifetime = br.ReadSingle();
            TextureSlot = br.ReadUInt32();
            EdgesPerSecond = br.ReadUInt32();
            TextureRows = br.ReadUInt32();
            TextureColumns = br.ReadUInt32();
            MaterialId = br.ReadUInt32();
            Gravity = br.ReadSingle();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KRHA: HeightAboveKeys = new Track<float>(tagname, br); break;
                    case KRHB: HeightBelowKeys = new Track<float>(tagname, br); break;
                    case KRAL: AlphaKeys = new Track<float>(tagname, br); break;
                    case KRVS: VisibilityKeys = new Track<float>(tagname, br); break;
                    case KRCO: ColorKeys = new Track<CVector3>(tagname, br); break;
                    case KRTX: TextureKeys = new Track<uint>(tagname, br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
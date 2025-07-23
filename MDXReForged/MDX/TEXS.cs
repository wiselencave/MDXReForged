using System.IO;

namespace MDXReForged.MDX
{
    public class TEXS : EnumerableBaseChunk<Texture>
    {
        public TEXS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Texture(br));
        }
    }

    public class Texture
    {
        public uint ReplaceableId { get; }
        public string Image { get; }
        public TEXFLAGS Flags { get; }

        public Texture(BinaryReader br)
        {
            ReplaceableId = br.ReadUInt32();
            Image = br.ReadCString(Constants.SizeFileName);
            Flags = (TEXFLAGS)br.ReadUInt32();
        }
    }
}
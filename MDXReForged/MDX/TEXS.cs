using System;
using System.IO;
using System.Linq;

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
        public TextureFlags Flags { get; }

        public Texture(BinaryReader br)
        {
            ReplaceableId = br.ReadUInt32();
            Image = br.ReadCString(Constants.SizeFileName);
            Flags = (TextureFlags)br.ReadUInt32();
        }

        private string FormatFlags()
        {
            return string.Join(" | ",
                ((TextureFlags[])Enum.GetValues(typeof(TextureFlags)))
                    .Where(flag => Flags.HasFlag(flag) && flag != 0));
        }
        public override string ToString()
        {
            string info = ReplaceableId != 0
                ? $"ReplaceableId: {ReplaceableId}"
                : $"Image: \"{Image}\"";

            string flags = Flags != 0 ? $" — Flags: {FormatFlags()}" : "";

            return $"Texture ({info}){flags}";
        }
    }
}
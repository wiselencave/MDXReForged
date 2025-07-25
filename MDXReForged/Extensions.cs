using System;
using System.IO;
using System.Text;

namespace MDXReForged
{
    public static class Extensions
    {
        public static string ReadCString(this BinaryReader br, int length) => Encoding.UTF8.GetString(br.ReadBytes(length)).TrimEnd('\0');

        public static string ReadString(this BinaryReader br, int length) => Encoding.UTF8.GetString(br.ReadBytes(length));

        public static uint ReadUInt32Tag(this BinaryReader br) => br.ReadUInt32();

        public static string TagToString(uint tag)
        {
            return new string(
            [
                (char)(tag & 0xFF),
                (char)((tag >> 8) & 0xFF),
                (char)((tag >> 16) & 0xFF),
                (char)((tag >> 24) & 0xFF)
            ]);
        }

        public static void AssertTag(this BinaryReader br, uint tag)
        {
            uint read = br.ReadUInt32Tag();
            if (read != tag)
                throw new Exception($"Expected '{TagToString(tag)}' at {br.BaseStream.Position - Constants.SizeTag} got '{TagToString(read)}'.");
        }

        public static bool HasTag(this BinaryReader br, uint tag)
        {
            uint read = br.ReadUInt32Tag();
            bool match = read == tag;
            if (!match)
                br.BaseStream.Position -= Constants.SizeTag;
            return match;
        }

        public static bool AtEnd(this BinaryReader br) => br.BaseStream.Position == br.BaseStream.Length;

    }
}
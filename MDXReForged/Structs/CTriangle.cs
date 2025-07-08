using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CTriangle(BinaryReader br)
    {
        public readonly ushort Vertex1 = br.ReadUInt16();
        public readonly ushort Vertex2 = br.ReadUInt16();
        public readonly ushort Vertex3 = br.ReadUInt16();
    }
}
using System.IO;

namespace MDXReForged.Structs
{
    public struct CTriangle
    {
        public ushort Vertex1;
        public ushort Vertex2;
        public ushort Vertex3;

        public CTriangle()
        {
        }

        public CTriangle(BinaryReader br)
        {
            Vertex1 = br.ReadUInt16();
            Vertex2 = br.ReadUInt16();
            Vertex3 = br.ReadUInt16();
        }
    }
}
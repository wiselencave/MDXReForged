using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CImVector
    {
        public readonly byte B;
        public readonly byte G;
        public readonly byte R;
        public readonly byte A;

        public CImVector()
        {
        }

        public CImVector(BinaryReader br)
        {
            B = br.ReadByte();
            G = br.ReadByte();
            R = br.ReadByte();
            A = br.ReadByte();
        }
    }
}
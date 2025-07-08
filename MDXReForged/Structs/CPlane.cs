using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CPlane
    {
        public readonly float Length;
        public readonly float Width;

        public CPlane(BinaryReader br)
        {
            Length = br.ReadSingle();
            Width = br.ReadSingle();
        }
    }
}
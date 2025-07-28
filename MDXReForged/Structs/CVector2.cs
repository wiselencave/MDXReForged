using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CVector2
    {
        public readonly float X;
        public readonly float Y;

        public CVector2(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
        }

        public override string ToString() => $"({X:0.000}, {Y:0.000})";
    }
}
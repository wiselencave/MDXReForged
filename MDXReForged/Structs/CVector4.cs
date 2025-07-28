using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CVector4
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        public CVector4(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();
            W = br.ReadSingle();
        }

        public override string ToString() => $"({X:0.000}, {Y:0.000}, {Z:0.000}, {W:0.000})";
    }
}
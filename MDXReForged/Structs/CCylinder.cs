using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CCylinder
    {
        public readonly CVector3 Base;
        public readonly float Height;
        public readonly float Radius;

        public CCylinder(BinaryReader br)
        {
            Base = new CVector3(br);
            Height = br.ReadSingle();
            Radius = br.ReadSingle();
        }
    }
}
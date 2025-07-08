using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CSphere
    {
        public readonly CVector3 Center;
        public readonly float Radius;

        public CSphere(BinaryReader br)
        {
            Center = new CVector3(br);
            Radius = br.ReadSingle();
        }
    }
}
using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CExtent
    {
        public readonly float Radius;
        public readonly CBox Extent;

        public CExtent(BinaryReader br)
        {
            Radius = br.ReadSingle();
            Extent = new CBox(br);
        }

        public override string ToString() => $"Extent — Radius: {Radius:0.000}, Min: {Extent.Min}, Max: {Extent.Max}";
    }
}
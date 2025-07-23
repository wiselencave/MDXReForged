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

        public override string ToString() => $"R: {Radius}, Min: {Extent.Min.ToString()}, Max: {Extent.Max.ToString()}";
    }
}
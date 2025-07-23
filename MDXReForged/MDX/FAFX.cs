using System.IO;

namespace MDXReForged.MDX
{
    /// <summary>
    /// FaceFX
    /// </summary>
    public class FAFX : EnumerableBaseChunk<FaceFX>
    {
        public FAFX(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new FaceFX(br));
        }
    }

    public class FaceFX(BinaryReader br)
    {
        public string Node { get; } = br.ReadCString(Constants.SizeName);
        public string FilePath { get; } = br.ReadCString(Constants.SizeFileName);
    }
}
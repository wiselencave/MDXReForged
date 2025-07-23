using System.IO;

namespace MDXReForged.MDX
{
    public class VERS : BaseChunk
    {
        public uint FormatVersion { get; }

        public VERS(BinaryReader br, uint version) : base(br, version) => FormatVersion = br.ReadUInt32();
    }
}
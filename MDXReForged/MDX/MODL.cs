using MDXReForged.Structs;
using System.IO;

namespace MDXReForged.MDX
{
    public class MODL : BaseChunk
    {
        public string Name { get; }
        public string AnimationFile { get; }
        public CExtent Bounds { get; }
        public uint BlendTime { get; }

        public MODL(BinaryReader br, uint version) : base(br, version)
        {
            Name = br.ReadCString(Constants.SizeName);
            AnimationFile = br.ReadCString(Constants.SizeFileName);
            Bounds = new CExtent(br);
            BlendTime = br.ReadUInt32();
        }
    }
}
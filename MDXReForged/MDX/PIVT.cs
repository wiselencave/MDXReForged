using MDXReForged.Structs;
using System.IO;

namespace MDXReForged.MDX
{
    public class PIVT : EnumerableBaseChunk<CVector3>
    {
        public PIVT(BinaryReader br, uint version) : base(br, version)
        {
            int count = (int)(Size / 0xC);
            Values.Capacity = count;
            for (int i = 0; i < count; i++)
                Values.Add(new CVector3(br));
        }
    }
}
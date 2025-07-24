using System.IO;

namespace MDXReForged.MDX
{
    public class GLBS : EnumerableBaseChunk<int>
    {
        public GLBS(BinaryReader br, uint version) : base(br, version)
        {
            int count = (int)(Size / 4);
            Values.Capacity = count;
            for (int i = 0; i < count; i++)
                Values.Add(br.ReadInt32());
        }
    }
}
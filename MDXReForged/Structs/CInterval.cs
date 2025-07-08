using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CInterval
    {
        public readonly int Start;
        public readonly int End;
        public readonly int Repeat;

        public CInterval()
        {
        }

        public CInterval(BinaryReader br)
        {
            Start = br.ReadInt32();
            End = br.ReadInt32();
            Repeat = br.ReadInt32();
        }
    }
}
using MDXReForged.Structs;
using System.IO;

namespace MDXReForged.MDX
{
    public class SEQS : EnumerableBaseChunk<Sequence>
    {
        public SEQS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Sequence(br));
        }
    }

    public class Sequence
    {
        public string Name { get; }
        public int MinTime { get; }
        public int MaxTime { get; }
        public float MoveSpeed { get; }
        public bool NonLooping { get; }
        public float Rarity { get; }
        public uint SyncPoint { get; }
        public CExtent Bounds { get; }

        public Sequence(BinaryReader br)
        {
            Name = br.ReadCString(Constants.SizeName);
            MinTime = br.ReadInt32();
            MaxTime = br.ReadInt32();
            MoveSpeed = br.ReadSingle();

            NonLooping = br.ReadInt32() == 1;
            Rarity = br.ReadSingle();
            SyncPoint = br.ReadUInt32();
            Bounds = new CExtent(br);
        }
        public override string ToString() =>
            $"Sequence \"{Name}\" [{MinTime}–{MaxTime}] — MoveSpeed: {MoveSpeed:0.000}, NonLooping: {NonLooping}, Rarity: {Rarity}, Sync: {SyncPoint}";

    }
}
using System.IO;

namespace MDXReForged.MDX
{
    public class SNDS : EnumerableBaseChunk<Sound>
    {
        public SNDS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Sound(br));
        }
    }

    public class Sound
    {
        public string SoundFile { get; }
        public float MaximumDistance { get; }
        public float MinimumDistance { get; }
        public uint Channel { get; }

        public Sound(BinaryReader br)
        {
            SoundFile = br.ReadCString(Constants.SizeFileName);
            MaximumDistance = br.ReadSingle();
            MinimumDistance = br.ReadSingle();
            Channel = br.ReadUInt32();
        }

        public override string ToString()
        {
            return $"Sound \"{SoundFile}\", Distance: {MinimumDistance} — {MaximumDistance}, Channel {Channel}";
        }
    }
}
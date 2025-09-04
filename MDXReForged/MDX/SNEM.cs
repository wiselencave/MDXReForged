using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class SNEM : EnumerableBaseChunk<SoundEmitter>
    {
        public SNEM(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new SoundEmitter(br));
        }
    }

    public class SoundEmitter : GenObject
    {
        public SimpleTrack SoundTrack { get; } = SimpleTrack.Empty;

        public SoundEmitter(BinaryReader br)
        {
            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GenObjectFlags)br.ReadUInt32();

            LoadTracks(br);

            if (br.HasTag(KSEK))
                SoundTrack = new SimpleTrack(br, false);
        }

        public override string ToString() => $"Sound Emitter \"{Name}\" (ObjectId: {ObjectId}, ParentId: {ParentId})";
    }
}
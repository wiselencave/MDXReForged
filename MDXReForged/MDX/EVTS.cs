using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class EVTS : EnumerableBaseChunk<Event>
    {
        public EVTS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Event(br));
        }
    }

    public class Event : GenObject
    {
        public SimpleTrack EventKeys { get; }

        public Event(BinaryReader br)
        {
            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            if (br.HasTag(KEVT))
                EventKeys = new SimpleTrack(br, false);
        }

        public override string ToString() => $"Event \"{Name}\" (ObjectId: {ObjectId}, ParentId: {ParentId})";
    }
}
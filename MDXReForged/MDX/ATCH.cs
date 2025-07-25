using System.IO;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class ATCH : EnumerableBaseChunk<Attachment>
    {
        public ATCH(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Attachment(br));
        }
    }

    public class Attachment : GenObject
    {
        public int AttachmentId { get;  }
        public string Path { get; }
        public Track<float> VisibilityKeys { get; } = Track<float>.Empty;

        public Attachment(BinaryReader br)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();

            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            Path = br.ReadCString(Constants.SizeFileName);
            AttachmentId = br.ReadInt32();

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KATV: VisibilityKeys = new Track<float>(br); break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
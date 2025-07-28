using System.IO;
using System.Linq;

namespace MDXReForged.MDX
{
    public class SimpleTrack
    {
        public static readonly SimpleTrack Empty = new SimpleTrack();
        public string Name { get; }
        public uint NrOfTracks { get;  }
        public int GlobalSequenceId { get;  }
        public uint[] Time { get; }
        public uint[] Keys { get; }

        private SimpleTrack() { }
        public SimpleTrack(BinaryReader br, bool hastime)
        {
            br.BaseStream.Position -= 4;

            Name = br.ReadString(4);
            NrOfTracks = br.ReadUInt32();
            GlobalSequenceId = br.ReadInt32();

            if (hastime)
            {
                Time = new uint[NrOfTracks];
                Keys = new uint[NrOfTracks];
                for (int i = 0; i < Keys.Length; i++)
                {
                    Time[i] = br.ReadUInt32();
                    Keys[i] = br.ReadUInt32();
                }
            }
            else
            {
                Keys = new uint[NrOfTracks];
                for (int i = 0; i < Keys.Length; i++)
                    Keys[i] = br.ReadUInt32();
            }
        }
        public bool IsEmpty => ReferenceEquals(this, Empty);

        public override string ToString()
        {
            if (IsEmpty)
                return $"SimpleTrack<{Name}> (empty)";

            string range = Time?.Length > 0 ? $" [{Time.First()}–{Time.Last()}]" : "";
            return $"SimpleTrack \"{Name}\" — {Keys.Length} key(s){range}" +
                   (GlobalSequenceId >= 0 ? $", GlobalSeqId: {GlobalSequenceId}" : "");
        }
    }
}
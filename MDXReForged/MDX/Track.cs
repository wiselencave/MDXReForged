using System;
using System.IO;
using MDXReForged.Structs;

namespace MDXReForged.MDX
{
    public class Track<T> where T : new()
    {
        public static readonly Track<T> Empty = new Track<T>();
        public string Name { get; private set; }
        public uint NrOfTracks { get; private set; }
        public InterpolationType InterpolationType { get; private set; }
        public int GlobalSequenceId { get; private set; }
        public CAnimatorNode<T>[] Nodes { get; private set; }

        private static readonly Func<BinaryReader, T> Reader;

        static Track()
        {
            if (typeof(T) == typeof(float))
                Reader = br => (T)(object)br.ReadSingle();
            else if (typeof(T) == typeof(int))
                Reader = br => (T)(object)br.ReadInt32();
            else
            {
                var ctor = typeof(T).GetConstructor(new[] { typeof(BinaryReader) });
                if (ctor != null)
                {
                    Reader = br => (T)ctor.Invoke(new object[] { br });
                }
                else
                {
                    Reader = br => new T();
                }
            }
        }

        private Track() 
        {
            Nodes = [];
        }

        public Track(uint tag, BinaryReader br) => Load(tag, br);
        private void Load(uint tag, BinaryReader br)
        {
            Name = Extensions.TagToString(tag);
            NrOfTracks = br.ReadUInt32();
            InterpolationType = (InterpolationType)br.ReadUInt32();
            GlobalSequenceId = br.ReadInt32();

            Nodes = new CAnimatorNode<T>[NrOfTracks];
            for (int i = 0; i < NrOfTracks; i++)
            {
                uint Time = br.ReadUInt32();
                T Value = ReadValue(br);

                if (InterpolationType > InterpolationType.Linear)
                {
                    T InTangent = ReadValue(br);
                    T OutTangent = ReadValue(br);

                    Nodes[i] = new CAnimatorNode<T>(Time, Value, InTangent, OutTangent);
                }
                else
                {
                    Nodes[i] = new CAnimatorNode<T>(Time, Value);
                }
            }
        }
        public bool IsEmpty => ReferenceEquals(this, Empty);
        private static T ReadValue(BinaryReader br) => Reader(br);

        public override string ToString()
        {
            if (IsEmpty)
                return $"Track<{typeof(T).Name}> (empty)";

            return $"Track \"{Name}\" ({typeof(T).Name}) — {Nodes.Length} key(s), Interpolation: {InterpolationType}" +
                   (GlobalSequenceId >= 0 ? $", GlobalSeqId: {GlobalSequenceId}" : "");
        }

    }
}
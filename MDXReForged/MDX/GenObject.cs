using MDXReForged.Structs;
using System.IO;

namespace MDXReForged.MDX
{
    public class GenObject
    {
        public uint ObjSize { get; protected set; }
        public string Name { get; protected set; }
        public int ObjectId { get; protected set; }
        public int ParentId { get; protected set; }
        public GENOBJECTFLAGS Flags { get; protected set; }

        public Track<CVector3> TranslationKeys { get; private set; } = Track<CVector3>.Empty;
        public Track<CVector4> RotationKeys { get; private set; } = Track<CVector4>.Empty;
        public Track<CVector3> ScaleKeys { get; private set; } = Track<CVector3>.Empty;

        public void LoadTracks(BinaryReader br)
        {
            while (!br.AtEnd())
            {
                string tagname = br.ReadString(4);
                switch (tagname)
                {
                    case "KGTR":
                        TranslationKeys = new Track<CVector3>(br);
                        break;

                    case "KGRT":
                        RotationKeys = new Track<CVector4>(br);
                        break;

                    case "KGSC":
                        ScaleKeys = new Track<CVector3>(br);
                        break;

                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
    }
}
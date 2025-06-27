using MDXReForged.Structs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MDXReForged.MDX
{
    public class MTLS : EnumerableBaseChunk<Material>
    {
        public MTLS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Material(br, version));
        }
    }

    public class Material
    {
        public uint TotalSize;
        public int PriorityPlane;
        public string Shader;
        public uint Flags;
        public uint NrOfLayers;
        public List<Layer> Layers = new List<Layer>();

        public Material(BinaryReader br, uint version)
        {
            TotalSize = br.ReadUInt32();     
            PriorityPlane = br.ReadInt32(); 
            Flags = br.ReadUInt32();         

            if (version >= 900 && version < 1100)
                Shader = br.ReadCString(Constants.SizeName);

            br.AssertTag("LAYS");   
            NrOfLayers = br.ReadUInt32();
            for (int i = 0; i < NrOfLayers; i++)
                Layers.Add(new Layer(br, version));
        }
    }

    public class Layer
    {
        public uint TotalSize;
        public MDLTEXOP BlendMode;
        public MDLGEO Flags;
        public uint TextureId;
        public int TextureAnimationId;
        public int CoordId;
        public float Alpha;
        public Track<float> AlphaKeys;
        public Track<int> FlipKeys;

        // If version > 800
        public float EmissiveGain;
        public Track<float> EmissiveGainKeys;

        // If version > 900
        public CVector3 FresnelColor;
        public float FresnelOpacity;
        public float FresnelTeamColor;
        public Track<CVector3> FresnelColorKeys;
        public Track<float> FresnelOpacityKeys;
        public Track<float> FresnelTeamColorKeys;

        // If version >= 1100
        public LayerShader ShaderId;
        public List<TextureEntry> Textures = new List<TextureEntry>();

        public Layer(BinaryReader br, uint version)
        {
            long end = br.BaseStream.Position + (TotalSize = br.ReadUInt32());
            BlendMode = (MDLTEXOP)br.ReadInt32();
            Flags = (MDLGEO)br.ReadUInt32();
            TextureId = br.ReadUInt32();
            TextureAnimationId = br.ReadInt32();
            CoordId = br.ReadInt32();
            Alpha = br.ReadSingle();

            if (br.BaseStream.Position < end && version >= 900)
            {
                EmissiveGain = br.ReadSingle();
                FresnelColor = new CVector3(br);
                FresnelOpacity = br.ReadSingle();
                FresnelTeamColor = br.ReadSingle();
            }

            // Version 1100+: Shader + multiple textures with semantics
            if (version >= 1100)
            {
                ShaderId = (LayerShader)br.ReadUInt32();
                uint textureCount = br.ReadUInt32();

                for (uint i = 0; i < textureCount; i++)
                {
                    var tex = new TextureEntry
                    {
                        TextureId = br.ReadUInt32(),
                        Semantic = (TextureSemantic)br.ReadUInt32()
                    };

                    string maybeTag = br.ReadCString(4);
                    if (maybeTag == "KMTF")
                    {
                        tex.FlipKeys = new Track<int>(br);
                    }
                    else
                    {
                        br.BaseStream.Position -= 4;
                    }

                    Textures.Add(tex);
                }
            }

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                string tagname = br.ReadCString(4);
                switch (tagname)
                {
                    case "KMTA": AlphaKeys = new Track<float>(br); break;
                    case "KMTE": EmissiveGainKeys = new Track<float>(br); break; // > 800
                    case "KFC3": FresnelColorKeys = new Track<CVector3>(br); break; // > 900
                    case "KFCA": FresnelOpacityKeys = new Track<float>(br); break; 
                    case "KFTC": FresnelTeamColorKeys = new Track<float>(br); break;
                    case "KMTF":
                        if (version < 1100)
                            FlipKeys = new Track<int>(br);
                        else if (Textures.Count > 0)
                            Textures[0].FlipKeys = new Track<int>(br); // apply to diffuse
                        break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
        public uint? GetTextureId(TextureSemantic semantic)
        {
            return Textures.FirstOrDefault(t => t.Semantic == semantic)?.TextureId;
        }
    }
    public enum LayerShader : uint
    {
        SD = 0,
        HD = 1
    }
    public enum TextureSemantic : uint
    {
        Diffuse = 0,
        Normal = 1,
        ORM = 2,
        Emissive = 3,
        TeamColor = 4,
        Reflection = 5
    }

    public class TextureEntry
    {
        public uint TextureId;
        public TextureSemantic Semantic;
        public Track<int> FlipKeys;
    }
}
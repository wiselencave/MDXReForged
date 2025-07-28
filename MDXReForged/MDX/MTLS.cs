using MDXReForged.Structs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MDXReForged.Tags;

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
        public int PriorityPlane { get; }
        public string Shader { get; }
        public uint Flags { get; }
        public uint NrOfLayers { get; }
        public List<Layer> Layers { get; } = new List<Layer>();

        public Material(BinaryReader br, uint version)
        {
            var totalSize = br.ReadUInt32();     
            PriorityPlane = br.ReadInt32(); 
            Flags = br.ReadUInt32();         

            if (version >= 900 && version < 1100)
                Shader = br.ReadCString(Constants.SizeName);

            br.AssertTag(Tags.LAYS);   
            NrOfLayers = br.ReadUInt32();
            for (int i = 0; i < NrOfLayers; i++)
                Layers.Add(new Layer(br, version));
        }
        public override string ToString()
        {
            string shaderInfo = string.IsNullOrEmpty(Shader) ? "" : $"Shader: \"{Shader}\", ";
            return $"Material — {shaderInfo}Layers: {Layers.Count}";
        }
    }

    public class Layer
    {
        public LAYER_FILTER_MODE BlendMode { get; }
        public MDLGEO Flags { get; }
        public uint TextureId { get; }
        public int TextureAnimationId { get; }
        public int CoordId { get; }
        public float Alpha { get; }
        public Track<float> AlphaKeys { get; } = Track<float>.Empty;
        public Track<int> FlipKeys { get; } = Track<int>.Empty;

        // If version >= 900
        public float? EmissiveGain { get; }
        public Track<float> EmissiveGainKeys { get; } = Track<float>.Empty;

        // If version >= 1000
        public CVector3? FresnelColor { get; }
        public float? FresnelOpacity { get; }
        public float? FresnelTeamColor { get; }
        public Track<CVector3> FresnelColorKeys { get; } = Track<CVector3>.Empty;
        public Track<float> FresnelOpacityKeys { get; } = Track<float>.Empty;
        public Track<float> FresnelTeamColorKeys { get; } = Track<float>.Empty;

        // If version >= 1100
        public LAYER_SHADER? ShaderId { get; }
        public IReadOnlyList<TextureEntry> Textures { get; }

        public Layer(BinaryReader br, uint version)
        {
            long end = br.BaseStream.Position + br.ReadUInt32();
            BlendMode = (LAYER_FILTER_MODE)br.ReadInt32();
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
            TextureEntry[] textures = [];
            if (version >= 1100)
            {
                ShaderId = (LAYER_SHADER)br.ReadUInt32();
                uint textureCount = br.ReadUInt32();
                textures = new TextureEntry[textureCount];

                for (uint i = 0; i < textureCount; i++)
                {
                    var tex = new TextureEntry
                    {
                        TextureId = br.ReadUInt32(),
                        Semantic = (TEXTURE_SEMANTIC)br.ReadUInt32()
                    };

                    uint maybeTag = br.ReadUInt32Tag();
                    if (maybeTag == KMTF)
                    {
                        tex.FlipKeys = new Track<int>(maybeTag, br);
                    }
                    else
                    {
                        br.BaseStream.Position -= 4;
                    }

                    textures[i] = tex;
                }
            }
            Textures = textures;

            while (br.BaseStream.Position < end && !br.AtEnd())
            {
                uint tagname = br.ReadUInt32Tag();
                switch (tagname)
                {
                    case KMTA: AlphaKeys = new Track<float>(tagname, br); break;
                    case KMTE: EmissiveGainKeys = new Track<float>(tagname, br); break; // >= 900
                    case KFC3: FresnelColorKeys = new Track<CVector3>(tagname, br); break; // >= 1000
                    case KFCA: FresnelOpacityKeys = new Track<float>(tagname, br); break;
                    case KFTC: FresnelTeamColorKeys = new Track<float>(tagname, br); break;
                    case KMTF:
                        if (version < 1100)
                            FlipKeys = new Track<int>(tagname, br);
                        else if (Textures.Count > 0)
                            Textures[0].FlipKeys = new Track<int>(tagname, br); // apply to diffuse
                        break;
                    default:
                        br.BaseStream.Position -= 4;
                        return;
                }
            }
        }
        public uint? GetTextureId(TEXTURE_SEMANTIC semantic)
        {
            return Textures.FirstOrDefault(t => t.Semantic == semantic)?.TextureId;
        }

        public override string ToString()
        {
            string shaderInfo = ShaderId.HasValue ? $", ShaderId: {ShaderId}" : "";
            return $"Layer — Blend: {BlendMode}, ShaderId: {ShaderId}";
        }
    }

    public class TextureEntry
    {
        public uint TextureId { get; internal set; }
        public TEXTURE_SEMANTIC Semantic { get; internal set; }
        public Track<int> FlipKeys { get; internal set; } = Track<int>.Empty;
        public override string ToString()
        {
            string flip = FlipKeys.IsEmpty ? "" : $"\r\n\t Flip Track: {FlipKeys}";
            return $"TextureId: {TextureId} ({Semantic}) — {flip}";
        }
    }
}
using System;

namespace MDXReForged
{
    public static class Constants
    {
        public const int SizeTag = 4;
        public const int SizeName = 80;
        public const int SizeFileName = 260;
    }

    #region ENUMS

    public enum LightType : uint
    {
        Omni = 0,
        Directional = 1,
        Ambient = 2
    }

    public enum GeometryShape : uint
    {
        Box = 0,
        Cylinder = 1,
        Sphere = 2,
        Plane = 3,
    }

    public enum BlendMode : uint
    {
        None = 0,
        Transparent = 1,
        Blend = 2,
        Add = 3,
        AddAlpha = 4,
        Modulate = 5,
        Modulate2x = 6
    }

    [Flags]
    public enum ShadingFlags : uint
    {
        Unshaded = 0x1,
        SphereEnvMap = 0x2,   // unused
        WrapWidth = 0x4,   // unused
        WrapHeight = 0x8,   // unused
        TwoSided = 0x10,
        Unfogged = 0x20,
        NoDepthTest = 0x40,
        NoDepthSet = 0x80
    }

    public enum FilterMode : uint
    {
        Blend = 0,
        Add = 1,
        Modulate = 2,
        Modulate2x = 3,
        AlphaKey = 4
    }

    public enum ParticleType : uint
    {
        Head = 0,
        Tail = 1,
        Both = 2
    }

    public enum InterpolationType : uint
    {
        NoInterp = 0,
        Linear = 1,
        Hermite = 2,
        Bezier = 3
    }

    [Flags]
    public enum TextureFlags : uint
    {
        WrapWidth = 0x1,
        WrapHeight = 0x2
    }

    [Flags]
    public enum GenObjectFlags : uint
    {
        DontInheritTranslation = 0x00000001,
        DontInheritScaling = 0x00000002,
        DontInheritRotation = 0x00000004,
        Billboarded = 0x00000008,
        BillboardedLockX = 0x00000010,
        BillboardedLockY = 0x00000020,
        BillboardedLockZ = 0x00000040,
        CameraAnchored = 0x00000080,
        BoneSection = 0x00000100,
        LightSection = 0x00000200,
        EventSection = 0x00000400,
        AttachmentSection = 0x00000800,
        ParticleEmitter = 0x00001000,
        HitTestShape = 0x00002000,
        RibbonEmitter = 0x00004000,
        EmitterUsesModel = 0x00008000,
        Unshaded = 0x00008000,
        EmitterUsesTga = 0x00010000,
        SortPrimitivesFarZ = 0x00010000,
        LineEmitter = 0x00020000,
        ParticleUnfogged = 0x00040000,
        ParticleUseModelSpace = 0x00080000,
        ParticleXYQuads = 0x00100000,
    }

    public enum PrimitiveType : uint
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quands = 7,
        QuadStrip = 8,
        Polygons = 9
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

    public enum LevelOfDetail : uint
    {
        Default = 0,
        High = 1,
        Medium = 2,
        Low = 3
    }

    #endregion ENUMS
}
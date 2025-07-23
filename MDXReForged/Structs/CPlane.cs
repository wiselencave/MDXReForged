using System.IO;
using static MDXReForged.MDX.CollisionShape;

namespace MDXReForged.Structs
{
    public readonly struct CPlane : ICollisionGeometry
    {
        public readonly float Length;
        public readonly float Width;

        public GEOM_SHAPE ShapeType => GEOM_SHAPE.SHAPE_PLANE;
        public CPlane(BinaryReader br)
        {
            Length = br.ReadSingle();
            Width = br.ReadSingle();
        }
    }
}
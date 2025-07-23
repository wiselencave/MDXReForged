using System.IO;
using static MDXReForged.MDX.CollisionShape;

namespace MDXReForged.Structs
{
    public readonly struct CBox : ICollisionGeometry
    {
        public readonly CVector3 Min;
        public readonly CVector3 Max;
        public GEOM_SHAPE ShapeType => GEOM_SHAPE.SHAPE_BOX;

        public CBox(BinaryReader br)
        {
            Min = new CVector3(br);
            Max = new CVector3(br);
        }
    }
}
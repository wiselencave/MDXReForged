using static MDXReForged.MDX.CollisionShape;
using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CSphere : ICollisionGeometry
    {
        public readonly CVector3 Center;
        public readonly float Radius;
        public GEOM_SHAPE ShapeType => GEOM_SHAPE.SHAPE_SPHERE;
        public CSphere(BinaryReader br)
        {
            Center = new CVector3(br);
            Radius = br.ReadSingle();
        }
    }
}
using static MDXReForged.MDX.CollisionShape;
using System.IO;

namespace MDXReForged.Structs
{
    public readonly struct CSphere : ICollisionGeometry
    {
        public readonly CVector3 Center;
        public readonly float Radius;
        public GeometryShape ShapeType => GeometryShape.Sphere;
        public CSphere(BinaryReader br)
        {
            Center = new CVector3(br);
            Radius = br.ReadSingle();
        }

        public override string ToString() =>$"Sphere — Center: {Center}, Radius: {Radius:0.000}";
    }
}
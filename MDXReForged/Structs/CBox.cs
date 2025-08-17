using System.IO;
using static MDXReForged.MDX.CollisionShape;

namespace MDXReForged.Structs
{
    public readonly struct CBox : ICollisionGeometry
    {
        public readonly CVector3 Min;
        public readonly CVector3 Max;
        public GeometryShape ShapeType => GeometryShape.Box;

        public CBox(BinaryReader br)
        {
            Min = new CVector3(br);
            Max = new CVector3(br);
        }

        public override string ToString() => $"Box — Min: {Min}, Max: {Max}";
    }
}
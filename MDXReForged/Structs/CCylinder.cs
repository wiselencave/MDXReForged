using System.IO;
using static MDXReForged.MDX.CollisionShape;

namespace MDXReForged.Structs
{
    public readonly struct CCylinder : ICollisionGeometry
    {
        public readonly CVector3 Base;
        public readonly float Height;
        public readonly float Radius;
        public GEOM_SHAPE ShapeType => GEOM_SHAPE.SHAPE_CYLINDER;
        public CCylinder(BinaryReader br)
        {
            Base = new CVector3(br);
            Height = br.ReadSingle();
            Radius = br.ReadSingle();
        }

        public override string ToString() => $"Cylinder — Base: {Base}, Height: {Height:0.000}, Radius: {Radius:0.000}";
    }
}
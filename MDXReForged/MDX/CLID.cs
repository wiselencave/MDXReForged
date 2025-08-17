using MDXReForged.Structs;
using System;
using System.Collections.Generic;
using System.IO;

namespace MDXReForged.MDX
{
    public class CLID : EnumerableBaseChunk<CollisionShape>
    {
        public CLID(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new CollisionShape(br));
        }
    }

    public class CollisionShape : GenObject
    {
        public ICollisionGeometry Geometry { get; }

        public CollisionShape(BinaryReader br)
        {
            ObjSize = br.ReadUInt32();
            Name = br.ReadCString(Constants.SizeName);
            ObjectId = br.ReadInt32();
            ParentId = br.ReadInt32();
            Flags = (GenObjectFlags)br.ReadUInt32();

            LoadTracks(br);

            var type = (GeometryShape)br.ReadUInt32();
            Geometry = type switch
            {
                GeometryShape.Box => new CBox(br),
                GeometryShape.Sphere => new CSphere(br),
                GeometryShape.Plane => new CPlane(br),
                GeometryShape.Cylinder => new CCylinder(br),
                _ => throw new NotSupportedException($"Unknown shape: {type}")
            };
        }
        public bool TryGetGeometry<T>(out T geometry) where T : struct, ICollisionGeometry
        {
            if (Geometry is T typed)
            {
                geometry = typed;
                return true;
            }

            geometry = default;
            return false;
        }

        public interface ICollisionGeometry
        {
            GeometryShape ShapeType { get; }
        }

        public override string ToString() => 
            $"Collision Shape \"{Name}\" (ObjectId: {ObjectId}, Parent: {ParentId}) — Type: {Geometry.ShapeType}, Geometry: {Geometry}";
    }
}
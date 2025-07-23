using MDXReForged.Structs;
using System;
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
            Flags = (GENOBJECTFLAGS)br.ReadUInt32();

            LoadTracks(br);

            var type = (GEOM_SHAPE)br.ReadUInt32();
            Geometry = type switch
            {
                GEOM_SHAPE.SHAPE_BOX => new CBox(br),
                GEOM_SHAPE.SHAPE_SPHERE => new CSphere(br),
                GEOM_SHAPE.SHAPE_PLANE => new CPlane(br),
                GEOM_SHAPE.SHAPE_CYLINDER => new CCylinder(br),
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
            GEOM_SHAPE ShapeType { get; }
        }
    }
}
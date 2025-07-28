using MDXReForged.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MDXReForged.Tags;

namespace MDXReForged.MDX
{
    public class GEOS : EnumerableBaseChunk<Geoset>
    {
        public GEOS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            uint id = 0;
            while (br.BaseStream.Position < end)
            {
                Values.Add(new Geoset(br, id, version));
                id++;
            }
        }
    }

    public class Geoset
    {
        public IReadOnlyList<CVector3> Vertices { get; }
        public IReadOnlyList<CVector3> Normals { get; }
        public IReadOnlyList<CVector4> Tangents { get; }

        public IReadOnlyList<PRIMITIVE_TYPE> FaceTypes { get; }
        public IReadOnlyList<uint> FaceGroups { get; }
        public IReadOnlyList<ushort> FaceIndices { get; }

        public IReadOnlyList<byte> VertexGroupIndices { get; }
        public IReadOnlyList<uint> MatrixGroups { get; }
        public IReadOnlyList<uint> MatrixIndexes { get; }

        public IReadOnlyList<CSkinData> Skin { get; }

        public IReadOnlyList<IReadOnlyList<CVector2>> TexCoords { get; }

        public uint MaterialId { get; }
        public CExtent Bounds { get; }
        public uint SelectionGroup { get; }
        public bool Unselectable { get; }
        public LEVEL_OF_DETAIL Level { get; }
        public string Name { get; }
        public uint GeosetId { get; }

        public IReadOnlyList<CExtent> Extents { get; }

        public Geoset(BinaryReader br, uint id, uint version)
        {
            CVector3[] vertices = [];
            CVector3[] normals = [];
            CVector4[] tangents = [];

            PRIMITIVE_TYPE[] faceTypes = [];
            uint[] faceGroups = [];
            ushort[] faceIndices = [];

            byte[] vertexGroups = [];
            uint[] matrixGroups = [];
            uint[] matrixIndexes = [];

            CSkinData[] skin = [];
            List<IReadOnlyList<CVector2>> texCoords = new();

            List<CExtent> extents = new();

            long totalSize = br.BaseStream.Position + br.ReadUInt32();

            if (br.HasTag(VRTX))
            {
                uint count = br.ReadUInt32();
                vertices = new CVector3[count];
                for (int i = 0; i < count; i++)
                    vertices[i] = new CVector3(br);
            }

            if (br.HasTag(NRMS))
            {
                uint count = br.ReadUInt32();
                normals = new CVector3[count];
                for (int i = 0; i < count; i++)
                    normals[i] = new CVector3(br);
            }

            if (br.HasTag(PTYP))
            {
                uint count = br.ReadUInt32();
                faceTypes = new PRIMITIVE_TYPE[count];
                for (int i = 0; i < count; i++)
                    faceTypes[i] = (PRIMITIVE_TYPE)br.ReadUInt32();
            }

            if (br.HasTag(PCNT))
            {
                uint count = br.ReadUInt32();
                faceGroups = new uint[count];
                for (int i = 0; i < count; i++)
                    faceGroups[i] = br.ReadUInt32();
            }

            if (br.HasTag(PVTX))
            {
                uint count = br.ReadUInt32();
                faceIndices = new ushort[count];
                for (int i = 0; i < count; i++)
                    faceIndices[i] = br.ReadUInt16();
            }

            if (br.HasTag(GNDX))
            {
                uint count = br.ReadUInt32();
                vertexGroups = br.ReadBytes((int)count);
            }

            if (br.HasTag(MTGC))
            {
                uint count = br.ReadUInt32();
                matrixGroups = new uint[count];
                for (int i = 0; i < count; i++)
                    matrixGroups[i] = br.ReadUInt32();
            }

            if (br.HasTag(MATS))
            {
                uint count = br.ReadUInt32();
                matrixIndexes = new uint[count];
                for (int i = 0; i < count; i++)
                    matrixIndexes[i] = br.ReadUInt32();
            }

            MaterialId = br.ReadUInt32();
            SelectionGroup = br.ReadUInt32();
            Unselectable = br.ReadUInt32() == 1;

            if (version >= 900)
            {
                Level = (LEVEL_OF_DETAIL)br.ReadUInt32();
                Name = br.ReadCString(Constants.SizeName);
            }

            Bounds = new CExtent(br);

            uint nrOfExtents = br.ReadUInt32();
            for (int i = 0; i < nrOfExtents; i++)
                extents.Add(new CExtent(br));

            if (br.HasTag(TANG))
            {
                uint count = br.ReadUInt32();
                tangents = new CVector4[count];
                for (int i = 0; i < count; i++)
                    tangents[i] = new CVector4(br);
            }

            if (br.HasTag(SKIN))
            {
                uint skinSize = br.ReadUInt32();
                int vertexCount = (int)(skinSize / 8);

                skin = new CSkinData[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                    skin[i] = new CSkinData(br);
            }

            if (br.HasTag(UVAS))
            {
                uint texSetCount = br.ReadUInt32();
                for (int i = 0; i < texSetCount; i++)
                {
                    if (br.HasTag(UVBS))
                    {
                        int uvCount = br.ReadInt32();
                        var uvArray = new CVector2[uvCount];
                        for (int j = 0; j < uvCount; j++)
                            uvArray[j] = new CVector2(br);
                        texCoords.Add(uvArray);
                    }
                    else
                    {
                        texCoords.Add([]);
                    }
                }
            }

            Vertices = vertices;
            Normals = normals;
            Tangents = tangents;
            FaceTypes = faceTypes;
            FaceGroups = faceGroups;
            FaceIndices = faceIndices;
            VertexGroupIndices = vertexGroups;
            MatrixGroups = matrixGroups;
            MatrixIndexes = matrixIndexes;
            Skin = skin;
            TexCoords = texCoords;
            Extents = extents;
            GeosetId = id;
        }

        public bool IsAllTriangles => FaceTypes.All(t => t == PRIMITIVE_TYPE.TYPE_TRIANGLES);

        /// <summary>
        /// Returns a flat index buffer, assuming the geoset contains only triangles.
        /// </summary>
        public ushort[] GetTriangleIndexBuffer()
        {
            if (!IsAllTriangles)
                throw new InvalidOperationException("Geoset contains non-triangle primitives.");

            return FaceIndices.ToArray();
        }

        /// <summary>
        /// Enumerates face groups with their primitive type and corresponding indices.
        /// </summary>
        public IEnumerable<(PRIMITIVE_TYPE Type, ushort[] Indices)> EnumeratePrimitiveGroups()
        {
            if (FaceTypes.Count != FaceGroups.Count)
                throw new InvalidOperationException("Mismatch between face type count and group size count.");

            long totalExpectedIndices = FaceGroups.Sum(g => (long)g);

            if (totalExpectedIndices > FaceIndices.Count)
                throw new InvalidOperationException("Face group sizes exceed total index count.");

            int offset = 0;
            for (int i = 0; i < FaceGroups.Count; i++)
            {
                int size = (int)FaceGroups[i];
                var indices = FaceIndices.Skip(offset).Take(size).ToArray();
                yield return (FaceTypes[i], indices);
                offset += size;
            }
        }

        /// <summary>
        /// Enumerates triangles as (A, B, C) index triplets. Skips non-triangle groups.
        /// </summary>
        public IEnumerable<(ushort A, ushort B, ushort C)> EnumerateTriangles()
        {
            foreach (var (type, indices) in EnumeratePrimitiveGroups())
            {
                if (type != PRIMITIVE_TYPE.TYPE_TRIANGLES || indices.Length % 3 != 0)
                    continue;

                for (int i = 0; i < indices.Length; i += 3)
                    yield return (indices[i], indices[i + 1], indices[i + 2]);
            }
        }
        public override string ToString() =>
           $"Geoset \"{Name}\" (ID: {GeosetId}, {Level}) — Vertices: {Vertices.Count}, Indices: {FaceIndices.Count}";
    }
}
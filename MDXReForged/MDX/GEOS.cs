using MDXReForged.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MDXReForged.MDX
{
    public class GEOS : EnumerableBaseChunk<Geoset>
    {
        public GEOS(BinaryReader br, uint version) : base(br, version)
        {
            long end = br.BaseStream.Position + Size;
            while (br.BaseStream.Position < end)
                Values.Add(new Geoset(br, version));
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

        public IReadOnlyList<CExtent> Extents { get; }

        public Geoset(BinaryReader br, uint version)
        {
            var vertices = new List<CVector3>();
            var normals = new List<CVector3>();
            var tangents = new List<CVector4>();

            var faceTypes = new List<PRIMITIVE_TYPE>();
            var faceGroups = new List<uint>();
            var faceIndices = new List<ushort>();

            var vertexGroups = new List<byte>();
            var matrixGroups = new List<uint>();
            var matrixIndexes = new List<uint>();

            var skin = new List<CSkinData>();
            var texCoords = new List<IReadOnlyList<CVector2>>();

            var extents = new List<CExtent>();

            long totalSize = br.BaseStream.Position + br.ReadUInt32();

            if (br.HasTag("VRTX"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    vertices.Add(new CVector3(br));
            }

            if (br.HasTag("NRMS"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    normals.Add(new CVector3(br));
            }

            if (br.HasTag("PTYP"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    faceTypes.Add((PRIMITIVE_TYPE)br.ReadUInt32());
            }

            if (br.HasTag("PCNT"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    faceGroups.Add(br.ReadUInt32());
            }

            if (br.HasTag("PVTX"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    faceIndices.Add(br.ReadUInt16());
            }

            if (br.HasTag("GNDX"))
            {
                uint count = br.ReadUInt32();
                vertexGroups.AddRange(br.ReadBytes((int)count));
            }

            if (br.HasTag("MTGC"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    matrixGroups.Add(br.ReadUInt32());
            }

            if (br.HasTag("MATS"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    matrixIndexes.Add(br.ReadUInt32());
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

            if (br.HasTag("TANG"))
            {
                uint count = br.ReadUInt32();
                for (int i = 0; i < count; i++)
                    tangents.Add(new CVector4(br));
            }

            if (br.HasTag("SKIN"))
            {
                uint skinSize = br.ReadUInt32();
                int vertexCount = (int)(skinSize / 8);

                for (int i = 0; i < vertexCount; i++)
                {
                    byte[] indices = br.ReadBytes(4);
                    byte[] weights = br.ReadBytes(4);
                    skin.Add(new CSkinData(indices, weights));
                }
            }

            if (br.HasTag("UVAS"))
            {
                uint texSetCount = br.ReadUInt32();
                for (int i = 0; i < texSetCount; i++)
                {
                    if (br.HasTag("UVBS"))
                    {
                        int uvCount = br.ReadInt32();
                        var uvList = new List<CVector2>(uvCount);

                        for (int j = 0; j < uvCount; j++)
                            uvList.Add(new CVector2(br));

                        texCoords.Add(uvList);
                    }
                    else
                    {
                        texCoords.Add(new List<CVector2>());
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
        }

        /// <summary>
        /// Enumerates all face groups in the mesh, returning their primitive type and corresponding index array.
        /// </summary>
        /// <returns>
        /// A sequence of tuples, where each tuple contains the <see cref="PRIMITIVE_TYPE"/> and a <see cref="ushort"/> array
        /// of indices corresponding to that group.
        /// </returns>
        /// <remarks>
        /// Uses <see cref="FaceTypes"/> and <see cref="FaceGroups"/> to split the flat index buffer <see cref="FaceIndices"/>
        /// into logical primitive groups. Each entry in <c>FaceGroups</c> specifies the number of indices in the corresponding group.
        /// </remarks>
        public IEnumerable<(PRIMITIVE_TYPE Type, ushort[] Indices)> EnumerateGroups()
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
        /// Returns all triangles in the mesh, assuming only triangle primitives are present.
        /// </summary>
        /// <returns>List of triangles as arrays of 3 indices.</returns>
        public IEnumerable<(ushort A, ushort B, ushort C)> EnumerateTriangles()
        {
            foreach (var (type, indices) in EnumerateGroups())
            {
                if (type != PRIMITIVE_TYPE.TYPE_TRIANGLES || indices.Length % 3 != 0)
                    continue;

                for (int i = 0; i < indices.Length; i += 3)
                    yield return (indices[i], indices[i + 1], indices[i + 2]);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDXReForged.Structs
{
    public readonly struct CSkinData
    {
        public readonly byte[] BoneIndices; // length 4
        public readonly byte[] BoneWeights; // length 4

        public CSkinData(BinaryReader br)
        {
            BoneIndices = br.ReadBytes(4);
            BoneWeights = br.ReadBytes(4);
        }

        public override string ToString() =>
            $"Indices: [{string.Join(", ", BoneIndices)}], Weights: [{string.Join(", ", BoneWeights)}]";
    }
}


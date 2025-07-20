using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDXReForged.Structs
{
    public readonly struct CSkinData
    {
        public readonly byte[] BoneIndices; // length 4
        public readonly byte[] BoneWeights; // length 4

        public CSkinData(byte[] boneIndices, byte[] boneWeights)
        {
            BoneIndices = boneIndices;
            BoneWeights = boneWeights;
        }

        public override string ToString() =>
            $"Indices: [{string.Join(", ", BoneIndices)}], Weights: [{string.Join(", ", BoneWeights)}]";
    }
}


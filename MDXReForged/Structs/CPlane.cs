﻿using System.IO;

namespace MDXReForged.Structs
{
    public struct CPlane
    {
        public float Length;
        public float Width;

        public CPlane(BinaryReader br)
        {
            Length = br.ReadSingle();
            Width = br.ReadSingle();
        }
    }
}
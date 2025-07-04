﻿using System.IO;

namespace MDXReForged.Structs
{
    public struct CSphere
    {
        public CVector3 Center;
        public float Radius;

        public CSphere(BinaryReader br)
        {
            Center = new CVector3(br);
            Radius = br.ReadSingle();
        }
    }
}
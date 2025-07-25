using System;
using System.IO;
using System.Numerics;

namespace MDXReForged.Structs
{
    public readonly struct C34Matrix
    {
        public readonly float M11, M12, M13; // Rotation row 1
        public readonly float M21, M22, M23; // Rotation row 2
        public readonly float M31, M32, M33; // Rotation row 3
        public readonly float M41, M42, M43; // Translation row

        public C34Matrix(BinaryReader br)
        {
            M11 = br.ReadSingle(); M12 = br.ReadSingle(); M13 = br.ReadSingle();
            M21 = br.ReadSingle(); M22 = br.ReadSingle(); M23 = br.ReadSingle();
            M31 = br.ReadSingle(); M32 = br.ReadSingle(); M33 = br.ReadSingle();
            M41 = br.ReadSingle(); M42 = br.ReadSingle(); M43 = br.ReadSingle();
        }
        public float GetDeterminant3x3()
        {
            return
                M11 * (M22 * M33 - M23 * M32) -
                M12 * (M21 * M33 - M23 * M31) +
                M13 * (M21 * M32 - M22 * M31);
        }

        public override string ToString()
        {
            static string Row(float a, float b, float c, float d)
                => $"| {a,8:0.000} {b,8:0.000} {c,8:0.000} {d,8:0.000} |";

            string matrix = string.Join(Environment.NewLine, new[]
            {
                Row(M11, M12, M13, 0),
                Row(M21, M22, M23, 0),
                Row(M31, M32, M33, 0),
                Row(M41, M42, M43, 1),
            });

            float det = GetDeterminant3x3();

            return $"{matrix}{Environment.NewLine}Determinant: {det:0.0000}";
        }

    }
}
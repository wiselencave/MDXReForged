namespace MDXReForged.Structs
{
    public readonly struct CAnimatorNode<T>
    {
        public readonly uint Time;
        public readonly T Value;
        public readonly T InTangent;
        public readonly T OutTangent;

        public CAnimatorNode(uint time, T value)
        {
            Time = time;
            Value = value;
        }

        public CAnimatorNode(uint time, T value, T inTangent, T outTangent)
        {
            Time = time;
            Value = value;
            InTangent = inTangent;
            OutTangent = outTangent;
        }

        public override string ToString() =>
            $"[{Time}]: {Value} (In: {InTangent}, Out: {OutTangent})";
    }
}

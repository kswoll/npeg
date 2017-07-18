namespace PEG
{
    public struct ParseMark
    {
        public int InputPosition { get; }
        public int OutputPosition { get; }

        public ParseMark(int inputPosition, int outputPosition)
        {
            InputPosition = inputPosition;
            OutputPosition = outputPosition;
        }
    }
}
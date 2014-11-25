using System.Text;
using PEG.Extensions;

namespace PEG
{
    public class ParseError
    {
        public string Input { get; set; }
        public int Position { get; set; }

        public ParseError(string input, int position)
        {
            Input = input;
            Position = position;
        }

        public override string ToString()
        {
            int offset;
            string line = Input.GetLine(Position, out offset);
            int oldLineLength = line.Length;
            line = line.Replace("\t", "\\t");
            offset += line.Length - oldLineLength;

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Parse Failure");
            builder.AppendLine(line);

            for (int i = 0; i < offset; i++)
                builder.Append(' ');
            builder.Append('^');
            builder.AppendLine();

            builder.AppendLine("Current Input Character: " + Input[Position]);

            return builder.ToString();
        }
    }
}
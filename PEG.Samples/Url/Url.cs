using System.Collections.Generic;
using System.Text;

namespace PEG.Samples.Url
{
    public class Url
    {
        public string Protocol { get; set; }
        public string Domain { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public List<NameValue> QueryString { get; set; }

        private static PegParser<Url> parser = new PegParser<Url>(UrlGrammar.Create());

        public static Url Parse(string url)
        {
            return parser.Parse(url);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(Protocol + "://" + Domain);
            if (Port != null)
                builder.Append(':' + Port);
            if (Path != null)
                builder.Append(Path);
            if (QueryString != null)
            {
                builder.Append('?');
                for (int i = 0; i < QueryString.Count; i++)
                {
                    var nameValue = QueryString[i];
                    builder.Append(nameValue.Name);
                    builder.Append('=');
                    builder.Append(nameValue.Value);
                    if (i < QueryString.Count - 1)
                        builder.Append('&');
                }
            }
            return builder.ToString();
        }

        public class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
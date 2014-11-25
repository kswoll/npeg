using System;

namespace PEG
{
    public class TokenAttribute : Attribute
    {
        public bool IsOmitted { get; set; }
        public bool TokenizeChildren { get; set; }
    }
}
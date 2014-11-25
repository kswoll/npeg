using System;

namespace PEG.Cst
{
    public class CstString : ICstNode
    {
        private string s;

        public CstString(string s)
        {
            this.s = s;
        }

        public string Coalesce()
        {
            return s;
        }
    }
}
using System;
using PEG.Cst;

namespace PEG
{
    public class StringReplacementTarget : IReplacementTarget
    {
        private ICstNode s;

        public StringReplacementTarget(string s)
        {
            this.s = new CstString(s);
        }

        public ICstNode Replace(CstCache cache)
        {
            return s;
        }
    }
}
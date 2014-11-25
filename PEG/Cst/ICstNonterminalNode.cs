using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG.Cst
{
    public interface ICstNonterminalNode : ICstNode
    {
        int AbsoluteIndex { get; }
        Nonterminal Nonterminal { get; }
        List<ICstNode> Children { get; }        
    }
}
using PEG.SyntaxTree;

namespace PEG.Cst
{
    public interface ICstTerminalNode : ICstNode
    {
        Terminal Terminal { get; }
    }
}
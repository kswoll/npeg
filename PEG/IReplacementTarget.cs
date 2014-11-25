using PEG.Cst;

namespace PEG
{
    public interface IReplacementTarget
    {
        ICstNode Replace(CstCache cache);
    }
}
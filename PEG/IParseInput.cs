using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG
{
    public interface IParseInput : IEnumerable<Terminal>
    {
        Terminal this[int index] { get; }
        int Length { get; }
    }
}
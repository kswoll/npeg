using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PEG.Builder;

namespace PEG.Tests.CppMacro
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Macro
    {
        private const string NullString = "<null>";
        private static readonly PegParser<Macro> parser = new PegParser<Macro>(MacroGrammar.Create());
        public OrModel Or { get; set; }

        public static Macro Parse(string macro)
        {
            return parser.Parse(macro);
        }

        public override string ToString()
        {
            return Or?.ToString() ?? NullString;
        }

        public class AndModel
        {
            [Consume] public List<PrimitiveConditionModel> PrimitiveCondition { get; set; }

            public override string ToString()
            {
                var value = string.Join(" && ", PrimitiveCondition);
                return PrimitiveCondition.Count > 1 ? $"({value})" : value;
            }
        }

        public class OrModel
        {
            [Consume] public List<AndModel> And { get; set; }

            public override string ToString()
            {
                var value = string.Join(" || ", And);
                return And.Count > 1 ? $"({value})" : value;
            }
        }

        public class PrimitiveConditionModel
        {
            public OrModel Or { get; set; }
            public NegateConditionModel NegateCondition { get; set; }
            public DefinedModel Defined { get; set; }
            public string Identifier { get; set; }

            public override string ToString()
            {
                return Identifier ?? Defined?.ToString() ?? NegateCondition?.ToString() ?? Or?.ToString() ?? NullString;
            }
        }

        public class DefinedModel
        {
            public IdentifierWrapModel IdentifierWrap { get; set; }

            public override string ToString()
            {
                return $"defined({IdentifierWrap})";
            }
        }

        public class NegateConditionModel
        {
            public OrModel Or { get; set; }

            public override string ToString()
            {
                return $"!({Or})";
            }
        }

        public class IdentifierWrapModel
        {
            public IdentifierWrapModel IdentifierWrap { get; set; }
            public string Identifier { get; set; }

            public override string ToString()
            {
                return Identifier ?? IdentifierWrap?.ToString() ?? NullString;
            }
        }
    }
}
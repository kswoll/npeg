using System;

namespace PEG.Builder
{
    public enum AstStructure
    {
        RecursiveList
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AstAttribute : Attribute
    {
        public Type Type { get; set; }
        public string Condition { get; set; }
        public AstStructure Structure { get; set; }

        public AstAttribute(string expression)
        {
            Condition = expression;
        }

        public AstAttribute(Type type)
        {
            Type = type;
        }

        public AstAttribute(string condition, Type type)
        {
            Condition = condition;
            Type = type;
        }

        public AstAttribute(Type type, AstStructure structure, string expression)
        {
            Type = type;
            Condition = expression;
            Structure = structure;
        }

        public ConsumeExpression GetExpression()
        {
            return Condition != null ? ConsumeExpressionCache.Get(Condition) : null;
        }
    }
}
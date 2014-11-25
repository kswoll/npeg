using System;

namespace PEG.Builder
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ConsumeAttribute : Attribute
    {
        public string Expression { get; set; }

        public object Value { get; set; }

        public Type Type { get; set; }

        public Type Converter { get; set; }

        public object ConverterArgs { get; set; }

        public int Production { get; set; }

        public ConsumeAttribute()
        {
        }

        public ConsumeAttribute(string expression)
            : this()
        {
            Expression = expression;
        }

        public ConsumeAttribute(string expression, Type valueType)
            : this()
        {
            Expression = expression;
            Type = valueType;
        }

        public ConsumeAttribute(int production)
        {
            Production = production;
        }

        public ConsumeExpression GetExpression()
        {
            return Expression != null ? ConsumeExpressionCache.Get(Expression) : null;
        }
    }
}
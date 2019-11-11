using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PEG.Cst;
using PEG.Extensions;
using PEG.SyntaxTree;
using PEG.Utils;

namespace PEG.Builder
{
    public class PegBuilder<T>
    {
        private Grammar grammar;
        private Dictionary<Type, string> nonterminalNames = new Dictionary<Type, string>();
        private Func<Type, object> factory;
        private DictionaryList<string, TypeRecord> astTypes = new DictionaryList<string, TypeRecord>();
        private DictionaryList<Nonterminal, AstAttribute> astAttributes = new DictionaryList<Nonterminal, AstAttribute>();

        public PegBuilder(Grammar grammar) : this(grammar, true)
        {
        }

        public PegBuilder(Grammar grammar, bool searchSubclasses)
        {
            this.grammar = grammar;

            foreach (var rule in grammar.Nonterminals)
            {
                string name = rule.Name;
                MethodInfo method = grammar.GetType().GetMethod(name, new Type[0]);
                if (method != null)
                {
                    AstAttribute[] asts = grammar.GetType().GetMethod(name, new Type[0]).GetAttributes<AstAttribute>();
                    foreach (AstAttribute ast in asts)
                    {
                        if (ast != null)
                        {
                            astTypes.Add(name, new TypeRecord(ast.GetExpression(), ast.Type));
                            Nonterminal nonterminal = grammar.GetNonterminal(method.Name);
                            astAttributes.Add(nonterminal, ast);
                        }
                    }
                }
            }

            HashSet<Type> types = new HashSet<Type>();
            WalkTypeForNonTerminals(typeof(T), types, searchSubclasses);

            foreach (Type type in types)
            {
                ConsumeAttribute[] nonterminalAttributes = type.GetAttributes<ConsumeAttribute>();
                foreach (var attribute in nonterminalAttributes)
                {
                    //                    nonterminalTypes[attribute.Expression] = type;
                    nonterminalNames[type] = attribute.Expression;
                }
            }
        }

        public Type GetSubclass(ref CstNonterminalNode node, string ruleName)
        {
            foreach (var type in astTypes[ruleName])
            {
                if (type.expression == null || type.expression.Resolve(node).Any())
                    return type.type;
            }
            if (node.Children.Count == 1 && node.Children[0] is CstNonterminalNode)
            {
                CstNonterminalNode childNode = (CstNonterminalNode)node.Children[0];
                Type result = GetSubclass(ref childNode, childNode.Nonterminal.Name);
                node = childNode;
                return result;
            }

            throw new InvalidOperationException("No subclass registered for " + ruleName + ".  Consider adding [Ast(typeof(YourAstNode)] to " + ruleName);
        }

        public T Build(CstNonterminalNode rootNode)
        {
            object astNode = Build(typeof(T), rootNode);
            return (T)astNode;
        }

        public Func<Type, object> Factory
        {
            get { return factory; }
            set { factory = value; }
        }

        class TypeRecord
        {
            public ConsumeExpression expression;
            public Type type;

            public TypeRecord(ConsumeExpression expression, Type type)
            {
                this.expression = expression;
                this.type = type;
            }
        }

        private void WalkTypeForNonTerminals(Type type, HashSet<Type> checkedTypes, bool searchSubclasses)
        {
            if (checkedTypes.Contains(type))
                return;
            checkedTypes.Add(type);

            foreach (PropertyInfo property in type.GetAllPropertiesInAncestry())
            {
                if (property.PropertyType.HasAttribute<ConsumeAttribute>() || (property.PropertyType.IsGenericList() && property.PropertyType.GetListElementType().HasAttribute<ConsumeAttribute>()))
                {
                    if (property.PropertyType.IsGenericList())
                        WalkTypeForNonTerminals(property.PropertyType.GetListElementType(), checkedTypes, searchSubclasses);
                    else
                        WalkTypeForNonTerminals(property.PropertyType, checkedTypes, searchSubclasses);
                }
            }

            // Also check all subclasses
            if (searchSubclasses)
            {
                Type[] subclasses = type.Assembly.GetTypes().Where(o => o != type && type.IsAssignableFrom(o)).ToArray();
                subclasses.Foreach(o => WalkTypeForNonTerminals(o, checkedTypes, searchSubclasses));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="astNodeType">The type of node we are creating.  A subclass might possibly be selected.</param>
        /// <param name="cstNode">The CST node that represents the structured input for the specified AST node</param>
        /// <returns>The new astNode of the specified astNodeType that consumes the cstNode</returns>
        private object Build(Type astNodeType, ICstNode cstNode)
        {
            int consumeIndex = 0;
            if (astNodeType.IsAbstract || astNodeType.IsInterface)
            {
                string ruleName = ((CstNonterminalNode)cstNode).Nonterminal.Name;
                CstNonterminalNode cstNonterminalNode = (CstNonterminalNode)cstNode;
                astNodeType = GetSubclass(ref cstNonterminalNode, ruleName);
                cstNode = cstNonterminalNode;
            }
            object astNode;
            try
            {
                if (astNodeType == typeof(string))
                    return cstNode.Coalesce();
                else if (astNodeType == typeof(int))
                    return int.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(byte))
                    return byte.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(short))
                    return short.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(long))
                    return long.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(float))
                    return float.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(double))
                    return double.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(decimal))
                    return decimal.Parse(cstNode.Coalesce());
                else if (astNodeType == typeof(bool))
                    return bool.Parse(cstNode.Coalesce());
                else
                    astNode = CreateAstNode(astNodeType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error instantiating " + astNodeType.FullName, e);
            }
            foreach (PropertyInfo property in astNode.GetType().GetAllPropertiesInAncestry())
            {
                ConsumeAttribute[] consumeAttributes = property.GetAttributes<ConsumeAttribute>();
                foreach (ConsumeAttribute consumeAttribute in consumeAttributes)
                {
                    if (consumeIndex > 0 && consumeIndex != consumeAttribute.Production)
                        continue;

                    CstNonterminalNode productionNode = cstNode as CstNonterminalNode;

                    // 1) Handle the simple case where the NonTerminal exactly matches the NonTerminal property of our consume attribute
                    // 2) Handle the empty [Consume] case, which means it goes into this block no matter what.
                    // Get the expression (parsed from the NonTerminal property)
                    ConsumeExpression expression = consumeAttribute.GetExpression();

                    // The goal of the expression is to bring us to a new CST node (a descendent of the current one)
                    ICstNode newCstNode;
                    try
                    {
                        newCstNode = expression != null ? expression.Resolve(productionNode).FirstOrDefault() : productionNode;
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException("Error resolving expression '" + consumeAttribute.Expression + "' for " + property.GetPath(), e);
                    }

                    // There are a number of reasons why the new cst node might be null.  It is perfectly
                    // legal to specify nonterminal expressions that only resolve under certain productions.
                    // In other scenarios, it will remain null.
                    if (newCstNode != null)
                    {
                        // If the Value property is set, then we simply assign the property to that value (since newCstNode is not null)
                        if (consumeAttribute.Value != null)
                        {
                            property.SetValue(astNode, consumeAttribute.Value, null);
                        }
                        else if (consumeAttribute.Type != null)
                        {
                            Coalesce(astNode, property, newCstNode, consumeAttribute.Type);
                        }
                        else
                        {
                            MapProperty(property, astNode, newCstNode);
                        }
                        goto nextProperty;
                    }
                }

                var impliedCstNode = ((CstNonterminalNode)cstNode).Children.OfType<ICstNonterminalNode>().SingleOrDefault(o => o.Nonterminal.Name == property.Name);
                if (impliedCstNode != null)
                {
                    MapProperty(property, astNode, impliedCstNode);
                }

            nextProperty: ;
            }
            return astNode;
        }

        private void MapProperty(PropertyInfo property, object astNode, ICstNode newCstNode)
        {
            /*
                                    else if (consumeAttribute.Converter != null)
                                    {
                                        CoalesceAndConvert(astNode, property, newCstNode, consumeAttribute.Converter, consumeAttribute.ConverterArgs);
                                    }
            */
            // If the property type is simple, we will just convert the text of the CST node into that type.
            if (IsSimpleType(property.PropertyType))
            {
                Coalesce(astNode, property, newCstNode, property.PropertyType);
            }
            // Lists are a special case to help unravel recursion.
            else if (property.PropertyType.IsGenericList())
            {
                BuildList(property, astNode, (CstNonterminalNode)newCstNode);
            }
            // Each enum value may contain a ConsumeAttribute indicating how to map it. Otherwise we do
            // a simple parse.
            else if (property.PropertyType.IsEnum)
            {
                var enums = property.PropertyType.GetEnums();
                string expectedName = newCstNode.Coalesce();
                foreach (var @enum in enums)
                {
                    string name = @enum.Name;
                    ConsumeAttribute[] enumConsumeAttributes = @enum.Field.GetAttributes<ConsumeAttribute>();
                    if (enumConsumeAttributes.Length > 0)
                    {
                        foreach (ConsumeAttribute enumConsume in enumConsumeAttributes)
                        {
                            if (enumConsume.Expression == expectedName)
                            {
                                name = enumConsume.Expression;
                                break;
                            }
                        }
                    }
                    if (name == expectedName)
                    {
                        property.SetValue(astNode, @enum.Value, null);
                        goto foundEnum;
                    }
                }
                throw new InvalidOperationException("No enum found for " + expectedName + " in " + property.PropertyType);
            foundEnum:
                ;
            }
            // Otherwise we create a new AST node and recurse into this method.
            else //if (nonterminalTypes.Values.Contains(property.PropertyType))
            {
                object newAstNode = Build(property.PropertyType, newCstNode);

                // Set the property to the new AST node
                property.SetValue(astNode, newAstNode, null);
            }
        }

        private object CreateAstNode(Type astNodeType)
        {
            if (factory != null)
                return factory(astNodeType);
            else
                return Activator.CreateInstance(astNodeType);
        }

        private static readonly HashSet<Type> simpleTypes = new HashSet<Type>(new[] { typeof(string), typeof(bool), typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(Decimal), typeof(DateTime), typeof(TimeSpan) });

        private bool IsSimpleType(Type type)
        {
            return simpleTypes.Contains(type);
        }

        private void Coalesce(object astNode, PropertyInfo property, ICstNode cstNode, Type targetType)
        {
            string value = cstNode.Coalesce();
            object o;
            try
            {
                o = Convert.ChangeType(value, targetType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error converting '" + value + "' to a " + targetType.FullName + " for property " + property.GetPath(), e);
            }
            property.SetValue(astNode, o, null);
        }

/*
        private void CoalesceAndConvert(object astNode, PropertyInfo property, ICstNode cstNode, Type typeConverter, object converterArgs)
        {
            string value = cstNode.Coalesce();
            ITypeConverter converter = (ITypeConverter)Activator.CreateInstance(typeConverter);
            object o = converter.Convert(value, property.PropertyType, converterArgs);
            property.SetValue(astNode, o, null);
        }
*/

        private IEnumerable<ICstNonterminalNode> GetChildrenByAstExpression(ICstNonterminalNode node, ConsumeExpression[] expressions)
        {
            IEnumerable<ICstNonterminalNode> result = new ICstNonterminalNode[0];
            foreach (var exp in expressions)
            {
                result = result.Concat(exp.Resolve(node).Cast<ICstNonterminalNode>());
            }
            return result;
        }

        private static IEnumerable<ICstNonterminalNode> GetChildrenByNonterminal(ICstNonterminalNode node, Nonterminal nonterminal)
        {
            if (nonterminal == null)
                return null;
            
            return node.Children
                .OfType<ICstNonterminalNode>()
                .Where(o => o.Nonterminal.Name == nonterminal.Name);
        }
        
        private static Nonterminal FindNonterminal(Expression current)
        {
            switch (current)
            {
                case null:
                case AndPredicate _:
                case AnyCharacter _:
                case CharacterSet _:
                case EmptyString _:
                case EncloseExpression _:
                case ForeignNonterminal _:
                    return null;
                case Nonterminal nonterminal:
                    return nonterminal;
                case NotPredicate _:
                    return null;
                case OneOrMore oneOrMore:
                    return FindNonterminal(oneOrMore.Operand);
                case Optional _:
                case OrderedChoice _:
                case Repeat _:
                    return null;
                case Sequence sequence:
                    return sequence.Expressions
                        .Select(FindNonterminal)
                        .FirstOrDefault(x => x != null);
                case Substitution _:
                case Terminal _:
                    return null;
                case ZeroOrMore zeroOrMore:
                    return FindNonterminal(zeroOrMore.Operand);
                default:
                    // throw new ArgumentOutOfRangeException(nameof(current));
                    return null;
            }
        }
        
        private void BuildList(PropertyInfo property, object astNode, CstNonterminalNode cstNode)
        {
            ConsumeExpression[] expressions = astAttributes[cstNode.Nonterminal]
                .Select(o => o.GetExpression())
                .ToArray();
            Expression expression = cstNode.Nonterminal.Expression;

            IList list = (IList) Activator.CreateInstance(property.PropertyType);
            Type listType = property.PropertyType.GetListElementType();

            IEnumerable<ICstNonterminalNode> itemNodes = null;

            if (expressions.Length > 0)
            {
                itemNodes = GetChildrenByAstExpression(cstNode, expressions);
            }
            else
            {
                switch (expression)
                {
                    case Sequence _:
                    case ZeroOrMore _:
                    case OneOrMore _:
                    case Optional _:
                    {
                        Nonterminal nonterminal = FindNonterminal(expression);
                        itemNodes = GetChildrenByNonterminal(cstNode, nonterminal);
                        break;
                    }
                }

                if (itemNodes == null)
                {
                    itemNodes = new[] {cstNode};
                }
            }

            foreach (var childNode in itemNodes)
            {
                object listItemAstNode = Build(listType, childNode);
                list.Add(listItemAstNode);
            }

            property.SetValue(astNode, list, null);
        }

        public T Build(IEnumerable<OutputRecord> outputStream)
        {
            CstNonterminalNode cstNode = CstBuilder.Build(outputStream);
            return Build((CstNonterminalNode)cstNode.Children[0]);
        }
    }
}
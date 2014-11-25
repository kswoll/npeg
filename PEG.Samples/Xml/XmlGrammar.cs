using PEG.SyntaxTree;

namespace PEG.Samples.Xml
{
    public class XmlGrammar : Grammar<XmlGrammar>
    {
        private XmlGrammar()
        {
        }

        public virtual Expression Symbol()
        {
            return Element() |
                   Text();
        }

        public virtual Expression Element()
        {
            return ElementStart() + -Symbol() + ElementEnd() |
                   ElementClosed();
        }

        public virtual Expression ElementStart()
        {
            return '<' + ElementHead() + '>';
        }

        public virtual Expression ElementClosed()
        {
            return '<' + ElementHead() + "/>";
        }

        public virtual Expression ElementEnd()
        {
            return "</" + QualifiableName() + '>';
        }

        public virtual Expression Text()
        {
            return !'<'._() + Any;
        }

        public virtual Expression ElementHead()
        {
            return ~WS() + QualifiableName() + Attributes() + ~WS();
        }

        public virtual Expression Attributes()
        {
            return -Attribute();
        }

        public virtual Expression Attribute()
        {
            return ' ' + QualifiableName() + ~WS() + '=' + ~WS() + AttributeValuePart();
        }

        public virtual Expression AttributeValuePart()
        {
            return '"' + DoubleQuoteAttributeValue() + '"' |
                   '\'' + SingleQuoteAttributeValue() + '\'';
        }

        public virtual Expression DoubleQuoteAttributeValue()
        {
            return -DoubleQuoteAttributeValueChar();
        }

        public virtual Expression SingleQuoteAttributeValue()
        {
            return -SingleQuoteAttributeValueChar();
        }

        public virtual Expression DoubleQuoteAttributeValueChar()
        {
            return !('"'._() | '\\') + Any | "\\\"" | "\\\\";
        }

        public virtual Expression SingleQuoteAttributeValueChar()
        {
            return !('\''._() | '\\') + Any | "\\\'" | "\\\\";
        }

        public virtual Expression QualifiableName()
        {
            return ~(Prefix() + ':') + Identifier();
        }

        public virtual Expression Prefix()
        {
            return Identifier();
        }

        public virtual Expression Identifier()
        {
            return IdentifierStartChar() + -IdentifierChar();
        }

        public virtual Expression IdentifierChar()
        {
            return IdentifierStartChar() | '0'.To('9') | '-';
        }

        public virtual Expression IdentifierStartChar()
        {
            return 'a'.To('Z');
        }

        public virtual Expression WS()
        {
            return +(' '._() | '\r' | '\n' | '\t');
        }
    }
}
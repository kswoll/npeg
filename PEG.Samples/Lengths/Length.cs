using System;
using PEG.SyntaxTree;

namespace PEG.Samples.Lengths
{
    public struct Length
    {
        public decimal Inches { get; private set; }

        private static LengthGrammar grammar = LengthGrammar.Create();
        private static PegParser<ParsedLength> parser = new PegParser<ParsedLength>(grammar, grammar.Start());

        public Length(decimal inches) : this()
        {
            Inches = inches;
        }

        public Length(int feet, decimal inches) : this()
        {
            Inches = feet * 12 + inches;
        }

        public decimal Feet
        {
            get { return Inches / 12; }
        }

        /// <summary>
        /// Valid measurement patterns:
        ///
        /// X' Y.Z"  (feet, then inches in decimal)
        /// X'Y.Z"   (feet, then inches in decimal without any space)
        /// X'  Y.Z" (feet, then inches in decimal, multiple spaces)
        /// X.Z'     (feet in decimal)
        /// Y.Z"     (inches in decimal)
        /// X'Y"     (feet, then inches)
        /// Y"       (inches)
        /// </summary>
        public static bool TryParse(string s, out Length result)
        {
            ParsedLength parsedLength;
            int amountRead;
            if (!parser.Parse(s.Trim(), out parsedLength, out amountRead))
            {
                result = default(Length);
                return false;
            }
            result =
                parsedLength.IntegerFeet > 0 ? new Length(parsedLength.IntegerFeet, parsedLength.Inches) :
                parsedLength.DecimalFeet > 0 ? new Length(parsedLength.DecimalFeet * 12) :
                new Length(parsedLength.Inches);
            return true;
        }

        public static Length Parse(string s)
        {
            Length result;
            if (TryParse(s, out result))
                return result;
            else
                throw new Exception("Unable to parse length: " + s);
        }

        public class ParsedLength
        {
            public int IntegerFeet { get; set; }
            public decimal DecimalFeet { get; set; }
            public decimal Inches { get; set; }
        }

        public class LengthGrammar : Grammar<LengthGrammar>
        {
            public virtual Expression Start()
            {
                return
                    IntegerFeet() + OptionalWhitespace() + '\'' + OptionalWhitespace() + Inches() + OptionalWhitespace() + '"' |
                    DecimalFeet() + OptionalWhitespace() + '\'' |
                    Inches() + OptionalWhitespace() + '"';
            }

            public virtual Expression IntegerFeet()
            {
                return Integer();
            }

            public virtual Expression DecimalFeet()
            {
                return Decimal();
            }

            public virtual Expression Inches()
            {
                return Decimal();
            }

            public virtual Expression OptionalWhitespace()
            {
                return -' '._();
            }

            public virtual Expression Integer()
            {
                return +'0'.To('9');
            }

            public virtual Expression Decimal()
            {
                return Integer() + ~('.' + Integer());
            }
        }
    }
}

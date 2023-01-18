using System;
using System.Globalization;

namespace BardMusicPlayer.Script.BasicSharp
{
    public enum ValueType
    {
        Real, // it's double
        String
    }

    public struct Value
    {
        public static readonly Value Zero = new Value(0);
        public ValueType Type { get; set; }

        public double Real { get; set; }
        public string String { get; set; }

        public Value(double real) : this()
        {
            this.Type = ValueType.Real;
            this.Real = real;
        }

        public Value(string str)
            : this()
        {
            this.Type = ValueType.String;
            this.String = str;
        }

        public Value Convert(ValueType type)
        {
            if (Type == type) 
                return this;

            switch (type)
            {
                case ValueType.Real:
                    Real = double.Parse(String);
                    Type = ValueType.Real;
                    break;
                case ValueType.String:
                    String = Real.ToString(CultureInfo.InvariantCulture);
                    Type = ValueType.String;
                    break;
            }
            return this;
        }

        public readonly Value UnaryOp(Token tok)
        {
            if (Type != ValueType.Real)
            {
                throw new Exception("Can only do unary operations on numbers.");
            }

            switch (tok)
            {
                case Token.Plus: return this;
                case Token.Minus: return new Value(-Real);
                case Token.Not: return new Value(Real == 0 ? 1 : 0);
            }

            throw new Exception("Unknown unary operator.");
        }

        public readonly Value BinOp(Value b, Token tok)
        {
            Value a = this;
            if (a.Type != b.Type)
            {
				// promote one value to higher type
                if (a.Type > b.Type)
                    b = b.Convert(a.Type);
                else
                    a = a.Convert(b.Type);
            }

            if (tok == Token.Plus)
            {
                if (a.Type == ValueType.Real)
                    return new Value(a.Real + b.Real);
                else
                    return new Value(a.String + b.String);
            }
            else if(tok == Token.Equal)
            {
                 if (a.Type == ValueType.Real)
                     return new Value(a.Real == b.Real ? 1 : 0);
                else
                     return new Value(a.String == b.String ? 1 : 0);
            }
            else if(tok == Token.NotEqual)
            {
                 if (a.Type == ValueType.Real)
                     return new Value(a.Real == b.Real ? 0 : 1);
                else
                     return new Value(a.String == b.String ? 0 : 1);
            }
            else
            {
                if (a.Type == ValueType.String)
                    throw new Exception("Cannot do binop on strings(except +).");

                switch (tok)
                {
                    case Token.Minus: return new Value(a.Real - b.Real);
                    case Token.Asterisk: return new Value(a.Real * b.Real);
                    case Token.Slash: return new Value(a.Real / b.Real);
                    case Token.Caret: return new Value(Math.Pow(a.Real, b.Real));
                    case Token.Less: return new Value(a.Real < b.Real ? 1 : 0);
                    case Token.More: return new Value(a.Real > b.Real ? 1 : 0);
                    case Token.LessEqual: return new Value(a.Real <= b.Real ? 1 : 0);
                    case Token.MoreEqual: return new Value(a.Real >= b.Real ? 1 : 0);
                    case Token.And: return new Value((a.Real != 0) && (b.Real != 0) ? 1 : 0);
                    case Token.Or: return new Value((a.Real != 0) || (b.Real != 0) ? 1 : 0);
                }
            }

            throw new Exception("Unknown binary operator.");
        }

        public readonly override string ToString()
        {
            if (this.Type == ValueType.Real)
                return this.Real.ToString();
            return this.String;
        }
    }
}

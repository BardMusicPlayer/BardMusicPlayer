#region

using System;
using System.Globalization;

#endregion

namespace BardMusicPlayer.Script.BasicSharp;

public enum ValueType
{
    Real, // it's double
    String
}

public struct Value
{
    public static readonly Value Zero = new(0);
    public ValueType Type { get; set; }

    public double Real { get; set; }
    public string String { get; set; }

    public Value(double real) : this()
    {
        Type = ValueType.Real;
        Real = real;
    }

    public Value(string str)
        : this()
    {
        Type = ValueType.String;
        String = str;
    }

    public Value Convert(ValueType type)
    {
        if (Type == type) return this;

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
        if (Type != ValueType.Real) throw new Exception("Can only do unary operations on numbers.");

        return tok switch
        {
            Token.Plus => this,
            Token.Minus => new Value(-Real),
            Token.Not => new Value(Real == 0 ? 1 : 0),
            _ => throw new Exception("Unknown unary operator.")
        };
    }

    public readonly Value BinOp(Value b, Token tok)
    {
        var a = this;
        if (a.Type != b.Type)
        {
            // promote one value to higher type
            if (a.Type > b.Type)
                b = b.Convert(a.Type);
            else
                a = a.Convert(b.Type);
        }

        switch (tok)
        {
            case Token.Plus:
                return a.Type == ValueType.Real ? new Value(a.Real + b.Real) : new Value(a.String + b.String);
            case Token.Equal when a.Type == ValueType.Real:
                return new Value(a.Real == b.Real ? 1 : 0);
            case Token.Equal:
                return new Value(a.String == b.String ? 1 : 0);
            case Token.NotEqual when a.Type == ValueType.Real:
                return new Value(a.Real == b.Real ? 0 : 1);
            case Token.NotEqual:
                return new Value(a.String == b.String ? 0 : 1);
        }

        if (a.Type == ValueType.String)
            throw new Exception("Cannot do binop on strings(except +).");

        return tok switch
        {
            Token.Minus => new Value(a.Real - b.Real),
            Token.Asterisk => new Value(a.Real * b.Real),
            Token.Slash => new Value(a.Real / b.Real),
            Token.Caret => new Value(Math.Pow(a.Real, b.Real)),
            Token.Less => new Value(a.Real < b.Real ? 1 : 0),
            Token.More => new Value(a.Real > b.Real ? 1 : 0),
            Token.LessEqual => new Value(a.Real <= b.Real ? 1 : 0),
            Token.MoreEqual => new Value(a.Real >= b.Real ? 1 : 0),
            Token.And => new Value(a.Real != 0 && b.Real != 0 ? 1 : 0),
            Token.Or => new Value(a.Real != 0 || b.Real != 0 ? 1 : 0),
            _ => throw new Exception("Unknown binary operator.")
        };
    }

    public readonly override string ToString()
    {
        return Type == ValueType.Real ? Real.ToString(CultureInfo.InvariantCulture) : String;
    }
}
/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Globalization;

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
        Type   = ValueType.String;
        String = str;
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
                Type   = ValueType.String;
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

        return tok switch
        {
            Token.Plus  => this,
            Token.Minus => new Value(-Real),
            Token.Not   => new Value(Real == 0 ? 1 : 0),
            _           => throw new Exception("Unknown unary operator.")
        };
    }

    public readonly Value BinOp(Value b, Token tok)
    {
        var a = this;
        const float tolerance = 0.0001f; // define a small tolerance value
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
            case Token.Equal:
                return a.Type == ValueType.Real ? new Value(Math.Abs(a.Real - b.Real) < tolerance ? 1 : 0) : new Value(a.String == b.String ? 1 : 0);
            case Token.NotEqual:
                return a.Type == ValueType.Real ? new Value(Math.Abs(a.Real - b.Real) < tolerance ? 0 : 1) : new Value(a.String == b.String ? 0 : 1);
            default:
            {
                if (a.Type == ValueType.String)
                    throw new Exception("Cannot do binop on strings(except +).");

                switch (tok)
                {
                    case Token.Minus:     return new Value(a.Real - b.Real);
                    case Token.Asterisk:  return new Value(a.Real * b.Real);
                    case Token.Slash:     return new Value(a.Real / b.Real);
                    case Token.Caret:     return new Value(Math.Pow(a.Real, b.Real));
                    case Token.Less:      return new Value(a.Real < b.Real ? 1 : 0);
                    case Token.More:      return new Value(a.Real > b.Real ? 1 : 0);
                    case Token.LessEqual: return new Value(a.Real <= b.Real ? 1 : 0);
                    case Token.MoreEqual: return new Value(a.Real >= b.Real ? 1 : 0);
                    case Token.And:       return new Value(a.Real != 0 && b.Real != 0 ? 1 : 0);
                    case Token.Or:        return new Value(a.Real != 0 || b.Real != 0 ? 1 : 0);
                }

                break;
            }
        }

        throw new Exception("Unknown binary operator.");
    }

    public override readonly string ToString()
    {
        return Type == ValueType.Real ? Real.ToString(CultureInfo.InvariantCulture) : String;
    }
}
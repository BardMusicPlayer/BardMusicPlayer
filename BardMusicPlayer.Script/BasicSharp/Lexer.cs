#region

using System;
using System.Globalization;

#endregion

namespace BardMusicPlayer.Script.BasicSharp;

public sealed class Lexer
{
    private readonly string source;
    private char lastChar;
    private Marker sourceMarker; // current position in source string

    public Lexer(string input)
    {
        source = input;
        sourceMarker = new Marker(0, 1, 1);
        lastChar = source[0];
    }

    public Marker TokenMarker { get; set; }

    public string Identifier { get; set; } // Last encountered identifier
    public Value Value { get; set; } // Last number or string

    public void GoTo(Marker marker)
    {
        sourceMarker = marker;
    }

    public string GetLine(Marker marker)
    {
        var oldMarker = sourceMarker;
        marker.Pointer--;
        GoTo(marker);

        var line = "";
        do
        {
            line += GetChar();
        } while (lastChar != '\n' && lastChar != (char)0);

        line.Remove(line.Length - 1);

        GoTo(oldMarker);

        return line;
    }

    private char GetChar()
    {
        sourceMarker.Column++;
        sourceMarker.Pointer++;

        if (sourceMarker.Pointer >= source.Length)
            return lastChar = (char)0;

        if ((lastChar = source[sourceMarker.Pointer]) != '\n') return lastChar;

        sourceMarker.Column = 1;
        sourceMarker.Line++;

        return lastChar;
    }

    public Token GetToken()
    {
        while (true)
        {
            // skip white chars
            while (lastChar is ' ' or '\t' or '\r') GetChar();

            TokenMarker = sourceMarker;

            if (char.IsLetter(lastChar))
            {
                Identifier = lastChar.ToString();
                while (char.IsLetterOrDigit(GetChar())) Identifier += lastChar;

                switch (Identifier.ToUpper())
                {
                    case "PRINT":
                        return Token.Print;
                    case "MACRO":
                        return Token.Macro;
                    case "CPRINT":
                        return Token.CPrint;
                    case "IF":
                        return Token.If;
                    case "ENDIF":
                        return Token.EndIf;
                    case "THEN":
                        return Token.Then;
                    case "ELSE":
                        return Token.Else;
                    case "FOR":
                        return Token.For;
                    case "TO":
                        return Token.To;
                    case "STEP":
                        return Token.Step;
                    case "NEXT":
                        return Token.Next;
                    case "GOTO":
                        return Token.Goto;
                    case "INPUT":
                        return Token.Input;
                    case "LET":
                        return Token.Let;
                    case "GOSUB":
                        return Token.Gosub;
                    case "RETURN":
                        return Token.Return;
                    case "END":
                        return Token.End;
                    case "OR":
                        return Token.Or;
                    case "AND":
                        return Token.And;
                    case "NOT":
                        return Token.Not;
                    case "ASSERT":
                        return Token.Assert;
                    case "SELECT":
                        return Token.Select;
                    case "UNSELECT":
                        return Token.UnSelect;
                    case "SLEEP":
                        return Token.Sleep;
                    case "TAPKEY":
                        return Token.TapKey;
                    case "REM":
                        while (lastChar != '\n') GetChar();
                        GetChar();
                        continue;
                    default:
                        return Token.Identifier;
                }
            }

            if (char.IsDigit(lastChar))
            {
                var num = "";
                do
                {
                    num += lastChar;
                } while (char.IsDigit(GetChar()) || lastChar == '.');

                if (!double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var real))
                    throw new Exception("ERROR while parsing number");

                Value = new Value(real);
                return Token.Value;
            }

            var tok = Token.Unknown;
            switch (lastChar)
            {
                case '\n':
                    tok = Token.NewLine;
                    break;
                case ':':
                    tok = Token.Colon;
                    break;
                case ';':
                    tok = Token.Semicolon;
                    break;
                case ',':
                    tok = Token.Comma;
                    break;
                case '=':
                    tok = Token.Equal;
                    break;
                case '+':
                    tok = Token.Plus;
                    break;
                case '-':
                    tok = Token.Minus;
                    break;
                case '/':
                    tok = Token.Slash;
                    break;
                case '*':
                    tok = Token.Asterisk;
                    break;
                case '^':
                    tok = Token.Caret;
                    break;
                case '(':
                    tok = Token.LParen;
                    break;
                case ')':
                    tok = Token.RParen;
                    break;
                case '\'':
                    // skip comment until new line
                    while (lastChar != '\n') GetChar();
                    GetChar();
                    continue;
                case '<':
                    GetChar();
                    switch (lastChar)
                    {
                        case '>':
                            tok = Token.NotEqual;
                            break;
                        case '=':
                            tok = Token.LessEqual;
                            break;
                        default:
                            return Token.Less;
                    }

                    break;
                case '>':
                    GetChar();
                    if (lastChar == '=')
                        tok = Token.MoreEqual;
                    else
                        return Token.More;

                    break;
                case '"':
                    var str = "";
                    while (GetChar() != '"')
                        if (lastChar == '\\')
                            // parse \n, \t, \\, \"
                            switch (char.ToLower(GetChar()))
                            {
                                case 'n':
                                    str += '\n';
                                    break;
                                case 't':
                                    str += '\t';
                                    break;
                                case '\\':
                                    str += '\\';
                                    break;
                                case '"':
                                    str += '"';
                                    break;
                            }
                        else
                            str += lastChar;

                    Value = new Value(str);
                    tok = Token.Value;
                    break;
                case (char)0:
                    return Token.EOF;
            }

            GetChar();
            return tok;
        }
    }
}
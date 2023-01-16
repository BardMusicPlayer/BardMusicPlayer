#region

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Structs;

#endregion

namespace BardMusicPlayer.Script.BasicSharp;

public sealed class Interpreter
{
    public delegate Value BasicFunction(Interpreter interpreter, List<Value> args);

    public delegate void CPrintFunction(string text);

    public delegate string InputFunction();

    public delegate void PrintFunction(ChatMessageChannelType type, string text);

    public delegate void SelectedBard(int num);

    public delegate void SelectedBardAsString(string name);

    public delegate void TapKeyFunction(string modifier, string character);

    public delegate void UnSelectBard(string name);

    private readonly Dictionary<string, BasicFunction> funcs; // all maped functions

    private readonly int ifcounter; // counter used for matching "if" with "else"
    private readonly Dictionary<string, Marker> labels; // already seen labels

    private readonly Lexer lex;
    private readonly Dictionary<string, Marker> loops; // for loops

    private readonly Dictionary<string, Value> loopsteps; // for loops steps

    private readonly Dictionary<string, Value> vars; // all variables are stored here
    public CPrintFunction cprintHandler;

    private bool exit; // do we need to exit?

    public InputFunction inputHandler;
    private Token lastToken; // last seen token

    private Marker lineMarker; // current line marker
    private Token prevToken; // token before last one

    public PrintFunction printHandler;
    public SelectedBardAsString selectedBardAsStringHandler;
    public SelectedBard selectedBardHandler;
    public TapKeyFunction tapKeyHandler;
    public UnSelectBard unselectBardHandler;

    public Interpreter(string input)
    {
        lex = new Lexer(input);
        vars = new Dictionary<string, Value>();
        labels = new Dictionary<string, Marker>();
        loops = new Dictionary<string, Marker>();
        loopsteps = new Dictionary<string, Value>();
        funcs = new Dictionary<string, BasicFunction>();
        ifcounter = 0;
        BuiltIns.InstallAll(this); // map all builtins functions
    }

    public void StopExec()
    {
        exit = true;
    }

    public Value GetVar(string name)
    {
        if (!vars.ContainsKey(name))
            throw new BasicException("Variable with name " + name + " does not exist.", lineMarker.Line);

        return vars[name];
    }

    public void SetVar(string name, Value val)
    {
        if (!vars.ContainsKey(name)) vars.Add(name, val);
        else vars[name] = val;
    }

    public string GetLine()
    {
        return lex.GetLine(lineMarker);
    }

    public void AddFunction(string name, BasicFunction function)
    {
        if (!funcs.ContainsKey(name)) funcs.Add(name, function);
        else funcs[name] = function;
    }

    private void Error(string text)
    {
        throw new BasicException(text, lineMarker.Line);
    }

    private void Match(Token tok)
    {
        // check if current token is what we expect it to be
        if (lastToken != tok)
            Error("Expect " + tok + " got " + lastToken);
    }

    public void Exec()
    {
        exit = false;
        GetNextToken();
        while (!exit) Line(); // do all lines
    }

    private Token GetNextToken()
    {
        prevToken = lastToken;
        lastToken = lex.GetToken();

        if (lastToken == Token.EOF && prevToken == Token.EOF)
            Error("Unexpected end of file");

        return lastToken;
    }

    private void Line()
    {
        // skip empty new lines
        while (lastToken == Token.NewLine) GetNextToken();

        if (lastToken == Token.EOF)
        {
            exit = true;
            return;
        }

        lineMarker = lex.TokenMarker; // save current line marker
        Statment(); // evaluate statment

        if (lastToken != Token.NewLine && lastToken != Token.EOF)
            Error("Expect new line got " + lastToken);
    }

    private void Statment()
    {
        while (true)
        {
            var keyword = lastToken;
            GetNextToken();
            switch (keyword)
            {
                case Token.Print:
                    Print();
                    break;
                case Token.Macro:
                    Macro();
                    break;
                case Token.CPrint:
                    CPrint();
                    break;
                case Token.Input:
                    Input();
                    break;
                case Token.Goto:
                    Goto();
                    break;
                case Token.If:
                    If();
                    break;
                case Token.Else:
                    Else();
                    break;
                case Token.EndIf:
                    break;
                case Token.For:
                    For();
                    break;
                case Token.Next:
                    Next();
                    break;
                case Token.Let:
                    Let();
                    break;
                case Token.End:
                    End();
                    break;
                case Token.Assert:
                    Assert();
                    break;
                case Token.Select:
                    Select();
                    break;
                case Token.UnSelect:
                    UnSelect();
                    break;
                case Token.Sleep:
                    Sleep();
                    break;
                case Token.TapKey:
                    TapKey();
                    break;
                case Token.Identifier:
                    if (lastToken == Token.Equal)
                        Let();
                    else if (lastToken == Token.Colon)
                        Label();
                    else
                        goto default;
                    break;
                case Token.EOF:
                    exit = true;
                    break;
                default:
                    Error("Expect keyword got " + keyword);
                    break;
            }

            if (lastToken != Token.Colon) return;
            // we can execute more statments in single line if we use ";"
            GetNextToken();
        }
    }

    private void TapKey()
    {
        var t = Expr().ToString();
        GetNextToken();
        var p = Expr().ToString();
        tapKeyHandler?.Invoke(t, p);
    }

    private void Print()
    {
        printHandler?.Invoke(ChatMessageChannelType.Say, Expr().ToString());
    }

    private void Macro()
    {
        printHandler?.Invoke(ChatMessageChannelType.None, "/" + Expr());
    }

    private void CPrint()
    {
        cprintHandler?.Invoke(Expr().ToString());
    }

    private void Input()
    {
        while (true)
        {
            Match(Token.Identifier);

            if (!vars.ContainsKey(lex.Identifier)) vars.Add(lex.Identifier, new Value());

            var input = inputHandler?.Invoke();
            // try to parse as double, if failed read value as string
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                vars[lex.Identifier] = new Value(d);
            else
                vars[lex.Identifier] = new Value(input);

            GetNextToken();
            if (lastToken != Token.Comma) break;

            GetNextToken();
        }
    }

    private void Goto()
    {
        Match(Token.Identifier);
        var name = lex.Identifier;

        if (!labels.ContainsKey(name))
            // if we didn't encaunter required label yet, start to search for it
            while (true)
            {
                if (GetNextToken() == Token.Colon && prevToken == Token.Identifier)
                {
                    if (!labels.ContainsKey(lex.Identifier))
                        labels.Add(lex.Identifier, lex.TokenMarker);
                    if (lex.Identifier == name)
                        break;
                }

                if (lastToken == Token.EOF) Error("Cannot find label named " + name);
            }

        lex.GoTo(labels[name]);
        lastToken = Token.NewLine;
    }

    private void If()
    {
        // check if argument is equal to 0
        var result = Expr().BinOp(new Value(0), Token.Equal).Real == 1;

        Match(Token.Then);
        GetNextToken();

        if (!result) return;
        // in case "if" evaulate to zero skip to matching else or endif
        var i = ifcounter;
        while (true)
        {
            switch (lastToken)
            {
                case Token.If:
                    i++;
                    break;
                case Token.Else when i == ifcounter:
                    GetNextToken();
                    return;
                case Token.EndIf when i == ifcounter:
                    GetNextToken();
                    return;
                case Token.EndIf:
                    i--;
                    break;
            }

            GetNextToken();
        }
    }

    private void Else()
    {
        // skip to matching endif
        var i = ifcounter;
        while (true)
        {
            switch (lastToken)
            {
                case Token.If:
                    i++;
                    break;
                case Token.EndIf when i == ifcounter:
                    GetNextToken();
                    return;
                case Token.EndIf:
                    i--;
                    break;
            }

            GetNextToken();
        }
    }

    private void Label()
    {
        var name = lex.Identifier;
        if (!labels.ContainsKey(name)) labels.Add(name, lex.TokenMarker);

        GetNextToken();
        Match(Token.NewLine);
    }

    private void End()
    {
        exit = true;
    }

    private void Let()
    {
        if (lastToken != Token.Equal)
        {
            Match(Token.Identifier);
            GetNextToken();
            Match(Token.Equal);
        }

        var id = lex.Identifier;

        GetNextToken();

        SetVar(id, Expr());
    }

    private void For()
    {
        Match(Token.Identifier);
        var var = lex.Identifier;

        GetNextToken();
        Match(Token.Equal);

        GetNextToken();
        var v = Expr();

        // save for loop marker
        if (loops.ContainsKey(var))
        {
            loops[var] = lineMarker;
        }
        else
        {
            SetVar(var, v);
            loops.Add(var, lineMarker);
        }

        Match(Token.To);

        GetNextToken();
        v = Expr();

        if (lastToken == Token.Step)
        {
            GetNextToken();
            var step = Expr();
            if (step.Type == ValueType.Real)
            {
                if (loopsteps.ContainsKey(var))
                    loopsteps[var] = step;
                else
                    loopsteps.Add(var, step);
            }
        }

        if (vars[var].BinOp(v, Token.More).Real != 1) return;

        while (true)
        {
            while (!(GetNextToken() == Token.Identifier && prevToken == Token.Next))
            {
            }

            if (lex.Identifier != var) continue;

            loops.Remove(var);
            loopsteps.Remove(var);
            GetNextToken();
            Match(Token.NewLine);
            break;
        }
    }

    private void Next()
    {
        // jump to begining of the "for" loop
        Match(Token.Identifier);
        var var = lex.Identifier;

        //check if the loop has a stepping
        if (loopsteps.ContainsKey(var))
            vars[var] = vars[var].BinOp(loopsteps[var], Token.Plus);
        else
            vars[var] = vars[var].BinOp(new Value(1), Token.Plus);

        lex.GoTo(new Marker(loops[var].Pointer - 1, loops[var].Line, loops[var].Column - 1));
        lastToken = Token.NewLine;
    }

    private void Assert()
    {
        var result = Expr().BinOp(new Value(0), Token.Equal).Real == 1;

        if (result) Error("Assertion fault"); // if out assert evaluate to false, throw error with souce code line
    }

    private void Select()
    {
        var v = Expr();
        if (v.Type == ValueType.Real)
            selectedBardHandler?.Invoke((int)v.Real);
        else
            selectedBardAsStringHandler?.Invoke(v.ToString());
    }

    private void UnSelect()
    {
        var v = Expr();
        if (v.Type == ValueType.String)
            unselectBardHandler?.Invoke(v.ToString());
    }

    private void Sleep()
    {
        var v = Expr();
        if (v.Type != ValueType.Real) return;

        var sleeptime = (int)v.Real;
        Task.Delay(sleeptime).Wait();
    }

    private Value Expr(int min = 0)
    {
        // originally we were using shunting-yard algorithm, but now we parse it recursively
        var precedens = new Dictionary<Token, int>
        {
            { Token.Or, 0 }, { Token.And, 0 },
            { Token.Equal, 1 }, { Token.NotEqual, 1 },
            { Token.Less, 1 }, { Token.More, 1 },
            { Token.LessEqual, 1 }, { Token.MoreEqual, 1 },
            { Token.Plus, 2 }, { Token.Minus, 2 },
            { Token.Asterisk, 3 }, { Token.Slash, 3 },
            { Token.Caret, 4 }
        };

        var lhs = Primary();

        while (true)
        {
            if (lastToken is < Token.Plus or > Token.And || precedens[lastToken] < min)
                break;

            var op = lastToken;
            var prec = precedens[lastToken]; // Operator Precedence
            GetNextToken();
            var rhs = Expr(prec);
            lhs = lhs.BinOp(rhs, op);
        }

        return lhs;
    }

    private Value Primary()
    {
        var prim = Value.Zero;

        switch (lastToken)
        {
            case Token.Value:
                // number | string
                prim = lex.Value;
                GetNextToken();
                break;
            case Token.Identifier:
            {
                // ident | ident '(' args ')'
                if (vars.ContainsKey(lex.Identifier))
                {
                    prim = vars[lex.Identifier];
                }
                else if (funcs.ContainsKey(lex.Identifier))
                {
                    var name = lex.Identifier;
                    var args = new List<Value>();
                    GetNextToken();
                    Match(Token.LParen);

                    start:
                    if (GetNextToken() != Token.RParen)
                    {
                        args.Add(Expr());
                        if (lastToken == Token.Comma)
                            goto start;
                    }

                    prim = funcs[name](null, args);
                }
                else
                {
                    Error("Undeclared variable " + lex.Identifier);
                }

                GetNextToken();
                break;
            }
            case Token.LParen:
                // '(' expr ')'
                GetNextToken();
                prim = Expr();
                Match(Token.RParen);
                GetNextToken();
                break;
            case Token.Plus:
            case Token.Minus:
            case Token.Not:
            {
                // unary operator
                // '-' | '+' primary
                var op = lastToken;
                GetNextToken();
                prim = Primary().UnaryOp(op);
                break;
            }
            default:
                Error("Unexpected token in primary!");
                break;
        }

        return prim;
    }
}
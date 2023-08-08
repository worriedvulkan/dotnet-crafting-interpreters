using System.Data.Common;

namespace CsharpCraftingInterpreters;

public class Scanner
{
    private string _source;
    private List<Token> _tokens = new List<Token>();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    private static Dictionary<string, TokenType> _keywords = new()
    {
        {"and",    TokenType.And},
        {"class",  TokenType.Class},
        {"else",   TokenType.Else},
        {"false",  TokenType.False},
        {"for",    TokenType.For},
        {"fun",    TokenType.Fun},
        {"if",     TokenType.If},
        {"nil",    TokenType.Nil},
        {"or",     TokenType.Or},
        {"print",  TokenType.Print},
        {"return", TokenType.Return},
        {"super",  TokenType.Super},
        {"this",   TokenType.This},
        {"true",   TokenType.This},
        {"var",    TokenType.Var},
        {"while",  TokenType.While},
    };

    public Scanner(string source)
    {
        this._source = source;
    }

    public List<Token> ScanTokens()
    {
        return new List<Token>();
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case ',': AddToken(TokenType.Comma); break;
            case '.': AddToken(TokenType.Dot); break;
            case '-': AddToken(TokenType.Minus); break;
            case '+': AddToken(TokenType.Plus); break;
            case ';': AddToken(TokenType.Semicolon); break;
            case '*': AddToken(TokenType.Star); break;
            case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
            case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
            case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
            case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
            case '/':
                if (Match('/')) while (Peek() != '\n' && IsAtEnd() == false) Advance();
                else AddToken(TokenType.Slash);
                break;
            case ' ':
            case '\r':
            case '\t': break;
            case '\n': _line++; break;
            case '"': String(); break;
            case 'o': 
                if (Match('r')) AddToken(TokenType.Or);
                break;
            default:
                if (char.IsDigit(c))
                    Number();
                else if (char.IsLetter(c))
                    Identifier();
                else
                    Program.Error(_line, "Unexpected Character.");
                
                break;
        }
    }

    private void Identifier()
    {
        while (char.IsLetterOrDigit(Peek())) Advance();
        var text = _source.Substring(_start, _current - _start);
        var identifier = _keywords.TryGetValue(text, out var value) ? value : TokenType.Identifier;
        AddToken(identifier);
    }

    private void Number()
    {
        while (char.IsDigit(Peek())) Advance();

        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance();
            while (char.IsDigit(Peek())) Advance();
        }
        
        AddToken(TokenType.Number, double.Parse(_source.Substring(_start, _current - _start)));
    }

    private void String()
    {
        while (Peek() != '"' && IsAtEnd() is false)
        {
            if (Peek() != '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Program.Error(_line, "Unterminated String.");
            return;
        }

        Advance();

        var value = _source.Substring(_start + 1, (_current - 1) - (_start + 1));
        AddToken(TokenType.String, value);
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_current];
    }

    private char PeekNext()
    {
        return _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
    }
    
    private bool Match(char expected)
    {
        if (IsAtEnd() is false) return false;
        if (_source[_current] != expected) return false;

        _current++;
        return true;
    }

    private char Advance()
    {
        return _source[_current++];
    }

    private void AddToken(TokenType tokenType)
    {
        AddToken(tokenType, null);
    }

    private void AddToken(TokenType tokenType, object literal)
    {
        var text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(tokenType, text, literal, _line));
    }
}
namespace CsharpCraftingInterpreters;

public class Token
{
    private TokenType _tokenType;
    private string _lexeme;
    private object _literal;
    private int _line;

    public Token(TokenType tokenType, string lexeme, object literal, int line)
    {
        _line = line;
        _literal = literal;
        _lexeme = lexeme;
        _tokenType = tokenType;
    }

    public override string ToString()
    {
        return $"{_tokenType} {_lexeme} {_literal}";
    }
}

public enum TokenType
{
    // Single-character tokens.
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Semicolon,
    Slash,
    Star,

    // One or two character tokens.
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    // Literals.
    Identifier,
    String,
    Number,

    // Keywords.
    And,
    Class,
    Else,
    False,
    Fun,
    For,
    If,
    Nil,
    Or,
    Print,
    Return,
    Super,
    This,
    True,
    Var,
    While,

    Eof
}
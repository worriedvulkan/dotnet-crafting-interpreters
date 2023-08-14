namespace CsharpCraftingInterpreters;

public class Token
{
    public TokenType TokenType;
    public string Lexeme;
    public object Literal;
    public int Line;

    public Token(TokenType tokenType, string lexeme, object literal, int line)
    {
        Line = line;
        Literal = literal;
        Lexeme = lexeme;
        TokenType = tokenType;
    }

    public override string ToString()
    {
        return $"{TokenType} {Lexeme} {Literal}";
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

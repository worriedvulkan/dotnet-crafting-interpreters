namespace CsharpCraftingInterpreters;

public class Parser 
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens) 
    {
        _tokens = tokens;
    }

    private Expr Expression() 
    {
        return Equality();
    }

    private Expr Equality() 
    {
        var expr = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            var op = Previous();
            var right = Comparison();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var t in types)
        {
            if (Check(t))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private bool Check(TokenType t) 
    {
        if (IsAtEnd()) return false;
        return Peek().TokenType == t;
    }

    private Token Advance()
    {
        if (IsAtEnd() == false) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().TokenType == TokenType.Eof;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }
    
    private Expr Comparison()
    {
        var expr = Term();
        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var op= Previous();
            var right = Term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();
        while(Match(TokenType.Minus, TokenType.Plus))
        {
            var op= Previous();
            var right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();
        while(Match(TokenType.Slash, TokenType.Star))
        {
            var op= Previous();
            var right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            var op= Previous();
            var right = Unary();
            return new Expr.Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.False)) return new Expr.Literal(false);
        if (Match(TokenType.True)) return new Expr.Literal(true);
        if (Match(TokenType.Nil)) return new Expr.Literal(null);

        if (Match(TokenType.Number, TokenType.String)) return new Expr.Literal(Previous());
        
        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression");
    }

    private Token Consume(TokenType tokenType, string message)
    {
        if (Check(tokenType)) return Advance();
        throw Error(Peek(), message);
    }

    private void Synchronize()
    {
        Advance();
        while (IsAtEnd() == false)
        {
            if (Previous().TokenType == TokenType.Semicolon) return;

            switch (Peek().TokenType)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return: return;
            }

            Advance();
        }
    }

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            return null;
        }
    }

    private ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    private class ParseError : Exception { }
}

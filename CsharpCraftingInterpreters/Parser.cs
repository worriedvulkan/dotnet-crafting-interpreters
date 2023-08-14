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
        return Assignment();
    }

    private Expr Assignment()
    {
        var expr = Equality();

        if (Match(TokenType.Equal))
        {
            var equals = Previous();
            var value = Assignment();

            if (expr is Expr.Variable varExpr)
            {
                var name = varExpr.Name;
                return new Expr.Assign(name, value);
            }
            
            Program.Error(equals, "Invalid assignment target.");
        }

        return expr;
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

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.Identifier))
        {
            return new Expr.Variable(Previous());
        }
        
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

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();
        while (IsAtEnd() is false)
        {
            statements.Add(Statement());
        }
        return statements;
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(TokenType.Var)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt Statement()
    {
        if (Match(TokenType.Print)) return PrintStatement();
        if (Match(TokenType.LeftBrace)) return new Stmt.Block(Block());
        return ExpressionStatement();
    }

    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();
        while (Check(TokenType.RightBrace) is false && IsAtEnd() is false)
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RightBrace, "Expect '}' after block");
        return statements;
    }

    private Stmt PrintStatement() 
    {
        var value = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after value");
        return new Stmt.Print(value);
    }

    private Stmt VarDeclaration()
    {
        var name = Consume(TokenType.Identifier, "Expect variable name");
        Expr initializer = null;
        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }

        Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
        return new Stmt.Var(name, initializer);
    }

    private Stmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after expression");
        return new Stmt.Expression(expr);
    }

    private ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    private class ParseError : Exception { }
}

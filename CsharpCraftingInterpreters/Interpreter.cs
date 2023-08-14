namespace CsharpCraftingInterpreters;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private Env _env = new();

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        _env.Assign(expr.Name, value);
        return value;
    }
    
    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Token.TokenType)
        {
            case TokenType.Minus: return (double)left - (double)right;
            case TokenType.Slash: return (double)left / (double)right; 
            case TokenType.Star: return (double)left * (double)right;
            case TokenType.Plus:
                switch (left)
                {
                    case double ld when right is double rd:
                        return ld + rd;
                    case string ls when right is string rs:
                        return ls + rs;
                }
                break;
            case TokenType.Greater: 
                CheckNumberOperands(expr.Token, left, right);
                return (double)left > (double)right;
            case TokenType.GreaterEqual: 
                CheckNumberOperands(expr.Token, left, right);
                return (double)left >= (double)right;
            case TokenType.Less: 
                CheckNumberOperands(expr.Token, left, right);
                return (double)left < (double)right;
            case TokenType.LessEqual: 
                CheckNumberOperands(expr.Token, left, right);
                return (double)left <= (double)right;
            case TokenType.BangEqual: 
                CheckNumberOperands(expr.Token, left, right);
                return IsEqual(left, right) is false;
            case TokenType.EqualEqual: 
                CheckNumberOperands(expr.Token, left, right);
                return IsEqual(left, right);
        }

        return null;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);
        switch (expr.Operator.TokenType) 
        {
            case TokenType.Bang: return IsTruthy(right) == false;
            case TokenType.Minus:

                return -(double)right;
            default: return null;
        }
    }
    
    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return _env.Get(expr.Name);
    }
    
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError err)
        {
            Program.RuntimeError(err);
        }
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private string Stringify(object? obj)
    {
        if (obj is null) return "nil";
        var text = obj.ToString()!;
        if (obj is double)
        {
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return text;
    }

    private void CheckNumberOperand(Token token, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(token,"Operand must be a number");
    }

    private void CheckNumberOperands(Token token, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(token, "Operands must be two numbers or two strings");
    }

    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private bool IsEqual(object? a, object? b)
    {
        return a switch
        {
            null when b is null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    private bool IsTruthy(object? obj)
    {
        return obj switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }

    public object? VisitExpressionStmt(Stmt.Expression expr)
    {
        Evaluate(expr.Expr);
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        object? value = null;
        value = Evaluate(stmt.Initializer);
        _env.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Env(_env));
        return null;
    }

    public void ExecuteBlock(List<Stmt> statements, Env env)
    {
        var previous = this._env;
        try
        {
            _env = env;
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        finally
        {
            _env = previous;
        }
    }
}

public class RuntimeError : Exception
{
    public Token Token;

    public RuntimeError(Token token, string message) : base(message)
    {
        this.Token = token;
    }
}

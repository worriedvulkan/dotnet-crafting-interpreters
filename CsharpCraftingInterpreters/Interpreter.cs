using System.IO.Compression;

namespace CsharpCraftingInterpreters;

public class Interpreter : Expr.IVisitor<Object>
{
    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evalute(expr.Left);
        var right = Evalute(expr.Right);

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
        return Evalute(expr.Expression);
    }

    public void Interpret(Expr expression)
    {
        try
        {
            var value = Evalute(expression);
            Console.WriteLine(Stringify(value));
        }
        catch (RuntimeError err)
        {
            Program.RuntimeError(err);
        }
    }

    private string Stringify(object? obj)
    {
        if (obj is null) return "nil";
        if (obj is double)
        {
            var text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString();
    }
    
    public object VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evalute(expr.Right);
        switch (expr.Operator.TokenType) 
        {
            case TokenType.Bang: return IsTruthy(right) == false;
            case TokenType.Minus:

                return -(double)right;
            default: return null;
        }
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

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    private object Evalute(Expr expr)
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
}

public class RuntimeError : Exception
{
    public Token Token;

    public RuntimeError(Token token, string message) : base(message)
    {
        this.Token = token;
    }
}
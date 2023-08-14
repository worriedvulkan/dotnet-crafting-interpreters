using System.Text;

namespace CsharpCraftingInterpreters;

public class AstPrinter : Expr.IVisitor<string>
{
    private string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public static void Run(string[] args)
    {
        var expression = new Expr.Binary(
            new Expr.Unary(new Token(TokenType.Minus, "-", null, 1), new Expr.Literal(123)),
            new Token(TokenType.Star, "*", null, 1),
            new Expr.Grouping(new Expr.Literal(45.67))
        );
    }

    public string VisitAssignExpr(Expr.Assign expr)
    {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Token.Lexeme, expr.Left, expr.Right);
    }
    
    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }
    
    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value == null ? "nil" : expr.Value.ToString();
    }

    public string VisitVariableExpr(Expr.Variable expr)
    {
        throw new NotImplementedException();
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var sb = new StringBuilder();
        sb.Append('(').Append(name);
        foreach (var expr in exprs)
        {
            sb.Append(' ').Append(expr.Accept(this));
        }

        sb.Append(')');

        return sb.ToString();
    }
}
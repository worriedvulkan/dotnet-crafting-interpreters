namespace CsharpCraftingInterpreters;

public abstract class Expr
{
    public interface IVisitor<T>
    {
         T VisitBinaryExpr(Binary expr);
         T VisitGroupingExpr(Grouping expr);
         T VisitUnaryExpr(Unary expr);
         T VisitLiteralExpr(Literal expr);

    }
    
    public abstract T Accept<T>(IVisitor<T> visitor);
    
    public class Binary : Expr
    {
        public Expr Left;
        public Token Token;
        public Expr Right;
        
        public Binary(Expr left, Token token, Expr right)
        {
            Left = left;
            Token = token;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Unary : Expr
    {
        public Token Operator;
        public Expr Right;

        public Unary(Token @operator, Expr right)
        {
            Operator = @operator;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression;

        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public object Value;

        public Literal(object literal)
        {
            Value = literal;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
}

namespace CsharpCraftingInterpreters;

public abstract class Stmt
{
    public interface IVisitor<T>
    {
        T VisitExpressionStmt(Expression stmt);
        T VisitPrintStmt(Print stmt);
        T VisitVarStmt(Var stmt);
        T VisitBlockStmt(Block stmt);
    }

    public abstract T Accept<T>(IVisitor<T> visitor);

    public class Expression : Stmt
    {
        public Expr Expr;

        public Expression(Expr expression)
        {
            Expr = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    public class Print : Stmt
    {
        public Expr Expr;

        public Print(Expr expression)
        { 
            Expr = expression;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    public class Var : Stmt
    {
        public Token Name;
        public Expr Initializer;

        public Var(Token name, Expr initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }

    public class Block : Stmt
    {
        public List<Stmt> Statements;

        public Block(List<Stmt> statements)
        {
            Statements = statements;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }
}

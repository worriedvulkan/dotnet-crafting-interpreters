namespace CsharpCraftingInterpreters;

public class Env
{
    private Dictionary<string, object> values = new();
    public Env Enclosing;

    public Env()
    {
        Enclosing = null;
    }

    public Env(Env enclosing)
    {
        Enclosing = enclosing;
    }

    public void Define(string name, object value)
    {
        values.Add(name, value);
    }

    public object Get(Token name)
    {
        if (values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        if (Enclosing != null) return Enclosing.Get(name);

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void Assign(Token name, object value)
    {
        if (values.ContainsValue(name.Lexeme))
        {
            values.Add(name.Lexeme, value);
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'");
    }
}
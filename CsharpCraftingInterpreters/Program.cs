namespace CsharpCraftingInterpreters;

public static class Program
{
    public static bool HadError = false;
    public static bool HadRuntimeError = false;
    private static Interpreter _interpreter = new Interpreter();

    public static void Main(string[] args)
    {
        switch (args.Length)
        {
            case > 1:
                Console.WriteLine("Usage: clox [script]");
                Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }

    public static void RunFile(string path)
    {
        var str = File.ReadAllText(path);
        Run(str);
        if (HadError) Environment.Exit(65);
        if (HadRuntimeError) Environment.Exit(70);
    }

    public static void RunPrompt()
    {
        using var input = Console.In;
        while (true)
        {
            Console.WriteLine("> ");
            var line = input.ReadLine();
            if (line.IsNullOrWhiteSpace()) break;
            Run(line!);
            HadError = false;
        }
    }

    public static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var expressions = parser.Parse();
        if (HadError) return;
        
        _interpreter.Interpret(expressions);
        foreach (var token in tokens)
        {
            Console.WriteLine(tokens);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.TokenType == TokenType.Eof)
        {
            Report(token.Line, " at end", message);
        }
        Report(token.Line, " at '" + token.Lexeme + "'", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        HadError = true;
    }

    public static void RuntimeError(RuntimeError err)
    {
        Console.Error.WriteLine(err.Message + "\n[line " + err.Token.Line + "]");
        HadRuntimeError = true;
    }
}
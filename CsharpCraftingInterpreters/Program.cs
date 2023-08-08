// See https://aka.ms/new-console-template for more information

using CsharpCraftingInterpreters;

namespace CsharpCraftingInterpreters;

public static class Program
{
    public static bool HadError = false;

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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        HadError = true;
    }
}
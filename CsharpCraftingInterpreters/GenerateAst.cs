using System.Data;
using System.Text;

namespace CsharpCraftingInterpreters;

public class GenerateAst
{
    public static void Run(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output directory");
            Environment.Exit(64);
        }

        var outputDir = args[0];
        DefineAst(outputDir, "Expr", new List<string>
        {
            "Binary   : Expr left, Token operator, Expr right",
            "Grouping : Expr expression",
            "Literal  : Object value",
            "Unary    : Token operator, Expr right"
        });
    }

    private static void DefineVisitor(StringBuilder sb, string baseName, List<string> types)
    {
        sb.AppendLine("interface Visitor<R> {");
        foreach (var t in types)
        {
            var typeName = t.Split(":")[0].Trim();
            sb.AppendLine($"R visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }

        sb.AppendLine("}");
    }

    private static void DefineAst(string outputDir, string basename, List<string> types)
    {
        var path = $"{outputDir}/{basename}.cs";
        
        var sb = new StringBuilder();
        sb
            .AppendLine("using System.Collections.Generic;")
            .AppendLine()
            .AppendLine("namespace Lox;")
            .AppendLine()
            .AppendLine($"abstract class {basename} {{");

        foreach (var t in types)
        {
            var classname = t.Split(":")[0].Trim();
            var fields = t.Split(":")[1].Trim();
            DefineType(sb, basename, classname, fields);
        }

        sb.AppendLine();
        sb.AppendLine("abstract <R> R accept(Visitor<R> visitor);");
        sb.AppendLine("}");
        File.WriteAllText(path, sb.ToString());
    }

    private static void DefineType(StringBuilder sb, string baseName, string className, string fieldList)
    {
        sb
            .AppendLine($"class {className} : {baseName} {{")
            .AppendLine($"public {className}({fieldList}) {{");

        var fields = fieldList.Split(", ");
        foreach (var field in fields)
        {
            var name = field.Split(" ")[1];
            sb.AppendLine($"this._{name} = {name};");
        }

        sb.AppendLine("}")
            .AppendLine()
            .AppendLine()
            .AppendLine("@Override")
            .AppendLine("<R> R accept(Visitor<R> visitor) {")
            .AppendLine($"return visitor.visit{className}{baseName}(this);")
            .AppendLine("}");

        foreach (var field in fields)
        {
            sb.AppendLine($"private {field}");
        }

        sb.AppendLine("}");

    }
}
using Template_net10.Tools.Commands;

namespace Template_net10.Tools;

/// <summary>
/// Entry point for the <c>do</c> CLI — a small command runner for the
/// Template-net10 starter kit. It only dispatches to a command; all real
/// work lives in the command classes under <see cref="Commands"/>.
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return 0;
        }

        var name = args[0];
        var rest = args.Skip(1).ToArray();

        try
        {
            return name switch
            {
                "rename" => RenameCommand.Run(rest),
                "key:generate" => KeyGenerateCommand.Run(rest),
                _ => Unknown(name),
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"  ERROR  {ex.Message}");
            return 1;
        }
    }

    private static bool IsHelp(string arg) =>
        arg is "-h" or "--help" or "help" or "list" or "-?";

    private static int Unknown(string name)
    {
        Console.Error.WriteLine($"  ERROR  Unknown command '{name}'.");
        Console.Error.WriteLine();
        PrintHelp();
        return 1;
    }

    private static void PrintHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Template-net10 do — project tooling");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet do <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        Console.WriteLine("  rename <NewName>      Rename the project, folders, files and namespaces.");
        Console.WriteLine("                        Use -y/--yes to skip the confirmation prompt.");
        Console.WriteLine("  key:generate          Generate a fresh JWT SecretKey into config/jwt.json.");
        Console.WriteLine("                        Use --show to print the key, --length <n> to size it.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet do rename Acme.Shop");
        Console.WriteLine("  dotnet do key:generate --show");
        Console.WriteLine();
    }
}

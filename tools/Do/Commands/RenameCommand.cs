using System.Text;

namespace Template_net10.Tools.Commands;

/// <summary>
/// Renames the whole starter kit: file/folder contents, file names and folder names.
/// It replaces both the hyphen form (<c>Template-net10</c>, used by project/folder names,
/// JWT issuer, email domains) and the underscore form (<c>Template_net10</c>, used by
/// namespaces and assembly names).
/// </summary>
internal static class RenameCommand
{
    private const string OldHyphen = "Template-net10";
    private const string OldUnderscore = "Template_net10";

    private static readonly string[] BinaryExtensions =
    [
        ".png", ".jpg", ".jpeg", ".gif", ".ico", ".webp", ".pdf", ".zip", ".dll", ".exe",
        ".pdb", ".snk", ".woff", ".woff2", ".ttf", ".eot", ".bmp",
    ];

    public static int Run(string[] args)
    {
        var positional = args.Where(a => !a.StartsWith('-')).ToArray();
        var skipConfirm = args.Any(a => a is "-y" or "--yes");

        if (positional.Length == 0)
        {
            Console.Error.WriteLine("  ERROR  Missing new name. Usage: dotnet do rename <NewName>");
            return 1;
        }

        var newHyphen = positional[0].Trim();
        if (!IsValidName(newHyphen))
        {
            Console.Error.WriteLine(
                "  ERROR  Invalid name. Use letters, digits, '.', '-' or '_' and start with a letter.");
            return 1;
        }

        var newUnderscore = newHyphen.Replace('-', '_').Replace('.', '_');

        if (string.Equals(newHyphen, OldHyphen, StringComparison.Ordinal))
        {
            Console.WriteLine("Nothing to do — the new name matches the current name.");
            return 0;
        }

        var root = Workspace.FindRoot();

        Console.WriteLine();
        Console.WriteLine($"Root:        {root}");
        Console.WriteLine($"Rename:      {OldHyphen}      -> {newHyphen}");
        Console.WriteLine($"             {OldUnderscore}      -> {newUnderscore}");
        Console.WriteLine();

        if (!skipConfirm)
        {
            Console.Write("This rewrites files in place. Continue? [y/N] ");
            var answer = Console.ReadLine();
            if (answer is null || !answer.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Aborted.");
                return 0;
            }
        }

        var filesChanged = RewriteFileContents(root, newHyphen, newUnderscore);
        var (filesRenamed, dirsRenamed) = RenamePaths(root, newHyphen, newUnderscore);

        Console.WriteLine();
        Console.WriteLine($"Done. {filesChanged} file(s) edited, {filesRenamed} file(s) and {dirsRenamed} folder(s) renamed.");
        Console.WriteLine("Next: run `dotnet build` to verify.");
        return 0;
    }

    private static int RewriteFileContents(string root, string newHyphen, string newUnderscore)
    {
        var changed = 0;

        foreach (var file in Workspace.EnumerateFiles(root))
        {
            if (BinaryExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var original = File.ReadAllText(file);
            if (!original.Contains(OldHyphen, StringComparison.Ordinal)
                && !original.Contains(OldUnderscore, StringComparison.Ordinal))
            {
                continue;
            }

            // Underscore form first so the hyphen replacement can never touch it.
            var updated = original
                .Replace(OldUnderscore, newUnderscore, StringComparison.Ordinal)
                .Replace(OldHyphen, newHyphen, StringComparison.Ordinal);

            if (!string.Equals(updated, original, StringComparison.Ordinal))
            {
                File.WriteAllText(file, updated, new UTF8Encoding(false));
                changed++;
            }
        }

        return changed;
    }

    private static (int Files, int Dirs) RenamePaths(string root, string newHyphen, string newUnderscore)
    {
        var files = 0;
        var dirs = 0;

        // Files first, then directories (deepest-first via EnumerateDirectories ordering).
        foreach (var file in Workspace.EnumerateFiles(root).ToList())
        {
            var target = ReplaceLeafName(file, newHyphen, newUnderscore);
            if (target is not null)
            {
                File.Move(file, target);
                files++;
            }
        }

        foreach (var dir in Workspace.EnumerateDirectories(root).ToList())
        {
            if (!Directory.Exists(dir))
            {
                continue; // parent may already have been renamed
            }

            var target = ReplaceLeafName(dir, newHyphen, newUnderscore);
            if (target is not null)
            {
                Directory.Move(dir, target);
                dirs++;
            }
        }

        return (files, dirs);
    }

    private static string? ReplaceLeafName(string path, string newHyphen, string newUnderscore)
    {
        var dir = Path.GetDirectoryName(path)!;
        var name = Path.GetFileName(path);

        if (!name.Contains(OldHyphen, StringComparison.Ordinal)
            && !name.Contains(OldUnderscore, StringComparison.Ordinal))
        {
            return null;
        }

        var newName = name
            .Replace(OldUnderscore, newUnderscore, StringComparison.Ordinal)
            .Replace(OldHyphen, newHyphen, StringComparison.Ordinal);

        return Path.Combine(dir, newName);
    }

    private static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !char.IsLetter(name[0]))
        {
            return false;
        }

        return name.All(c => char.IsLetterOrDigit(c) || c is '.' or '-' or '_');
    }
}

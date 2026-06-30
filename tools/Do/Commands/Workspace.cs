namespace Template_net10.Tools.Commands;

/// <summary>
/// Helpers shared by the artisan commands: locating the repository root and
/// enumerating the project's text files while skipping build output and VCS folders.
/// </summary>
internal static class Workspace
{
    private static readonly string[] IgnoredDirectories =
    [
        "bin", "obj", ".git", ".vs", ".vscode", ".idea", "node_modules", "TestResults",
    ];

    /// <summary>
    /// Walks up from the current directory until it finds the folder containing the
    /// solution file (<c>*.slnx</c> or <c>*.sln</c>). Falls back to the current directory.
    /// </summary>
    public static string FindRoot()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (dir is not null)
        {
            if (dir.EnumerateFiles("*.slnx").Any() || dir.EnumerateFiles("*.sln").Any())
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// Enumerates every file under <paramref name="root"/>, skipping build output,
    /// VCS and editor folders.
    /// </summary>
    public static IEnumerable<string> EnumerateFiles(string root)
    {
        foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            if (!IsIgnored(path, root))
            {
                yield return path;
            }
        }
    }

    /// <summary>
    /// Enumerates directories under <paramref name="root"/> deepest-first, skipping
    /// build output / VCS folders. Deepest-first ordering makes directory renames safe.
    /// </summary>
    public static IEnumerable<string> EnumerateDirectories(string root)
    {
        return Directory
            .EnumerateDirectories(root, "*", SearchOption.AllDirectories)
            .Where(p => !IsIgnored(p, root))
            .OrderByDescending(p => p.Length);
    }

    private static bool IsIgnored(string path, string root)
    {
        var relative = Path.GetRelativePath(root, path);
        var segments = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return segments.Any(s => IgnoredDirectories.Contains(s, StringComparer.OrdinalIgnoreCase));
    }
}

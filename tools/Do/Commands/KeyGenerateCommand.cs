using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Template_net10.Tools.Commands;

/// <summary>
/// Generates a cryptographically strong JWT signing key and writes it into the
/// <c>SecretKey</c> field of every <c>config/**/jwt.json</c> file under the API
/// project. Comments and trailing commas in those files are preserved because the
/// value is patched with a regex rather than re-serialised.
/// </summary>
internal static partial class KeyGenerateCommand
{
    private const int DefaultLength = 64;
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static int Run(string[] args)
    {
        var show = args.Any(a => a is "--show");
        var length = ReadLength(args);

        if (length < 32)
        {
            Console.Error.WriteLine("  ERROR  --length must be at least 32 (HMAC-SHA256 requirement).");
            return 1;
        }

        var key = GenerateKey(length);
        var root = Workspace.FindRoot();
        var configDir = Path.Combine(root, "src", "API", "config");

        if (!Directory.Exists(configDir))
        {
            Console.Error.WriteLine($"  ERROR  Config folder not found: {configDir}");
            return 1;
        }

        var updated = 0;
        foreach (var file in Directory.EnumerateFiles(configDir, "jwt.json", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(file);
            var patched = SecretKeyRegex().Replace(content, $"$1{key}$2", 1);

            if (!string.Equals(patched, content, StringComparison.Ordinal))
            {
                File.WriteAllText(file, patched);
                Console.WriteLine($"  updated  {Path.GetRelativePath(root, file)}");
                updated++;
            }
        }

        if (updated == 0)
        {
            Console.WriteLine("No jwt.json file contained a \"SecretKey\" field to update.");
            return 1;
        }

        Console.WriteLine();
        Console.WriteLine($"Generated a {length}-char JWT secret and updated {updated} file(s).");
        if (show)
        {
            Console.WriteLine($"Key: {key}");
        }

        return 0;
    }

    private static int ReadLength(string[] args)
    {
        var index = Array.IndexOf(args, "--length");
        if (index >= 0 && index + 1 < args.Length && int.TryParse(args[index + 1], out var value))
        {
            return value;
        }

        return DefaultLength;
    }

    private static string GenerateKey(int length)
    {
        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new string(chars);
    }

    [GeneratedRegex("(\"SecretKey\"\\s*:\\s*\")[^\"]*(\")")]
    private static partial Regex SecretKeyRegex();
}

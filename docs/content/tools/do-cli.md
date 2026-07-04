# do CLI

`do` is a small command-line companion for Template-net10, in the spirit of Laravel's `php artisan`. It lives in `tools/Do/` and ships as a packable .NET tool.

## Commands

| Command | Purpose |
|---------|---------|
| `rename` | Rebrand the whole starter kit for a new project |
| `key:generate` | Generate a fresh JWT signing key into every `config/**/jwt.json` |

## How to Run

### From source (no install)

```powershell
dotnet run --project tools/Do -- key:generate --show
dotnet run --project tools/Do -- rename Acme.Shop
```

The `--` separates `dotnet run` arguments from the tool's own arguments.

### As a global dotnet sub-command

```powershell
dotnet pack tools/Do/Template-net10.Tools.csproj -o ./nupkg
dotnet tool install --global --add-source ./nupkg Template-net10.Tools

dotnet do key:generate --show
dotnet do rename Acme.Shop
```

> **Why `dotnet do`?** `do` is a reserved keyword in PowerShell. The tool exposes itself as `dotnet do`.

### Discover commands

```powershell
dotnet do
```

## rename

```powershell
dotnet do rename <NewName> [-y|--yes]
```

Replaces both naming forms:

| Form | Example (`Acme.Shop`) |
|------|------------------------|
| `Template-net10` (hyphen) | `Acme.Shop` |
| `Template_net10` (underscore) | `Acme_Shop` |

Pass `-y` to skip the confirmation prompt.

## key:generate

```powershell
dotnet do key:generate [--show] [--length <n>]
```

| Option | Default | Meaning |
|--------|---------|---------|
| `--show` | off | Print the generated key to stdout |
| `--length <n>` | `64` | Key length in characters (minimum 32) |

Updates every `config/**/jwt.json` while preserving JSON comments.

## Related

- [dotnet new Template](/docs/tools/dotnet-template)
- [JWT Configuration](/docs/configuration/jwt)

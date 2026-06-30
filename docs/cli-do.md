# The `do` CLI — Project Tooling

`do` is a small command-line companion for the **Template-net10** starter kit, in the spirit of
Laravel's `php artisan`. It lives in [`tools/Do/`](../tools/Do/Template-net10.Tools.csproj) and ships
as a packable **.NET tool**.

It currently provides two commands:

| Command | Purpose |
|---------|---------|
| [`rename`](#rename) | Rebrand the whole starter kit (files, folders, namespaces) for a new project. |
| [`key:generate`](#keygenerate) | Generate a fresh JWT signing key into every `config/**/jwt.json`. |

---

## How to run it

There are three equivalent ways to invoke the tool.

### 1. From source (no install)

Works straight from a clone — nothing to install:

```powershell
dotnet run --project tools/Do -- key:generate --show
dotnet run --project tools/Do -- rename Acme.Shop
```

The `--` separates `dotnet run` arguments from the tool's own arguments.

### 2. As a global `dotnet` sub-command

Pack and install once, then call it as `dotnet do …`:

```powershell
dotnet pack tools/Do/Template-net10.Tools.csproj -o ./nupkg
dotnet tool install --global --add-source ./nupkg Template-net10.Tools

dotnet do key:generate --show
dotnet do rename Acme.Shop
```

> **Why `dotnet do` and not just `do`?** `do` is a reserved keyword in PowerShell (the `do { } while`
> loop), so a bare `do` command is intercepted by the shell. The tool's command name is therefore
> `dotnet-do`, which the .NET CLI exposes as the sub-command `dotnet do`.

### 3. As a local tool (per-repo)

```powershell
dotnet new tool-manifest          # once, creates .config/dotnet-tools.json
dotnet tool install --add-source ./nupkg Template-net10.Tools
dotnet do key:generate
```

### Discovering commands

Run with no arguments (or `help` / `--help`) to print usage:

```powershell
dotnet do
```

---

## Where it operates

Every command resolves the **repository root** by walking up from the current directory until it finds
the solution file (`*.slnx` / `*.sln`). You can run the tool from any sub-folder of the repo.

---

## `rename`

Rebrands the entire starter kit so you can copy it into a new project and make it your own.

```powershell
dotnet do rename <NewName> [-y|--yes]
```

It replaces **both** naming forms used throughout the repo:

| Form | Where it is used | Example replacement (`Acme.Shop`) |
|------|------------------|------------------------------------|
| `Template-net10` (hyphen) | project / folder names, `.slnx` paths, JWT `Issuer`/`Audience`, email domains | `Acme.Shop` |
| `Template_net10` (underscore) | C# namespaces and assembly names | `Acme_Shop` |

The new underscore form is derived automatically from the name you pass (`-` and `.` become `_`).

What it does, in order:

1. **Rewrites file contents** — every text file under the repo (the underscore form is replaced first
   so the hyphen pass can never corrupt it). UTF-8, no BOM.
2. **Renames files** whose names contain either form (e.g. `Template-net10.API.csproj`).
3. **Renames folders** (deepest-first, so parent renames don't invalidate child paths).

It **skips** build output and tooling folders: `bin`, `obj`, `.git`, `.vs`, `.vscode`, `.idea`,
`node_modules`, `TestResults`, and known binary file extensions (images, `.dll`, `.pdb`, fonts, …).

By default it prints a summary and asks for confirmation. Pass `-y` / `--yes` to skip the prompt.

```text
Root:        C:\src\Template-net10
Rename:      Template-net10      -> Acme.Shop
             Template_net10      -> Acme_Shop

This rewrites files in place. Continue? [y/N] y
Done. 84 file(s) edited, 12 file(s) and 9 folder(s) renamed.
Next: run `dotnet build` to verify.
```

> **Tip:** commit (or stash) before running `rename` so you can review the diff and revert easily. The
> command rewrites files in place — including its own — which is intended and keeps naming consistent.

**Name rules:** must start with a letter and contain only letters, digits, `.`, `-`, or `_`.

---

## `key:generate`

Generates a cryptographically strong JWT signing key (via `RandomNumberGenerator`) and writes it into
the `SecretKey` field of **every** `config/**/jwt.json` under `src/API` — the base file *and* every
per-environment override (`Development`, `Staging`, `Production`).

```powershell
dotnet do key:generate [--show] [--length <n>]
```

| Option | Default | Meaning |
|--------|---------|---------|
| `--show` | off | Print the generated key to stdout. |
| `--length <n>` | `64` | Length of the key in characters (minimum `32`, the HMAC-SHA256 requirement). |

The value is patched with a regex rather than re-serialised, so the `//` comments and trailing commas
in the config files are **preserved**:

```jsonc
{
  "Jwt": {
    "Issuer": "Template-net10",
    "Audience": "Template-net10.Client",
    // SecretKey — HMAC-SHA256 signing key; MUST be >= 32 chars. Replace with a real secret
    // stored in user-secrets / environment variables / a vault. Never commit production secrets.
    "SecretKey": "qN8tCxUKtYFvDEMir3F7EtYFvMexFWtRy6Zla14qrjXlmnf5hjsnS3Q0UQr0QmYq",
    "ExpiryMinutes": 60
  }
}
```

Sample output:

```text
  updated  src\API\config\jwt.json
  updated  src\API\config\Development\jwt.json
  updated  src\API\config\Production\jwt.json
  updated  src\API\config\Staging\jwt.json

Generated a 64-char JWT secret and updated 4 file(s).
```

> **Security:** the keys written by this command are for local/dev convenience. For real environments,
> keep the secret out of source control — supply it via user-secrets, environment variables, or a vault
> (the config file comments say the same).

---

## Adding a new command

The CLI uses a tiny hand-rolled dispatcher (no external argument parser):

- [`Program.cs`](../tools/Do/Program.cs) — maps the first argument to a command and prints help.
- [`Commands/Workspace.cs`](../tools/Do/Commands/Workspace.cs) — shared helpers (root detection, file
  enumeration with ignore rules).
- One static class per command under [`tools/Do/Commands/`](../tools/Do/Commands/) exposing
  `int Run(string[] args)`.

To add a command, create `Commands/{Name}Command.cs` with a `Run` method, add a `case` to the `switch`
in `Program.cs`, and document it in the help text and in this file.

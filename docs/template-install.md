# Using Template-net10 as a `dotnet new` template

This repository ships a [`dotnet new`](https://learn.microsoft.com/dotnet/core/tools/custom-templates)
template so you can spin up a fresh, fully-renamed project in one command instead of copying the repo
and running a find-and-replace.

The template rewrites **both** naming forms used throughout the starter kit:

| Form | Used by | Example (`Acme.Shop`) |
|------|---------|------------------------|
| `Template-net10` (hyphen) | project / folder names, `.slnx` paths, JWT Issuer/Audience, email domains | `Acme.Shop` |
| `Template_net10` (underscore) | C# namespaces and assembly names | `Acme_Shop` |

The underscore form is derived automatically from the name you pass (`-` and `.` become `_`), so you
only ever specify the name once.

---

## Prerequisites

- .NET 10 SDK (`dotnet --version` → `10.x`)

---

## Option A — Install from the local folder (fastest for your own use)

From the **repository root** (the folder that contains `.template.config/`):

```powershell
# 1. Register the template with the local dotnet CLI
dotnet new install .

# 2. Create a new project anywhere you like
dotnet new cleanapi -n Acme.Shop -o C:\src\Acme.Shop

# 3. Build to verify
cd C:\src\Acme.Shop
dotnet build Acme.Shop.slnx
```

`-n` sets the project name (drives the rename), `-o` sets the output folder. Omit `-o` to generate
into a new folder named after `-n` in the current directory.

To update the template after you change the repo, just run `dotnet new install .` again.

To remove it:

```powershell
dotnet new uninstall <full-path-to-this-repo>
```

---

## Option B — Package as a NuGet template pack (to share with others)

A packaging project, [`Template-net10.Template.csproj`](../Template-net10.Template.csproj), is included.
It is **not** part of `Template-net10.slnx` and produces no build output — it only bundles the repo
into a template `.nupkg`.

```powershell
# 1. Pack the template into ./nupkg
dotnet pack Template-net10.Template.csproj -o ./nupkg

# 2. Install the produced package
dotnet new install ./nupkg/Template.Net10.CleanArchitecture.1.0.0.nupkg
```

Now anyone with that `.nupkg` (or your NuGet feed) can run:

```powershell
dotnet new install Template.Net10.CleanArchitecture
dotnet new cleanapi -n Acme.Shop
```

To publish to nuget.org (optional):

```powershell
dotnet nuget push ./nupkg/Template.Net10.CleanArchitecture.1.0.0.nupkg `
  --api-key <YOUR_KEY> --source https://api.nuget.org/v3/index.json
```

---

## After creating a project

1. **Generate a JWT signing key** — secrets are never baked into the template:

   ```powershell
   dotnet run --project tools/Do -- key:generate
   ```

   (or `dotnet do key:generate` if the `do` tool is installed globally).

2. **Set real secrets** for `config/jwt.json` (`SecretKey`), `config/mail.json` credentials, and the
   database connection string via user-secrets, environment variables, or a vault — never commit them.

3. **Build & run:**

   ```powershell
   dotnet build Acme.Shop.slnx
   dotnet run --project Acme.Shop.AppHost      # via Aspire (preferred)
   ```

---

## Template command reference

| Command | Purpose |
|---------|---------|
| `dotnet new install .` | Register the template from this folder. |
| `dotnet new install ./nupkg/<pkg>.nupkg` | Register from a packed `.nupkg`. |
| `dotnet new cleanapi -n <Name> [-o <dir>]` | Create a new project (`cleanapi` is the template short name). |
| `dotnet new uninstall <path-or-id>` | Remove the template. |
| `dotnet new list cleanapi` | Confirm the template is installed. |

---

## How it works (for maintainers)

The template is driven by [`.template.config/template.json`](../.template.config/template.json):

- `sourceName: "Template-net10"` replaces the **hyphen** form in file contents, file names, and folder
  names at instantiation time.
- A `derived` symbol `underscoreName` with the `toUnderscore` form (regex `[.\-]` → `_`) replaces the
  **underscore** namespace form, matching the logic in
  [`tools/Do/Commands/RenameCommand.cs`](../tools/Do/Commands/RenameCommand.cs).
- `sources.modifiers.exclude` skips `bin`, `obj`, IDE folders, `nupkg`, and the packaging project so
  they never land in generated output.

> The `dotnet do rename` command still exists for people who copy the repo manually; the template is
> the preferred path for new projects.

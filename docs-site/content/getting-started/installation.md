# Installation

This repository ships a [`dotnet new`](https://learn.microsoft.com/dotnet/core/tools/custom-templates) template so you can spin up a fresh, fully-renamed project in one command.

The template rewrites **both** naming forms used throughout the starter kit:

| Form | Used by | Example (`Acme.Shop`) |
|------|---------|------------------------|
| `Template-net10` (hyphen) | project / folder names, `.slnx` paths, JWT Issuer/Audience | `Acme.Shop` |
| `Template_net10` (underscore) | C# namespaces and assembly names | `Acme_Shop` |

The underscore form is derived automatically from the name you pass (`-` and `.` become `_`).

## Prerequisites

- .NET 10 SDK (`dotnet --version` → `10.x`)
- Node.js 18+ (only if you want to run this documentation site)

## Option A — Install from the Local Folder

From the **repository root** (the folder that contains `.template.config/`):

```powershell
# Register the template
dotnet new install .

# Create a new project
dotnet new cleanapi -n Acme.Shop -o C:\src\Acme.Shop

# Build to verify
cd C:\src\Acme.Shop
dotnet build Acme.Shop.slnx
```

`-n` sets the project name; `-o` sets the output folder. Omit `-o` to generate into a folder named after `-n`.

To update the template after repo changes, run `dotnet new install .` again.

## Option B — Package as a NuGet Template

```powershell
dotnet pack Template-net10.Template.csproj -o ./nupkg
dotnet new install ./nupkg/Template.Net10.CleanArchitecture.1.0.0.nupkg
dotnet new cleanapi -n Acme.Shop
```

## After Creating a Project

1. **Generate a JWT signing key** — secrets are never baked into the template:

   ```powershell
   dotnet run --project tools/Do -- key:generate
   ```

2. **Set real secrets** for JWT, mail, and database via user-secrets or environment variables.

3. **Build and run:**

   ```powershell
   dotnet build Acme.Shop.slnx
   dotnet run --project Acme.Shop.AppHost
   ```

## Template Command Reference

| Command | Purpose |
|---------|---------|
| `dotnet new install .` | Register the template from this folder |
| `dotnet new cleanapi -n <Name> [-o <dir>]` | Create a new project |
| `dotnet new uninstall <path-or-id>` | Remove the template |
| `dotnet new list cleanapi` | Confirm the template is installed |

## Related

- [do CLI](/docs/tools/do-cli) — rebrand manually or generate JWT keys
- [Configuration](/docs/getting-started/configuration) — set up config files and secrets

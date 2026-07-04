# dotnet new Template

Spin up a fresh, fully-renamed project in one command using the built-in `dotnet new` template.

## Prerequisites

- .NET 10 SDK

## Install from Local Folder

From the repository root (contains `.template.config/`):

```powershell
dotnet new install .
dotnet new cleanapi -n Acme.Shop -o C:\src\Acme.Shop
cd C:\src\Acme.Shop
dotnet build Acme.Shop.slnx
```

## Naming

The template rewrites both forms automatically:

| Form | Used by |
|------|---------|
| `Template-net10` | Project/folder names, JWT Issuer, email domains |
| `Template_net10` | C# namespaces and assembly names |

Pass `-n Acme.Shop` once — underscore form (`Acme_Shop`) is derived automatically.

## After Creating a Project

1. Generate JWT key: `dotnet run --project tools/Do -- key:generate`
2. Set secrets via user-secrets or environment variables
3. Run: `dotnet run --project Acme.Shop.AppHost`

## Package as NuGet Template

```powershell
dotnet pack Template-net10.Template.csproj -o ./nupkg
dotnet new install ./nupkg/Template.Net10.CleanArchitecture.1.0.0.nupkg
dotnet new cleanapi -n Acme.Shop
```

## Command Reference

| Command | Purpose |
|---------|---------|
| `dotnet new install .` | Register template from this folder |
| `dotnet new cleanapi -n <Name> [-o <dir>]` | Create new project |
| `dotnet new uninstall <path-or-id>` | Remove template |
| `dotnet new list cleanapi` | Confirm installation |

## Related

- [Installation](/docs/getting-started/installation)
- [do CLI](/docs/tools/do-cli)

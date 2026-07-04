# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy central package management + project files first to maximize restore layer caching.
COPY Directory.Packages.props ./
COPY src/Domain/Template-net10.Domain.csproj src/Domain/
COPY src/Application/Template-net10.Application.csproj src/Application/
COPY src/Infrastructure/Template-net10.Infrastructure.csproj src/Infrastructure/
COPY src/API/Template-net10.API.csproj src/API/
COPY Template-net10.ServiceDefaults/Template-net10.ServiceDefaults.csproj Template-net10.ServiceDefaults/

RUN dotnet restore src/API/Template-net10.API.csproj

# Copy the remaining source and publish a framework-dependent app.
COPY src/ src/
COPY Template-net10.ServiceDefaults/ Template-net10.ServiceDefaults/
RUN dotnet publish src/API/Template-net10.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# QuestPDF/SkiaSharp needs fontconfig (+ freetype) to render PDFs on Linux.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

# Writable directory for the local file Storage driver, owned by the non-root runtime user.
RUN mkdir -p /app/storage/app && chown -R app:app /app/storage
USER app

# The .NET runtime image listens on 8080 by default (ASPNETCORE_HTTP_PORTS=8080).
EXPOSE 8080
ENTRYPOINT ["dotnet", "Template_net10.API.dll"]

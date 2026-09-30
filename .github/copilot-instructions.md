# Copilot Instructions for MyAlbum

## Project overview
- **MyAlbum** is a .NET console application. The solution lives in `v7.0/v7.0.slnx` with the single project `v7.0/MyAlbum/MyAlbum.csproj`.
- Entry point: `v7.0/MyAlbum/Program.cs` (uses top-level statements).

## Target framework & language settings
- Target framework: **.NET 10** (`net10.0`).
- `ImplicitUsings` is **enabled** — do not add `using` directives for namespaces already provided implicitly (e.g. `System`, `System.Linq`, `System.Collections.Generic`).
- Nullable reference types are **enabled** — write null-aware code, annotate nullable types with `?`, and avoid introducing nullable warnings.
- Output type is `Exe`.

## Coding conventions
- Prefer **top-level statements** for the program entry point, consistent with the existing `Program.cs`.
- Use modern C# features supported by .NET 10 (e.g. pattern matching, records, collection expressions, `var` where the type is obvious).
- Follow standard .NET naming: `PascalCase` for types/methods/properties, `camelCase` for locals/parameters, `_camelCase` for private fields.
- Keep code null-safe; avoid `!` (null-forgiving) unless a null value is genuinely impossible and document why.
- Use `async`/`await` for I/O-bound work and return `Task`/`Task<T>` rather than blocking.

## Building & running
- Build: `dotnet build v7.0/MyAlbum/MyAlbum.csproj`
- Run: `dotnet run --project v7.0/MyAlbum/MyAlbum.csproj`

## General guidance
- Keep changes minimal and focused; match the existing style of surrounding code.
- When adding new files, place them within the `v7.0/MyAlbum/` project directory.
- Prefer standard library APIs; discuss before adding new NuGet dependencies.

# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first so restore is cached independently of source changes
COPY Directory.Build.props .
COPY src/BlindMatch.Core/BlindMatch.Core.csproj                      src/BlindMatch.Core/
COPY src/BlindMatch.Infrastructure/BlindMatch.Infrastructure.csproj  src/BlindMatch.Infrastructure/
COPY src/BlindMatch.Web/BlindMatch.Web.csproj                        src/BlindMatch.Web/

RUN dotnet restore src/BlindMatch.Web/BlindMatch.Web.csproj

# Copy source and publish
COPY src/ src/
RUN dotnet publish src/BlindMatch.Web/BlindMatch.Web.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BlindMatch.Web.dll"]

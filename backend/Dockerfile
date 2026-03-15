# ── Build stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY CafeManager.sln .
COPY src/ src/

RUN dotnet restore
RUN dotnet publish src/CafeManager.API/CafeManager.API.csproj -c Release -o /app/publish

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for the Docker health check — not present in the base aspnet image
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Create the uploads directory so static file serving doesn't warn on startup
RUN mkdir -p /app/wwwroot && mkdir -p /app/uploads

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "CafeManager.API.dll"]
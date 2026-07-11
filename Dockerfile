# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file first and restore, so Docker caches the restored packages
# and skips re-downloading them when only source code changes.
COPY src/Pokedex.Api/Pokedex.Api.csproj src/Pokedex.Api/
RUN dotnet restore src/Pokedex.Api/Pokedex.Api.csproj

# Now copy the rest of the source and publish the API in Release (no test project).
COPY . .
RUN dotnet publish src/Pokedex.Api/Pokedex.Api.csproj -c Release -o /app/publish --no-restore

# ---------- runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Bring in ONLY the published output — no SDK, no sources.
COPY --from=build /app/publish .

# Run as the predefined non-root user (least privilege).
USER $APP_UID

# Documents the port the app listens on inside the container (.NET 8+ default: 8080).
EXPOSE 8080

ENTRYPOINT ["dotnet", "Pokedex.Api.dll"]

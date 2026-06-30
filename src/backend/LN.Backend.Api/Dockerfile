# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los proyectos del backend (rutas relativas a la raíz del repo, ver context en docker-compose)
COPY src/backend/ ./backend/
RUN dotnet restore backend/LN.Backend.Api/LN.Backend.Api.csproj
RUN dotnet publish backend/LN.Backend.Api/LN.Backend.Api.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LN.Backend.Api.dll"]

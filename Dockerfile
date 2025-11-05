# Imagen base de .NET 8 para producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagen SDK para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StoreOnline_Backend.csproj", "."]
RUN dotnet restore "./StoreOnline_Backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./StoreOnline_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar el proyecto compilado
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StoreOnline_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final de ejecución
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreOnline_Backend.dll"]

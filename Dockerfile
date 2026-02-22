# Usa el SDK 9 para que reconozca MapStaticAssets
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia todo y restaura
COPY . .
RUN dotnet restore "axcan.csproj"

# Publica el proyecto (asegúrate que el nombre del csproj sea correcto)
RUN dotnet publish "axcan.csproj" -c Release -o /out

# Runtime de .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
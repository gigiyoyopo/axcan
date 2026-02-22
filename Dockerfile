# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia TODO el contenido del repo
COPY . .

# Restaura las librerías 
RUN dotnet restore

# Compila apuntando a la carpeta en minúsculas
RUN dotnet publish "axcan/axcan.csproj" -c Release -o /out

# 2. Runtime para que la app corra
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Render usa el puerto 10000 por defecto
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# El ejecutable final
ENTRYPOINT ["dotnet", "axcan.dll"]
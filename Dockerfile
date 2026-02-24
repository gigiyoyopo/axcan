# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos TODO de una vez
COPY . .

# Buscamos el archivo .csproj donde sea que esté y restauramos
RUN dotnet restore $(find . -name "*.csproj")

# Publicamos el proyecto buscando el archivo .csproj automáticamente
RUN dotnet publish $(find . -name "*.csproj") -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos lo publicado
COPY --from=build /out .

# Copiamos las vistas y wwwroot buscando su ubicación real
# Usamos un comando más flexible para no fallar por rutas anidadas
RUN cp -r $(find /src -name Views -type d | head -n 1) ./ 2>/dev/null || :
RUN cp -r $(find /src -name wwwroot -type d | head -n 1) ./ 2>/dev/null || :

# Puerto Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
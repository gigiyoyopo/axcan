# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiamos el csproj específicamente primero para aprovechar la caché
COPY axcan.csproj ./
RUN dotnet restore "axcan.csproj"

# Copiamos todo lo demás
COPY . ./

# AQUÍ ESTÁ EL TRUCO: Le decimos exactamente qué archivo publicar
RUN dotnet publish "axcan.csproj" -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos lo que salió del publish
COPY --from=build /out .

# Copiamos las vistas y los archivos estáticos para que no dé el error de "Index not found"
COPY --from=build /app/Views ./Views
COPY --from=build /app/wwwroot ./wwwroot

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
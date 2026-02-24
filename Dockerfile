# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos todo el contenido al contenedor
COPY . .

# ENTRAMOS a la carpeta real del proyecto donde está el .csproj
WORKDIR "/src/axcan"

# Restauramos y publicamos desde la carpeta interna
RUN dotnet restore "axcan.csproj"
RUN dotnet publish "axcan.csproj" -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos lo publicado
COPY --from=build /out .

# FORZAMOS la copia de las vistas desde la ruta correcta
# (Ajustando a la estructura anidada que vi en tus fotos)
COPY --from=build /src/axcan/Views ./Views
COPY --from=build /src/axcan/wwwroot ./wwwroot

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
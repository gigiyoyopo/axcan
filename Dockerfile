# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiamos todo el contenido de la raíz (donde me dices que está todo)
COPY . .

# Publicamos el proyecto directamente
RUN dotnet publish "axcan.csproj" -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos los binarios publicados
COPY --from=build /out .

# --- COPIA DIRECTA DE VISTAS DESDE LA RAÍZ ---
# Si la raíz es /app en el build, las buscamos ahí mero
COPY --from=build /app/Views ./Views
COPY --from=build /app/wwwroot ./wwwroot

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
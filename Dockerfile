# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiamos el proyecto y restauramos
COPY axcan.csproj ./
RUN dotnet restore "axcan.csproj"

# Copiamos todo y publicamos
COPY . ./
RUN dotnet publish "axcan.csproj" -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos los binarios (lo que hace que la app funcione)
COPY --from=build /out .

# --- AQUÍ ESTÁ EL TRUCO: COPIAR LAS VISTAS MANUALMENTE ---
# Esto asegura que la carpeta Views y wwwroot existan en el servidor
COPY --from=build /app/Views ./Views
COPY --from=build /app/wwwroot ./wwwroot
# --------------------------------------------------------

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
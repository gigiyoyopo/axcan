# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ... (etapa build anterior)
WORKDIR /app
COPY . .

# Ahora sí, el restore va a funcionar porque el archivo XML ya está bien
RUN dotnet restore

# Publicamos (si el archivo está en la raíz, no necesitas poner la carpeta)
RUN dotnet publish -c Release -o /out
# ...



# 2. Runtime para que la app corra
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Render usa el puerto 10000 por defecto
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# El ejecutable final
ENTRYPOINT ["dotnet", "axcan.dll"]

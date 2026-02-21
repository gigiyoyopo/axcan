# Capa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar archivos de solución y proyecto
# Ajustamos las rutas para que coincidan con tu repo
COPY *.sln .
COPY axcan/*.csproj ./axcan/
RUN dotnet restore

# Copiar todo el contenido y publicar
COPY . .
WORKDIR /source/axcan
RUN dotnet publish -c Release -o /app/out

# Capa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configurar el puerto dinámico de Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
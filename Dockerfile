# ETAPA 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar solución y proyecto (Rutas según tu repo)
COPY *.sln .
COPY axcan/*.csproj ./axcan/
RUN dotnet restore

# Copiar todo y publicar
COPY . .
WORKDIR /source/axcan
RUN dotnet publish -c Release -o /app/out

# ETAPA 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configurar el puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# El nombre del archivo de salida
ENTRYPOINT ["dotnet", "axcan.dll"]
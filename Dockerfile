# ETAPA 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# 1. Copiamos la solución
COPY *.sln .

# 2. Copiamos el proyecto respetando la MAYÚSCULA de la carpeta Axcan
COPY Axcan/*.csproj ./Axcan/
RUN dotnet restore

# 3. Copiamos todo lo demás y publicamos
COPY . .
WORKDIR /source/Axcan
RUN dotnet publish -c Release -o /app/out

# ETAPA 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# El nombre del archivo de salida también lleva la mayúscula
ENTRYPOINT ["dotnet", "Axcan.dll"]
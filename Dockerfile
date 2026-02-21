# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos la solución y el proyecto respetando las mayúsculas
COPY *.sln .
COPY Axcan/*.csproj ./Axcan/
RUN dotnet restore

# Copiamos todo lo demás y publicamos
COPY . .
WORKDIR /app/Axcan
RUN dotnet publish -c Release -o /out

# Etapa 2: Ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Configuración del puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# IMPORTANTE: El nombre del archivo generado suele ser el mismo que el .csproj
ENTRYPOINT ["dotnet", "Axcan.dll"]
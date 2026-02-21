# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiamos la solución y el proyecto buscando la carpeta axcan
COPY *.sln .
COPY axcan/*.csproj ./axcan/
RUN dotnet restore

# Copiamos todo lo demás y publicamos
COPY . .
WORKDIR /source/axcan
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime (Ejecución)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# IMPORTANTE: Configurar el puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Asegúrate de que el nombre del proyecto sea axcan.dll
ENTRYPOINT ["dotnet", "axcan.dll"]
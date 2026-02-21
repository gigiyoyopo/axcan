# Capa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar archivos y restaurar dependencias
COPY *.sln .
COPY axcan/*.csproj ./axcan/
RUN dotnet restore

# Copiar todo y publicar
COPY . .
WORKDIR /source/axcan
RUN dotnet publish -c Release -o /app/out

# Capa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configurar puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
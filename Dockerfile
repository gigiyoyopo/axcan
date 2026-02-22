# ETAPA 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar archivos .sln y .csproj (Ajustado a tu estructura)
COPY *.sln .
COPY axcan/*.csproj ./axcan/
RUN dotnet restore

# Copiar todo el código y publicar
COPY . .
WORKDIR /source/axcan
RUN dotnet publish -c Release -o /app/out

# ETAPA 2: Ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Asegúrate de que el nombre del proyecto sea axcan.dll
ENTRYPOINT ["dotnet", "axcan.dll"]

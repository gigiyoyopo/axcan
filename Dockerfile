# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el .sln de la raíz
COPY Axcan.sln .

# Copiar el .csproj respetando la MAYÚSCULA de la carpeta
COPY Axcan/Axcan.csproj ./Axcan/
RUN dotnet restore

# Copiar todo y publicar
COPY . .
WORKDIR /app/Axcan
RUN dotnet publish -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Puerto Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# El nombre del DLL debe llevar la A mayúscula
ENTRYPOINT ["dotnet", "Axcan.dll"]
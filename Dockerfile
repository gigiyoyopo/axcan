# 1. SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiamos el archivo de proyecto y restauramos
COPY *.csproj ./
RUN dotnet restore

# Copiamos TODO y publicamos
COPY . ./
RUN dotnet publish "axcan.csproj" -c Release -o /out

# 2. Runtime para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos lo que salió del publish
COPY --from=build /out .

COPY --from=build /app/Views ./Views
COPY --from=build /app/wwwroot ./wwwroot
# ----------------------------------------------------

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
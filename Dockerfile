# ETAPA DE COMPILACIÓN
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiamos todo el contenido del repo
COPY . .

# Restauramos las dependencias
RUN dotnet restore *.sln

# Publicamosl proyecto apuntando a la carpeta en minúsculas
RUN dotnet publish "axcan/axcan.csproj" -c Release -o /app/out

# ETAPA DE EJECUCIÓN
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configuración de puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# El nombre del archivo de salida
ENTRYPOINT ["dotnet", "axcan.dll"]
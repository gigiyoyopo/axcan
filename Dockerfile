# Capa de compilación (SDK de .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos los archivos del proyecto para restaurar
COPY *.sln .
COPY axcan/*.csproj ./axcan/
RUN dotnet restore

# Copiamos todo lo demás y publicamos
COPY . .
WORKDIR /app/axcan
RUN dotnet publish -c Release -o /out

# Capa de ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Configuración del puerto para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Nombre de tu archivo de salida (verifica que sea axcan.dll)
ENTRYPOINT ["dotnet", "axcan.dll"]
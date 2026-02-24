FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiamos todo
COPY . ./

# Restauramos y publicamos
RUN dotnet restore "axcan.csproj"
RUN dotnet publish "axcan.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiamos lo publicado
COPY --from=build /out .

# Copiamos las vistas a la fuerza respetando mayúsculas
COPY --from=build /app/Views ./Views
COPY --from=build /app/wwwroot ./wwwroot

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "axcan.dll"]
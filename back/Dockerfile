# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo .csproj y restaura dependencias
COPY ["back.csproj", "./"]
RUN dotnet restore

# Copia el archivo .env para cargar FRONT_URL
COPY [".env", "./"]

# Copia el resto del código fuente y publica la app
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Copia .env para configuration en runtime
COPY [".env", "."]
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Ejecuta la app
ENTRYPOINT ["dotnet", "back.dll"]

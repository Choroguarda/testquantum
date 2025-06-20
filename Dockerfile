# Build stage: usa SDK para compilar y publicar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo csproj y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia todo el código y publica la app
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage: usa solo runtime para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Expone el puerto 80 para ASP.NET (puedes cambiarlo si usas otro)
EXPOSE 80

# Ejecuta la app
ENTRYPOINT ["dotnet", "TestQuantumDocs.dll"]

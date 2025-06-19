# Usar la imagen oficial SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código y publicar release
COPY . ./
RUN dotnet publish -c Release -o out

# Imagen runtime para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out ./

# Exponer el puerto que usa la app (cambia si es otro)
EXPOSE 5221

ENTRYPOINT ["dotnet", "TestQuantumDocs.dll"]

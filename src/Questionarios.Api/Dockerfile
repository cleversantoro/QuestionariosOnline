# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia tudo e restaura
COPY . .
RUN dotnet restore ./Questionarios.Api.csproj

# Publica em modo Release
RUN dotnet publish ./Questionarios.Api.csproj -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Expor porta padrão da API
EXPOSE 8080

# ASP.NET Core vai ouvir nessa porta
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Questionarios.Api.dll"]

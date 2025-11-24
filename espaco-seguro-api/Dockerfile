FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Caminho CORRIGIDO do csproj (2 níveis)
COPY ["espaco-seguro-api/espaco-seguro-api/espaco-seguro-api.csproj", "espaco-seguro-api/espaco-seguro-api/"]

RUN dotnet restore "espaco-seguro-api/espaco-seguro-api/espaco-seguro-api.csproj"

# Copia o resto do projeto
COPY . .

# Acessa a pasta onde está o csproj
WORKDIR "/src/espaco-seguro-api/espaco-seguro-api"

RUN dotnet build "espaco-seguro-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "espaco-seguro-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "espaco-seguro-api.dll"]

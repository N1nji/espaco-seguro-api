FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia apenas o csproj para restaurar dependências
COPY espaco-seguro-api/espaco-seguro-api.csproj ./ 
RUN dotnet restore "./espaco-seguro-api.csproj"

# copia todo o código
COPY espaco-seguro-api/ ./

# build
RUN dotnet build "./espaco-seguro-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./espaco-seguro-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "espaco-seguro-api.dll"]

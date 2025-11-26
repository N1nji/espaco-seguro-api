FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia o csproj do diretório atual (Root Directory já ajustado no Render)
COPY espaco-seguro-api.csproj ./
RUN dotnet restore "espaco-seguro-api.csproj"

# copia todo o código para dentro do container
COPY . ./

# build
RUN dotnet build "espaco-seguro-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "espaco-seguro-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "espaco-seguro-api.dll"]

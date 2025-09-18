FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LexiMon.API/LexiMon.API.csproj", "LexiMon.API/"]
COPY ["LexiMon.Service/LexiMon.Service.csproj", "LexiMon.Service/"]
COPY ["LexiMon.Repository/LexiMon.Repository.csproj", "LexiMon.Repository/"]
RUN dotnet restore "LexiMon.API/LexiMon.API.csproj"
COPY . .
WORKDIR "/src/LexiMon.API"
RUN dotnet build "./LexiMon.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LexiMon.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LexiMon.API.dll"]
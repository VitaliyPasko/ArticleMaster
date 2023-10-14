FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ArticleMaster.Scraper/ArticleMaster.Scraper.csproj", "ArticleMaster.Scraper/"]
RUN dotnet restore "ArticleMaster.Scraper/ArticleMaster.Scraper.csproj"
COPY . .
WORKDIR "/src/ArticleMaster.Scraper"
RUN dotnet build "ArticleMaster.Scraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArticleMaster.Scraper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArticleMaster.Scraper.dll"]

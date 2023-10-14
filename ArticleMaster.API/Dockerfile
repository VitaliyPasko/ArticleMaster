FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ArticleMaster.API/ArticleMaster.API.csproj", "ArticleMaster.API/"]
RUN dotnet restore "ArticleMaster.API/ArticleMaster.API.csproj"
COPY . .
WORKDIR "/src/ArticleMaster.API"
RUN dotnet build "ArticleMaster.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArticleMaster.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArticleMaster.API.dll"]

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Services/Tweet/Tweet.API/Tweet.API.csproj", "Services/Tweet/Tweet.API/"]
COPY ["BuildingBlocks/PromRequestMiddleware/Common.RequestMiddleware/Common.RequestMiddleware.csproj", "BuildingBlocks/PromRequestMiddleware/Common.RequestMiddleware/"]
COPY ["BuildingBlocks/Logging/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Logging/Common.Logging/"]
RUN dotnet restore "Services/Tweet/Tweet.API/Tweet.API.csproj"
COPY . .
WORKDIR "/src/Services/Tweet/Tweet.API"
RUN dotnet build "Tweet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tweet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tweet.API.dll"]
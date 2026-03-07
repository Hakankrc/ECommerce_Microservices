FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/BuildingBlocks/Shared/Shared.csproj", "src/BuildingBlocks/Shared/"]
COPY ["src/Services/BasketService/BasketService.csproj", "src/Services/BasketService/"]
RUN dotnet restore "src/Services/BasketService/BasketService.csproj"
COPY . .
WORKDIR "/src/src/Services/BasketService"
RUN dotnet build "BasketService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasketService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketService.dll"]
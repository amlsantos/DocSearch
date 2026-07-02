FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DocSearch.WebApi/DocSearch.WebApi.csproj", "src/DocSearch.WebApi/"]
RUN dotnet restore "src/DocSearch.WebApi/DocSearch.WebApi.csproj"
COPY . .
WORKDIR "/src/src/DocSearch.WebApi"
RUN dotnet build "./DocSearch.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DocSearch.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocSearch.WebApi.dll"]

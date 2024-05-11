FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["big-agi-syncserver/big-agi-syncserver.csproj", "big-agi-syncserver/"]
RUN dotnet restore "big-agi-syncserver/big-agi-syncserver.csproj"
COPY . .
WORKDIR "/src/big-agi-syncserver"
RUN dotnet build "big-agi-syncserver.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "big-agi-syncserver.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "big-agi-syncserver.dll"]

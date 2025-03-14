FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.13-noble-chiseled AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /source

RUN apt update && apt install -y --no-install-recommends clang zlib1g-dev

COPY ["Anvil.sln", "Anvil.sln"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["src/Anvil.Server/Anvil.Server.csproj", "src/Anvil.Server/"]
RUN dotnet restore "src/Anvil.Server/Anvil.Server.csproj"

COPY src/ src/
WORKDIR /source/src/Anvil.Server

RUN dotnet build -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p AssemblyName=anvil-server

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/anvil-server"]

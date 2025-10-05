FROM mcr.microsoft.com/dotnet/runtime-deps:9.0.9-noble-chiseled@sha256:ef120bd54269271d0c64101d81df3e754345a17fac1ca3925159e451f7c36036 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:123b43e4d9775451d8ed63af324a42132707b4edb14770bcdf5c85cf55bc45f1 AS build
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

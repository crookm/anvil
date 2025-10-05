FROM mcr.microsoft.com/dotnet/runtime-deps:9.0.8-noble-chiseled@sha256:a7242461161c037e1a02397f8374e777000e678d19325046afc0108c58dde882 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:eee11b0bf11715710bbe8339b9641f0ef8b5d8a8e07f2d6ff3cd4361c1a4e5a7 AS build
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

ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

RUN mkdir -p /root/.nuget/NuGet && cat > /root/.nuget/NuGet/NuGet.Config <<'EOF'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
  <fallbackPackageFolders>
    <clear />
  </fallbackPackageFolders>
</configuration>
EOF

COPY Siem.Platform/Siem.Platform.Api/Siem.Platform.Api.csproj Siem.Platform/Siem.Platform.Api/
COPY Siem.Platform/Siem.Platform.Shared.Application.Abstractions/Siem.Platform.Shared.Application.Abstractions.csproj Siem.Platform/Siem.Platform.Shared.Application.Abstractions/
COPY Siem.Platform/Siem.Platform.Shared.Infrastructure/Siem.Platform.Shared.Infrastructure.csproj Siem.Platform/Siem.Platform.Shared.Infrastructure/

COPY Modules/User/Siem.Platform.User.Application/Siem.Platform.User.Application.csproj Modules/User/Siem.Platform.User.Application/
COPY Modules/User/Siem.Platform.User.Domain/Siem.Platform.User.Domain.csproj Modules/User/Siem.Platform.User.Domain/
COPY Modules/User/Siem.Platform.User.Infrastructure/Siem.Platform.User.Infrastructure.csproj Modules/User/Siem.Platform.User.Infrastructure/
COPY Modules/User/Siem.Platform.User.Api/Siem.Platform.User.Api.csproj Modules/User/Siem.Platform.User.Api/

COPY Modules/Logging/Siem.Platform.Logging.Application/Siem.Platform.Logging.Application.csproj Modules/Logging/Siem.Platform.Logging.Application/
COPY Modules/Logging/Siem.Platform.Logging.Domain/Siem.Platform.Logging.Domain.csproj Modules/Logging/Siem.Platform.Logging.Domain/
COPY Modules/Logging/Siem.Platform.Logging.Infrastructure/Siem.Platform.Logging.Infrastructure.csproj Modules/Logging/Siem.Platform.Logging.Infrastructure/
COPY Modules/Logging/Siem.Platform.Logging.Api/Siem.Platform.Logging.Api.csproj Modules/Logging/Siem.Platform.Logging.Api/

RUN dotnet restore Siem.Platform/Siem.Platform.Api/Siem.Platform.Api.csproj \
    --configfile /root/.nuget/NuGet/NuGet.Config

COPY . .

RUN dotnet restore Siem.Platform/Siem.Platform.Api/Siem.Platform.Api.csproj \
    --configfile /root/.nuget/NuGet/NuGet.Config

RUN dotnet publish Siem.Platform/Siem.Platform.Api/Siem.Platform.Api.csproj \
    -c Release -o /app/publish --no-restore \
    /p:PublishTrimmed=false /p:PublishAot=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

EXPOSE 8100
ENV ASPNETCORE_URLS=http://+:8100

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Siem.Platform.Api.dll"]

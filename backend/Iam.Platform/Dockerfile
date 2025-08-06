FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Iam.Platform.sln", "./"]

COPY ["src/Iam.Platform.Domain/Iam.Platform.Domain.csproj", "src/Iam.Platform.Domain/"]
COPY ["src/Iam.Platform.Application/Iam.Platform.Application.csproj", "src/Iam.Platform.Application/"]
COPY ["src/Iam.Platform.Infrastructure/Iam.Platform.Infrastructure.csproj", "src/Iam.Platform.Infrastructure/"]
COPY ["src/Iam.Platform.Api/Iam.Platform.Api.csproj", "src/Iam.Platform.Api/"]

RUN dotnet restore "Iam.Platform.sln"

COPY ["src", "src"]

WORKDIR "/src/src/Iam.Platform.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8095
ENV ASPNETCORE_URLS=http://+:8095

ENTRYPOINT ["dotnet", "Iam.Platform.Api.dll"]

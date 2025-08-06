FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MailService.sln", "./"]

COPY ["src/MailService.Domain/MailService.Domain.csproj", "src/MailService.Domain/"]
COPY ["src/MailService.Application/MailService.Application.csproj", "src/MailService.Application/"]
COPY ["src/MailService.Infrastructure/MailService.Infrastructure.csproj", "src/MailService.Infrastructure/"]
COPY ["src/MailService.Api/MailService.Api.csproj", "src/MailService.Api/"]

RUN dotnet restore "MailService.sln"

COPY ["src", "src"]

WORKDIR "/src/src/MailService.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8090
ENV ASPNETCORE_URLS=http://+:8090

ENTRYPOINT ["dotnet", "MailService.Api.dll"]

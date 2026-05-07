FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["FitLife.TrainingLog.Api/FitLife.TrainingLog.Api.csproj", "FitLife.TrainingLog.Api/"]
COPY ["FitLife.TrainingLog.Tests/FitLife.TrainingLog.Tests.csproj", "FitLife.TrainingLog.Tests/"]
RUN dotnet restore "FitLife.TrainingLog.Api/FitLife.TrainingLog.Api.csproj"

COPY . .
WORKDIR "/src/FitLife.TrainingLog.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR "/src"
RUN dotnet test FitLife.TrainingLog.Tests/FitLife.TrainingLog.Tests.csproj --no-restore

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitLife.TrainingLog.Api.dll"]
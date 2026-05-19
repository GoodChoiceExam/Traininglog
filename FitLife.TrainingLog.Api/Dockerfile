FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FitLife.TrainingLog.slnx .
COPY FitLife.TrainingLog.Api/FitLife.TrainingLog.Api.csproj FitLife.TrainingLog.Api/
COPY FitLife.TrainingLog.Tests/FitLife.TrainingLog.Tests.csproj FitLife.TrainingLog.Tests/
RUN dotnet restore FitLife.TrainingLog.slnx

COPY . .
RUN dotnet test FitLife.TrainingLog.Tests/FitLife.TrainingLog.Tests.csproj --no-restore
RUN dotnet publish FitLife.TrainingLog.Api/FitLife.TrainingLog.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FitLife.TrainingLog.Api.dll"]

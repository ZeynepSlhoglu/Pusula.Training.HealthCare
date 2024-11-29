FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g npm

COPY . .

RUN dotnet tool restore
RUN dotnet tool install -g Volo.Abp.Cli --version 8.0.*
ENV PATH="$PATH:/root/.dotnet/tools"

RUN abp install-libs

WORKDIR /src/src/Pusula.Training.HealthCare.Blazor
RUN dotnet publish "Pusula.Training.HealthCare.Blazor.csproj" -c Release -o /app/blazor

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS blazor
WORKDIR /app

COPY --from=build /app/blazor .

ENTRYPOINT ["dotnet", "Pusula.Training.HealthCare.Blazor.dll"]
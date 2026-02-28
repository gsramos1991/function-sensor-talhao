FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

# Configurações de Globalização e Timezone
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=America/Sao_Paulo \
    ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AF_AgroSolutions.Sinalizador.Talhao/AF_AgroSolutions.Sinalizador.Talhao.csproj", "AF_AgroSolutions.Sinalizador.Talhao/"]
COPY ["src/AgroSolutions.Busines/AgroSolutions.Busines.csproj", "AgroSolutions.Busines/"]
COPY ["src/AgroSolutions.Domain/AgroSolutions.Domain.csproj", "AgroSolutions.Domain/"]
COPY ["src/AgroSolutions.Infra/AgroSolutions.Infra.csproj", "AgroSolutions.Infra/"]

RUN dotnet restore "./AF_AgroSolutions.Sinalizador.Talhao/AF_AgroSolutions.Sinalizador.Talhao.csproj"

COPY . .

WORKDIR "src/AF_AgroSolutions.Sinalizador.Talhao"

RUN dotnet build "AF_AgroSolutions.Sinalizador.Talhao.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AF_AgroSolutions.Sinalizador.Talhao.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
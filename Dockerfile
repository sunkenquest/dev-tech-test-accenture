FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY dev-tech-test-accenture/dev-tech-test-accenture.csproj ./dev-tech-test-accenture/
RUN dotnet restore ./dev-tech-test-accenture/dev-tech-test-accenture.csproj

COPY dev-tech-test-accenture/ ./dev-tech-test-accenture/
RUN dotnet publish ./dev-tech-test-accenture/dev-tech-test-accenture.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080

USER appuser
EXPOSE 8080

ENTRYPOINT ["dotnet", "dev-tech-test-accenture.dll"]
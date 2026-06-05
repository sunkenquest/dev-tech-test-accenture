FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY dev-tech-test-accenture/dev-tech-test-accenture.csproj ./dev-tech-test-accenture/
COPY dev-tech-test-accenture.Tests/dev-tech-test-accenture.Tests.csproj ./dev-tech-test-accenture.Tests/
RUN dotnet restore ./dev-tech-test-accenture/dev-tech-test-accenture.csproj
RUN dotnet restore ./dev-tech-test-accenture.Tests/dev-tech-test-accenture.Tests.csproj

COPY dev-tech-test-accenture/ ./dev-tech-test-accenture/
COPY dev-tech-test-accenture.Tests/ ./dev-tech-test-accenture.Tests/

FROM build AS test
RUN dotnet test ./dev-tech-test-accenture.Tests/dev-tech-test-accenture.Tests.csproj \
        --no-restore \
        --verbosity normal

FROM build AS publish
RUN dotnet publish ./dev-tech-test-accenture/dev-tech-test-accenture.csproj \
        --configuration Release \
        --no-restore \
        --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080
USER appuser
EXPOSE 8080
ENTRYPOINT ["dotnet", "dev-tech-test-accenture.dll"]
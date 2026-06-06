# Coffee Machine API

A .NET 9 Web API that controls an imaginary internet-connected coffee machine.

## API Behaviour

### `GET /brew-coffee`

| Condition | Status | Body |
|-----------|--------|------|
| Normal request | `200 OK` | JSON with message and timestamp |
| Every 5th call (per session) | `503 Service Unavailable` | Empty |
| April 1st (any call) | `418 I'm a Teapot` | Empty |

**200 OK response example:**
```json
{
  "message": "Your piping hot coffee is ready",
  "prepared": "2021-02-03T11:56:24+0900"
}
```

> **Note:** The 5th-call counter is tracked **per session**. Each new browser session or API client starts its own counter independently.


---
## Running Locally (without Docker)

Requires the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
# 1. Restore dependencies
dotnet restore

# 2. Run the API
dotnet run --project dev-tech-test-accenture/dev-tech-test-accenture.csproj
```

The API starts in **Development** mode automatically, which enables Swagger UI.

| Profile | Base URL | Swagger |
|---------|----------|---------|
| HTTP | http://localhost:5027 | http://localhost:5027/swagger |
| HTTPS | https://localhost:7241 | https://localhost:7241/swagger |

To choose a specific profile:

```bash
# HTTP only
dotnet run --project dev-tech-test-accenture/dev-tech-test-accenture.csproj --launch-profile http

# HTTPS
dotnet run --project dev-tech-test-accenture/dev-tech-test-accenture.csproj --launch-profile https
```

If running HTTPS for the first time, trust the dev certificate:

```bash
dotnet dev-certs https --trust
```

### Run tests locally

```bash
dotnet test dev-tech-test-accenture.Tests/dev-tech-test-accenture.Tests.csproj
```

To see verbose output:

```bash
dotnet test dev-tech-test-accenture.Tests/dev-tech-test-accenture.Tests.csproj --verbosity normal
```
---

## Running with Docker

Requires [Docker Desktop](https://www.docker.com/get-started) installed and running.

From the project root (where `docker-compose.yml` lives):

```bash
# Start the API
docker compose up api
```

This will automatically:
1. Restore NuGet packages
2. Run all unit tests — the API will not start if tests fail
3. Start the API on **http://localhost:8080**

| URL | Description |
|-----|-------------|
| http://localhost:8080/brew-coffee | Main endpoint |
| http://localhost:8080/swagger | Swagger UI (interactive docs) |

**Other Docker commands:**

```bash
# Run tests only — launches a temporary container and drops you inside it
docker compose run --rm test

# Then inside the container, run:
dotnet test ./dev-tech-test-accenture.Tests/dev-tech-test-accenture.Tests.csproj
```

---

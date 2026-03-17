# Music Service

Mikrostoritev `music-service` upravlja glasbeno knjiznico sistema.

## Funkcionalnosti

- CRUD nad skladbami (`/api/songs`)
- Iskanje skladb (`/api/songs/search?term=...`)
- Filtriranje po zanru in izvajalcu (`/api/songs?genre=...&artist=...`)
- OpenAPI/Swagger (`/swagger` v Development)

## Tehnologija

- ASP.NET Core Web API (.NET 10)
- EF Core + PostgreSQL
- xUnit testi (repozitorij + vse koncne tocke)

## Lokalni zagon

```powershell
cd C:\Users\Klemen\RiderProjects\ITA-music-repo\music-service
docker compose up --build
```

API bo dosegljiv na `http://localhost:8080`, Swagger na `http://localhost:8080/swagger`.

## Testi

```powershell
cd C:\Users\Klemen\RiderProjects\ITA-music-repo\music-service\WorkerService1
dotnet test WorkerService1.sln
```


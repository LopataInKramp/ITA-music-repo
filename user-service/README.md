# User Service (gRPC)

Mikrostoritev za upravljanje uporabnikov v glasbenem informacijskem sistemu.

## Funkcionalnosti

- `CreateUser`
- `GetUser`
- `ListUsers`
- `UpdateUser`
- `DeleteUser`

## Tehnologije

- Node.js
- gRPC (`@grpc/grpc-js`, `@grpc/proto-loader`)
- SQLite (`better-sqlite3`)
- Logiranje (`pino`)
- Unit testi (`node:test`)

## Zagon

```bash
npm install
npm start
```

Storitev se privzeto zazene na `0.0.0.0:50051`.

## Testi

```bash
npm test
```

## Docker

```bash
docker build -t user-service .
docker run --rm -p 50051:50051 user-service
```

## CI

GitHub Actions workflow v `.github/workflows/ci.yml` ob vsakem `push` izvede teste in Docker build.


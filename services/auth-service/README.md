# Iron Exiles Auth Service

Minimal authentication microservice for Iron Exiles. Provides register/login endpoints with JWT issuance for game client authentication.

## Quick Start

```bash
# From project root:
docker compose up

# Service available at http://localhost:3000
```

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check (DB connectivity) |
| POST | `/register` | Create new account |
| POST | `/login` | Authenticate, receive JWT |
| POST | `/validate` | Validate a JWT (used by game server) |

## Register

```bash
curl -X POST http://localhost:3000/register \
  -H "Content-Type: application/json" \
  -d '{"email": "player@example.com", "password": "MySecurePass123"}'
```

Response (201):
```json
{
  "account_id": "uuid",
  "email": "player@example.com",
  "created_at": "2026-06-29T..."
}
```

## Login

```bash
curl -X POST http://localhost:3000/login \
  -H "Content-Type: application/json" \
  -d '{"email": "player@example.com", "password": "MySecurePass123"}'
```

Response (200):
```json
{
  "token": "eyJhbG...",
  "account_id": "uuid",
  "expires_at": "2026-06-30T..."
}
```

## Validate Token (Game Server)

```bash
curl -X POST http://localhost:3000/validate \
  -H "Content-Type: application/json" \
  -d '{"token": "eyJhbG..."}'
```

Response (200):
```json
{
  "valid": true,
  "account_id": "uuid",
  "email": "player@example.com",
  "expires_at": "2026-06-30T..."
}
```

## Business Rules

- **BR-1:** Passwords hashed with bcrypt (min 8 characters enforced)
- **BR-2:** JWT expires in 24h (configurable via `JWT_EXPIRES_IN`)
- **BR-3:** Login rate limited: 5 attempts/minute/IP
- **BR-4:** Game server rejects connections without valid token (set `AUTH_MODE_ENABLED=false` to bypass for dev)

## Environment Variables

See `../../.env.example` for all configuration options.

## Running Tests

```bash
# Requires PostgreSQL running (use docker compose for DB):
docker compose up postgres -d
cd services/auth-service
npm install
DB_HOST=localhost npm test
```

## Architecture

```
Client (Unity) --> POST /login --> Auth Service --> PostgreSQL
                                        |
Game Server --> POST /validate --> Auth Service
```

The Unity client authenticates via the login screen, receives a JWT, then includes it in the Netcode connection payload. The dedicated server validates this token via the `/validate` endpoint before approving the connection.

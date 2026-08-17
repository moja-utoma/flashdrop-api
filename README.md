# FlashDrop API

A limited-inventory flash-sale platform, built as a learning project focused on **concurrency, thread safety, caching, and building APIs that survive real traffic contention**.

Full project write-up (business case, domain model, architecture decisions): see [`/docs`](./docs) or the linked [FlashDrop Docs](#) repo.

## Why this project exists

It's built specifically to force hard problems: many concurrent requests racing over the same limited stock, cache invalidation under write pressure, async messaging with retries and idempotency, and SQL-level locking and deadlocks reproduced and fixed on purpose. See [Learning goals](#learning-goals) below.

## Tech stack

- .NET 8, ASP.NET Core Web API
- SQL Server / EF Core
- Redis (cache-aside)
- RabbitMQ (outbox pattern, retries, DLQ)
- JWT auth
- Serilog
- xUnit + WebApplicationFactory, NBomber/k6 for load testing

## Getting started

```bash
git clone https://github.com/yourname/flashdrop-api.git
cd flashdrop-api
dotnet restore
dotnet ef database update
dotnet run
```

Swagger UI available at `/swagger` once running.

### Prerequisites
- .NET 8 SDK
- SQL Server (local or Docker)
- Redis (local or Docker)
- RabbitMQ (local or Docker)

A `docker-compose.yml` for local dependencies is provided at the repo root — `docker compose up -d` before running the app.

## Project status

Tracked on the [FlashDrop project board](#). Current milestone: **_(update as you go)_**.

## Learning goals

| Area | Status |
|---|---|
| Web API lifecycle & middleware | 🔲 |
| Auth (JWT, roles, OAuth2/OIDC) | 🔲 |
| Concurrency & thread safety | 🔲 |
| SQL window functions | 🔲 |
| SQL transactions & locking | 🔲 |
| Redis caching | 🔲 |
| Queues / messaging (RabbitMQ) | 🔲 |
| Design patterns | 🔲 |
| C# advanced types | 🔲 |

## Architecture notes

See [`/docs/domain-model.md`](./docs/domain-model.md) for the full entity design and [`/docs/architecture.md`](./docs/architecture.md) for decisions worth remembering (why optimistic vs pessimistic concurrency was chosen where, why the outbox pattern is used, etc.).

## Running tests

```bash
dotnet test
```

Load tests live in `/loadtests` — see that folder's README for how to run them against a local instance.

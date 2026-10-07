# FlashDrop — Limited Inventory Flash-Sale Platform

## 1. Business Overview

**The pitch:** A platform where sellers list products with strictly limited stock and a defined sale window ("100 units, sale starts at 12:00:00 PM"). Buyers race to purchase before stock runs out or the sale ends. Think concert tickets, sneaker drops, or limited hardware restocks (PS5-launch style).

**Why this business model, technically:** it creates real contention. At the moment a sale opens, potentially thousands of requests hit the same handful of database rows within milliseconds. This isn't artificial load — it's the natural shape of the business, which is exactly why it's a good teaching project: every hard problem on your list (locking, race conditions, caching invalidation, queueing, idempotency) shows up because the *business* needs it, not because you bolted it on.

### Core actors
- **Admin/Seller** — creates products, defines stock count, schedules sale windows, views sales analytics.
- **Customer** — browses upcoming/live sales, purchases limited-stock items, views order history.
- **System** — enforces that stock never goes negative, processes orders asynchronously, expires unpaid reservations, sends confirmations.

### Core business rules (these drive the technical design)
1. Stock count is a hard ceiling — two customers can never both "win" the last unit.
2. A purchase reserves stock for a short window (e.g. 10 minutes) to allow "payment"; if unpaid, the reservation releases back to available stock.
3. Sales have a start and end time; no purchases allowed outside that window.
4. A customer can buy at most N units per sale (prevents one user draining stock — adds another contention dimension).
5. Order confirmation (email/notification) must eventually be sent, even if the notification service is briefly down — but never sent twice for the same order.
6. Admins need near-real-time visibility into remaining stock and order velocity during a live sale.

### Suggested milestones (business-flavored, doubles as your build order)
1. **MVP catalog** — browse products, view a sale, see live stock count.
2. **Purchase flow** — authenticated purchase against limited stock, correctness under concurrency.
3. **Resilience** — the system survives a stock decrement it can't naively support (this is where you deliberately break it, then fix it).
4. **Async order pipeline** — confirmations, retries, dead-letter handling.
5. **Admin analytics** — rankings, time-series views, per-sale reporting (window functions).
6. **Hardening** — caching, load testing, graceful degradation under traffic spikes.

---

## 2. Backend — ASP.NET Core Web API

### 2.1 Suggested tech stack
- **.NET 8, ASP.NET Core Web API**
- **SQL Server** (or PostgreSQL if you prefer — either works for the window-function/locking practice; SQL Server gives you `ROWLOCK`/`UPDLOCK` hints which are nice to see explicitly)
- **EF Core** for the general CRUD surface, with a couple of places where you deliberately drop to raw SQL (to practice both ORM concurrency tokens *and* explicit locking)
- **Redis** for the live stock cache and cache-aside pattern
- **RabbitMQ** for order confirmation events, retries, and a dead-letter queue
- **Serilog** for structured logging
- **JWT Bearer auth**, roles: `Customer`, `Admin`
- **xUnit + WebApplicationFactory** for integration tests; **NBomber or k6** for load testing (this is how you'll actually *see* the race conditions and later confirm the fix)
- **Swagger/OpenAPI** for the API surface

### 2.2 Domain model (starting point)

| Entity | Key fields | Notes |
|---|---|---|
| `Product` | Id, Name, Description, Price | Owned by a seller/admin |
| `Sale` | Id, ProductId, TotalStock, AvailableStock, StartsAt, EndsAt | The contended resource lives here |
| `Reservation` | Id, SaleId, UserId, Quantity, Status (Pending/Paid/Expired/Cancelled), CreatedAt, ExpiresAt | Represents "held" stock before confirmed |
| `Order` | Id, ReservationId, UserId, Status, IdempotencyKey, CreatedAt | Created once payment "succeeds" |
| `OutboxMessage` | Id, Type, Payload, ProcessedAt | For reliable event publishing (transactional outbox pattern) |
| `User` | Id, Email, PasswordHash, Role | Standard identity |

### 2.3 API surface (representative, not exhaustive)

**Public / customer**
- `GET /api/sales` — list sales (live, upcoming, ended) — cached
- `GET /api/sales/{id}` — sale detail with live available-stock — cache-aside from Redis
- `POST /api/sales/{id}/reserve` — the hot path: attempt to reserve N units. **This is the endpoint you'll build three times**: naive → broken under load → fixed with proper concurrency control
- `POST /api/orders/{reservationId}/confirm` — simulate payment, finalize order, publish `OrderConfirmed` event
- `GET /api/orders/mine` — order history

**Admin**
- `POST /api/products`, `POST /api/sales` — standard authorized CRUD
- `GET /api/sales/{id}/analytics` — orders per minute, top buyers, remaining stock over time (window functions + CTEs)

**Cross-cutting**
- Global exception handling middleware → consistent problem-details responses
- Custom logging/action filter → structured request/response logging with correlation IDs
- Rate limiting middleware on `/reserve` (built-in ASP.NET Core rate limiting — nice complement to the concurrency work)

### 2.4 Where each of your learning topics lands

- **Concurrency & thread safety** → the `/reserve` endpoint. Build it naively first (read stock, check, decrement — classic TOCTOU race), load-test it, watch it oversell, then fix with EF Core concurrency tokens (`RowVersion`) or explicit `UPDLOCK, ROWLOCK` SQL, and compare against an in-memory `SemaphoreSlim`-per-sale approach to *feel* the difference between DB-level and app-level locking.
- **SQL transactions/locking/blocking** → wrap reservation creation + stock decrement in a transaction; deliberately reproduce a deadlock (two transactions locking `Sale` and `Reservation` rows in opposite order) and fix it by fixing lock ordering.
- **Redis/caching** → cache sale listings and product detail (cache-aside), invalidate on stock change; consider caching *available stock* with a short TTL as a read-optimization in front of the authoritative DB value, and discuss/experiment with why you can't use the cache as the source of truth for the decrement itself.
- **Queues/messaging** → `OrderConfirmed` published via outbox pattern to RabbitMQ; a consumer sends the "email"; add retry with backoff and a DLQ; use the idempotency key so a redelivered message doesn't double-process.
- **SQL window functions** → admin analytics: `ROW_NUMBER`/`RANK` for leaderboard of top buyers, `LAG`/`LEAD` for stock-depletion-rate over time, paginate order history with `ROW_NUMBER` + CTE.
- **Auth** → JWT with role claims, `[Authorize(Roles = "Admin")]` on management endpoints, refresh tokens if you want to go further, OAuth2/OIDC as a stretch goal (e.g. swap in an external identity provider like Keycloak or Duende IdentityServer later).
- **Design patterns** → Repository/Unit of Work around the EF context, Strategy for pluggable "payment" simulation, Decorator for a caching layer around a repository, Mediator (MediatR) for the reserve→publish→respond flow if you want to decouple it.
- **C# advanced types** → custom `[Idempotent]` attribute + filter, events for domain notifications, extension methods for mapping entities to DTOs, delegates for retry-policy configuration.
- **Web API lifecycle** → this whole project gives you a real request pipeline to reason about: rate limiter → auth → routing → custom logging filter → controller → EF/Redis → response. Good material for explaining it in a code review.

---

## 3. Frontend — Angular

### 3.1 Suggested tech stack
- **Angular (latest LTS)**, standalone components, Angular Signals for local state
- **RxJS** for the live-stock polling/streaming stream — this is a natural place to practice reactive patterns on the frontend that mirror your backend concurrency work
- **Angular Material** or **Tailwind** for UI — pick whichever you want more practice with; Tailwind if you want more control, Material if you want to move faster
- **JWT auth** stored appropriately (httpOnly cookie ideally, or interceptor-based bearer token if you keep it simple)
- **HTTP interceptors** for auth token attachment, global error handling, and correlation-ID propagation matching the backend logging
- Optional: **SignalR** (backend) + Angular client, or simple polling, to push live stock updates during an active sale — great way to demonstrate the backend's cache/queue work visually

### 3.2 Key screens
1. **Sales list** — upcoming/live/ended, live countdown to `StartsAt`
2. **Sale detail** — live "X of 100 remaining" (via polling or SignalR), quantity selector, reserve button, disabled instantly on sellout
3. **Reservation/checkout** — countdown showing reservation expiry, confirm button
4. **Order history** — customer's past orders and their status
5. **Admin dashboard** — create product/sale, live analytics view (orders/minute chart, top buyers), demonstrates the window-function-backed endpoints visually

### 3.3 Why this frontend is a good match
The interesting frontend problem here isn't CRUD forms — it's **handling contention and staleness gracefully from the client's point of view**: optimistic UI when reserving stock, immediately reflecting a sellout, handling a `409 Conflict` from `/reserve` gracefully (someone else got the last unit), and reconciling live stock counts against user actions. It gives Angular practice that's actually tied to the backend concepts instead of being a disconnected UI shell.

---

## 4. Suggested build order

1. Domain model + EF migrations + basic CRUD for `Product`/`Sale` (no concurrency yet)
2. JWT auth + roles
3. Naive `/reserve` endpoint → load test with k6/NBomber → observe overselling
4. Fix with DB-level concurrency control (concurrency token or explicit locking) → re-run load test → confirm correctness
5. Redis cache-aside for sale listings/detail + invalidation on stock change
6. Reservation expiry background service (`IHostedService`, `CancellationToken`)
7. Outbox + RabbitMQ publisher/consumer for order confirmation, retry + DLQ, idempotency
8. Admin analytics endpoints with window functions
9. Global exception handling, logging filter, Swagger polish, tests
10. Angular frontend wired up end-to-end, including the "handle sellout gracefully" UX
11. Stretch: rate limiting, SignalR live stock push, OAuth2/OIDC swap-in

---

## 5. What "done" looks like

You'll know the project has done its job when you can:
- Run a load test against the naive `/reserve` endpoint and reproduce overselling on command.
- Explain, with the deadlock you caused and fixed, exactly what SQL Server (or Postgres) was doing and why the fix worked.
- Point to the cache invalidation path and explain why the cache can never be the source of truth for the decrement.
- Show a message getting retried and landing in the DLQ, and explain how the idempotency key prevents a duplicate order.
- Walk through the full request lifecycle for `/reserve` from an interviewer's "explain how this API works" question, middleware by middleware.

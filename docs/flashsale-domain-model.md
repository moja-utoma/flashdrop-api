# FlashDrop — Domain Model

A detailed entity design for the flash-sale/limited-inventory platform, close to what you'd actually implement in EF Core.

## Entities

### `User`
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| Email | string | unique index |
| PasswordHash | string | |
| Role | enum: `Customer`, `Admin` | consider a join table later if you want multi-role |
| CreatedAt | DateTimeOffset | |
| IsActive | bool | soft-disable instead of hard delete |

### `Product`
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| SellerId | Guid | FK → User (Admin) |
| Name | string | |
| Description | string | |
| ImageUrl | string | |
| BasePrice | decimal(10,2) | |
| Category | string / enum | optional, useful for filtering the sales list |
| CreatedAt | DateTimeOffset | |

A `Product` is the catalog entry; it can have zero or more `Sale`s over its lifetime (e.g. the same sneaker restocked three times).

### `Sale` — the contended resource
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| ProductId | Guid | FK |
| TotalStock | int | immutable once sale starts |
| AvailableStock | int | **the hot field** — decremented under contention |
| PricePerUnit | decimal(10,2) | can differ from `Product.BasePrice` (sale pricing) |
| MaxUnitsPerUser | int | enforces "no single user drains stock" business rule |
| StartsAt | DateTimeOffset | |
| EndsAt | DateTimeOffset | |
| Status | enum: `Scheduled`, `Live`, `Ended`, `Cancelled` | can be derived from timestamps, but materializing it simplifies queries/indexing |
| RowVersion | byte[] (rowversion/timestamp) | **EF Core optimistic concurrency token** — this is what makes the naive vs. fixed `/reserve` comparison possible |

Index: `(Status, StartsAt)` for the sales-list query; this table is also your Redis cache-aside target for stock lookups.

### `Reservation` — the "hold"
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| SaleId | Guid | FK |
| UserId | Guid | FK |
| Quantity | int | |
| Status | enum: `Pending`, `Paid`, `Expired`, `Cancelled` | |
| CreatedAt | DateTimeOffset | |
| ExpiresAt | DateTimeOffset | `CreatedAt` + hold window (e.g. 10 min) |
| ConfirmedAt | DateTimeOffset? | set when it transitions to `Paid` |

Index: `(SaleId, UserId)` to enforce the per-user purchase cap efficiently; `(Status, ExpiresAt)` for the background expiry sweep.

This table is the audit trail of *every attempt to claim stock*, successful or not — worth keeping even expired/cancelled rows rather than deleting them, since that's what your admin analytics (sell-through rate, abandonment rate) will query against.

### `Order` — the finalized purchase
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| ReservationId | Guid | FK, 1:1 with a `Paid` reservation |
| UserId | Guid | FK (denormalized for query convenience) |
| SaleId | Guid | FK (denormalized) |
| Quantity | int | |
| TotalPrice | decimal(10,2) | |
| Status | enum: `Created`, `ConfirmationSent`, `Failed` | tracks the async messaging pipeline, not payment |
| IdempotencyKey | Guid (or string) | unique index — this is what makes retrying "confirm payment" safe |
| CreatedAt | DateTimeOffset | |

### `OutboxMessage` — reliable event publishing
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| Type | string | e.g. `"OrderConfirmed"` |
| Payload | string (JSON) | serialized event |
| CreatedAt | DateTimeOffset | |
| ProcessedAt | DateTimeOffset? | null until a background publisher picks it up |
| RetryCount | int | |

This exists so that "decrement stock + create order" and "publish OrderConfirmed to RabbitMQ" happen atomically from the app's point of view — you write the outbox row in the *same DB transaction* as the order, then a separate background service reads unprocessed rows and publishes them. It's the standard fix for "what if the DB commit succeeds but the RabbitMQ publish fails."

### `NotificationLog` (optional, but nice for the DLQ story)
| Field | Type | Notes |
|---|---|---|
| Id | Guid | PK |
| OrderId | Guid | FK |
| Channel | enum: `Email` | |
| Status | enum: `Sent`, `Failed`, `DeadLettered` | |
| Attempts | int | |
| LastAttemptAt | DateTimeOffset | |

Gives you something concrete to point at when demonstrating "this message failed 3 times and landed in the DLQ, here's the log row proving it."

## Relationships at a glance

```
User (Admin) 1───* Product 1───* Sale 1───* Reservation 1───0..1 Order 1───* NotificationLog
User (Customer) 1───* Reservation
Order 1───* OutboxMessage (or OutboxMessage is generic and just references OrderId in payload)
```

## Why this level of detail matters

- **`Sale.RowVersion`** is what lets you demonstrate optimistic concurrency (EF throws `DbUpdateConcurrencyException` on a stale write) as one option, versus explicit pessimistic locking (`SELECT ... WITH (UPDLOCK, ROWLOCK)`) as the other — you genuinely need both paths modeled to compare them.
- **`Reservation` as a separate table from `Order`**, rather than collapsing them, is what gives you the expiry/release mechanism to build a background hosted service around — and it mirrors how real ticketing/inventory systems actually work.
- **`IdempotencyKey` on `Order`** and **`OutboxMessage`** together are what make the queue/retry/DLQ section non-trivial instead of a toy example — you're implementing the actual patterns production systems use for exactly-once-effect delivery over an at-least-once transport.
- Keeping **expired/cancelled reservations** instead of deleting them is what feeds your window-function analytics (conversion rate, abandonment, depletion curve) — without that data, `LAG`/`LEAD` wouldn't have anything interesting to compute over.

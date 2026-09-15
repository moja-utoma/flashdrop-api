# Commit Message Convention

Format:
```
<type>(<scope>): <short summary>

<body — optional, explain why not just what>

<footer — optional, e.g. Closes #12>
```

## Types
- `feat` — new functionality
- `fix` — bug fix
- `refactor` — behavior-unchanged code change (e.g. introducing a pattern)
- `test` — adding/adjusting tests
- `perf` — performance work (concurrency/caching fixes land here)
- `docs` — README, domain model, comments
- `chore` — tooling, CI, scaffolding
- `style` — formatting only

## Scope
Name the area — matches topic labels: `reserve`, `auth`, `cache`, `outbox`, `sales`, `orders`, etc.

## Rules of thumb
- Subject line: imperative mood, under ~50 chars ("add X", not "added X")
- Always link the issue in the footer: `Closes #12` or `Refs #12`
- For anything non-obvious (a fix, a deliberate break-then-fix, a concurrency change) — **write the body**. The subject says what changed; the body is where the reasoning and the lesson live. Future-you needs the body.

## Example
```
fix(reserve): prevent overselling with optimistic concurrency token

Load testing with k6 showed the naive read-check-decrement caused
overselling under concurrent requests. Switched Sale.AvailableStock
updates to use EF Core's RowVersion concurrency token and retry on
DbUpdateConcurrencyException.

Closes #14
```

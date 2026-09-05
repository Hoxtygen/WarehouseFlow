# WarehouseFlow

A production-ready warehouse management REST API built with **ASP.NET Core 10**, **Entity Framework Core 10**, and **PostgreSQL**. It handles the full order lifecycle — from product inventory stocking through order placement, inventory reservation, payment processing, and automated reservation expiry — with clean architecture, strong consistency guarantees, and security baked in from the start.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [Background Services](#background-services)
- [Audit Logging](#audit-logging)
- [Error Handling](#error-handling)
- [Configuration](#configuration)
- [Getting Started](#getting-started)
- [Database Migrations](#database-migrations)
- [Tech Stack](#tech-stack)

---

## Features

- **Order lifecycle management** — Pending → Reserved → Paid → Dispatched → Delivered / Cancelled
- **Pessimistic inventory locking** via `SELECT … FOR UPDATE` to eliminate overselling under concurrent load
- **Multi-warehouse stock distribution** — a single order draws from multiple warehouses when needed
- **15-minute reservation window** with automated background expiry and stock return
- **JWT Bearer authentication** with ASP.NET Core Identity
- **Role-based access control** across 10 distinct roles
- **Tiered rate limiting** — global (100 req/min per IP), strict auth (5 req/min per IP), order placement (10 req/min per user)
- **Automatic audit logging** via an EF Core `SaveChangesInterceptor` — every entity mutation is recorded in the same transaction
- **Structured logging** with Serilog (console + rolling file)
- **RFC 7807 ProblemDetails** error responses on every failure path
- **Security headers** middleware (CSP, HSTS, X-Frame-Options, etc.)
- **Clean Architecture** — Application layer has zero infrastructure dependencies

---

## Architecture

The solution follows Clean Architecture with a strict dependency rule: outer layers depend on inner layers, never the reverse.

```
┌─────────────────────────────────────────────────┐
│                   API Layer                      │
│  Controllers · Middleware · Rate Limiting        │
│  JWT Auth · Swagger · Security Headers           │
└──────────────────┬──────────────────────────────┘
                   │ depends on
┌──────────────────▼──────────────────────────────┐
│              Application Layer                   │
│  Business Services · Interfaces · DTOs           │
│  Domain Exceptions · Validation logic            │
└──────────────────┬──────────────────────────────┘
                   │ depends on
┌──────────────────▼──────────────────────────────┐
│               Domain Layer                       │
│  Entities · Enums · Domain Exceptions            │
└─────────────────────────────────────────────────┘
         ▲                    ▲
         │ implements         │ implements
┌────────┴────────────────────┴───────────────────┐
│             Infrastructure Layer                 │
│  EF Core · Repositories · UnitOfWork            │
│  Identity · JWT · Migrations · Interceptors      │
│  Background Services                            │
└─────────────────────────────────────────────────┘
```

**Key design decisions:**

- **No CQRS, no MediatR, no generic `Repository<T>`** — deliberate. Each aggregate has a focused, purpose-built repository interface.
- **`IUnitOfWork` seam** — services orchestrate transactions via `BeginTransactionAsync / CommitAsync / RollbackAsync`. Repositories never call `SaveChangesAsync`.
- **One scoped `AppDbContext`** shared by all repositories and `IUnitOfWork` in a request — this is a hard invariant. Registering `DbContext` as transient would break it.
- **`AuditInterceptor` is singleton** — EF resolves interceptors once per `DbContextOptions`. `IHttpContextAccessor` (also singleton) safely accesses per-request state via an internal `AsyncLocal`.

---

## Project Structure

```
WarehouseFlow/
├── WarehouseFlow.sln
└── src/
    ├── WarehouseFlow.Api/
    │   ├── controllers/            # HTTP controllers
    │   ├── Contracts/              # ApiResponse<T> wrapper
    │   ├── Middleware/             # ApiExceptionHandler, SecurityHeadersMiddleware
    │   ├── appsettings.json
    │   ├── appsettings.Example.json
    │   └── Program.cs              # Composition root
    │
    ├── WarehouseFlow.Application/
    │   ├── Interfaces/             # Service + repository contracts
    │   ├── Services/               # Business logic (CustomerService, OrderService, …)
    │   ├── Dtos/                   # Request/response DTOs
    │   ├── Validations/            # Input validators
    │   └── DependencyInjection.cs
    │
    ├── WarehouseFlow.Domain/
    │   ├── Entities/               # Domain entities + AuditLog
    │   ├── Enum/                   # OrderStatus, UserRole
    │   └── Exceptions/             # Domain exception types
    │
    └── WarehouseFlow.Infrastructure/
        ├── Data/
        │   ├── AppDbContext.cs
        │   ├── Configurations/     # IEntityTypeConfiguration per entity
        │   └── Interceptors/       # AuditInterceptor
        ├── Repositories/           # EF Core repository implementations
        ├── Implementations/        # AuthenticationService, TokenService
        ├── Identity/               # IdentitySeeder (roles + Super Admin)
        ├── BackgroundServices/     # ReservationCleanupService
        ├── Migrations/
        └── DependencyInjection.cs
```

---

## Domain Model

### Entities

| Entity | Description |
|---|---|
| `Customer` | A registered customer with address and a 1-to-1 ASP.NET Identity link |
| `Employee` | A staff member with role, department, and an Identity link |
| `Warehouse` | A physical location with a fixed capacity (unit count) |
| `Product` | A catalogued item with SKU (auto-generated), price, and category |
| `ProductCategory` | A grouping label for products |
| `Inventory` | Stock of a `Product` in a `Warehouse` — tracks `AvailableQuantity` and `ReservedQuantity` |
| `Order` | A customer's purchase intent, holding `OrderItems` and `Reservations` |
| `OrderItem` | One line of an `Order` (product × quantity × unit price) |
| `Reservation` | A temporary hold on inventory for a pending order; expires after 15 minutes |
| `Payment` | A payment record linking an `Order` to the amount paid |
| `AuditLog` | Immutable record of every entity state change (see [Audit Logging](#audit-logging)) |

### Order Status Flow

```
Pending ──► Reserved ──► Paid ──► Dispatched ──► Delivered
   │
   └──► Cancelled  (manual or automatic on reservation expiry)
```

### User Roles

| Role | Typical access |
|---|---|
| `Super_Admin` | Full system access; creates other employees |
| `Admin` | Manage products, warehouses, employees |
| `Warehouse_Manager` | Manage inventory in their warehouse |
| `Inventory_Manager` | Create and adjust inventory records |
| `Sales_Representative` | View orders and customers |
| `Dispatcher` | Mark orders as dispatched |
| `Accountant` | View payment records |
| `Support_Staff` | Read-only customer / order data |
| `Driver` | View dispatch assignments |
| `Quality_Assurance` | Inspection and quality workflows |
| `Customer` | Place orders and make payments |

---

## API Endpoints

All successful responses are wrapped in `ApiResponse<T>`:

```json
{
  "success": true,
  "statusCode": 201,
  "message": "Order created successfully",
  "data": { ... },
  "errors": null
}
```

Error responses follow RFC 7807 `ProblemDetails`:

```json
{
  "status": 400,
  "title": "Insufficient stock for Product …",
  "type": "https://httpstatuses.com/400"
}
```

---

### Authentication — `api/v1/auth`

**Rate limit:** 5 requests per IP per minute (credential-stuffing protection).

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/auth/login` | Public | Sign in with email + password; returns a JWT |
| `POST` | `/api/v1/auth/register` | Public | Self-register as a `Customer` |
| `POST` | `/api/v1/auth/employee/register` | `Super_Admin` | Create an employee account with a specified role |

#### Login

```json
// POST /api/v1/auth/login
{
  "email": "user@example.com",
  "password": "P@ssw0rd!"
}

// 200 OK
{
  "success": true,
  "statusCode": 200,
  "message": "Login successful.",
  "data": {
    "token": "<JWT>",
    "expiresAt": "2026-09-05T11:30:00Z"
  }
}
```

---

### Warehouses — `api/v1/warehouses`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/warehouses` | `Super_Admin`, `Admin` | Register a new warehouse with a capacity limit |

```json
// POST /api/v1/warehouses
{
  "name": "Lagos Central Warehouse",
  "location": "Apapa, Lagos",
  "capacity": 50000
}
```

---

### Products — `api/v1/products`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/products` | `Super_Admin`, `Admin` | Create a product (SKU auto-generated) |

```json
// POST /api/v1/products
{
  "name": "Industrial Conveyor Belt",
  "description": "Heavy-duty 10m belt",
  "unitPrice": 4500.00,
  "categoryId": "<uuid>"
}
```

---

### Inventory — `api/v1/inventories`

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/inventories` | `Super_Admin`, `Admin`, `Warehouse_Manager` | Stock a product in a warehouse |

```json
// POST /api/v1/inventories
{
  "productId": "<uuid>",
  "warehouseId": "<uuid>",
  "availableQuantity": 1000,
  "reservedQuantity": 0
}
```

> **Capacity check:** The service validates that `availableQuantity + reservedQuantity` does not push the warehouse over its registered capacity limit before inserting.

---

### Orders — `api/v1/orders`

**Rate limit:** 10 requests per authenticated user per minute.

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/orders` | `Customer` | Place a new order |

```json
// POST /api/v1/orders
{
  "orderItems": [
    { "productId": "<uuid>", "quantity": 5 },
    { "productId": "<uuid>", "quantity": 2 }
  ]
}
```

**What happens on order creation (all within one database transaction):**

1. Customer identity is resolved from the JWT `NameIdentifier` claim.
2. For each line item, inventory rows are locked with `SELECT … FOR UPDATE` (pessimistic locking).
3. Available stock is decremented and moved to `ReservedQuantity`, distributed across warehouses as needed.
4. `Reservation` rows are created with a **15-minute expiry**.
5. The `Order`, `OrderItems`, and `Reservations` are committed together.
6. If stock is insufficient for any item, the entire transaction is rolled back and a `400` is returned — no partial orders.

---

### Payments — `api/v1/payment`

**Rate limit:** 10 requests per authenticated user per minute.

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/payment/processPayment` | `Customer` | Pay for a pending order |

```json
// POST /api/v1/payment/processPayment
{
  "orderId": "<uuid>",
  "amount": 22500.00
}
```

**What happens on payment (all within one database transaction):**

1. Validates the caller owns the order.
2. Validates `amount` exactly matches `order.TotalAmount`.
3. Validates the order status is `Pending`.
4. Decrements `ReservedQuantity` on each inventory row touched by the order's reservations.
5. Marks the order as `Paid`.
6. Inserts a `Payment` record.
7. Rolled back entirely on any failure.

---

## Security

### JWT Authentication

- Tokens are signed with HMAC-SHA256 using a secret from configuration.
- Validation enforces: issuer, audience, lifetime, signing key, and **zero clock-skew**.
- Expired or missing tokens return a structured `401 ProblemDetails` (no `WWW-Authenticate` header leakage).
- Forbidden requests return a structured `403 ProblemDetails`.

### Rate Limiting

Three fixed-window policies, partitioned per IP or per authenticated user:

| Policy | Applied to | Limit |
|---|---|---|
| Global | Every request | 100 req / min / IP |
| `auth` | `POST /auth/**` | 5 req / min / IP |
| `orders` | `POST /orders`, `POST /payment/processPayment` | 10 req / min / user |

Rejections return `429 Too Many Requests` as a `ProblemDetails` response.

### Security Headers

Every response includes:

| Header | Value |
|---|---|
| `X-Content-Type-Options` | `nosniff` |
| `X-Frame-Options` | `DENY` |
| `Referrer-Policy` | `strict-origin-when-cross-origin` |
| `Content-Security-Policy` | `default-src 'none'; frame-ancestors 'none'` |
| `Strict-Transport-Security` | `max-age=31536000; includeSubDomains; preload` |
| `Server` | *(removed)* |
| `X-Powered-By` | *(removed)* |

### Role-Based Access Control

Controllers declare `[Authorize(Roles = "…")]` per endpoint. Model validation returns human-readable errors when an invalid role string is submitted.

---

## Background Services

### `ReservationCleanupService`

An `IHostedService` that runs every **3 minutes**. For each batch of expired reservations:

1. Releases `ReservedQuantity` back to `AvailableQuantity` on the affected inventory rows.
2. Marks the parent order as `Cancelled`.
3. Deletes the stale reservation rows.
4. Each order's cleanup runs in its own transaction — a failure on one order does not block the others.

The service creates a DI scope per cycle to safely resolve scoped services (`IOrderService`) from the singleton hosted-service lifetime.

---

## Audit Logging

Every entity mutation (Create / Update / Delete) is automatically recorded in `audit_logs` within the **same database transaction** as the originating operation — no separate round-trip, no risk of orphaned audit records.

### How it works

`AuditInterceptor` extends EF Core's `SaveChangesInterceptor` and hooks into both `SavingChanges` (sync) and `SavingChangesAsync`:

- The change tracker is **snapshotted before EF flushes**, while original values are still readable.
- **Added** — all current property values are recorded.
- **Deleted** — all original values are recorded.
- **Modified** — only changed properties are recorded as `{ "from": …, "to": … }` pairs. Touch-only saves with no differing scalar properties produce no audit row.
- The `AuditLog` table itself is excluded to prevent infinite recursion.
- Caller identity is read from the JWT `NameIdentifier` claim; falls back to `"system"` for background jobs and migrations.
- Client IP honours `X-Forwarded-For` for reverse-proxy deployments.

### `audit_logs` table

| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK, `uuidv7()` default |
| `UserId` | `varchar(450)` | ASP.NET Identity user ID or `"system"` |
| `Action` | `varchar(20)` | `"Added"` \| `"Modified"` \| `"Deleted"` |
| `EntityName` | `varchar(200)` | C# class name of the changed entity |
| `EntityId` | `varchar(200)` | Primary key value(s) of the changed row |
| `ChangesJson` | `jsonb` | Property changes payload (PostgreSQL native JSON) |
| `IpAddress` | `varchar(50)` | Client IP address |
| `CreatedAt` | `timestamptz` | `now()` default — rows are immutable |

**Indexes:** composite `(EntityName, EntityId)` for entity history queries; `UserId` for user-activity queries.

---

## Error Handling

`ApiExceptionHandler` maps domain exceptions to structured `ProblemDetails` responses:

| Exception | HTTP Status |
|---|---|
| `ValidationException` | `400 Bad Request` (with `errors` array extension) |
| `InsufficientStockException` | `400 Bad Request` |
| `NotFoundException` | `404 Not Found` |
| `DuplicateException` | `409 Conflict` |
| `UnauthorizedAccessException` | `401 Unauthorized` |
| Anything else | `500 Internal Server Error` |

Model validation failures (malformed request body) return `400 ValidationProblemDetails` with field-level error messages, including friendly guidance for invalid enum values.

---

## Configuration

Copy `appsettings.Example.json` to `appsettings.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "WarehouseFlowDb": "Host=localhost;Port=5432;Database=warehouseflow;Username=postgres;Password=your-password"
  },
  "JwtSettings": {
    "Secret": "<at-least-32-character-random-string>",
    "Issuer": "WarehouseFlow",
    "Audience": "https://www.warhouseflowclient.com",
    "ExpiryMinutes": 30
  }
}
```

> **Warning:** Never commit real secrets. Use environment variables or a secrets manager in production. `appsettings.json` is gitignored — only `appsettings.Example.json` is tracked.

### Super Admin seeding

On first startup, `IdentitySeeder` creates all roles and a Super Admin account. Pass credentials as command-line arguments:

```bash
dotnet run --project src/WarehouseFlow.Api \
  --SuperAdmin:Email=admin@example.com \
  --SuperAdmin:Password=P@ssw0rd!
```

The seeder is idempotent — it will not create duplicate roles or duplicate Super Admin accounts on subsequent starts.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/) with the `uuidv7()` function available (via `pg_uuidv7` extension or Postgres 17+)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Clone and restore

```bash
git clone <repo-url>
cd WarehouseFlow
dotnet restore
```

### 2. Configure

```bash
cp src/WarehouseFlow.Api/appsettings.Example.json src/WarehouseFlow.Api/appsettings.json
# Edit appsettings.json — set the connection string and JWT secret
```

### 3. Apply migrations

```bash
dotnet ef database update \
  --project src/WarehouseFlow.Infrastructure \
  --startup-project src/WarehouseFlow.Api
```

### 4. Run

```bash
dotnet run --project src/WarehouseFlow.Api \
  --SuperAdmin:Email=admin@example.com \
  --SuperAdmin:Password=P@ssw0rd!
```

Swagger UI is available at **`https://localhost:<port>/swagger`** in the Development environment.

---

## Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> \
  --project src/WarehouseFlow.Infrastructure \
  --startup-project src/WarehouseFlow.Api

# Apply pending migrations
dotnet ef database update \
  --project src/WarehouseFlow.Infrastructure \
  --startup-project src/WarehouseFlow.Api

# Roll back to a specific migration
dotnet ef database update <PreviousMigrationName> \
  --project src/WarehouseFlow.Infrastructure \
  --startup-project src/WarehouseFlow.Api
```

---

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 10 / ASP.NET Core 10 |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL 15+ (Npgsql driver) |
| Identity | ASP.NET Core Identity |
| Authentication | JWT Bearer (`Microsoft.IdentityModel.Tokens`) |
| Logging | Serilog (console + rolling daily file) |
| API documentation | Swagger / OpenAPI |
| Background jobs | `IHostedService` / `BackgroundService` |
| Rate limiting | ASP.NET Core built-in `RateLimiter` |
| JSON serialization | `System.Text.Json` |

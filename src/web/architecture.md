# Architecture Guide — LastMile TMS

> Team document. Describes the frontend structure, rules, and standards.

---

## Stack

| Layer | Technology |
|---|---|
| Framework | Next.js 16, App Router |
| UI | React 19, shadcn/ui, Tailwind CSS 4 |
| Server state | TanStack Query |
| Client state | React Context + hooks |
| Forms | react-hook-form + Zod |
| Typing | TypeScript, manual typing |
| Authentication | OAuth 2.0 (OpenIddict), access + refresh token |
| Theme | next-themes, system (auto by device) |
| Testing | Vitest (unit), Playwright (e2e) |
| Linting | ESLint + Prettier |

---

## Folder Structure

```
src/
├── app/                        # Next.js App Router
│   ├── (auth)/                 # Public routes (login)
│   ├── (dashboard)/            # Protected routes (orders, drivers, users)
│   ├── layout.tsx              # Root layout — providers, theme
│   └── globals.css             # CSS variables, base styles
│
├── components/
│   ├── ui/                     # shadcn/ui — do not edit
│   ├── common/                 # Our components (DataTable, PageHeader…)
│   └── layout/                 # Sidebar, Header, ThemeProvider
│
├── context/                    # React Context
│   ├── AuthContext.tsx          # User, tokens, roles
│   └── UIContext.tsx            # UI state (sidebar, modals)
│
├── hooks/                      # Custom hooks (useDebounce, usePagination…)
├── services/                   # API client and entity services
├── queries/                    # TanStack Query hooks per entity
├── types/                      # TypeScript types per entity
├── lib/                        # Utilities, Zod schemas, constants
└── middleware.ts                # Route protection, redirect logic
```

---

## Routing — App Router

We use Next.js App Router. Routes are split into two groups via bracket notation:

- `(auth)/` — public pages. Accessible without a token.
- `(dashboard)/` — protected pages. Redirects to `/login` without a token.

Each group has its own `layout.tsx`. Route protection is implemented via `middleware.ts` — it checks for an `access_token` in cookies and redirects when necessary.

By default, all components are Server Components. The `"use client"` directive is added only where hooks, events, or browser APIs are needed.

---

## Components

`components/ui/` — shadcn files. This is vendor code: it gets overwritten when shadcn is updated. Do not edit directly.

`components/common/` — our components. They use `ui/` as building blocks, adding business logic and custom props. Each component is a separate folder containing the component file, a types file, and an `index.ts`.

Customizing the appearance of shadcn components is done exclusively via CSS variables in `globals.css`, not by editing component files.

---

## State Management

**Server data** → TanStack Query. Each entity has its own file in `queries/` with a key factory (`orderKeys`, `driverKeys`…).

**Global client state** → React Context. Only two contexts:
- `AuthContext` — user, tokens, login/logout/refresh methods
- `UIContext` — UI state (sidebar, modals)

**Local state** → `useState` / `useReducer` inside the component.

**Forms** → react-hook-form + Zod. Zod schemas live in `lib/validations.ts`.

---

## TypeScript Types

Types are written manually. Contracts are agreed with the backend via Swagger UI (`/swagger`).

- `types/api.ts` — shared wrappers: `PaginatedResponse<T>`, `ApiError`
- `types/auth.ts` — tokens, user, OAuth requests
- `types/orders.ts`, `types/drivers.ts`, etc. — entity types including DTOs

Rules: no `any`, use `unknown` when the type is uncertain. Request DTOs (`CreateOrderDto`) are always kept separate from response models (`Order`).

---

## API Layer

A single fetch client in `services/api.ts`. Automatically attaches the `Authorization: Bearer` header. On receiving a `401`, it attempts to refresh the token via the refresh grant — on success it retries the request; on failure it clears the tokens and redirects to `/login`.

Entity services (`orders.service.ts`, `drivers.service.ts`) use this client and know nothing about authentication.

The base URL is taken from `NEXT_PUBLIC_API_URL` (`.env.local`).

---

## Authentication

OAuth 2.0 via OpenIddict on the backend. The frontend uses the password grant for login and the refresh token grant for session renewal.

- `access_token` — stored in localStorage, short-lived
- `refresh_token` — stored in an httpOnly cookie (preferred) or localStorage
- The entire flow is encapsulated in `AuthContext` and `auth.service.ts`

---

## Theme and Colors

The theme automatically adapts to the device's system settings (`defaultTheme="system"`). Manual switching is available via `ThemeToggle` in the header.

**Accent color:** blue (`--primary: 217 91% 60%`)

| Token | Light | Dark | Purpose |
|---|---|---|---|
| `--background` | `#ffffff` | `#09090b` | Page background |
| `--foreground` | `#18181b` | `#fafafa` | Primary text |
| `--primary` | `#2563eb` | `#3b82f6` | Buttons, links, accents |
| `--muted` | `#f4f4f5` | `#18181b` | Secondary surfaces |
| `--border` | `#e4e4e7` | `#27272a` | Borders, dividers |
| `--destructive` | `#ef4444` | `#dc2626` | Errors, deletion |

Status colors (badges, order statuses):

| Status | Light | Dark |
|---|---|---|
| Active | blue background + blue text | dark blue + light blue |
| Completed | green background + green text | dark green + light green |
| Pending | yellow background + amber text | dark amber + yellow |
| Cancelled | red background + red text | dark red + pink |

All variables are declared in `globals.css` inside `:root {}` (light) and `.dark {}` (dark) blocks.

---

## Testing

**Vitest** — unit tests. Coverage includes: all utilities from `lib/`, custom hooks (`@testing-library/react`), services.

**Playwright** — e2e. Required coverage: auth flow (login, redirect, logout), core user scenarios (order creation, etc.).

Tests live alongside the file being tested in a `__tests__/` folder; e2e tests live in the root `e2e/` folder.

---

## Linting and Formatting

ESLint with `next/core-web-vitals` + `next/typescript` rules. Additionally enabled: `no-console: warn`, `no-explicit-any: error`, `import/order: error` (auto-sort imports).

Prettier: `semi: true`, `singleQuote: false`, `tabWidth: 2`, `trailingComma: es5`, `printWidth: 100`.

Run manually before committing: `pnpm lint && pnpm format`.

---

## Team Conventions

| Rule | Standard |
|---|---|
| Components | PascalCase, named export |
| Hooks | `use` prefix, camelCase |
| Page files | kebab-case |
| Component files | PascalCase |
| Imports | `@/` alias for `src/` |
| Server vs Client | Server by default, `"use client"` only when necessary |
| Types | no `any`, DTOs are separate from models |
| Forms | react-hook-form + Zod everywhere |
| API errors | 401 is always handled via the refresh flow |

---

## Backend — Reference

**Technologies:** .NET 10, ASP.NET Core, PostgreSQL, Redis, OpenIddict, MediatR (CQRS), FluentValidation, Hangfire, Serilog, SignalR.

**Structure:**
```
LastMile.TMS.Api            → Controllers, Program.cs
LastMile.TMS.Application    → CQRS, Validators
LastMile.TMS.Domain         → Entities, Value Objects
LastMile.TMS.Infrastructure → OpenIddict, SignalR, Hangfire
LastMile.TMS.Persistence    → EF Core, PostgreSQL
```

**Key endpoints:**

| Method | Path | Description |
|---|---|---|
| POST | `/connect/token` | OAuth 2.0 (password + refresh grant) |
| GET | `/api/users/me` | Current user |
| GET | `/swagger` | Swagger UI (dev only) |
| GET | `/hangfire` | Background jobs (dev only) |

---

## Running the Project

```bash
# Backend
cd src/backend && dotnet run --project src/LastMile.TMS.Api
# → "applicationUrl": "https://localhost:61615;http://localhost:61616"

# Frontend
cd src/web && pnpm install && pnpm dev
# → http://localhost:3000

# Full stack
docker-compose up -d
# PostgreSQL + Redis + API + Frontend + Caddy
```

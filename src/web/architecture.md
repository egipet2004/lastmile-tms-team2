# Web Architecture Guide

This document defines the target architecture for `src/web/src`.
It describes the structure we actively maintain. Old layouts and deprecated import paths are not part of the architecture, even if they still appear in git history.

## Core Principles

1. Organize by layer first, then by domain inside the layer.
2. Use backend domain names as the canonical vocabulary:
   `depots`, `drivers`, `parcels`, `routes`, `users`, `vehicles`, `zones`.
3. Keep `app/` thin. Routes compose pages; they do not own feature logic.
4. Keep data flow consistent:
   page or component -> query hook -> service -> GraphQL client.
5. Keep `components/ui` primitive-only. Business behavior belongs elsewhere.
6. Keep `lib/` for shared non-React helpers, validation, navigation, networking, and labels.
7. Keep `hooks/` for generic React or browser hooks only.
8. Prefer explicit naming over catch-all folders such as `common`, `helpers`, or `misc`.

## Stack

- Framework: Next.js 16 App Router
- UI: React 19, Tailwind CSS 4, shadcn primitives
- Server state: TanStack Query
- Forms: react-hook-form + Zod
- Auth: NextAuth credentials flow against OpenIddict
- Tests: Vitest + Playwright

## Canonical Folder Tree

```text
src/
  app/
    (auth)/
    (dashboard)/
    admin/
    api/
    layout.tsx
    page.tsx
    providers.tsx

  components/
    auth/
    dashboard/
    depots/
    detail/
    feedback/
    form/
    layout/
    list/
    routes/
    ui/
    users/
    vehicles/
    zones/

  graphql/
    depots.ts
    drivers.ts
    parcels.ts
    routes.ts
    users.ts
    vehicles.ts
    zones.ts

  hooks/
    use-debounce.ts
    use-floating-dropdown-position.ts

  lib/
    auth.ts
    utils.ts
    depots/
    forms/
    labels/
    navigation/
    network/
    parcels/
    query/
    toast/
    validation/

  mocks/
  queries/
  services/
  types/
  proxy.ts
```

## Layer Contracts

### `app/`

Purpose:
- route files
- layouts
- top-level providers
- redirects and route entry points

Rules:
- keep route files thin
- move large tables, dialogs, forms, and detail pages into `components/`
- auth and dashboard route groups own composition, not business logic
- legacy URLs may redirect from `app/admin/*` into canonical dashboard routes

### `components/`

Purpose:
- reusable UI composed for this product
- feature and domain presentation
- page-level client components extracted out of `app/`

Structure:
- `components/ui`: shadcn-style primitives only
- `components/form`: reusable custom inputs and selectors
- `components/feedback`: alerts and query-state feedback
- `components/list`: list/table/page-shell building blocks
- `components/detail`: detail/edit page shells and form scaffolding
- `components/layout`: app chrome such as sidebar and header
- `components/auth`, `dashboard`, `depots`, `routes`, `users`, `vehicles`, `zones`: domain-specific components

Rules:
- no domain logic in `components/ui`
- no new `components/common`
- shared components should move into the narrowest folder that matches their purpose
- if a component knows about a specific entity, keep it under that entity folder

### `queries/`

Purpose:
- TanStack Query hooks
- query keys
- mutations with cache invalidation

Rules:
- components call queries, not services directly, unless there is a strong non-React reason
- query modules are domain-based: `queries/users.ts`, `queries/zones.ts`, etc.
- mutation toast metadata belongs here or in shared query helpers, not in page files

### `services/`

Purpose:
- domain API operations
- request and response mapping
- GraphQL variable construction
- domain-level normalization

Rules:
- services do not render UI
- services do not import React
- services import GraphQL documents from `graphql/`
- enum serialization and mapping belong in the owning service, not in a shared GraphQL helper

### `graphql/`

Purpose:
- document strings only

Rules:
- one file per domain
- no React, no fetch logic, no enum mapping
- examples: `graphql/vehicles.ts`, `graphql/users.ts`

### `hooks/`

Purpose:
- generic hooks that are not tied to one domain

Rules:
- keep only reusable React or browser hooks here
- entity data hooks belong in `queries/`, not `hooks/`
- deprecated: `lib/hooks/*`

### `lib/`

Purpose:
- cross-cutting helpers that are not components and not query hooks

Allowed subfolders:
- `lib/network/`: low-level network and GraphQL client helpers
- `lib/query/`: query client and mutation integration helpers
- `lib/navigation/`: dashboard nav and layout constants
- `lib/validation/`: Zod schemas and field validators
- `lib/labels/`: shared label, badge, and display helpers
- `lib/forms/`: shared option builders and non-visual form helpers
- `lib/depots/`: depot-specific pure helpers shared across layers
- `lib/parcels/`: parcel-specific pure helpers shared across layers
- `lib/toast/`: application toast wrapper

Root exceptions:
- `lib/auth.ts`
- `lib/utils.ts`

Rules:
- no React hooks in `lib/`
- do not put entity data access in `lib/api/*`
- do not recreate catch-all files like `validations.ts`

### `types/`

Purpose:
- domain contracts and shared transport types

Rules:
- file names use backend domain terms
- shared transport wrappers live in `types/api.ts`
- shared non-visual form types live in `types/forms.ts`
- UI components must not be the source of shared domain types

## Allowed Import Direction

Preferred direction:

```text
app -> components -> queries -> services -> graphql
                         |          |
                         v          v
                        types      lib
```

Allowed:
- `app` may import `components`, `lib`, `types`, and `auth`
- `components` may import `queries`, `hooks`, `lib`, `types`, and `ui` primitives
- `queries` may import `services`, `types`, and `lib/query`
- `services` may import `graphql`, `lib/network`, `lib/*` pure helpers, and `types`

Do not do this:
- `services` importing `queries`
- `graphql` importing `services`
- `components/ui` importing domain types or domain services
- `hooks/` duplicating entity-fetching behavior from `queries/`
- `lib/` depending on component implementation types

## Data Flow

Standard request flow:

1. A page component or client component reads route params and UI state.
2. The component calls a domain hook from `queries/`.
3. The query hook calls a function from `services/`.
4. The service builds variables and calls the GraphQL client.
5. The service normalizes the response into app-facing types.
6. The query hook manages caching and invalidation.
7. The component renders UI and mutation states.

This flow applies to all entities, including `depots` and `zones`.

## Auth and Runtime Flow

- `app/providers.tsx` wires `SessionProvider`, `QueryClientProvider`, toasts, and React Query Devtools.
- `lib/auth.ts` owns NextAuth configuration and token refresh behavior.
- `proxy.ts` guards protected routes and redirects unauthenticated users to `/login`.
- `app/(auth)` contains public auth screens.
- `app/(dashboard)` contains protected routes and dashboard layout composition.

## Naming Rules

- Use backend names for files and modules: `users`, not `user-management`; `zones`, not `zone`.
- Use kebab-case for file names except where framework conventions require otherwise.
- Keep page entrypoints named `page.tsx`; keep extracted component files descriptive, such as `vehicles-page.tsx` or `route-detail-page.tsx`.
- Prefer singular component names and plural domain module names.

## What Belongs Where

### `components/ui`

Belongs here:
- `button.tsx`
- `card.tsx`
- `input.tsx`
- `label.tsx`
- future shadcn-generated primitives

Does not belong here:
- date-time pickers
- filter dropdowns
- domain-specific status filters
- query error wrappers
- table cells with business meaning

### `components/form`

Belongs here:
- reusable custom inputs
- dropdown wrappers
- listbox-based selectors
- numeric input controls

Examples:
- `date-time-picker.tsx`
- `select-dropdown.tsx`
- `weight-capacity-input.tsx`

### `components/feedback`

Belongs here:
- query and mutation error display
- empty-state or retry shells that are presentation-focused

### `components/list`

Belongs here:
- list/table shells
- pagination controls
- reusable data-table helpers
- generic table cells such as overflow tooltip rendering

### Domain component folders

Belongs here:
- entity-specific filters
- entity page clients
- entity forms and dialogs
- row actions tied to one domain

Examples:
- vehicle status filter -> `components/vehicles/`
- route status filter -> `components/routes/`
- user management page -> `components/users/`

### `queries`

Belongs here:
- `useUsers`
- `useZones`
- query keys
- mutation invalidation rules

Does not belong here:
- raw GraphQL document strings
- presentational toast components

### `services`

Belongs here:
- `users.service.ts`
- `zones.service.ts`
- domain-specific mapping helpers

Does not belong here:
- React hooks
- JSX
- query cache logic

### `graphql`

Belongs here:
- `USERS_LIST`
- `CREATE_VEHICLE`
- `ZONE_BY_ID`

Does not belong here:
- `graphqlRequest`
- enum conversion helpers
- domain normalization

### `lib`

Belongs here:
- `lib/network/graphql-client.ts`
- `lib/query/query-client.ts`
- `lib/navigation/dashboard-nav.ts`
- `lib/validation/users.ts`
- `lib/forms/depots.ts`
- `lib/depots/operating-hours.ts`

Does not belong here:
- entity query hooks
- page components
- domain service modules

## Deprecated Structure

These paths are deprecated and must not be reintroduced:

- `components/common`
- `lib/hooks`
- `lib/api`
- `graphql/operations.ts`
- `graphql/enum-maps.ts`
- `lib/validations.ts`
- domain names that drift from backend vocabulary such as `user-management.ts` or `zone.ts`

## Testing Rules

- Unit and component tests live next to the code they cover or in the layer test folder that already owns that module.
- Every move or rename must keep tests aligned with the new canonical import path.
- Minimum validation for structural refactors:
  - `npm run build`
  - `npm run test:run`
- E2E coverage should protect the main auth and dashboard flows:
  - login
  - navigation
  - vehicles
  - users
  - depots and zones when their CRUD flow changes

## Refactor Checklist

Use this checklist when adding or moving code:

1. Pick the domain-first name that matches the backend.
2. Put route composition in `app/` and feature logic in `components/`.
3. Put data fetching in `queries/`, not `hooks/`.
4. Put API mapping in `services/`, not `components/`.
5. Put GraphQL strings in the matching `graphql/<domain>.ts`.
6. Keep shared types in `types/`, not inside visual components.
7. If a module does not fit a contract, move it before adding more code around it.

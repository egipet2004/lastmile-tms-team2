# CLAUDE.md

## Core Rules

### 1. TDD First
- For every feature, bug fix, or behavior change, follow `Red -> Green -> Refactor`.
- Start by writing a failing test that proves the expected behavior.
- Implement the smallest change that makes the test pass.
- Refactor only after the tests are green again.
- If the task is documentation, config-only, or another change that cannot be meaningfully driven by a test, say that explicitly and run the closest relevant verification instead.

### 2. Read Architecture Before Coding
- For backend tasks, read [src/backend/architecture.md](/C:/Users/User/source/repos/lastmile-tms-team2/src/backend/architecture.md).
- For web/frontend tasks, read [src/web/architecture.md](/C:/Users/User/source/repos/lastmile-tms-team2/src/web/architecture.md).
- Do not introduce structure or dependencies that conflict with those documents.
- If code and architecture docs diverge, move the code toward the documented target state and call out the mismatch.

### 3. Conventions
- Preserve the project vocabulary: `depots`, `drivers`, `parcels`, `routes`, `users`, `vehicles`, `zones`.
- Keep transport and composition layers thin. Business logic belongs in the owning application/domain layer.
- Prefer small, focused files and clear responsibilities over large mixed modules.
- Update tests together with code changes.
- Update architecture docs when architectural rules or structure change.
- Follow repository formatting conventions: C# uses 4 spaces, TypeScript uses 2 spaces, line endings are LF.

## Verification
- Backend: `cd src/backend && dotnet test`
- Web: `cd src/web && npm run test:run && npm run build`
- Mobile: `cd src/mobile && npx tsc --noEmit`
- Run the smallest relevant test scope first, then broader verification before finishing.

## Repo Map
- `src/backend` - .NET backend
- `src/web` - Next.js web app
- `src/mobile` - Expo mobile app

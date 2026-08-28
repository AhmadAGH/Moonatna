# Moonatna - Project Context

## Project Overview

**Moonatna** (مونتنا) is an ASP.NET Core web application built with C# that serves as a **family grocery and recipe management app**. The name comes from the Arabic word for "our pantry" — it is a shared inventory system for families to manage groceries, track pantry items, create shopping lists, and organize recipes.

### Key Technologies & Architecture
- **Framework**: .NET 10 (LTS — released Nov 2025, supported until Nov 2028), targeting `net10.0`
- **Web Framework**: ASP.NET Core MVC (`MapControllerRoute`)
- **Data Access**: Dapper (micro-ORM) with raw SQL, via a singleton SQL connection factory
- **Authentication**:
  - Firebase Admin SDK for Google Sign-In verification
  - Cookie-based authentication for session management
  - Firebase JWT ID token verification on server-side
- **Database**: Microsoft SQL Server Express (`.\\SQLEXPRESS` in dev)
  - Schema migrations via **DbUp** (`dbup-sqlserver`) with scripts embedded in the assembly
- **Logging**: Serilog with Seq sink for log aggregation
- **Localization**: Arabic (ar-SA) first; English pending

### Feature Areas
Based on controllers, services, and database schema:
1. **Auth** — User registration/login via Firebase + Google Sign-In
2. **Family** — Multi-user family sharing with join codes
   - Role-based access (Owner = 0, Member = 1)
   - Auto-promote ad-hoc items (configurable per family)
3. **Items** — Unified catalog with three states:
   - Mojoud (موجود): available items (in pantry)
   - Naqis (ناقص): low-stock / expiring items
   - Mukhlis (مخلص): depleted items (feeds the shopping list)
4. **Pantry** — View and manage household inventory
5. **Shopping** — Shopping list generated from depleted items
6. **Recipes** — Recipe management with ingredients linked to Items
7. **ItemsController** — CRUD for individual items
8. **Organize** — Item categorization workflow

### Database Schema Overview
Primary tables (`dbo.` schema):
- `Users` — Firebase-backed user accounts linked by UID
- `Families` — Household/group definitions with join codes
- `FamilyMembers` — Role assignment for family access
- `Items` — Core inventory table (ad-hoc + categorized items)
- `Recipes` — Recipe definitions
- `RecipeIngredients` — Ingredients linking recipes to Items

Lookup tables (`Lookup.` schema):
- `Categories` — Food/drink categories with bilingual labels
- `Localizations` — UI/text localization strings
- `BadgeLabels` — Item state labels (ar-SA)
- `StateLabels` — Mojoud/Naqis/Mukhlis labels

Seed scripts are embedded as `EmbeddedResource` in the assembly and deployed via DbUp migrations.

---

## Building & Running

### Prerequisites
- **SDK**: .NET 10 SDK
- **Runtime**: Windows dev machine with SQL Server Express (or adjust connection string)
- **Firebase**: Service account JSON file (never commit to repo — excluded in `.gitignore`)

### Connection Configuration
Development connection string example:
```
Server=.\\SQLEXPRESS;Database=MounatnaDb;Integrated Security=true;TrustServerCertificate=true;
```

See `appsettings.Development.json` for full config including:
- Firebase credentials path
- Seq logger URL (`http://localhost:5341`)

### Build Commands
```cmd
dotnet restore
dotnet build --configuration Release
dotnet publish -c Release -o ./publish
```

### Run Locally (Development)
```cmd
dotnet run --environment Development
```

**Note**: Ensure the Firebase credentials file exists at the configured path before starting.

### First Run / Test Running
- Database is created automatically on first launch via `EnsureDatabase` in `Program.cs`
- DbUp migration scripts upgrade the schema to the current version embedded in the assembly
- Default route: `Pantry/Index` (home)
- UI defaults to Arabic (`ar-SA`) until the culture switcher is implemented

---

## Development Conventions

### Code Style
- **Implicit usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Nullable reference types**: Enabled (`<Nullable>enable</Nullable>`)
- **Repository pattern**: Interface-based (e.g., `IUsersRepository`, `IFamiliesService`)

### Repository/Service Pattern
Each feature area follows consistent layering:
```
[Feature]Controller.cs                         // HTTP handling + returns View()
[Feature]/Repositories/I[Feature]Repository.cs // Data access contract
[Feature]/Repositories/[Feature]Repository.cs  // Dapper + raw SQL implementation
[Feature]/Services/I[Feature]Service.cs        // Business logic contract
[Feature]/Services/[Feature]Service.cs         // Business logic implementation
```

### Service Contracts
- **Interface-first**: Define `I*Service` and `I*Repository` interfaces
- **Scoped registration**: Services registered with DI as scoped in `Program.cs`
- **ConnectionFactory**: SQL connection factory registered as singleton

### Database Migrations
- **DbUp** for schema migration using embedded SQL scripts
- Scripts named `Script####_*` (sequential numbering)
- Embedded as resources in the main assembly
- Auto-run on startup via `PerformUpgrade()`

### Localization Strategy
- Current state: Arabic-only UI (`ar-SA`)
- Browser `Accept-Language` is ignored until multi-language lands
- All string keys defined once in `Lookup.Localizations` with `(ValueAr, ValueEn)` columns
- Culture switching planned but not yet implemented

### Firebase Integration
- **Service account JSON**: Never commit to Git (`.gitignore` excludes `*firebase-adminsdk*.json`)
- ID token verification via `FirebaseAuth.DefaultInstance.VerifyIdTokenAsync()`
- Claims extraction for `name`, `uid` mapped to local User records
- User records auto-created on first login

### Security Considerations
- HTTPS redirection (`UseHttpsRedirection()`)
- Firebase Admin SDK for secure auth handling
- No secrets in `appsettings.json` — check `.gitignore` for credential exclusions

---

## Key Files to Know

### Entry Point
- **`Program.cs`** — DI setup, migrations, localization, routes
- **`Controllers/**.cs`** — MVC controllers with views
  - `HomeController.cs` — Redirects to Pantry
  - `AuthController.cs` — Firebase auth integration
  - `FamilyController.cs` — Join codes, family management
  - `PantryController.cs` — Inventory view
  - `ShoppingController.cs` — Shopping list

### Models (in `/Models`)
- `User.cs` — Firebase UID + display name + timestamps
- `Family.cs` — Group with join code + auto-promote config
- `Item.cs` — Core inventory unit (name, category, state, image)
- `Recipe.cs` / `RecipeIngredient.cs` — Recipe definitions

### Configuration
- **`appsettings.json`** — Base configuration (no secrets)
- **`appsettings.Development.json`** — Connection strings + dev Firebase path
- **`appsettings.Production.json`** — Production settings

### Database
- **`Database/Scripts/*.sql`** — Migration scripts embedded in the project
  - `Script0001_init.sql` — Schema foundation
  - `Script0002_seedCategories.sql` — Initial categories (Ar + EN)
  - Subsequent scripts add localizations, badges, features

---

## Current Development State

### Known In-Progress Items
1. **Multi-language UI** — Accept-Language routing + culture switcher
2. **Quick Add Feature** — Dialog-driven add with category lookup chips
3. **Photo Uploads** — Gallery/camera picker integration for items/recipes
4. **Item States** — Three-state system (Mojoud/Naqis/Mukhlis) driving the shopping workflow

### Tech Debt / Notes
- No unit/integration test project yet — tests should follow the existing layering conventions
- Firebase Admin credentials must exist at the configured path or the app fails to start

---

## Troubleshooting

### App Won't Start
```
Firebase:CredentialsPath is not configured.
```
→ Set the path in `appsettings.Development.json` and ensure the file exists

### Database Not Created
- First run creates the database automatically via `EnsureDatabase.For.SqlDatabase()`
- Verify the connection string matches the local SQL Server Express instance

### Migration Failure
- Check DbUp output logs for the specific error (usually missing table, constraint conflict)
- Manually verify schema via SSMS against the current `Database/Scripts/` folder

### Seq Logs Not Appearing
- Seq server should run locally at port 5341 in dev
- URL configurable via `Seq:ServerUrl` setting

---

## Contribution Guidelines

1. **Never commit Firebase credentials** — excluded in `.gitignore`
2. **Database migrations first** — New schema changes go in `/Database/Scripts/Script####_*.sql`
3. **Follow the repository pattern** — Repository → Service → Controller pipeline
4. **Arabic-first UI** — All string keys defined with `(ValueAr, ValueEn)` in the localizations table
5. **Tests** — New service logic should come with unit tests once the test project exists

---

## Working Preferences (Project Owner)

These instructions override defaults. Follow them in every session.

### How I work
- I develop in **Visual Studio IDE** (not VS Code). You work via the terminal; don't suggest VS Code extensions or workflows.
- **Plan before code**: for anything non-trivial, propose a short step-by-step plan and wait for my approval before editing files.
- **Review-before-commit**: never run `git commit` or `git push`. Show me diffs; I commit myself.
- One feature/fix per session — don't refactor adjacent code unless I ask.
- If context is missing or a file seems absent, **ask before guessing**. Never invent file contents, endpoints, or table schemas.

### Code preferences
- C# / ASP.NET Core MVC, **Dapper + raw SQL** for data access — do not introduce EF Core.
- Thin controllers; business logic belongs in services.
- Interface-first for new services/repositories, registered scoped in DI.
- New SQL changes must be DbUp migration scripts — never modify existing released scripts.
- Keep code comments minimal; prefer self-explanatory names.
- Target framework stays on the current .NET LTS (currently .NET 10).

### Communication
- Be concise. Explain only non-obvious decisions, briefly.
- **Challenge my ideas.** If my approach, design, or assumption looks wrong — or there's a clearly better way — say so directly and argue your case with reasons. Don't just comply to be agreeable; I'd rather defend a good idea than silently ship a bad one.
- I read Arabic and English — either is fine; keep code identifiers in English, UI strings Arabic-first.
- When you finish a task, summarize: files changed, why, and how to verify (build/run/test command).

### Environment notes
- Dev machine: Windows 11, SQL Server Express, everything local.
- Production: Linux server (systemd service + nginx reverse proxy).
- You (the agent) run locally via Ollama — if a task feels too large for one pass, say so and propose splitting it instead of attempting a giant change.

---

## TODO Items

- [ ] Implement culture switcher for EN UI routes
- [ ] Add unit test project; start with services
- [ ] Quick Add dialog with category chips
- [ ] Photo uploads for items and recipes

---

*Generated: 2026-08-29 (Qwen Code `/init`), corrected and extended by the project owner | Project: Moonatna — Arabic Family Pantry Manager | Status: MVP with family sharing + grocery tracking + recipes*
# Clinic API

A full-stack exam project for managing a clinic's clinics, doctors, specialities, appointment categories, patients, and appointments. ASP.NET Core 10 Web API + Next.js 16 frontend with JWT authentication.

## Project Structure

```
Backend/
  ClinicAPI/             ASP.NET Core 10 Web API
    Controllers/         HTTP endpoints
    Services/            Business logic + data access
    Models/
      Entities/          EF Core entities
      Dtos/              Request/response DTOs
      Config/            JwtSettings binding
    Data/                DbContext + migrations + seed
    Authorization/       JWT claim helpers
    Program.cs           DI, Swagger, JWT, CORS, pipeline
  ClinicAPI.Tests/       Integration tests (xUnit)

frontend/                Next.js 16 + TypeScript + Tailwind
  src/
    app/                 Pages (App Router)
      admin/             Admin CRUD pages
      bookings/          Patient bookings
      book/              Booking flow
      profile/           Patient profile
      search/            Doctor search
      login, register/   Auth pages
    components/          Reusable UI components
    services/            Backend API wrappers
    modules/             Server actions
    types/               Types mirroring backend DTOs
    lib/                 Auth guard + utilities
```

## Setup and Run

### Prerequisites

- .NET 10 SDK
- Node.js 20+ and npm
- SQL Server (the default connection string targets `localhost:1433` with `sa` / `@Admin123`)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

### Backend

1. Update the connection string in [Backend/ClinicAPI/appsettings.json](Backend/ClinicAPI/appsettings.json) if your SQL Server is elsewhere.
2. Create and seed the database:
   ```bash
   cd Backend/ClinicAPI
   dotnet ef database update
   ```
3. Run the API:
   ```bash
   dotnet run
   ```
   It listens on `http://localhost:5074`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend runs on `http://localhost:3000` and expects the backend at `http://localhost:5074`. To point it elsewhere, create `frontend/.env.local`:

```
NEXT_PUBLIC_BACKEND_API_URL=http://localhost:5074
```

### Default Credentials

The database is seeded with these accounts:

| Role    | Email              | Password    |
| ------- | ------------------ | ----------- |
| Admin   | admin@clinic.com   | Admin123    |
| Patient | jonas@email.com    | Patient123  |
| Patient | marie@email.com    | Patient123  |

### Swagger / API Docs

With the backend running, open `http://localhost:5074/doc`. All endpoints and request/response schemas are documented and can be invoked from the UI. Protected endpoints require a JWT — paste one via the "Authorize" button at the top.

## Testing

The backend includes an integration test project that spins up an in-memory `WebApplicationFactory` with the seeded database. Run it from the test project folder:

```bash
cd Backend/ClinicAPI.Tests
dotnet test
```

The suite covers authentication, authorization, every controller's happy paths, and the main failure modes (validation, conflicts, forbidden access, missing resources). It uses [xUnit](https://xunit.net/) for the test runner and [FluentAssertions](https://fluentassertions.com/) for the assertions.

### Code Coverage

To collect coverage and generate a browsable HTML report, run from `Backend/ClinicAPI.Tests`:

```bash
dotnet test --settings ../../coverlet.runsettings --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/TestResults/**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
start coverage/index.html
```

`reportgenerator` is a global tool: install once with `dotnet tool install --global dotnet-reportgenerator-globaltool`.

The frontend is not currently covered by automated tests.

## Endpoints

All endpoints are under the base path `/api/v1`. The same descriptions are surfaced in the Swagger UI at `/doc`.

### Auth (`/auth`)

| Method | Path                | Auth   | Description                                           |
| ------ | ------------------- | ------ | ----------------------------------------------------- |
| POST   | `/login`            | Public | Authenticate a user and return a JWT.                 |
| POST   | `/register/patient` | Public | Register a new patient with a login account.          |
| POST   | `/register/guest`   | Public | Create a guest patient record (no login).             |

### Clinics (`/clinics`)

| Method | Path                  | Auth   | Description                                           |
| ------ | --------------------- | ------ | ----------------------------------------------------- |
| GET    | `/`                   | Public | List active clinics.                                  |
| GET    | `/all`                | Admin  | List all clinics, including inactive ones.            |
| GET    | `/{id}`               | Public | Get an active clinic by id.                           |
| GET    | `/all/{id}`           | Admin  | Get any clinic by id, including inactive ones.        |
| POST   | `/`                   | Admin  | Create a new clinic.                                  |
| PUT    | `/{id}`               | Admin  | Update an existing clinic.                            |
| DELETE | `/{id}`               | Admin  | Deactivate a clinic.                                  |
| PATCH  | `/{id}/reactivate`    | Admin  | Reactivate a previously deactivated clinic.           |

### Specialities (`/specialities`)

| Method | Path        | Auth   | Description                                                       |
| ------ | ----------- | ------ | ----------------------------------------------------------------- |
| GET    | `/`         | Public | List all specialities.                                            |
| GET    | `/{id}`     | Public | Get a speciality by id.                                           |
| POST   | `/`         | Admin  | Create a new speciality.                                          |
| PUT    | `/{id}`     | Admin  | Update an existing speciality.                                    |
| DELETE | `/{id}`     | Admin  | Delete a speciality (fails if any doctors are assigned to it).    |

### Appointment Categories (`/appointmentcategories`)

| Method | Path        | Auth   | Description                                                            |
| ------ | ----------- | ------ | ---------------------------------------------------------------------- |
| GET    | `/`         | Public | List all appointment categories.                                       |
| GET    | `/{id}`     | Public | Get an appointment category by id.                                     |
| POST   | `/`         | Admin  | Create a new appointment category.                                     |
| PUT    | `/{id}`     | Admin  | Update an existing appointment category.                               |
| DELETE | `/{id}`     | Admin  | Delete a category (fails if any appointments are assigned to it).      |

### Doctors (`/doctors`)

| Method | Path                  | Auth   | Description                                                      |
| ------ | --------------------- | ------ | ---------------------------------------------------------------- |
| GET    | `/`                   | Public | List active doctors.                                             |
| GET    | `/all`                | Admin  | List all doctors, including inactive ones.                       |
| GET    | `/search?name=...`    | Public | Search active doctors by first or last name (case-insensitive).  |
| GET    | `/{id}`               | Public | Get an active doctor by id.                                      |
| GET    | `/all/{id}`           | Admin  | Get any doctor by id, including inactive ones.                   |
| POST   | `/`                   | Admin  | Create a new doctor.                                             |
| PUT    | `/{id}`               | Admin  | Update an existing doctor.                                       |
| DELETE | `/{id}`               | Admin  | Deactivate a doctor.                                             |
| PATCH  | `/{id}/reactivate`    | Admin  | Reactivate a previously deactivated doctor.                      |

### Patients (`/patients`)

| Method | Path                  | Auth    | Description                                                                |
| ------ | --------------------- | ------- | -------------------------------------------------------------------------- |
| GET    | `/`                   | Admin   | List active patients.                                                      |
| GET    | `/all`                | Admin   | List all patients, including inactive ones.                                |
| GET    | `/{id}`               | Admin   | Get an active patient by id.                                               |
| GET    | `/all/{id}`           | Admin   | Get any patient by id, including inactive ones.                            |
| PUT    | `/{id}`               | Admin   | Update an existing patient.                                                |
| DELETE | `/{id}`               | Admin   | Deactivate a patient (also cancels scheduled appointments).                |
| PATCH  | `/{id}/reactivate`    | Admin   | Reactivate a previously deactivated patient.                               |
| GET    | `/me`                 | Patient | Get the current patient's own profile.                                     |
| PUT    | `/me`                 | Patient | Update the current patient's own profile.                                  |
| DELETE | `/me`                 | Patient | Deactivate the current patient's own account.                              |

### Appointments (`/appointments`)

| Method | Path                  | Auth            | Description                                                                |
| ------ | --------------------- | --------------- | -------------------------------------------------------------------------- |
| GET    | `/?status=...`        | Admin           | List all appointments. Optional `status` query filters by Scheduled, Completed or Cancelled. |
| GET    | `/mine`               | Patient         | List the current patient's appointments.                                   |
| GET    | `/{id}`               | Patient / Admin | Get an appointment by id. Patient may only view their own.                 |
| POST   | `/`                   | Patient         | Book an appointment.                                                       |
| POST   | `/guest`              | Public          | Create a guest patient and book an appointment in a single call.           |
| PATCH  | `/{id}/reschedule`    | Patient / Admin | Reschedule a scheduled appointment. Patient may only modify their own.     |
| PATCH  | `/{id}/cancel`        | Patient / Admin | Cancel an appointment. Patient may only cancel their own.                  |
| PATCH  | `/{id}/complete`      | Admin           | Mark a scheduled appointment as completed.                                 |

## Decisions

### 1. Two-tier patient model (registered vs guest)

A patient is either **registered** (has a `User` row with email + password, can log in) or a **guest** (`Patient` row only, no login, created through the guest booking flow). Admin can edit registered patients but only deactivate/reactivate guests. Guests are appointment-scoped throwaway records, not managed entities — if their info is wrong, cancel and rebook.

### 2. Soft delete for entities with history, hard delete for lookup tables

Clinics, doctors and patients are soft-deleted (`IsActive` flag) so their past appointments stay meaningful. Specialities and appointment categories are hard-deleted, gated by a referential check that rejects the delete if any rows still reference them. Pure lookup tables don't need historical preservation.

### 3. HttpOnly cookies + Server Actions for auth

The JWT and role are stored in HttpOnly cookies set by Next.js Server Actions (`await cookies()`), not in `localStorage`. The token is read server-side and forwarded as a Bearer header to the backend. XSS can't read HttpOnly cookies, and Server Actions are the App Router's first-class write path.

### 4. Page guards: role check everywhere, `requireAuth()` where the token is needed

Every protected page does an explicit role check (`role === "Admin"` or `role === "Patient"`); pages that also need to call the backend additionally call `requireAuth()` to read the token and pass it as a Bearer header. The role check on its own is sufficient as an access gate (the role and token cookies are set and cleared together by login/logout), so pages like `/admin/clinics/new` that only render a form don't bother calling `requireAuth()`. The role check also prevents the awkward "logged in as the wrong role" state where the page renders but every backend call 403s.

### 5. Status-code-only error messages on booking

The booking actions distinguish 409 (time slot conflict — "Please pick a different time"), 403 (unauthorized — "log in again"), and anything else (generic "re-check your details"). The backend's specific 400 reasons (e.g., "doctor does not work at the chosen clinic") are intentionally not surfaced — the cascading dropdowns prevent those cases client-side, and showing them server-side risks leaking internal state.

### 6. Services return `null` / `boolean`, not exceptions

Frontend service functions return `T | null` for reads and `boolean` for writes. Errors from the backend are not thrown — pages and actions branch on the return value. This keeps page-level error UI consistent (`if (data === null) return <error>`) and matches the Server Component pattern of awaiting plain values.

### 7. `Patient.Email` via subquery, not navigation property

`PatientResponseDto.Email` is populated by a subquery into `_context.Users` inside the `Select` projection, rather than adding a `User` navigation property to the `Patient` entity. The subquery is one line per projection; a navigation property would require an EF model change and a migration.

### 8. Layout `cookies()` extracted into `<Suspense>`

The root layout originally read cookies directly to render the auth-aware nav. Per Next.js 16 docs, uncached layout data blocks `loading.tsx` from firing — navigations would freeze on the previous page until both layout and page finished. The cookies-using nav was extracted into a child component wrapped in `<Suspense>`, so the shell renders instantly and `loading.tsx` shows reliably during navigation.

### 9. Client-side doctor search on `/search`

`/search` fetches all active doctors via the public `GET /doctors` endpoint and filters in the browser by first or last name. The backend already has a `GET /doctors/search?name=` endpoint, but the dataset is small enough that filtering client-side gives instant feedback as the user types without a round-trip per keystroke.

### 10. Enums serialized as strings

The API is configured with `JsonStringEnumConverter`, so enum values are sent and received as their string names (`"Scheduled"`, `"Male"`) rather than as integers. This lets the frontend use string literal unions (`type Status = "Scheduled" | "Completed" | "Cancelled"`) directly, with no conversion layer at the network boundary. If an enum is renamed or extended on the backend, the frontend type just needs to be updated to match — there's no hidden integer mapping to keep in sync.

## Changes I Would Make

These are improvements I would have made if I had more time. Some I simply didn't get to; others I didn't spot until I was too close to the deadline to do anything about safely.

### 1. Snapshot entity data on each appointment

The biggest architectural gap. Appointments currently reference patients, doctors and clinics by id, so anything shown about a past appointment reflects the *current* state of those entities, not the state at the time of booking. The clearest example is a doctor switching clinics: a past appointment will show the doctor's new clinic, even though the appointment actually happened at the old one. Name changes, email changes and religion changes have the same problem — the history is silently rewritten as the underlying records change. Patient name, doctor name, clinic name and any other field surfaced in appointment history should be snapshotted onto the appointment row at booking time. (Active/inactive status is *not* part of this — that's a "now" state, not a historical one, and it does belong to the related entity.)

### 2. GDPR-compliant anonymization on patient deactivation

Patient deactivation is the system's soft-delete. Under GDPR's right to erasure, the patient's personal data should be stripped from the appointment history (and from the patient record itself) at that point — name, email, SSN, etc. should be replaced with anonymized placeholders. The project currently keeps everything intact, which is not compliant.

### 3. Automatic completion of past appointments

Today, an admin has to manually mark a scheduled appointment as `Completed` after it happens. A background job — or a check on read that flips any past `Scheduled` row to `Completed` — would remove that manual step entirely.

### 4. Pick one doctor-search implementation

The backend has a `GET /doctors/search?name=` endpoint that the frontend doesn't call — the `/search` page does its filtering client-side instead. Either is fine on its own; having both is just dead code on one side. The right move is to choose one and remove the other.

### 5. Re-enable HTTPS redirect outside Development

`app.UseHttpsRedirection()` is commented out so the local frontend can talk to the backend over plain HTTP without a trusted cert. In any real deployment this should be re-enabled, ideally gated to non-Development environments.

### 6. Allow admin to edit guest patient records

Decided against (see Decisions #1) on the grounds that guests are appointment-scoped throwaway records. If a use case for receptionist typo-fixes shows up, this would need the backend's `UpdatePatientRequestDto.Email` to become optional and the admin frontend to conditionally render the email field.

### 7. Add frontend tests

The Next.js frontend has no automated test coverage.

## References and AI Usage

### References

- Examples from class projects and lessons — the Swagger / JWT setup pattern, the integration test setup (`WebApplicationFactory`, xUnit, FluentAssertions), and the overall controller / service / DTO structure are based on patterns shown in class.
- [Next.js 16 documentation](https://nextjs.org/docs) — App Router, Server Actions, `loading.tsx` and Suspense behavior.
- [ASP.NET Core documentation](https://learn.microsoft.com/aspnet/core) — OpenAPI, JWT bearer authentication, EF Core.
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — Swagger UI configuration.
- [Tailwind CSS documentation](https://tailwindcss.com/docs) — utility class reference.
- [xUnit](https://xunit.net/) and [FluentAssertions](https://fluentassertions.com/) documentation — test runner and assertion library.

### AI Usage

AI assistance was used to help with the following parts of the project:

- **Tailwind CSS** — help choosing class combinations for styling.
- **Regex patterns** — help drafting the validation regexes for SSN, tax / license / insurance numbers, and password complexity.
- **Backend integration tests** — help applying the same test pattern across the controller tests once the shape was established.
- **Seed data** — help generating the bulk seed entries (additional doctors, patients and appointments) once the shape and constraints were established.
- **Frontend** — help applying the same pattern across the copy-and-paste-heavy CRUD pages, forms and services once the shape was established.
- **This README** — help structuring the sections and tightening the wording.

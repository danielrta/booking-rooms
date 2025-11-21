# Booking Rooms

Small full-stack sample application to manage **meeting room reservations**.

The solution includes:

- **Backend**: .NET 8 Web API (feature/vertical-slice style, EF Core + SQLite, JWT auth, xUnit tests).
- **Frontend**: React + TypeScript + Vite + Material UI, with role-based protected routes.

---

## Project structure

```text
booking-rooms/
  backend/
    BookingRooms.Api/      # ASP.NET Core Web API (.NET 8)
    BookingRooms.Specs/    # Tests (xUnit, integration tests)
  frontend/
    booking-rooms-web/     # React + TypeScript + Vite + MUI
  README.md               
```

---

## Requirements

- .NET SDK **8.0**
- Node.js **18+**
- npm

---

## Running the backend

From the repo root:

```bash
cd backend/BookingRooms.Api
dotnet restore
dotnet run
```

The API will listen on the URL configured in `launchSettings.json`
(typically something like `http://localhost:5158`).

SQLite is used as database (a local file such as `meetingrooms.db`).
Migrations are applied automatically at startup (if `Database.Migrate()` is used) just for review purposes.

Seeded admin user

```text
Admin:
  userName: admin@test.com
  password: Admin@123
```

---

## Running the frontend

From the repo root:

```bash
cd frontend/booking-rooms-web
npm install
```

Edit `.env` if needed, for example:

```env
VITE_API_BASE_URL=http://localhost:5158
```

Then:

```bash
npm run dev
```

Vite will print the URL in the console (commonly `http://localhost:5173`).

---

## Quick tour

- Login page (JWT auth).
- **Rooms** (Admin only):
  - CRUD for rooms (name, capacity, location, equipments).
- **Reservations**:
  - Any authenticated user can create reservations.
  - Admin can see all, User only sees their own.
  - Validations:
    - No past reservations.
    - No overlapping reservations for the same room.
    - Cannot reserve on public public holidays (external holiday API).
- **Users** (Admin only):
  - Simple form to create users (email, password, role).

---

## Running tests (backend)

From the repo root:

```bash
cd backend
dotnet test
```

This runs integration tests using xUnit.

# AASTU Registration and Approval System

A comprehensive role-based online university registration system for Addis Ababa Science and Technology University.

## Tech Stack

- **Frontend**: Next.js 14+ (React)
- **Backend**: ASP.NET Core Web API (.NET 8)
- **Database**: Microsoft SQL Server
- **Authentication**: JWT + Email OTP
- **Network Restriction**: AASTU Wi-Fi only
- **Email Domain**: @aastustudent.edu.et and official staff emails

## Project Structure

```
aastu/
├── backend/          # ASP.NET Core Web API
├── frontend/         # Next.js application
└── database/         # SQL scripts and migrations
```

## Getting Started

### 1. Clone the Repository

```
git clone https://github.com/7Dagm21/RegistrationSystem.git
cd aastu
```

### 2. Prerequisites

- **Node.js** (v18+ recommended)
- **npm** (v9+ recommended)
- **.NET 8 SDK**
- **Microsoft SQL Server** (Express or Developer edition is fine)
- **Git**

### 3. Database Setup

1. Install SQL Server and ensure it is running.
2. Open SQL Server Management Studio (SSMS) or Azure Data Studio.
3. Run the scripts in the `database/` folder in this order:
   - `CREATE_TABLES.sql`
   - `ADD_STUDENTS.sql` and `ADD_STAFF.sql` (and/or `ADD_30_STUDENTS.sql`, `ADD_30_STAFF.sql` as needed)
   - Any other seed or update scripts as required
4. Update your connection string in `backend/AASTU.RegistrationSystem.API/appsettings.json` (see `CONNECTION_STRING_GUIDE.md`).

### 4. Backend Setup

```
cd backend
# Restore dependencies
 dotnet restore
# Apply migrations (if using EF Core)
 dotnet ef database update
# Run the API
 dotnet run
```

### 5. Frontend Setup

```
cd frontend
# Install dependencies
npm install
# Copy and edit .env.local if needed
cp .env.example .env.local
# Start the development server
npm run dev
```

### 6. Application Flow

1. **User Registration/Login:**
   - Students and staff register or log in using their university email.
2. **Role-Based Dashboard:**
   - After login, users see a dashboard tailored to their role (student, advisor, etc.).
3. **Student Actions:**
   - Register, view slips, submit cost sharing forms, download PDFs.
4. **Staff Actions:**
   - Approve/reject student requests, manage grade reports, generate PDFs.
5. **PDF/QR Generation:**
   - Downloadable documents for grade reports, cost sharing, and registration slips.
6. **Audit & Security:**
   - All actions are logged. Access is restricted by role and network (AASTU Wi-Fi).

---

## Troubleshooting

- Ensure SQL Server is running and accessible.
- Double-check your connection string.
- Use the provided SQL scripts to seed or reset your database.
- For CORS/API issues, verify the frontend `.env.local` and backend CORS settings.





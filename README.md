# Library Management

A Blazor web app for managing library books, lenders, and loans using .NET 9, Radzen.Blazor, and EF Core with In memory databse (IndexDB).

## Prerequisites
- .NET 9 SDK

## Setup
1. Clone the repo or extract the project files.
2. Navigate to the LibraryManagement folder.
3. Run dotnet restore to install NuGet packages.
4. Run dotnet build to verify the build.
5. Run dotnet run --project LibraryApp to start the app.
6. Access at https://localhost:5001 (or the port shown in the console).
7. To run tests, navigate to LibraryManagement.Tests and run dotnet test.

The app creates a in-memory database (library.db) and seeds initial data on first run.

## Implemented Features
- **Books**: CRUD with validations (unique ISBN, CopiesAvailable <= TotalCopies).
- **Lenders**: CRUD with validations (required fields, email format).
- **Loans**: Create with rules (availability, max 5 active loans, no duplicates, no overdue loans), return updates copies, grid shows status and highlights overdue loans.
- Radzen components used for grids, dialogs, forms, and notifications.
- Unit tests for services covering key business rules.
to run test open cmd in project root folder 
- **dotnet test**
- ![img.png](img.png)

## Notes
- Use the sidebar to navigate to Books, Lenders, or Loans.
- In-Memory database is stored as library.db in the LibraryApp folder.
- Tests use an in-memory database for isolation.
# LibraryManagement
System sequence diagram

![Component Interaction Diagram.png](Component%20Interaction%20Diagram.png)

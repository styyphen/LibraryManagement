# Design Decisions

## Application Organization
The solution is structured into:
- **LibraryApp**: Blazor Web App with UI (Pages, Components, Shared).
- **Models**: Entity classes with DataAnnotations for validation.
- **Data**: EF Core DbContext and seeding logic.
- **Services**: Business logic for CRUD and loan rules.
- **LibraryManagement.Tests**: xUnit tests for services.
This separation ensures maintainability and scalability (e.g., easy to add APIs).

## Validation and Rules Enforcement
- **Model-level**: DataAnnotations ([Required], [EmailAddress], [Range]) for UI form validation.
- **Service-level**: Business rules (e.g., ISBN uniqueness, loan limits) checked via EF queries before DB operations. Why? Prevents invalid data from reaching the database, ensuring consistency.
- **DB-level**: Unique index on ISBN for final integrity.

## Trade-off
Used simple exception handling instead of EF transactions for loan operations to prioritize development speed. This is sufficient for a small app but may need transactions for concurrency in a larger system.

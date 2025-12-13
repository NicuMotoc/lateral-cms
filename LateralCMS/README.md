# LateralCMS

A .NET 10 CMS event ingestion and query service.

## Features
- Webhook endpoint `/cms/events` for ingesting CMS events (add, update, publish, unpublish, delete)
- REST API `/entities` for listing and disabling entities
- Basic Authentication for both CMS and user endpoints
- In-memory EF Core database
- Clean & Screaming Architecture with CQRS folder structure
- Logging, validation and sanitization
- Uses [FluentValidation](https://docs.fluentvalidation.net/en/latest/) for input validation

## Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (experimental, non-standard)

## Setup & Run

1. **Clone the repo**
2. **Run**:
   ```bash
   dotnet run --project LateralCMS/LateralCMS.csproj
   ```
3. **API Endpoints**:
   - `POST /cms/events` (Basic Auth - cms user)
   - `GET /entities` (Basic Auth - all users)
   - `POST /entities/{id}/disable` (Basic Auth - admin only)

## Testing
- Use tools like Postman or Insomnia. You can import the API into your Insomnia by going to https://localhost:7146/openapi/v1.json

## OpenAPI/Swagger
- OpenAPI docs are available at `/openapi` (in development mode).

## Authentication
- User credentials are configured in `appsettings.json`.

## Notes
- All data is confidential and requires authentication.
- Unpublished entities are not deleted, only hard deletes remove data.
- Admins see all entities, including disabled ones.
- If you encounter a `System.Text.Json.JsonException` about object cycles, the project is configured to handle circular references using `ReferenceHandler.Preserve`.

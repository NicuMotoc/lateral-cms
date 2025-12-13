# LateralCMS

A .NET 10 CMS event ingestion and query service.

## Features
- Webhook endpoint `/cms/events` for ingesting CMS events (add, update, publish, unpublish, delete)
- REST API `/entities` for listing and disabling entities
- Basic Authentication for both CMS and user endpoints
- In-memory EF Core database
- Clean & Screaming Architecture with CQRS folder structure
- Logging, validation and sanitization

## Setup & Run

1. **Clone the repo**
2. **Run**:
   ```bash
   dotnet run --project LateralCMS/LateralCMS.csproj
   ```
3. **API Endpoints**: (users are in the appsettings)
   - `POST /cms/events` (Basic Auth - cms user)
   - `GET /entities` (Basic Auth - all users)
   - `POST /entities/{id}/disable` (Basic Auth - admin only)

## Testing
- Use tools like Postman or Insomnia. You can import thee API into your insomnia for example by going to https://localhost:7146/openapi/v1.json

## Notes
- All data is confidential and requires authentication.
- Unpublished entities are not deleted, only hard deletes remove data.
- Admins see all entities, including disabled ones.

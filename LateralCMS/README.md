# LateralCMS

A Clean Architecture .NET 10 CMS event ingestion and query service.

## Features
- Webhook endpoint `/cms/events` for ingesting CMS events (publish, unpublish, delete)
- REST API `/entities` for listing and disabling entities
- Basic Authentication for both CMS and user endpoints
- In-memory EF Core database
- Clean Architecture & Screaming Architecture folder structure
- Logging and validation

## Setup & Run

1. **Clone the repo**
2. **Run**:
   ```bash
   dotnet run --project LateralCMS/LateralCMS.csproj
   ```
3. **API Endpoints**:
   - `POST /cms/events` (Basic Auth: `cms_webhook_user` / `b7e8e2e2-8c2a-4e2e-9b2e-2e2e2e2e2e2e`)
   - `GET /entities` (Basic Auth: `admin_user` / `adminpass123` or `normal_user` / `userpass123`)
   - `POST /entities/{id}/disable` (admin only)

## Testing
- Use tools like Postman or curl to test endpoints with the credentials above.
- Unit/integration tests are in the `LateralCMS.Tests` project.

## Notes
- All data is confidential and requires authentication.
- Unpublished entities are not deleted, only hard deletes remove data.
- Admins see all entities, including disabled ones.

---

**.NET 10, Clean Architecture, Screaming Architecture**

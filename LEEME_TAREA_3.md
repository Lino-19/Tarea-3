# Gestor de Reservas para Excursiones Acuáticas

## Arquitectura
- **GestorExcursiones.Domain**: entidades del dominio.
- **GestorExcursiones.Infrastructure**: DbContext, relaciones EF Core y repositorios genéricos/concretos.
- **1--Gestión de excursiones acuáticas**: API REST, DTOs, controladores e inyección de dependencias.

Todos los controladores usan repositorios conectados al mismo `ApplicationDbContext` y SQL Server. Las reservas calculan el importe según precio de excursión por participantes.

## Requisitos y preparación
Requiere SDK .NET 10 y SQL Server LocalDB. Si usas otra instancia, cambia `ConnectionStrings:DefaultConnection` en `appsettings.json`.
Desde la carpeta de la solución ejecuta:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project GestorExcursiones.Infrastructure/GestorExcursiones.Infrastructure.csproj --startup-project "1--Gestión de excursiones acuáticas/1--Gestión de excursiones acuáticas.csproj"
dotnet ef database update --project GestorExcursiones.Infrastructure/GestorExcursiones.Infrastructure.csproj --startup-project "1--Gestión de excursiones acuáticas/1--Gestión de excursiones acuáticas.csproj"
dotnet restore "1-Gestión de excursiones acuáticas.slnx"
dotnet run --project "1--Gestión de excursiones acuáticas/1--Gestión de excursiones acuáticas.csproj"
```
Si `dotnet-ef` ya está instalado, omite el primer comando. Swagger se habilita en Development.

## Endpoints
- `/api/Clients`: GET, GET por id, POST, PUT, DELETE.
- `/api/Excursions`: GET, GET por id, POST, PUT, DELETE; `/future` lista excursiones futuras.
- `/api/Guides`: GET, GET por id, POST, PUT, DELETE.
- `/api/Reservations`: GET, GET por id, POST, DELETE; `PATCH /api/Reservations/{id}/pago` actualiza el pago.

La solución se revisó estáticamente. No se pudo ejecutar `dotnet build` en este entorno porque no tiene SDK .NET instalado.

# Sistema de Ventas de Cafetería (CafeSales)

Implementación completa del sistema según la documentación técnica en [SistemaVentasCafeteria.docx](file:///d:/cremoso/SistemaVentasCafeteria.docx). Aplicación web ASP.NET Core MVC con arquitectura en capas, SQL Server y Entity Framework Core.

## User Review Required

> [!IMPORTANT]
> **Conexión a SQL Server**: La documentación indica el servidor `DESKTOP-RJ1GCOM` con Windows Authentication. ¿Es correcto este servidor o debemos usar otro? Si no tienes SQL Server disponible, puedo configurar SQLite como alternativa para desarrollo local.

> [!IMPORTANT]
> **Versión .NET**: Tienes .NET 10 SDK instalado. Crearé el proyecto con .NET 10 (la documentación menciona ASP.NET Core genérico, así que usaremos la versión más reciente disponible).

> [!WARNING]
> **Datos semilla**: Crearé datos iniciales (seed data) con un usuario Administrador por defecto (`admin` / `Admin123!`) y categorías de productos de cafetería. ¿Deseas otros datos iniciales?

## Open Questions

1. ¿Tienes SQL Server instalado y accesible en `DESKTOP-RJ1GCOM`? ¿O prefieres usar **LocalDB** / **SQLite** para desarrollo?
2. ¿Quieres que incluya Bootstrap 5 para el diseño responsivo de las vistas Razor, o prefieres otro framework CSS?
3. ¿Deseas que genere las **migraciones EF Core** automáticamente y las aplique a la base de datos?

## Proposed Changes

La estructura completa del proyecto será:

```
d:\cremoso\CafeSales\
├── CafeSales.sln
├── CafeSales.Web/           ← Capa Presentación (ASP.NET Core MVC)
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── DashboardController.cs
│   │   ├── ProductoController.cs
│   │   ├── ClienteController.cs
│   │   ├── VentaController.cs
│   │   ├── UsuarioController.cs
│   │   └── ReporteController.cs
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _LoginPartial.cshtml
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   ├── Account/ (Login, AccessDenied)
│   │   ├── Dashboard/ (Index)
│   │   ├── Producto/ (Index, Crear, Editar, Detalles)
│   │   ├── Cliente/ (Index, Crear, Editar)
│   │   ├── Venta/ (Index, Registrar, Detalles)
│   │   ├── Usuario/ (Index, Crear, Editar)
│   │   └── Reporte/ (Index, PorPeriodo, ProductosMasVendidos, PorVendedor)
│   ├── ViewModels/
│   ├── wwwroot/ (CSS, JS)
│   ├── Program.cs
│   └── appsettings.json
│
├── CafeSales.Business/      ← Capa Lógica de Negocio
│   ├── Interfaces/
│   │   ├── IProductoService.cs
│   │   ├── IClienteService.cs
│   │   ├── IVentaService.cs
│   │   ├── IUsuarioService.cs
│   │   └── IReporteService.cs
│   └── Services/
│       ├── ProductoService.cs
│       ├── ClienteService.cs
│       ├── VentaService.cs
│       ├── UsuarioService.cs
│       └── ReporteService.cs
│
├── CafeSales.Data/           ← Capa Acceso a Datos
│   ├── Context/
│   │   └── CafeDbContext.cs
│   └── Migrations/
│
├── CafeSales.Models/         ← Entidades y DTOs
│   ├── Entities/
│   │   ├── Producto.cs
│   │   ├── Cliente.cs
│   │   ├── Venta.cs
│   │   ├── DetalleVenta.cs
│   │   ├── Usuario.cs
│   │   ├── Rol.cs
│   │   └── Categoria.cs
│   └── ViewModels/
│       ├── LoginViewModel.cs
│       ├── VentaViewModel.cs
│       ├── DashboardViewModel.cs
│       └── ReporteViewModel.cs
│
└── CafeSales.Tests/          ← Pruebas Unitarias
    └── ProductoServiceTests.cs
```

---

### 1. CafeSales.Models (Entidades y ViewModels)

#### [NEW] Entities/Producto.cs, Cliente.cs, Venta.cs, DetalleVenta.cs, Usuario.cs, Rol.cs, Categoria.cs
- Todas las entidades POCO según la documentación
- Propiedades de navegación para relaciones EF Core
- Data Annotations para validación

#### [NEW] ViewModels/LoginViewModel.cs, VentaViewModel.cs, DashboardViewModel.cs, ReporteViewModel.cs
- ViewModels para las vistas que requieren datos combinados

---

### 2. CafeSales.Data (Acceso a Datos)

#### [NEW] Context/CafeDbContext.cs
- DbSets para todas las entidades
- Configuración Fluent API para:
  - Columna calculada `Subtotal` en `DetalleVenta`
  - Relaciones y restricciones
  - Datos semilla (roles, categorías, usuario admin)

---

### 3. CafeSales.Business (Lógica de Negocio)

#### [NEW] Interfaces/ (IProductoService, IClienteService, IVentaService, IUsuarioService, IReporteService)
- Contratos para cada servicio CRUD + reportes

#### [NEW] Services/ (ProductoService, ClienteService, VentaService, UsuarioService, ReporteService)
- Implementación con inyección de `CafeDbContext`
- Baja lógica (soft delete) en productos, clientes y usuarios
- Descuento automático de stock en ventas
- Queries para reportes (por período, productos más vendidos, por vendedor)

---

### 4. CafeSales.Web (Presentación)

#### [NEW] Program.cs
- Configuración de EF Core con SQL Server
- Autenticación por cookies
- Inyección de dependencias de todos los servicios

#### [NEW] Controllers/
- **AccountController**: Login, Logout, AccessDenied
- **DashboardController**: KPIs (ventas del día, ingresos, stock bajo)
- **ProductoController**: CRUD con autorización por roles
- **ClienteController**: CRUD con autorización
- **VentaController**: Registro de ventas con carrito dinámico
- **UsuarioController**: Gestión de usuarios (solo Admin)
- **ReporteController**: Reportes con filtros (solo Admin)

#### [NEW] Views/
- Layout moderno con sidebar de navegación, Bootstrap 5
- Vistas Razor para cada controlador
- Formularios con validación client-side
- Tablas con filtros y búsqueda
- Dashboard con tarjetas KPI

#### [NEW] wwwroot/
- CSS personalizado con tema de cafetería
- JavaScript para carrito de ventas dinámico
- Bootstrap 5 + Bootstrap Icons

---

### 5. CafeSales.Tests (Pruebas)

#### [NEW] ProductoServiceTests.cs
- Tests con xUnit + InMemory DB
- `CrearProducto_DebeAgregarAlContexto`
- `EliminarProducto_DebePonerseBajaLogica`

---

## Verification Plan

### Automated Tests
```bash
dotnet build CafeSales.sln
dotnet test CafeSales.Tests/
```

### Manual Verification
1. Ejecutar `dotnet run` en `CafeSales.Web`
2. Verificar que la aplicación carga en el navegador
3. Login con usuario admin por defecto
4. Navegar por todos los módulos (Dashboard, Productos, Clientes, Ventas, Usuarios, Reportes)

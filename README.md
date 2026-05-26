# CafeCremoso - Sistema de Ventas para Cafeteria

Aplicacion web para administrar las operaciones principales de una cafeteria: productos, clientes, ventas, usuarios, inventario y reportes.

El sistema fue desarrollado con **ASP.NET Core MVC**, **.NET 10**, **Entity Framework Core** y **SQL Server**, aplicando una arquitectura por capas.

## Funcionalidades

- Inicio de sesion con autenticacion por cookies.
- Gestion de productos, categorias y stock.
- Registro y administracion de clientes.
- Registro de ventas con multiples productos.
- Descuento automatico de inventario al confirmar ventas.
- Administracion de usuarios y roles.
- Dashboard con indicadores principales.
- Reportes de ventas, productos mas vendidos y ventas por vendedor.
- Pruebas automatizadas para la logica de negocio.

## Tecnologias Utilizadas

| Tecnologia | Uso |
| --- | --- |
| .NET 10 | Plataforma principal |
| ASP.NET Core MVC | Aplicacion web y vistas Razor |
| Entity Framework Core 10 | Acceso y mapeo de datos |
| SQL Server | Base de datos |
| Cookie Authentication | Inicio de sesion |
| BCrypt.Net-Next | Proteccion de contrasenas |
| Bootstrap | Diseno responsivo |
| xUnit | Pruebas automatizadas |
| Coverlet | Cobertura de codigo |

## Arquitectura del Proyecto

```text
CafeSales/
|-- CafeSales.Web/       Interfaz web, controladores y vistas
|-- CafeSales.Business/  Logica de negocio y servicios
|-- CafeSales.Data/      Contexto de base de datos y migraciones
|-- CafeSales.Models/    Entidades y ViewModels
|-- CafeSales.Tests/     Pruebas automatizadas
|-- coverage.runsettings Configuracion de cobertura
```

## Modulos del Sistema

### Productos

Permite registrar, editar, buscar y desactivar productos. Cada producto incluye categoria, precio, stock y estado.

### Clientes

Permite registrar y administrar clientes, conservando su historial mediante baja logica.

### Ventas

Permite registrar ventas seleccionando uno o varios productos. El sistema valida stock, calcula el total y descuenta automaticamente las unidades vendidas.

### Usuarios

Permite administrar usuarios con roles de acceso:

- `Administrador`
- `Vendedor`

### Dashboard y Reportes

Incluye informacion como:

- Ventas del dia.
- Ingresos diarios y mensuales.
- Productos con bajo stock.
- Ventas recientes.
- Productos mas vendidos.
- Ventas por vendedor.

## Requisitos

Antes de ejecutar el proyecto, necesitas:

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server
- Entity Framework Core CLI

Instalar la herramienta de Entity Framework Core:

```powershell
dotnet tool install --global dotnet-ef
```

## Instalacion

Clona el repositorio:

```powershell
git clone https://github.com/jhonespinoza27/cremoso.git
cd cremoso
```

Restaura las dependencias:

```powershell
dotnet restore .\cremoso.sln
```

## Configuracion de Base de Datos

La cadena de conexion se encuentra en:

```text
CafeSales/CafeSales.Web/appsettings.json
```

Ejemplo de configuracion con autenticacion de Windows:

```json
{
  "ConnectionStrings": {
    "CafeConnection": "Server=TU_SERVIDOR;Database=CafeSalesDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Cambia `TU_SERVIDOR` por el nombre de tu instancia local de SQL Server.

Luego aplica las migraciones:

```powershell
dotnet ef database update `
  --project .\CafeSales\CafeSales.Data\CafeSales.Data.csproj `
  --startup-project .\CafeSales\CafeSales.Web\CafeSales.Web.csproj
```

## Ejecucion

Ejecuta la aplicacion web:

```powershell
dotnet run --project .\CafeSales\CafeSales.Web\CafeSales.Web.csproj
```

Abre en el navegador la direccion que aparezca en la terminal.

## Credenciales Iniciales

El proyecto incluye un usuario administrador de demostracion:

```text
Usuario: admin
Contrasena: Admin123
```



## Modelo de Datos

El sistema utiliza las siguientes entidades principales:

| Entidad | Descripcion |
| --- | --- |
| Categoria | Clasificacion de productos |
| Producto | Productos disponibles para venta |
| Cliente | Informacion de compradores |
| Rol | Perfil de acceso del usuario |
| Usuario | Usuarios autorizados del sistema |
| Venta | Cabecera de cada transaccion |
| DetalleVenta | Productos incluidos en cada venta |

Relaciones principales:

```text
Categoria 1 ----- N Producto
Rol       1 ----- N Usuario
Cliente   1 ----- N Venta
Usuario   1 ----- N Venta
Venta     1 ----- N DetalleVenta
Producto  1 ----- N DetalleVenta
```

## Pruebas

Para ejecutar las pruebas automatizadas:

```powershell
dotnet test .\CafeSales\CafeSales.Tests\CafeSales.Tests.csproj
```

Las pruebas utilizan una base SQL Server aislada para pruebas. Por defecto:

```text
CafeSalesDB_Tests
```

Tambien puedes definir otra conexion mediante la variable:

```powershell
$env:CAFE_TEST_CONNECTION_STRING="Server=TU_SERVIDOR;Database=CafeSalesDB_Tests;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Cobertura de Codigo

Para ejecutar pruebas con cobertura:

```powershell
dotnet test .\CafeSales\CafeSales.Tests\CafeSales.Tests.csproj `
  --settings .\CafeSales\coverage.runsettings `
  --collect:"XPlat Code Coverage" `
  --results-directory .\TestResults\CoverageFinal
```


## Autor

**Jhon Espinoza**

Repositorio: [github.com/jhonespinoza27/cremoso](https://github.com/jhonespinoza27/cremoso)

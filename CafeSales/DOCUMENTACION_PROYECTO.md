# Sistema de Ventas de Cafeteria - CafeSales

## 1. Descripcion general

**CafeSales** es una aplicacion web para administrar las operaciones principales
de una cafeteria. Permite iniciar sesion, gestionar productos y clientes,
registrar ventas, administrar usuarios y consultar reportes de ingresos y
productos vendidos.

El sistema fue desarrollado con **ASP.NET Core MVC** y **.NET 10**, utilizando
**Entity Framework Core** para el acceso a datos y **SQL Server** como motor de
base de datos.

En la maquina actual, la aplicacion se conecta mediante autenticacion de
Windows al servidor SQL Server:

```text
Servidor: JHON
Base de datos: CafeSalesDB
Autenticacion: Windows Authentication
```

La cadena de conexion utilizada es:

```json
"Server=JHON;Database=CafeSalesDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

## 2. Objetivo del sistema

El objetivo es disponer de un sistema ordenado para controlar las ventas de
una cafeteria y evitar operaciones manuales que pueden generar errores en el
stock o en el calculo de ingresos.

Las funciones principales son:

- Controlar productos, categorias, precios y stock.
- Registrar clientes.
- Registrar ventas con uno o varios productos.
- Reducir automaticamente el stock al confirmar una venta.
- Administrar usuarios con roles de acceso.
- Mostrar indicadores en un dashboard.
- Generar reportes de ventas, productos mas vendidos y ventas por vendedor.

## 3. Tecnologias utilizadas

| Tecnologia | Uso en el proyecto |
| --- | --- |
| .NET 10 | Plataforma de ejecucion |
| ASP.NET Core MVC | Aplicacion web, controladores y vistas Razor |
| Entity Framework Core 10.0.8 | Mapeo y acceso a base de datos |
| SQL Server | Persistencia de la informacion |
| Cookie Authentication | Inicio y cierre de sesion |
| BCrypt.Net-Next | Hash seguro de contrasenas |
| Bootstrap | Interfaz visual responsiva |
| xUnit | Pruebas unitarias |
| EF Core InMemory | Base en memoria para pruebas |
| Coverlet | Medicion de cobertura |

## 4. Arquitectura de la solucion

La aplicacion esta separada en capas. Esta organizacion permite mantener la
interfaz, la logica y los datos en proyectos independientes.

```text
CafeSales/
|-- CafeSales.Web/       Presentacion: controladores, vistas y configuracion
|-- CafeSales.Business/  Reglas de negocio y servicios
|-- CafeSales.Data/      DbContext, mapeos y migraciones
|-- CafeSales.Models/    Entidades y ViewModels
|-- CafeSales.Tests/     Pruebas unitarias
|-- coverage.runsettings Configuracion de cobertura
```

### 4.1. CafeSales.Web

Es la capa visible para el usuario. Contiene:

- `Program.cs`: configuracion de SQL Server, autenticacion por cookies,
  inyeccion de dependencias y rutas.
- `Controllers/`: recibe las acciones del usuario y coordina los servicios.
- `Views/`: paginas Razor para formularios, listados, dashboard y reportes.
- `wwwroot/`: archivos CSS, JavaScript y librerias de interfaz.

### 4.2. CafeSales.Business

Contiene las reglas del negocio mediante interfaces y servicios:

- `ProductoService`: listado, busqueda, creacion, actualizacion y baja logica
  de productos.
- `ClienteService`: gestion y busqueda de clientes.
- `VentaService`: registro de ventas, calculo de total y descuento de stock.
- `UsuarioService`: autenticacion, creacion y desactivacion de usuarios.
- `ReporteService`: calculo de dashboard y reportes.

Los controladores utilizan interfaces como `IProductoService` o
`IVentaService`, lo que facilita realizar pruebas y mantener el codigo.

### 4.3. CafeSales.Data

Contiene `CafeDbContext`, que representa la base de datos en Entity Framework
Core. Define las tablas, relaciones, restricciones, tipos decimales, datos
iniciales y la columna calculada del subtotal.

Tambien contiene las migraciones de Entity Framework. La migracion inicial
crea la estructura completa de `CafeSalesDB`.

### 4.4. CafeSales.Models

Contiene:

- Entidades que corresponden a tablas: `Producto`, `Categoria`, `Cliente`,
  `Venta`, `DetalleVenta`, `Usuario` y `Rol`.
- ViewModels para formularios o reportes: login, registro de venta, dashboard
  y reportes.

Las entidades usan anotaciones de validacion, por ejemplo: campos requeridos,
longitudes maximas, formato de correo y rangos de precio o cantidad.

### 4.5. CafeSales.Tests

Contiene pruebas unitarias con xUnit y una base de datos en memoria. Esto
permite comprobar la logica sin modificar la base SQL Server real.

## 5. Modelo de datos

### 5.1. Tablas principales

| Tabla | Funcion |
| --- | --- |
| `Categorias` | Clasifica los productos de la cafeteria |
| `Productos` | Guarda nombre, descripcion, precio, stock y estado |
| `Clientes` | Guarda los datos del comprador |
| `Roles` | Define permisos como Administrador o Vendedor |
| `Usuarios` | Usuarios que ingresan al sistema |
| `Ventas` | Cabecera de cada venta, cliente, vendedor, fecha y total |
| `DetalleVentas` | Productos y cantidades que forman una venta |

### 5.2. Relaciones

```text
Categoria 1 ----- N Producto
Rol       1 ----- N Usuario
Cliente   1 ----- N Venta
Usuario   1 ----- N Venta
Venta     1 ----- N DetalleVenta
Producto  1 ----- N DetalleVenta
```

### 5.3. Reglas importantes de base de datos

- El nombre de una categoria es unico.
- El nombre de usuario es unico.
- El nombre de un rol es unico.
- El precio y total se almacenan como `decimal(10,2)`.
- El subtotal de un detalle se calcula en SQL Server como:

```sql
[Cantidad] * [PrecioUnit]
```

- Al eliminar una venta, sus detalles se eliminan en cascada.
- Productos, clientes y usuarios utilizan baja logica: se marcan como
  inactivos en lugar de eliminarse fisicamente.

## 6. Datos iniciales

Al crear la base mediante la migracion, el sistema inserta datos iniciales:

- Roles: `Administrador` y `Vendedor`.
- Seis categorias: Cafe, Te e Infusiones, Bebidas Frias, Panaderia, Postres y
  Snacks.
- Un cliente general con documento `00000000`.
- Diez productos de ejemplo.
- Un usuario administrador inicial.

Credenciales iniciales de desarrollo:

```text
Usuario: admin
Contrasena: Admin123!
```

La contrasena no se almacena como texto; se almacena su hash BCrypt.

## 7. Funcionamiento de la aplicacion

### 7.1. Inicio de sesion

La ruta inicial abre el login (`Account/Login`). El usuario ingresa sus
credenciales y `UsuarioService` valida:

1. Que el usuario exista y este activo.
2. Que la contrasena coincida con el hash BCrypt almacenado.

Cuando la validacion es correcta, la aplicacion crea una cookie de sesion con
datos del usuario:

- Nombre de usuario.
- Rol.
- Identificador del usuario.
- Nombre completo.

La sesion tiene una duracion configurada de ocho horas.

### 7.2. Permisos por rol

La aplicacion usa atributos de autorizacion:

- Un usuario autenticado puede ingresar al dashboard, clientes, ventas y
  consultas de productos.
- El rol `Administrador` es necesario para crear, editar o eliminar productos,
  administrar usuarios y consultar reportes administrativos.

### 7.3. Gestion de productos

El modulo de productos permite:

- Mostrar productos activos.
- Buscar por nombre o descripcion.
- Filtrar por categoria.
- Ver detalles.
- Crear y editar productos.
- Desactivar productos mediante baja logica.

Un producto inactivo no esta disponible para nuevas ventas.

### 7.4. Gestion de clientes

El modulo de clientes permite registrar compradores y buscar por nombre,
documento o correo. La eliminacion tambien es logica: el cliente se mantiene en
la base para conservar el historial, pero deja de aparecer como cliente activo.

### 7.5. Registro de ventas

El flujo principal de negocio ocurre en el registro de ventas:

1. El usuario selecciona un cliente.
2. Agrega uno o mas productos y cantidades.
3. El sistema valida que la cantidad sea mayor a cero.
4. Verifica que cada producto exista, este activo y tenga stock disponible.
5. Toma el precio actual registrado en la base de datos.
6. Descuenta el stock de cada producto.
7. Calcula el total de la venta.
8. Guarda la venta y sus detalles.

Si el proveedor de base de datos es relacional, el servicio utiliza una
transaccion para confirmar la operacion completa o revertirla ante un error.
Esto evita guardar ventas incompletas o descontar stock incorrectamente.

### 7.6. Dashboard

El dashboard resume el estado actual del negocio:

- Cantidad de ventas del dia.
- Ingresos del dia.
- Ingresos del mes.
- Productos con bajo stock.
- Total de clientes y productos activos.
- Productos populares del mes.
- Ventas recientes.

### 7.7. Reportes

El sistema ofrece los siguientes reportes:

- Ventas por periodo: muestra ventas realizadas y total recaudado.
- Productos mas vendidos: agrupa cantidades e ingresos por producto.
- Ventas por vendedor: agrupa cantidad de ventas e ingresos por usuario.

## 8. Configuracion de la base de datos en la nueva maquina

Debido al cambio de maquina, la conexion anterior apuntaba al servidor
`DESKTOP-RJ1GCOM`. Se actualizo la configuracion para utilizar SQL Server
`JHON` con autenticacion de Windows.

Archivo modificado:

```text
CafeSales.Web/appsettings.json
```

Tambien se instalo la herramienta `dotnet-ef` y se aplico la migracion inicial
para crear `CafeSalesDB` con sus tablas y datos semilla:

```powershell
dotnet ef database update --project .\CafeSales.Data\CafeSales.Data.csproj `
  --startup-project .\CafeSales.Web\CafeSales.Web.csproj `
  --connection "Server=JHON;Database=CafeSalesDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

## 9. Pruebas unitarias

Las pruebas se realizan con xUnit contra SQL Server. Por defecto utilizan
`Server=JHON;Database=CafeSalesDB_Tests;Trusted_Connection=True;TrustServerCertificate=True;`.
La base de pruebas se recrea antes de cada caso y nunca debe ser la base de la
aplicacion. Para usar otra conexion aislada, definir `CAFE_TEST_CONNECTION_STRING`;
como proteccion, el nombre de la base debe contener `Test`.

### 9.1. Areas probadas

| Archivo de pruebas | Comportamiento verificado |
| --- | --- |
| `ProductoServiceTests.cs` | Crear, editar, filtrar, consultar y desactivar productos |
| `ClienteServiceTests.cs` | Crear, buscar, editar, consultar y desactivar clientes |
| `VentaServiceTests.cs` | Registro, stock, precio, busqueda y validaciones de venta |
| `UsuarioServiceTests.cs` | Autenticacion, hash, roles, actualizacion y baja logica |
| `ReporteServiceTests.cs` | Dashboard y calculos de reportes |

Se agrego tambien la prueba `Actualizar_IdInexistente_NoDebeLanzarExcepcion`
para comprobar que el servicio de usuarios maneja correctamente una edicion de
un identificador que no existe.

### 9.2. Resultado verificado

```text
Pruebas ejecutadas: 53
Pruebas correctas: 53
Pruebas fallidas: 0
```

Comando para ejecutarlas:

```powershell
dotnet test .\CafeSales\CafeSales.Tests\CafeSales.Tests.csproj
```

## 10. Cobertura de codigo

Inicialmente el reporte de cobertura era bajo porque contabilizaba las
migraciones generadas automaticamente por Entity Framework Core. Esos archivos
crean la base de datos, pero no contienen reglas de negocio que deban cubrirse
con pruebas unitarias.

Por esa razon se creo `coverage.runsettings`, que excluye solamente:

```text
**/Migrations/*.cs
```

Esto no elimina pruebas de la logica implementada; permite medir de forma
correcta el codigo mantenido por el desarrollador.

Resultado final verificado:

| Proyecto | Cobertura de lineas | Cobertura de ramas |
| --- | ---: | ---: |
| `CafeSales.Business` | 99.31% | 91.30% |
| `CafeSales.Data` | 100.00% | 100.00% |
| `CafeSales.Models` | 96.19% | 100.00% |
| **Total** | **98.83%** | **91.30%** |

Comando para ejecutar pruebas y generar cobertura:

```powershell
dotnet test .\CafeSales\CafeSales.Tests\CafeSales.Tests.csproj `
  --settings .\CafeSales\coverage.runsettings `
  --collect:"XPlat Code Coverage" `
  --results-directory .\TestResults\CoverageFinal
```

El resultado se genera como archivo `coverage.cobertura.xml` dentro del
directorio de resultados.

## 11. Como ejecutar la aplicacion

### Requisitos

- .NET SDK 10 instalado.
- SQL Server disponible en `JHON`.
- Acceso mediante Windows Authentication.
- Base `CafeSalesDB` creada mediante la migracion.

### Ejecucion

Desde la carpeta raiz del repositorio:

```powershell
dotnet run --project .\CafeSales\CafeSales.Web\CafeSales.Web.csproj
```

Luego se puede ingresar con el usuario administrador inicial para comprobar
los modulos del sistema.

## 12. Conclusiones

CafeSales fue construido aplicando una arquitectura en capas, separando
presentacion, logica de negocio, acceso a datos, modelos y pruebas. La
aplicacion administra correctamente los procesos principales de una cafeteria:
productos, clientes, ventas, usuarios y reportes.

El registro de ventas valida disponibilidad y descuenta stock de manera
controlada; la autenticacion protege el acceso mediante roles; y las pruebas
unitarias verifican las principales reglas del negocio con una cobertura
superior al 90% solicitado.

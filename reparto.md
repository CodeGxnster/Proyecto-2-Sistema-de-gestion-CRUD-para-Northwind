# Reparto de Trabajo — Northwind Gestion CRUD

> **Alcance:** este documento cubre únicamente código y artefactos de código (script SQL, compilación y logs de ejemplo). Diagramas UML/ER, README y video quedan fuera de este reparto.

## 0. Reglas técnicas obligatorias

Estas reglas aplican a **todo** el código, sin importar quién sea el dueño del módulo:

- **Dirección de dependencias:** `Presentacion -> LogicaNegocio -> AccesoDatos -> Entidades`; `Comun` es transversal. `Presentacion` no referencia `AccesoDatos`.
- **Acceso a datos primero:** ninguna rama de módulo puede iniciar hasta que `feature/acceso-datos-uow` esté mergeada en `main`.
- **Repositorio genérico:** `IRepositorio<T>` y `Repositorio<T>` sobre `NorthwindDbContext`. Los repositorios específicos deben implementar o extender el repositorio genérico.
- **Unit of Work:** `IUnitOfWork` agrupa repositorios y expone `SaveChanges()`, `BeginTransaction()`, `Commit()` y `Rollback()`. Los repositorios **no** llaman `SaveChanges()`; lo hace el servicio o el UoW.
- **Sin SQL crudo:** prohibido `FromSqlRaw`, `ExecuteSqlRaw`, `DbCommand` o SQL embebido. Todas las consultas se hacen con LINQ sobre EF Core. La única excepción permitida son los reportes analíticos si se justifica y se documenta.
- **DTO Pattern:** las entidades EF no se exponen a la UI. La UI consume DTOs.
- **FluentValidation:** un validador por entidad/DTO editable, ejecutado en `LogicaNegocio` antes de persistir.
- **Errores de validación:** se devuelven a la UI y se muestran con `ErrorProvider` junto al control correspondiente.
- **Configuración:** `appsettings.json` en la raíz del ejecutable, leído con `Microsoft.Extensions.Configuration` e `IOptions`. Sin cadenas de conexión ni configuración hardcodeada.
- **Logging:** Serilog con consola y archivo rolling diario. Cada servicio usa `ILogger<T>` o `ForContext<T>()` para que el `SourceContext` no quede vacío.
- **Convenciones:** PascalCase para clases/métodos/propiedades, camelCase para variables/parámetros, prefijo `I` para interfaces, XML docs en métodos públicos, un archivo por clase, sin código comentado y `using` ordenados.
- **Git:** ambos integrantes deben tener ramas y commits significativos. No se hace commit directo a `main`.

## 1. Asignación de trabajo

### Frankelly

#### 1.1 Configuración base y capa Comun
- Crear la solución en `src` y los 5 proyectos: `Northwind.Entidades`, `Northwind.AccesoDatos`, `Northwind.LogicaNegocio`, `Northwind.Comun` y `Northwind.Presentacion`.
- Configurar referencias entre proyectos respetando la dirección de dependencias.
- `Northwind.Comun`:
  - Lectura de `appsettings.json` con `Microsoft.Extensions.Configuration`.
  - Clases de configuración con `IOptions` (`AplicacionSettings`, `RegistroSettings` con sección `Logging`, `BaseDatosSettings`, `ReportesSettings`).
  - Serilog con sinks de consola y archivo rolling diario.
  - Plantilla de log con timestamp, nivel, `SourceContext` y mensaje.
  - Registrar inicio y cierre de la aplicación.

#### 1.2 Módulo Categorías
- DTO `CategoryDTO`.
- `CategoriaValidator` con FluentValidation:
  - Nombre obligatorio, entre 3 y 15 caracteres.
  - Descripción máximo 200 caracteres.
  - Nombre único (validación en el servicio).
- `CategoriaServicio`:
  - Listar, crear, editar y eliminar.
  - No eliminar categorías con productos asociados.
  - Registrar cada CRUD exitoso con el ID de la categoría.
  - Registrar errores con excepción completa.
- `FrmCategorias` y `FrmCategoriaDetalle`:
  - Búsqueda en tiempo real.
  - `ErrorProvider` junto al campo.
  - Confirmación antes de eliminar.
  - Sin SQL crudo ni lógica de acceso a datos en el formulario.

#### 1.3 Módulo Productos
- DTO `ProductDTO` y `FiltroProductoDTO`.
- `ProductoValidator` con FluentValidation:
  - Nombre obligatorio.
  - Precio unitario **mayor o igual a 0**.
  - Unidades en existencia, unidades pedidas y nivel de reorden **mayores o iguales a 0**.
  - Categoría y suplidor existentes (validación en el servicio).
- `ProductoServicio`:
  - Listar, crear, editar y eliminar.
  - Mostrar nombre de categoría y suplidor, no solo IDs.
  - Filtros combinables: categoría, suplidor, rango de precios y estado (activo/descontinuado).
  - Registrar cada CRUD exitoso con el ID del producto.
- `FrmProductos` y `FrmProductoDetalle`:
  - Búsqueda en tiempo real.
  - `ErrorProvider` junto al campo.
  - Confirmación antes de eliminar.
  - Sin SQL crudo ni lógica de acceso a datos en el formulario.

#### 1.4 Reportes analíticos
- Tres consultas LINQ sobre EF Core:
  1. Top 10 productos más vendidos por cantidad total en `Order Details`.
  2. Ventas por categoría en un rango de fechas.
  3. Suplidores con mayor valor de inventario (`productos × precio × existencia`).
- `FrmReportes` con `DataGridView`, selector de reporte, rango de fechas con `DateTimePicker` y exportación a CSV y Excel.
- Registrar la generación de cada reporte en el log.
- Las consultas viven en `AccesoDatos`; Frankelly define la consulta y Ángel la implementa en el repositorio, o se coordina para que quede en la capa correcta.

#### 1.5 Integración
- Co-responsable de `Program.cs` y `FrmPrincipal` junto con Ángel.
- Registrar en DI todos los servicios y formularios de los módulos.
- Verificar que `Presentacion` no referencie `AccesoDatos`.

### Ángel

#### 2.1 Capa de acceso a datos (primera rama)
- Entidades EF completas en `Northwind.Entidades`:
  - `Categoria`, `Suplidor`, `Producto`, `Pedido`, `DetallePedido`.
  - `Cliente`, `Empleado`, `Transportista` (tablas `Customers`, `Employees`, `Shippers`).
- Campos completos según `instnwnd.sql`:
  - `Suplidor`: `SupplierID`, `CompanyName`, `ContactName`, `ContactTitle`, `Address`, `City`, `Region`, `PostalCode`, `Country`, `Phone`, `Fax`, `HomePage` y la decisión sobre `Email`.
  - `Pedido`: `OrderID`, `CustomerID`, `EmployeeID`, `OrderDate`, `RequiredDate`, `ShippedDate`, `ShipVia`, `Freight`, `ShipName`, `ShipAddress`, `ShipCity`, `ShipRegion`, `ShipPostalCode`, `ShipCountry`.
  - `DetallePedido`: `OrderID`, `ProductID`, `UnitPrice`, `Quantity`, `Discount`.
- `NorthwindDbContext` con `DbSet<T>` para cada entidad y mapeos Fluent API contra las tablas reales.
- `IRepositorio<T>` y `Repositorio<T>` genéricos.
- `IUnitOfWork` y `UnitOfWork` con transacciones.
- Repositorios específicos para Categorías, Productos, Suplidores, Órdenes, Clientes, Empleados, Transportistas y Reportes.
- Ningún repositorio llama `SaveChanges()`.
- Ninguna consulta usa SQL crudo.
- Script `database/InstalarNorthwind.sql` basado en `instnwnd.sql`.

#### 2.2 Módulo Suplidores
- DTO `SuplidorDTO`.
- `SuplidorValidator` con FluentValidation:
  - Nombre de empresa, contacto y país obligatorios.
  - Dirección, ciudad, teléfono y fax.
  - Email válido si se proporciona.
- `SuplidorServicio`:
  - Listar, crear, editar y eliminar.
  - No eliminar suplidores con productos asociados.
  - Registrar cada CRUD exitoso con el ID del suplidor.
- `FrmSuplidores` y `FrmSuplidorDetalle`:
  - Búsqueda en tiempo real.
  - `ErrorProvider` junto al campo.
  - Confirmación antes de eliminar.

#### 2.3 Módulo Órdenes con Detalle (Master-Detail)
- DTOs: `OrdenDTO`, `OrdenListadoDTO`, `DetalleOrdenDTO`, `ClienteDTO`, `EmpleadoDTO`, `TransportistaDTO`.
- `OrdenValidator`:
  - Cliente y empleado obligatorios.
  - Fecha de orden y fecha requerida obligatorias.
  - Fecha de orden menor o igual a fecha requerida.
  - Al menos un detalle.
  - Flete mayor o igual a 0.
  - Transportista y dirección de envío.
- `DetalleOrdenValidator`:
  - Producto existente.
  - Cantidad mayor a 0.
  - Precio unitario mayor a 0.
  - Descuento entre 0 y 1.
  - Sin productos duplicados en la misma orden.
- `OrdenServicio`:
  - Listar con ID, cliente, fecha, empleado, total calculado y cantidad de líneas.
  - Filtrar por rango de fechas, cliente y empleado.
  - Crear y editar con `IDbContextTransaction`.
  - Eliminar la orden y todos sus detalles en una sola transacción.
  - Sugerir el precio unitario actual del producto al agregarlo al detalle.
  - Calcular el total automáticamente.
  - Registrar cambios con el `OrderID`.
  - En caso de error: `Rollback` y log del error con stack trace.
- `FrmOrdenes` y `FrmOrdenDetalle`:
  - Cabecera completa: cliente, empleado, fecha de orden, fecha requerida, fecha de envío, transportista, flete y dirección de envío.
  - Detalle con producto, cantidad, precio unitario y descuento.
  - Total calculado en pantalla mientras se editan los detalles.
  - Búsqueda en tiempo real, filtro por rango de fechas con `DateTimePicker`, filtro por cliente y empleado.
  - Confirmación obligatoria antes de eliminar.
  - `ErrorProvider` en cabecera y errores de fila en el detalle.
- Mecanismo de **rollback forzado demostrable** para el video: botón o modo de prueba controlado, sin dejar código muerto en producción.

#### 2.4 Base de datos y entrega compilada
- Copiar/adaptar `instnwnd.sql` a `database/InstalarNorthwind.sql`.
- Documentar que el script elimina y recrea `Northwind`.
- Verificar codificación del script (está en UTF-16); guardarlo en UTF-8 si se ejecuta con `sqlcmd`.
- Decidir y documentar el tema del `Email` de suplidores, porque `Suppliers` no tiene esa columna en el esquema original.
- Compilar en Release y publicar en `build/` con `appsettings.json` configurable.
- Generar `logs/app-ejemplo.log` con un ejemplo real.

### Ambos

- Integración final de `Program.cs`, `FrmPrincipal`, navegación y DI. **Un solo dueño de la integración** para evitar conflictos; el otro revisa.
- Pruebas de humo de todos los módulos.
- Prueba de rollback forzado de órdenes.
- Revisión cruzada del código del otro integrante.
- Commits significativos de ambos en el repositorio.
- Preparación de la defensa oral: ambos deben poder explicar cualquier módulo.

## 2. Orden de ramas e integración

1. `feature/setup-arquitectura` — Frankelly.
2. `feature/acceso-datos-uow` — Ángel. **Debe incluir todas las entidades EF, DbContext, mapeos, repositorio genérico, UoW y repositorios específicos.**
3. `feature/modulo-categorias` — Frankelly.
4. `feature/modulo-productos` — Frankelly.
5. `feature/modulo-suplidores` — Ángel.
6. `feature/modulo-ordenes` — Ángel.
7. `feature/reportes-analiticos` — Frankelly.
8. `feature/database-build` — Ángel.
9. `feature/integracion-final` — ambos.

Reglas:

- No iniciar una rama de módulo hasta que `feature/acceso-datos-uow` esté mergeada.
- Antes de cada rama: `git checkout main && git pull origin main`.
- Cada rama se prueba y se mergea a `main`; no commits directos.
- Si se mantiene la idea original de que cada módulo cree sus entidades, crear primero `feature/entidades-base` y mergearla antes de `feature/acceso-datos-uow`. La opción recomendada es que Ángel sea dueño de todas las entidades EF y Frankelly de los DTOs.

## 3. Contratos técnicos

### 3.1 Reglas de validación exactas

| Entidad | Reglas obligatorias |
| :--- | :--- |
| Categoría | Nombre obligatorio, 3–15 caracteres; descripción máximo 200; nombre único |
| Suplidor | Nombre empresa, contacto y país obligatorios; dirección, ciudad, teléfono y fax; email válido si se proporciona |
| Producto | Nombre obligatorio; precio ≥ 0; existencia ≥ 0; unidades pedidas ≥ 0; nivel de reorden ≥ 0; categoría y suplidor existentes |
| Orden | Cliente y empleado obligatorios; fecha de orden ≤ fecha requerida; al menos 1 detalle; flete ≥ 0; transportista y dirección de envío |
| Detalle | Producto existente; cantidad > 0; precio unitario > 0; descuento entre 0 y 1; sin producto duplicado |

### 3.2 `appsettings.json` mínimo

```json
{
  "ConnectionStrings": {
    "Northwind": "Server=.;Database=Northwind;Trusted_Connection=True;"
  },
  "Logging": {
    "RutaArchivo": "logs/app-.log",
    "NivelMinimo": "Information"
  },
  "Aplicacion": {
    "ItemsPorPagina": 50,
    "FormatoFecha": "dd/MM/yyyy",
    "SimboloMoneda": "RD$"
  }
}
```

- La sección debe llamarse `Logging`, no `Registro`.
- `Aplicacion` debe incluir `ItemsPorPagina`, `FormatoFecha` y `SimboloMoneda`.
- La lectura debe hacerse con `IOptions` o equivalente.
- Prohibido hardcodear la cadena de conexión o estos valores en el código.

### 3.3 Logging

- Dos sinks mínimo: consola y archivo rolling diario.
- Ruta: `logs/app-YYYYMMDD.log`.
- Formato: timestamp, nivel, `SourceContext` y mensaje.
- Registrar:
  - Inicio y cierre de la aplicación.
  - Cada operación CRUD exitosa, con el ID de la entidad.
  - Cada error capturado, con excepción y stack trace.
  - Cambios de órdenes, con el `OrderID`.
- Cada servicio debe usar `ILogger<T>` o `logger.ForContext<T>()`.
- El sink de consola en un WinExe puede no verse al ejecutar desde Windows; si se va a demostrar, agregar una consola de depuración o usar además un sink de depuración.

### 3.4 Órdenes: criterios de aceptación

- **Listado:** ID, cliente, fecha, empleado, total calculado y cantidad de líneas.
- **Filtros:** rango de fechas con `DateTimePicker`, cliente, empleado y búsqueda en tiempo real.
- **Cabecera:** cliente, empleado, fecha de orden, fecha requerida, fecha de envío, transportista, flete y dirección de envío.
- **Detalle:** producto, cantidad, precio unitario y descuento.
- **Precio sugerido:** al seleccionar un producto, cargar su precio unitario actual; permitir editarlo.
- **Total:** recalcular automáticamente al cambiar cantidad, precio o descuento.
- **Guardado:** transacción con `IDbContextTransaction`; si falla cualquier detalle, `Rollback` de toda la orden.
- **Eliminación:** transacción; eliminar detalles en cascada; confirmación obligatoria.
- **Validación:** al menos un detalle, sin productos duplicados, cantidad > 0, precio > 0, descuento 0–1.
- **Rollback demostrable:** definir un mecanismo controlado para el video y documentarlo.

### 3.5 Base de datos

- `instnwnd.sql` crea y elimina la base `Northwind`; no ejecutarlo en una base con datos que se quieran conservar.
- El script está en UTF-16. Para `sqlcmd`, guardarlo en UTF-8 o ejecutarlo desde SSMS.
- La tabla `Suppliers` **no tiene columna `Email`**. Si se exige la validación de email, agregar una columna `Email nvarchar(100) NULL` al script y mapearla; si no, documentar la excepción.
- `Orders` usa `CustomerID`, `EmployeeID` y `ShipVia`; por eso se necesitan `Cliente`, `Empleado` y `Transportista`.
- `Order Details` tiene clave compuesta `(OrderID, ProductID)`; el detalle no puede repetir producto en la misma orden.

## 4. Carpetas y archivos a crear

- `/Northwind.GestionCRUD` (raíz del repositorio)
  - `README.md` (fuera de este reparto)
  - `.gitignore`
  - `/database`
    - `InstalarNorthwind.sql`
  - `/docs` (fuera de este reparto)
    - `DiagramaClases.pdf`
    - `DiagramaER.pdf`
  - `/build`
    - `Northwind.Presentacion.exe`
    - `appsettings.json`
  - `/logs`
    - `app-ejemplo.log`
  - `/src`
    - `Northwind.sln`
    - `/Northwind.Entidades`
    - `/Northwind.AccesoDatos`
    - `/Northwind.LogicaNegocio`
    - `/Northwind.Comun`
    - `/Northwind.Presentacion`

## 5. Contenido de `.gitignore`

```gitignore
## Archivos de compilacion
bin/
obj/
out/

## Configuracion local de IDEs
.vs/
*.user
*.suo
*.userosscache
*.sln.docstates
.idea/
*.DotSettings.user

## Logs generados dinamicamente
logs/*.log
!logs/app-ejemplo.log

## Archivos del sistema operativo
Thumbs.db
ehthumbs.db
Desktop.ini
.DS_Store
```

## 6. Git

### Flujo

- Rama base: `main`. Sin commits directos.
- Cada tarea en su rama.
- Antes de empezar: `git checkout main && git pull origin main`.
- Al terminar: probar, subir la rama y hacer merge a `main`.

### Ramas de Frankelly

- `feature/setup-arquitectura`
- `feature/modulo-categorias`
- `feature/modulo-productos`
- `feature/reportes-analiticos`

### Ramas de Ángel

- `feature/acceso-datos-uow`
- `feature/modulo-suplidores`
- `feature/modulo-ordenes`
- `feature/database-build`

### Rama conjunta

- `feature/integracion-final`

### Commits sugeridos

#### Frankelly

- `feat(setup): crear solucion y estructura de 5 capas`
- `feat(comun): configurar serilog y lectura de appsettings con IOptions`
- `feat(categorias): implementar DTO, CategoriaValidator y servicio de categorias`
- `feat(categorias): crear formulario de categorias con busqueda en tiempo real`
- `feat(productos): implementar ProductoValidator, servicio y filtros combinables`
- `feat(productos): crear formulario de productos con nombres de categoria y suplidor`
- `feat(reportes): agregar consultas LINQ y exportador a Excel y CSV`
- `fix(calidad): corregir reglas de validacion y eliminar SQL crudo`

#### Ángel

- `feat(datos): crear entidades EF completas y mapeos de Northwind`
- `feat(datos): implementar IRepositorio generico y UnitOfWork transaccional`
- `feat(datos): agregar repositorios de clientes, empleados y transportistas`
- `feat(suplidores): implementar SuplidorDTO, SuplidorValidator y servicio`
- `feat(suplidores): crear formulario de suplidores con ErrorProvider`
- `feat(ordenes): implementar DTOs y validadores de orden y detalle`
- `feat(ordenes): implementar servicio transaccional con Commit y Rollback`
- `feat(ordenes): crear formulario master-detail con calculo de total en tiempo real`
- `chore(database): agregar script de instalacion de Northwind y empaquetado en build`
- `chore(logs): agregar log de ejemplo`

#### Ambos

- `feat(ui): enlazar menu de navegacion y configurar contenedor de dependencias`
- `test: verificar transaccion de ordenes y prueba de rollback forzado`
- `fix(integracion): corregir conflictos de DI y navegacion`

## 7. Checklist de requisitos

| Requisito | Descripción | Responsable |
| :--- | :--- | :--- |
| Arquitectura 5 capas | Entidades, AccesoDatos, LogicaNegocio, Comun, Presentacion | Frankelly |
| Sin referencia directa | Presentacion no referencia AccesoDatos | Frankelly |
| EF Core 6+ | DbContext, DbSet y mapeo Fluent API de todas las entidades | Ángel |
| Entidades de apoyo | Cliente, Empleado y Transportista mapeados | Ángel |
| Patrón Repositorio | `IRepositorio<T>` genérico implementado y usado | Ángel |
| Patrón Unit of Work | `IUnitOfWork` con SaveChanges, BeginTransaction, Commit y Rollback | Ángel |
| Uso real del UoW | Servicios usan UoW; repositorios no llaman SaveChanges | Ambos |
| Patrón DTO | DTOs entre Lógica y UI; entidades EF no expuestas | Ambos |
| Inyección de dependencias | Inyección por constructor en servicios y formularios | Ambos |
| FluentValidation | 5 validadores con las reglas exactas | Ambos (según módulo) |
| ErrorProvider en UI | Errores visuales junto al control | Ambos (según módulo) |
| CRUD Categorías | Listar, crear, editar, eliminar, nombre único y bloqueo con productos | Frankelly |
| CRUD Suplidores | Listar, crear, editar, eliminar, campos completos y bloqueo con productos | Ángel |
| CRUD Productos | Listar, crear, editar, eliminar, nombres de categoría/suplidor y filtros | Frankelly |
| Master-Detail Órdenes | Cabecera, detalle, precio sugerido y total automático | Ángel |
| Listado de Órdenes | ID, cliente, fecha, empleado, total y cantidad de líneas | Ángel |
| Filtros de Órdenes | Fechas, cliente y empleado | Ángel |
| Transacción Órdenes | Guardado y eliminación con `IDbContextTransaction` y Rollback | Ángel |
| Rollback demostrable | Mecanismo controlado para el video | Ambos |
| Búsqueda y filtros | Búsqueda en tiempo real en todos los listados | Ambos (según módulo) |
| 3 Consultas analíticas | Top 10 productos, ventas por categoría y valor de inventario | Frankelly |
| Exportación de reportes | Excel y CSV | Frankelly |
| Configuración JSON | `Logging`, `ItemsPorPagina`, `FormatoFecha`, `SimboloMoneda` con IOptions | Frankelly |
| Logging con Serilog | Consola + archivo diario, CRUD, errores y órdenes con ID | Frankelly |
| Sin SQL crudo | Solo LINQ sobre EF Core | Ambos |
| Calidad y convenciones | PascalCase, camelCase, XML docs, sin código muerto | Ambos |
| Estructura del repositorio | `src`, `database`, `build`, `logs` | Ambos |
| Script de Base de Datos | `database/InstalarNorthwind.sql` | Ángel |
| Ejecutable en build | Release con `appsettings.json` configurable | Ángel |
| Log de ejemplo | `logs/app-ejemplo.log` | Ángel |
| Commits de ambos | Ramas y commits significativos | Ambos |

## 8. Definition of Done por módulo

Un módulo se considera terminado cuando:

- Compila sin errores.
- CRUD completo funcionando.
- Validadores con las reglas exactas.
- `ErrorProvider` mostrando errores.
- Búsqueda y filtros funcionando.
- Logs de éxito y error.
- Sin SQL crudo.
- DTOs en la UI.
- XML docs en métodos públicos.
- Prueba manual documentada.
- Commit en la rama correspondiente.

## 9. Pruebas mínimas antes de entregar

- **Categorías:** crear, editar, nombre duplicado, eliminar con productos y eliminar sin productos.
- **Productos:** crear, editar, filtros combinables, precio negativo, categoría/suplidor inexistente y eliminar con pedidos.
- **Suplidores:** crear, editar, email inválido, eliminar con productos y eliminar sin productos.
- **Órdenes:** crear con dos detalles, verificar total, editar, eliminar, filtros por fecha/cliente/empleado y rollback forzado.
- **Reportes:** los 3 reportes y exportación a CSV y Excel.
- **Configuración:** cambiar `appsettings.json` y verificar que la aplicación lo lee.
- **Logging:** verificar consola y archivo `logs/app-YYYYMMDD.log`.
- **Arquitectura:** verificar que `Presentacion` no referencia `AccesoDatos`.
- **Git:** verificar ramas y commits de ambos integrantes.

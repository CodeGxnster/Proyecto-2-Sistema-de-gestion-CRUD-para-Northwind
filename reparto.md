# Reparto de Trabajo

## Asignacion de Trabajo

### Frankelly
- Configuracion base: Creacion de la solucion en src y definicion de los 5 proyectos.
- Northwind.Comun: Configuracion mediante appsettings.json con IOptions y logging estructurado con Serilog (consola y archivo diario).
- Modulo Categorias:
  - Category y CategoryDTO.
  - CategoriaValidator con FluentValidation.
  - Servicio de Categorias con reglas de negocio (nombre unico, no borrar con productos vinculados).
  - Formulario WinForms de Categorias con ErrorProvider y busqueda en tiempo real.
- Modulo Productos:
  - Product y ProductDTO.
  - ProductoValidator con FluentValidation.
  - Servicio de Productos con mapeo de nombres de categoria y suplidor.
  - Formulario WinForms de Productos con filtros combinables (categoria, suplidor, precio, estado).
- Reportes Analiticos:
  - 3 consultas LINQ: Top 10 productos mas vendidos, Ventas por categoria por rango de fechas, Suplidores con mayor valor de inventario.
  - Formulario WinForms de Reportes con DataGridView y exportacion a Excel o CSV.
- Documentacion grafica: Diagrama de clases UML y diagrama entidad-relacion en docs.

### Angel
- Northwind.AccesoDatos:
  - DbContext de Northwind con EF Core (DbSet para cada entidad y mapeo Fluent API).
  - Repositorio generico (IRepositorio<T> y Repositorio<T>).
  - Unit of Work (IUnitOfWork y UnitOfWork) con transacciones (BeginTransaction, Commit, Rollback).
- Modulo Suplidores:
  - Supplier y SupplierDTO.
  - SuplidorValidator con FluentValidation.
  - Servicio de Suplidores con validaciones e integridad referencial.
  - Formulario WinForms de Suplidores con ErrorProvider y busqueda en tiempo real.
- Modulo Ordenes con Detalle (Master-Detail):
  - Order, OrderDetail y sus DTOs.
  - OrdenValidator y DetalleOrdenValidator con FluentValidation.
  - Servicio transaccional con IDbContextTransaction (crear, editar, eliminar en cascada, calculo automatico del total, sugerencia de precio).
  - Formulario WinForms de Ordenes (cabecera, detalle, busqueda y filtro por fechas).
- Base de datos y entrega compilada:
  - Script SQL de Northwind y restauracion en database.
  - Compilacion en Release y publicacion de ejecutables en build.

### Ambos
- Integracion del Formulario Principal, navegacion e inyeccion de dependencias en Program.cs.
- Redaccion del README.md con instrucciones, capturas y detalle del reparto.
- Grabacion del video demostrativo de 8 a 12 minutos (ambos explicando y mostrando el rollback).
- Pruebas finales y preparacion para la defensa oral.

## Carpetas y Archivos a Crear

- `/Northwind.GestionCRUD` (Raiz del repositorio)
  - `README.md` (Documentacion principal y capturas)
  - `.gitignore` (Exclusiones de Git)
  - `/database`
    - `InstalarNorthwind.sql` (Script para restaurar o crear la base de datos)
  - `/docs`
    - `DiagramaClases.pdf` (Diagrama UML)
    - `DiagramaER.pdf` (Diagrama Entidad-Relacion)
  - `/build`
    - `Northwind.Presentacion.exe` (Binario compilado en modo Release)
    - `appsettings.json` (Archivo de configuracion para el ejecutable)
  - `/logs`
    - `app-ejemplo.log` (Archivo de log de muestra generado)
  - `/src`
    - `Northwind.sln` (Archivo de solucion)
    - `/Northwind.Entidades` (Biblioteca de clases: Entidades y DTOs)
    - `/Northwind.AccesoDatos` (Biblioteca de clases: DbContext, Repositorios, UnitOfWork)
    - `/Northwind.LogicaNegocio` (Biblioteca de clases: Servicios y Validadores)
    - `/Northwind.Comun` (Biblioteca de clases: Serilog, configuracion, helpers)
    - `/Northwind.Presentacion` (Aplicacion Windows Forms: Formularios, UserControls, Program.cs)

## Contenido de .gitignore

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

## Instrucciones de Git

### Flujo de ramas
- La rama base de integracion es `main`. No hacer commits directos a `main`.
- Cada integrante crea una rama especifica para su tarea a partir de `main`.
- Al finalizar y probar la tarea, se sube la rama y se hace merge a `main`.
- Antes de iniciar una nueva rama o mergear, ejecutar: `git checkout main && git pull origin main`.

### Ramas de Frankelly
- `feature/setup-arquitectura` (Creacion de solucion, proyectos y capa Comun)
- `feature/modulo-categorias` (Entidades, DTO, validador, servicio y UI de Categorias)
- `feature/modulo-productos` (Entidades, DTO, validador, servicio y UI de Productos con filtros)
- `feature/reportes-analiticos` (Consultas LINQ, exportacion CSV/Excel y UI de reportes)
- `feature/docs-diagramas` (Diagrama de clases y diagrama ER)

### Ramas de Angel
- `feature/acceso-datos-uow` (DbContext EF Core, Repositorio generico y UnitOfWork)
- `feature/modulo-suplidores` (Entidades, DTO, validador, servicio y UI de Suplidores)
- `feature/modulo-ordenes` (Entidades, DTOs, validadores, servicio transaccional y UI Master-Detail)
- `feature/database-build` (Scripts SQL en database y compilacion final en build)

### Rama conjunta
- `feature/integracion-final` (Menu principal, inyeccion de dependencias, README.md)

### Commits sugeridos

#### Frankelly
- `feat(setup): crear solucion y estructura de 5 capas`
- `feat(comun): configurar serilog y lectura de appsettings con IOptions`
- `feat(categorias): implementar DTO, CategoriaValidator y servicio de categorias`
- `feat(categorias): crear formulario de categorias con busqueda en tiempo real`
- `feat(productos): implementar ProductoValidator, servicio y filtros combinables`
- `feat(productos): crear formulario de productos con nombres de categoria y suplidor`
- `feat(reportes): agregar consultas LINQ y exportador a Excel y CSV`
- `docs: agregar diagramas de clase y entidad relacion en docs`

#### Angel
- `feat(datos): configurar DbContext Northwind y mapeos en EF Core`
- `feat(datos): implementar IRepositorio y UnitOfWork transaccional`
- `feat(suplidores): implementar SupplierDTO, SuplidorValidator y servicio`
- `feat(suplidores): crear formulario de suplidores con ErrorProvider`
- `feat(ordenes): implementar DTOs y validadores de orden y detalle`
- `feat(ordenes): implementar servicio transaccional con Commit y Rollback`
- `feat(ordenes): crear formulario master-detail con calculo de total en tiempo real`
- `chore(database): agregar script de instalacion de Northwind y empaquetado en build`

#### Ambos
- `feat(ui): enlazar menu de navegacion y configurar contenedor de dependencias`
- `docs: redactar README con capturas y pasos de instalacion`
- `test: verificar transaccion de ordenes y prueba de rollback forzado`

## Checklist de Requisitos

| Requisito | Descripcion | Responsable |
| :--- | :--- | :--- |
| Arquitectura 5 capas | Solucion separada en Entidades, AccesoDatos, LogicaNegocio, Comun, Presentacion | Frankelly |
| Sin referencia directa | Presentacion no referencia directamente a Acceso a Datos | Frankelly |
| EF Core 6+ | DbContext, DbSet y mapeo con Fluent API o atributos | Angel |
| Patron Repositorio | IRepositorio<T> generico implementado sobre DbContext | Angel |
| Patron Unit of Work | UnitOfWork con SaveChanges y control de transacciones (BeginTransaction, Commit, Rollback) | Angel |
| Patron DTO | Transferencia entre Logica y UI usando DTOs sin exponer entidades EF | Ambos |
| Inyeccion de dependencias | Inyeccion por constructor en servicios y formularios | Ambos |
| FluentValidation | Un validador por entidad/DTO en LogicaNegocio con reglas completas | Ambos (Segun modulo) |
| ErrorProvider en UI | Mostrar errores de validacion visualmente en controles correspondientes | Ambos (Segun modulo) |
| CRUD Categorias | Listar, crear, editar, eliminar, nombre unico y validacion de productos asociados | Frankelly |
| CRUD Suplidores | Listar, crear, editar, eliminar, validacion de campos y bloqueo de eliminacion con productos | Angel |
| CRUD Productos | Listar, crear, editar, eliminar, nombres de categoria/suplidor y filtros combinables | Frankelly |
| Master-Detail Ordenes | Cabecera y detalle, sugerencia de precio unitario y calculo de total en pantalla | Angel |
| Transaccion Ordenes | Guardado y eliminacion con IDbContextTransaction y Rollback comprobable | Angel |
| Busqueda y filtros | Busqueda en tiempo real en listados y DateTimePicker en ordenes | Ambos (Segun modulo) |
| 3 Consultas analiticas | Top 10 productos, ventas por categoria por fecha y valor de inventario con LINQ | Frankelly |
| Exportacion de reportes | Exportar resultados analiticos a Excel o CSV | Frankelly |
| Configuracion JSON | appsettings.json leido con Microsoft.Extensions.Configuration e IOptions | Frankelly |
| Logging con Serilog | Sinks de consola y archivo rolling diario (logs/app-YYYYMMDD.log) para inicio, CRUD y errores | Frankelly |
| Estructura del repositorio | Carpetas src, database, docs, build, logs y README.md | Ambos |
| Script de Base de Datos | Script de Northwind en carpeta database | Angel |
| Ejecutable en build | Compilacion Release con appsettings.json configurable en carpeta build | Angel |
| Diagramas UML y ER | Diagrama de clases y diagrama de entidad-relacion en carpeta docs | Frankelly |
| README completo | Instrucciones paso a paso, capturas de cada modulo y division del trabajo | Ambos |
| Video demostrativo | 8 a 12 minutos, ambos hablando, explicacion tecnica y demostracion de rollback | Ambos |
| Calidad y convenciones | PascalCase, camelCase, comentarios XML en metodos publicos, sin codigo muerto | Ambos |

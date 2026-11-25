# EduNation

EduNation es una plataforma de gestión educativa cuyo desarrollo comienza con el módulo de gestión de estudiantes. La versión actual permite registrar y administrar estudiantes mediante una API REST construida con ASP.NET Core.

## Funcionalidades implementadas

- Registrar estudiantes.
- Consultar un estudiante activo por ID y listar los estudiantes activos.
- Actualizar nombre, apellido, correo y fecha de nacimiento.
- Desactivar estudiantes; los estudiantes desactivados dejan de aparecer en las consultas de estudiantes activos.
- Validar en el dominio nombres, formato de correo y fecha de nacimiento.
- Prevenir correos duplicados mediante un índice único y responder con `409 Conflict`.
- Manejar excepciones de forma centralizada y devolver errores con `ProblemDetails`.
- Desacoplar las respuestas de consulta y actualización de la entidad de dominio mediante `StudentResponse`.
- Ejecutar pruebas unitarias y de integración; las pruebas de integración utilizan SQLite en memoria.

### Rutas disponibles

| Método | Ruta | Comportamiento |
|---|---|---|
| `POST` | `/api/students` | Registra un estudiante. |
| `GET` | `/api/students/{id}` | Consulta un estudiante activo por ID. |
| `GET` | `/api/students` | Lista estudiantes activos. |
| `GET` | `/api/students/search` | Busca estudiantes activos y devuelve resultados paginados. |
| `PUT` | `/api/students/{id}` | Actualiza la información del estudiante. |
| `DELETE` | `/api/students/{id}` | Desactiva al estudiante. |

La sensibilidad a mayúsculas y minúsculas de la búsqueda depende de la collation configurada en SQL Server.

## Arquitectura

La solución separa responsabilidades en estos proyectos:

| Proyecto | Responsabilidad |
|---|---|
| `EduNation.Domain` | Entidad `Student` y reglas de validación del dominio. |
| `EduNation.Application` | Casos de uso, abstracciones de persistencia y respuestas de aplicación. |
| `EduNation.Infrastructure` | Persistencia con Entity Framework Core, repositorio, configuración y migraciones. |
| `EduNation.API` | Endpoints HTTP, configuración de dependencias y manejo centralizado de excepciones. |
| `EduNation.Application.Tests` | Pruebas unitarias y de integración. |

## Tecnologías

- C# y .NET 10.
- ASP.NET Core Web API.
- Entity Framework Core 10.
- SQL Server para la persistencia de la API.
- SQLite en memoria para pruebas de integración.
- xUnit y Moq.
- Git y GitHub.

## Requisitos previos

- .NET 10 SDK.
- SQL Server accesible desde el entorno donde se ejecuta la API.
- Git.
- Herramienta `dotnet-ef` 10.0.14 para aplicar migraciones.

  Si no está instalada:

  ```bash
  dotnet tool install --global dotnet-ef --version 10.0.14
  ```

  Si ya está instalada en otra versión:

  ```bash
  dotnet tool update --global dotnet-ef --version 10.0.14
  ```

## Instalación y ejecución

1. Clona el repositorio en una carpeta llamada `EduNation`:

   ```bash
   git clone https://github.com/paulinoalam0810-sketch/Student-Management-API.git EduNation
   cd EduNation
   ```

2. Restaura los paquetes:

   ```bash
   dotnet restore EduNation.slnx
   ```

3. Configura la conexión de SQL Server descrita a continuación.

4. Aplica las migraciones existentes:

   ```bash
   dotnet ef database update --project EduNation.Infrastructure/EduNation.Infrastructure.csproj --startup-project EduNation.API/EduNation.API.csproj
   ```

5. Ejecuta la API:

   ```bash
   dotnet run --project EduNation.API/EduNation.API.csproj
   ```

## Conexión a SQL Server

La API lee la cadena de conexión `DefaultConnection` mediante la configuración de ASP.NET Core. Antes de ejecutar la migración o la API, reemplaza el valor de esa clave en `EduNation.API/appsettings.json` por una cadena válida para tu instancia de SQL Server. No guardes credenciales reales en archivos versionados.

La API configura SQL Server como proveedor de producción. SQLite se usa únicamente en el factory de pruebas de integración.

## Ejecución de pruebas

Desde la raíz del repositorio, ejecuta todas las pruebas de la solución:

```bash
dotnet test EduNation.slnx
```

Las pruebas de integración levantan la API mediante `CustomWebApplicationFactory` y usan una conexión SQLite en memoria independiente por factory.

## Autor

**Alam Paulino**

GitHub: [paulinoalam0810-sketch](https://github.com/paulinoalam0810-sketch)
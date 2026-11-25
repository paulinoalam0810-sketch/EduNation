# EduNation agent instructions

## Project and structure

EduNation is currently a student-management REST API. Treat only the features present in this repository as implemented; do not describe planned education modules as available.

- `EduNation.slnx`: solution entry point.
- `Students.Domain`: `Student` entity and domain validation rules.
- `Students.Application`: use cases, persistence abstractions, search criteria and response models; references Domain.
- `Students.Infraestructure`: EF Core `ApplicationDbContext`, entity configuration and repository implementations. The folder is spelled `Infraestructure`; the assembly and namespaces use `Infrastructure`.
- `Students.API`: ASP.NET Core controllers, dependency-injection composition root, exception handling and SQL Server setup.
- `Students.Application.Tests`: xUnit unit and integration tests; integration tests use SQLite in memory through `CustomWebApplicationFactory`.

Technologies verified in the projects: C#, .NET 10, ASP.NET Core Web API, EF Core 10, SQL Server for production persistence, SQLite for integration tests, xUnit and Moq.

## Code conventions

- Preserve the existing project, namespace, folder and endpoint names.
- Follow the existing block-scoped namespace and brace style; keep nullable reference types and implicit usings enabled.
- Name asynchronous methods with the `Async` suffix and pass `CancellationToken` through asynchronous boundaries.
- Keep HTTP request binding and HTTP-specific validation in API. Keep use-case criteria, orchestration and response mapping in Application.
- Keep `StudentResponse` as the student response DTO. Avoid adding mapping packages or other dependencies without approval.
- Treat the user's request as authorization for the requested behavior. Use judgment for routine implementation details; ask only when a material product decision is unclear, a significant risk emerges, or the work would expand beyond the request. Keep changes focused and do not refactor unrelated code.

## Clean Architecture rules

- Domain must not depend on Application, Infrastructure, API, EF Core or ASP.NET Core.
- Application may depend on Domain and its own abstractions. It must not depend on Infrastructure, EF Core or ASP.NET Core.
- Infrastructure implements Application persistence abstractions and owns EF Core queries, mappings and provider-specific code. Do not expose `IQueryable`, `DbContext`, or EF Core types outside Infrastructure.
- API is the composition root and HTTP boundary. It may depend on Application and Infrastructure; controllers delegate business work to use cases.
- Keep production SQL Server configuration separate from the SQLite test factory. Do not change database schema or add migrations without explicit scope.

## Required implementation and review flow

1. Inspect the solution, relevant files, current Git status, and existing tests before planning changes. Preserve all pre-existing user changes.
2. Have Architect assess cross-layer impact and propose the smallest design when a task spans layers or changes a contract. Resolve material, blocking product ambiguity before dependent implementation; do not pause for routine design choices.
3. Have Developer implement only the approved scope and add/update tests for behavior changed by that scope.
4. Have QA run the applicable test suite and build; use the sequential commands below for full validation in this solution. QA identifies and documents failures with reproduction evidence and does not change production code.
5. The main agent analyzes QA's results and assigns authorized corrections to Developer. Developer implements those corrections, then QA reruns the affected tests and any other required validation.
6. Have Code Reviewer perform one independent, read-only review of the full diff, including new untracked files. The main agent triages findings; fix confirmed defects and material risks, while recording non-blocking suggestions without starting another broad review cycle. After a fix, request one targeted recheck of the changed area when needed.
7. Run `git diff --check` for unstaged changes and `git diff --cached --check` for staged changes, inspect `git status --short`, and report files changed, validation results, and remaining risks. Do not claim completion while required validation fails.

For independent read-only investigations, the coordinator may delegate in parallel. Do not run concurrent edits against shared files. A single Developer owns implementation; QA and Reviewer work after the implementation is available.

## Build and test commands

Run from the repository root. Sequential MSBuild completed successfully for this solution; use these commands for full validation, especially when parallel project-reference resolution fails. The build command restores NuGet packages automatically:

```powershell
dotnet build EduNation.slnx -m:1 /p:BuildInParallel=false
dotnet test EduNation.slnx -m:1 /p:BuildInParallel=false
git diff --check
git diff --cached --check
```

The existing EF migration command is:

```powershell
dotnet ef database update --project Students.Infraestructure/Students.Infrastructure.csproj --startup-project Students.API/Students.API.csproj
```

Do not run migrations against a real or shared database unless the user specifically authorizes it.

## Security

- Never commit or print passwords, connection strings with credentials, API keys, tokens, certificates, or local secrets. Do not add real credentials to tracked configuration.
- Use EF Core parameterized LINQ queries; do not build SQL from user input.
- Validate external input at the API boundary and preserve application-level invariants.
- Do not expose SQL exceptions, provider details, connection strings, or sensitive student data in API errors or logs.
- Do not add authentication, authorization, encryption, or other security behavior unless it exists in the requested scope and is approved.

## Git restrictions

- Read-only commands such as `git status`, `git diff`, `git diff --cached`, `git log`, `git diff --check`, and `git diff --cached --check` are allowed for inspection.
- Never run `git add`, `git commit`, `git push`, `git pull`, `git merge`, `git rebase`, `git reset`, `git clean`, `git restore`, or checkout/discard operations unless the user explicitly authorizes that specific operation.
- Never force-push or discard user changes. Do not stage unrelated files. Do not infer permission to commit or synchronize from a request to implement or review code.

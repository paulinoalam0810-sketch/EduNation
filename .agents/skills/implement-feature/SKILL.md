---
name: implement-feature
description: Implement an approved EduNation feature or bug fix across the correct Clean Architecture layers, with focused tests and validation. Use when the user asks to add or change application behavior.
---

# Implement an EduNation feature

1. Read the root `AGENTS.md`, inspect the solution and relevant code, and run `git status --short`. Preserve all existing changes.
2. Confirm the requested behavior, compatibility requirements, affected projects, and tests. Ask for clarification only when a required product decision cannot be inferred safely.
3. Keep the design within current project boundaries: Domain rules in Domain; orchestration and contracts in Application; EF Core and SQL in Infrastructure; HTTP binding and status codes in API.
4. Make the smallest approved changes. Do not add dependencies, migrations, schema changes, or unrelated refactors unless explicitly authorized.
5. Add or update tests for the changed behavior and relevant edge cases.
6. Run the requested commands. For full validation in this solution, use the sequential commands `dotnet build EduNation.slnx -m:1 /p:BuildInParallel=false` and `dotnet test EduNation.slnx -m:1 /p:BuildInParallel=false`; these completed successfully here after parallel project-reference resolution failed. Run `git diff --check` for unstaged changes and `git diff --cached --check` for staged changes when authorized by the task.
7. Inspect `git diff` and `git ls-files --others --exclude-standard`; report files, behavior, validation results, and limitations.

Do not stage, commit, push, pull, merge, rebase, reset, clean, restore, or discard changes unless the user explicitly authorizes that specific operation.

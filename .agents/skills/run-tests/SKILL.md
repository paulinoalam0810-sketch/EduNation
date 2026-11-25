---
name: run-tests
description: Run EduNation builds and unit/integration tests, diagnose failures, and report reliable validation results. Use when the user asks to test, verify, or fix test failures.
---

# Run EduNation tests

1. Read root `AGENTS.md`, check the requested scope and current Git status, and preserve existing changes.
2. Use the solution and project paths that exist in the repository. For full validation in this solution, run `dotnet build EduNation.slnx -m:1 /p:BuildInParallel=false` and `dotnet test EduNation.slnx -m:1 /p:BuildInParallel=false`; this sequential mode completed successfully here after parallel project-reference resolution failed. Use a narrower test target only when requested or when diagnosing a failure.
3. Capture exit codes, test counts, and failure output. If a test fails, identify the first actionable cause and reproduce it before proposing a fix.
4. Change tests or application code only when the user or coordinator authorizes that work. Never weaken assertions just to obtain a pass.
5. After authorized fixes, rerun the relevant failing tests and the requested full suite. Run `git diff --check` to validate unstaged changes and `git diff --cached --check` to validate staged changes; report both results when applicable.
6. Do not perform Git write operations.

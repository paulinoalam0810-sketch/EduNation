---
name: review-code
description: Perform a read-only EduNation code audit of a diff or implementation for correctness, security, architecture, compatibility, and test coverage. Use when the user requests a review or final audit.
---

# Review EduNation code

1. Read root `AGENTS.md` and inspect `git status --short`, `git diff`, and untracked files. Never assume untracked files are irrelevant.
2. Trace behavior across API, Application, Domain, Infrastructure, and tests as applicable. Verify dependency direction and existing endpoint contracts.
3. Check error handling, validation, security, database-provider behavior, cancellation, isolation, and edge-case coverage.
4. Do not edit files or perform Git write operations.
5. Report actionable findings first, grouped by Critical, High, Medium, and Low. Cite file and line. Separate confirmed defects from risks and test gaps. If none, say so.
6. Include scope assessment and relevant validation results; do not claim tests passed unless their output is available.

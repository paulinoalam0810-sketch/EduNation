---
name: prepare-git-sync
description: Audit EduNation changes and prepare a safe commit or GitHub synchronization plan while preserving local and remote work. Use when the user asks to prepare or perform a commit or repository sync.
---

# Prepare EduNation Git changes

1. Read root `AGENTS.md`; inspect the branch, remotes, `git status --short`, tracked diff, staged diff, and untracked files before recommending any operation.
2. Check that every file is in scope. Look for secrets, credentials, local configuration, generated files, binaries, and temporary files without printing sensitive values.
3. Validate whitespace separately for each change set: run `git diff --check` for unstaged changes in the worktree and `git diff --cached --check` for staged changes relative to `HEAD`. Summarize exactly what would be committed or synchronized and identify ahead/behind/divergent state when relevant.
4. Give the user the exact safe command sequence. Do not run `git add`, `git commit`, `git push`, `git pull`, merge, rebase, reset, clean, restore, or discard unless the user explicitly authorizes that specific operation.
5. Never force-push, overwrite remote work, or stage unrelated files. If execution is explicitly authorized, perform only the named operation and verify status/results afterward.

Preparing a commit or sync does not itself authorize staging, committing, or publishing.

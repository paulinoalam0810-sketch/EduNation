# EduNation agent coordination

## Ownership

- **Architect**: inspects the repository and returns a design, dependency impact, contract risks, and test requirements. Read-only by default.
- **Developer**: owns the approved implementation and coordinates any required test edits with QA.
- **QA**: validates the implemented behavior and reports reproducible failures. It changes tests only when assigned; it does not edit application logic.
- **Code Reviewer**: independently inspects the complete diff, including untracked files. Read-only; reports findings by severity.
- **Main agent**: owns the user conversation, scope and approvals; dispatches work, resolves conflicting advice, waits for results, and provides the consolidated report.

## Workflow

1. Main agent checks `git status --short` and identifies the user-approved scope.
2. For cross-layer or contract changes, delegate a bounded, read-only design task to Architect. Keep small, local changes in the main agent.
3. The user's request defines the authorized behavior. Main agent resolves only material product ambiguity, significant risk, or work beyond that scope; routine implementation choices do not need another approval. Main then gives Developer the behavior, scope, and constraints.
4. Developer implements and reports its file list and checks. Do not assign simultaneous write tasks to agents sharing this working tree.
5. QA validates after implementation. Independent read-only review can run alongside test execution, but Reviewer must inspect the same final diff after all edits stop.
6. Main agent triages findings and assigns confirmed defects or material risks for scoped Developer fixes. QA reruns relevant checks. Reviewer performs one targeted recheck of the changed area when a fix affects a finding; do not repeat the full audit unless the changes materially alter the reviewed scope. Record non-blocking suggestions without creating a fix-and-review loop.
7. Main agent checks tracked and untracked changes, reports test/build results and unresolved risks, and performs Git writes only when explicitly authorized by the user.

## Delegation boundaries

Parallelize only independent tasks, such as separate read-only architecture and security reviews. Keep dependent work sequential: design before implementation, implementation before final QA/review, and fixes before final validation. Give each agent a precise question, owned files or read-only boundary, expected output, and whether it should wait for other agents.

Agents share the project workspace and inherit the parent session's effective permissions. Read-only wording in an agent file is behavioral guidance; it does not restrict tools, create a separate sandbox, or prevent writes. For a strictly read-only audit, the coordinator must run the parent session with effective read-only permissions or use an isolated workspace where writes are disabled, then verify no files changed. Do not describe a prompt-only restriction as enforced read-only access. The main agent must prevent file overlap, preserve the pre-existing worktree, collect every result, and synthesize one user-facing conclusion.

## Codex Desktop usage

Open this trusted repository in Codex Desktop and start a project chat. Ask the main agent to delegate to a named project agent, for example:

> Implement the approved student search change. Ask Architect for a read-only design first, then have Developer implement it, QA run the tests, and Code Reviewer inspect the final diff. Wait for each result and summarize them. Do not commit or push.

For a single specialist, ask naturally: “Use the Architect agent to review the dependency direction” or “Have Code Reviewer audit the current diff read-only.” Codex's subagent panel can be used to inspect active and completed agent threads.

Skills live in `.agents/skills/`. Ask for a skill by name, such as “Use the `implement-feature` skill,” or let Codex select it from its description when the task matches. Project agent files live in `.codex/agents/`; these are subagents for delegation, not separate top-level conversations.

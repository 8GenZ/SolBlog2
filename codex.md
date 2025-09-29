powershell -NoProfile -Command ^

"@'

\# Project Brief (Repo-wide)

Solutions:

\- apps/Blog/Blog.sln (Blazor United .NET 8/9, InteractiveServer)

\- apps/WasmHost/WasmHost.sln (Blazor WASM hosted)

\- services/Api/Api.sln (ASP.NET Core API)



\# Canon

\- .NET 8/9, EF Core, Identity

\- Dev DB: SQLite; Prod DB: Postgres

\- Tests: xUnit



\# Guardrails

\- Do not rename solutions or move startup files.

\- Respect Clean Architecture boundaries.

\- Separate commits per solution.

\- Migrations must include reversible plan.



\# Commands (per solution)

\[Blog.United]

build: dotnet build apps/Blog/Blog.sln

run:   dotnet watch --project apps/Blog/src/Blog.Web

test:  dotnet test apps/Blog/Blog.sln



\[Wasm.Hosted]

build: dotnet build apps/WasmHost/WasmHost.sln

run:   dotnet run --project apps/WasmHost/src/Server

test:  dotnet test apps/WasmHost/WasmHost.sln



\[Api]

build: dotnet build services/Api/Api.sln

run:   dotnet watch --project services/Api/src/Api.Web

test:  dotnet test services/Api/Api.sln



\# Definition of Done

\- Build + tests green for the touched solution

\- Lint/format applied

\- Concise, scoped commit

'@ | Set-Content -Encoding UTF8 codex.md"




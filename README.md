# Workflow MVP (ASP.NET + React + PostgreSQL + Keycloak + Camunda + OpenClaw)

## Planned folder structure

```text
.
├── backend/
│   ├── Dockerfile
│   └── src/WorkflowMvp.Api/
├── frontend/
│   ├── Dockerfile
│   └── src/
├── camunda/
│   └── bpmn/request-approval.bpmn
├── keycloak/
│   └── realm-workflow-mvp.json
├── openclaw/
│   ├── Dockerfile
│   └── app.py
├── docker-compose.yml
├── .env.example
└── README.md
```

## MVP scope

This project demonstrates one business process: **Request for approval**.

1. User creates request in React frontend.
2. ASP.NET Core API stores request in PostgreSQL.
3. API starts Camunda process (`request_approval_process`).
4. Process stages are represented in API domain status:
   - `PendingManagerApproval`
   - `PendingFinanceApproval`
   - `Approved` / `Rejected`
5. Frontend shows list, details, statuses, comments, and audit logs.
6. OpenClaw is a sidecar assistant only (summary and draft comments).

## Services in docker-compose

- `postgres` (business data)
- `keycloak` (auth / roles)
- `zeebe` (Camunda 8 orchestration engine)
- `openclaw` (assistant sidecar stub)
- `backend` (.NET 8 API)
- `frontend` (React + TypeScript Vite)

## WSL prerequisites

- WSL2 on Windows
- Docker Desktop with WSL integration enabled
- Docker Compose v2 (`docker compose version`)
- Optional for local dev outside containers:
  - .NET 8 SDK
  - Node 20+

## Local startup (WSL)

```bash
cp .env.example .env
docker compose up --build -d
```

### Deploy BPMN process to Zeebe

After the stack is up, deploy BPMN from WSL:

```bash
docker cp camunda/bpmn/request-approval.bpmn mvp-zeebe:/tmp/request-approval.bpmn
docker exec -it mvp-zeebe /usr/local/zeebe/bin/zbctl --insecure deploy /tmp/request-approval.bpmn
```

## Run backend/frontend without docker (optional)

### Backend

```bash
cd backend/src/WorkflowMvp.Api
dotnet run
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## API endpoints

Base URL: `http://localhost:8088`

- `POST /api/requests`
- `GET /api/requests`
- `GET /api/requests/{id}`
- `POST /api/requests/{id}/approve`
- `POST /api/requests/{id}/reject`
- `GET /api/requests/{id}/summary`

Assistant-safe endpoints (no direct workflow/db mutation):

- `GET /api/assistant/requests/{id}/status`
- `GET /api/assistant/requests/{id}/summary`
- `POST /api/assistant/requests/{id}/draft-comment`

## Keycloak bootstrap

Realm import is auto-wired via `keycloak/realm-workflow-mvp.json`.

- Realm: `workflow-mvp`
- Client: `workflow-api`
- Roles: `user`, `manager`, `finance`, `admin`

### Demo users

| Username | Password | Role |
|---|---|---|
| `alice.user` | `Passw0rd!` | user |
| `marta.manager` | `Passw0rd!` | manager |
| `frank.finance` | `Passw0rd!` | finance |
| `adam.admin` | `Passw0rd!` | admin |

## OpenClaw sidecar policy

OpenClaw is intentionally limited:

- can read status
- can generate summary
- can draft comments
- **cannot** write to Camunda internals
- **cannot** administer Keycloak
- **cannot** directly update business tables

## Known limitations (intentional for MVP)

- Simple role extraction from token claims.
- No production SSO frontend flow yet (login page is placeholder).
- Camunda user tasks are modeled in BPMN, while action endpoints simulate role decisions in API.
- No advanced retries, outbox, or distributed transactions.
- No Kafka/Kubernetes/observability stack yet.

## Final run commands (in order)

```bash
cp .env.example .env
docker compose up --build -d
docker compose logs -f backend frontend keycloak zeebe openclaw
docker cp camunda/bpmn/request-approval.bpmn mvp-zeebe:/tmp/request-approval.bpmn
docker exec -it mvp-zeebe /usr/local/zeebe/bin/zbctl --insecure deploy /tmp/request-approval.bpmn
```

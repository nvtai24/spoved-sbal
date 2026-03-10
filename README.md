# Deploy a Full-Stack App — Hands-On Roadmap

> Learn to ship a real app (Frontend + Backend + Database) from zero to a live VPS.
> Each phase produces something working. No fluff.

---

## The Stack We're Deploying

| Layer | Tech |
|---|---|
| Frontend | React / Next.js (or any SPA) |
| Backend | Your choice (Node.js, Go, etc.) |
| Database | PostgreSQL |
| Cache | Redis (optional) |
| Reverse Proxy | Nginx |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Server | VPS (Ubuntu) — DigitalOcean / Hetzner / Vultr |

---

## Phase 1 — Dockerize the App
> Goal: every service runs in a container, works the same on any machine.

### 1.1 — Dockerize the Backend
- Write a `Dockerfile` for the backend
- Use multi-stage build: build stage → runtime stage (smaller image)
- Run as a non-root user
- Use `.dockerignore` to exclude `node_modules`, `.env`, etc.

```
labs/01-docker/backend/Dockerfile
```

**Checkpoint:** `docker build` + `docker run` → API responds on `localhost:8080`

---

### 1.2 — Dockerize the Frontend
- Write a `Dockerfile` for the frontend
- Multi-stage: install → build → serve with Nginx (or `serve`)
- Pass `VITE_API_URL` (or `NEXT_PUBLIC_*`) as build args

```
labs/01-docker/frontend/Dockerfile
```

**Checkpoint:** `docker build` + `docker run` → UI loads on `localhost:3000`

---

### 1.3 — Docker Compose (Full Local Stack)
- Wire all services together: `frontend`, `backend`, `postgres`, `redis`
- Use a custom bridge network so services reach each other by name
- Mount a named volume for PostgreSQL data persistence
- Use `depends_on` + health checks so the backend waits for the DB

```
labs/01-docker/docker-compose.yml
labs/01-docker/.env.example
```

**Checkpoint:** `docker compose up` → full app works locally, DB data survives restarts

---

### 1.4 — Environment & Secrets
- Never hardcode secrets in `Dockerfile` or `docker-compose.yml`
- Use `.env` files locally (gitignored), understand how they map into containers
- Understand the difference: build-time args (`ARG`) vs runtime env (`ENV`)

---

## Phase 2 — Nginx as Reverse Proxy
> Goal: one entry point that routes to the right service.

### 2.1 — Nginx Basics
- Understand: Nginx sits in front, the public only talks to Nginx
- Write a basic `nginx.conf` to proxy `/api/*` → backend, `/` → frontend
- Run Nginx as a container inside Docker Compose

```
labs/02-nginx/nginx.conf
labs/02-nginx/docker-compose.yml   ← updated to include nginx
```

**Checkpoint:** everything accessible on port `80`, no more direct container ports exposed

---

### 2.2 — SSL / HTTPS with Let's Encrypt
- Get a domain name (cheap: Namecheap, or free subdomain)
- Use `certbot` (standalone or webroot) to issue a free TLS cert
- Configure Nginx: redirect HTTP → HTTPS, serve on port `443`
- Set up auto-renewal with a cron job

```
labs/02-nginx/nginx-ssl.conf
```

**Checkpoint:** `https://yourdomain.com` loads with a valid cert, HTTP redirects to HTTPS

---

### 2.3 — Nginx Hardening
- Add security headers: `X-Frame-Options`, `X-Content-Type-Options`, `HSTS`
- Enable gzip compression
- Configure rate limiting on `/api/*`
- Serve frontend static files directly from Nginx (skip the frontend container)

---

## Phase 3 — VPS Setup
> Goal: a production-ready Ubuntu server, locked down and ready to receive deployments.

### 3.1 — Provision the VPS
- Create a VPS (Ubuntu 22.04 LTS) on DigitalOcean / Hetzner / Vultr
- Generate an SSH key pair, add the public key during provisioning
- Disable password login — SSH key only

```
labs/03-vps/provision.sh   ← setup script to automate this
```

---

### 3.2 — Harden the Server
- Create a non-root deploy user with sudo
- Configure `ufw` firewall: allow only SSH (22), HTTP (80), HTTPS (443)
- Install `fail2ban` to block brute-force SSH attempts
- Keep the system updated: `unattended-upgrades`

```
labs/03-vps/harden.sh
```

**Checkpoint:** server is accessible only via SSH key, all unnecessary ports closed

---

### 3.3 — Install Docker on the VPS
- Install Docker Engine + Docker Compose plugin
- Add the deploy user to the `docker` group
- Test: `docker run hello-world` as the deploy user

---

### 3.4 — Manual First Deploy
- Copy your `docker-compose.yml` and `.env` to the server
- Pull images from the registry, run `docker compose up -d`
- Point your domain's DNS A record to the server IP
- Run `certbot` on the server, verify HTTPS works

**Checkpoint:** `https://yourdomain.com` — the real app, live on the internet

---

## Phase 4 — CI/CD with GitHub Actions
> Goal: push to `main` → tests pass → app auto-deploys to the VPS.

### 4.1 — Build & Push Docker Image
- On every push to `main`: build the Docker image, tag it with the Git SHA
- Push the image to GitHub Container Registry (GHCR) — it's free
- Store registry credentials in GitHub Secrets

```
.github/workflows/build.yml
```

---

### 4.2 — Deploy to VPS
- After image push: SSH into the VPS from the GitHub Actions runner
- Pull the new image, run `docker compose up -d --no-deps --pull always <service>`
- Use `appleboy/ssh-action` or raw SSH with a deploy key

```
.github/workflows/deploy.yml
```

**GitHub Secrets needed:**
```
VPS_HOST
VPS_USER
VPS_SSH_KEY
GHCR_TOKEN
```

---

### 4.3 — Add Tests as a Gate
- Run unit/integration tests in the pipeline before building the image
- If tests fail → skip build and deploy
- Structure: `test` job → `build` job → `deploy` job (each depends on previous)

```
.github/workflows/ci.yml   ← full pipeline
```

---

### 4.4 — Database Migrations in the Pipeline
- Run `migrate up` (or your ORM's migration command) as a step before the new container starts
- Handle failure: if migration fails, stop the deploy — don't start the new container

---

### 4.5 — Zero-Downtime Deploys
- Use `docker compose` rolling restart or a health-check-based swap
- Configure Nginx upstream with a short `proxy_next_upstream` timeout
- Understand the limitation: true zero-downtime needs 2+ replicas (or Kubernetes)

---

### 4.6 — Rollback
- Keep the previous image tag in a file on the server (`.last-deploy`)
- Write a `rollback.yml` workflow: manually triggered, re-deploys the previous tag

```
.github/workflows/rollback.yml
```

---

## Final Architecture

```
Internet
    │
    ▼
[ Nginx :443 ] ── SSL termination, rate limiting, gzip
    │
    ├── /          → [ Frontend container ]
    ├── /api/*     → [ Backend container ]
    │                       │
    │                  [ PostgreSQL ]
    │                  [ Redis ]
    │
[ GitHub Actions ]
    │  push to main
    ▼
  build image → push to GHCR → SSH into VPS → docker compose pull & up
```

---

## Folder Structure

```
.
├── README.md
├── .github/
│   └── workflows/
│       ├── ci.yml
│       ├── deploy.yml
│       └── rollback.yml
└── labs/
    ├── 01-docker/
    │   ├── backend/Dockerfile
    │   ├── frontend/Dockerfile
    │   ├── docker-compose.yml
    │   └── README.md
    ├── 02-nginx/
    │   ├── nginx.conf
    │   ├── nginx-ssl.conf
    │   └── README.md
    ├── 03-vps/
    │   ├── provision.sh
    │   ├── harden.sh
    │   └── README.md
    └── 04-cicd/
        └── README.md
```

---

## Checkpoints Summary

| Phase | You've learned it when... |
|---|---|
| 1 — Docker | `docker compose up` runs the full stack locally, data persists |
| 2 — Nginx | Single port `443`, HTTPS works, `/api` and `/` route correctly |
| 3 — VPS | Server is live, hardened, domain resolves, app is manually deployed |
| 4 — CI/CD | Push to `main` → tests → build → deploy, no manual steps |

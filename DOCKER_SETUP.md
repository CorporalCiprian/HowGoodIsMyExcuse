# Docker Setup Guide

This project can be run entirely in Docker using Docker Compose with three services: PostgreSQL database, ASP.NET Core API, and Angular frontend.

## Prerequisites

- Docker and Docker Compose installed
- No need to install Node.js, .NET SDK, or PostgreSQL locally when using Docker

## Quick Start

### 1. Create environment file

```bash
cp .env.example .env
```

Then edit `.env` and fill in your secrets:

```env
DB_USER=postgres
DB_PASSWORD=your_very_secure_password_here
DB_NAME=howgoodismyexcuse

GROQ_API_KEY=gsk_your_groq_api_key_here
JWT_SECRET=your_jwt_secret_must_be_at_least_32_characters_long_and_secure
```

### 2. Build and start all services

```bash
docker-compose up -d
```

This will:
- Build the PostgreSQL database
- Build the ASP.NET Core API
- Build the Angular frontend with Nginx
- Start all services

### 3. Access the application

- **Frontend**: http://localhost:80 (or http://localhost)
- **API**: http://localhost:5000/api (direct access not typical in Docker - usually proxied)
- **Database**: localhost:5432 (PostgreSQL)

### 4. Initialize the database

The first time you run the backend, it will apply Entity Framework migrations automatically.

## Common Commands

### View logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f frontend
docker-compose logs -f db
```

### Stop services

```bash
# Stop all without removing
docker-compose stop

# Stop and remove containers (volumes persist)
docker-compose down

# Stop and remove containers AND volumes (clean slate)
docker-compose down -v
```

### Rebuild after code changes

```bash
# Frontend changes
docker-compose up -d --build frontend

# Backend changes
docker-compose up -d --build api

# All changes
docker-compose up -d --build
```

### Shell into a service

```bash
# API container
docker-compose exec api /bin/sh

# Frontend container
docker-compose exec frontend sh

# Database
docker-compose exec db psql -U postgres
```

## Architecture

```
┌─────────────────────────────────────────┐
│           Docker Network                │
├─────────────────────────────────────────┤
│                                         │
│  Frontend (Nginx)                       │
│  ├─ Port: 80                            │
│  ├─ Static Angular app                  │
│  └─ Reverse proxy for /api → Backend    │
│                                         │
│  API (.NET Core)                        │
│  ├─ Port: 8080                          │
│  ├─ REST endpoints                      │
│  └─ Connects to PostgreSQL              │
│                                         │
│  Database (PostgreSQL)                  │
│  ├─ Port: 5432                          │
│  └─ Persistent volume: pgdata           │
│                                         │
└─────────────────────────────────────────┘
```

### Service Communication

- **Frontend → API**: Via Nginx reverse proxy (`/api/` → `http://api:8080/api/`)
- **API → Database**: Via Docker DNS (`db:5432`)
- **External → Frontend**: Via port 80
- **External → API**: Through Nginx proxy on port 80

## Troubleshooting

### API can't connect to database

Make sure the PostgreSQL service is healthy:

```bash
docker-compose ps
# Check that 'db' service status is "Up"
```

If not, check logs:

```bash
docker-compose logs db
```

### Frontend shows 404 or can't reach API

1. Verify API service is running: `docker-compose ps api`
2. Check API logs: `docker-compose logs api`
3. Ensure nginx proxy is configured correctly (check `frontend/nginx.conf`)

### Database migration errors

The Entity Framework migrations run automatically on API startup. If there are issues:

```bash
# Rebuild from scratch
docker-compose down -v
docker-compose up -d
```

## Environment Variables

See `.env.example` for all configurable variables. The following are used in docker-compose:

- `DB_USER`: PostgreSQL username (default: postgres)
- `DB_PASSWORD`: PostgreSQL password (no default - **must set**)
- `DB_NAME`: Database name (default: howgoodismyexcuse)
- `GROQ_API_KEY`: API key for Groq LLM service (**must set**)
- `JWT_SECRET`: Secret for JWT token signing (**must set**, min 32 chars)

## Production Deployment

For production, consider:

1. Use a reverse proxy (Nginx, Traefik) in front of the Docker services
2. Use environment-specific compose files (`docker-compose.prod.yml`)
3. Configure HTTPS/SSL certificates
4. Set up proper logging and monitoring
5. Use Docker secrets management for sensitive data
6. Configure resource limits in compose file
7. Set up automated backups for PostgreSQL volume

## Development Without Docker

If you prefer local development:

### Backend
```bash
cd backend/HowGoodIsMyExcuse.Api
dotnet build
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm start
```

Then update API URLs:
- ExcuseService: `http://localhost:5000/api/excuses`
- AuthService: `http://localhost:5000/api/auth`

# HowGoodIsMyExcuse 🎭

A web app that uses AI to judge, roast, and score your excuses. Submit an excuse, pick a judge personality, and let it tell you exactly how bad it is.

---

## Tech Stack

| Layer    | Technology                         |
|----------|------------------------------------|
| Backend  | ASP.NET Core 8, C#                 |
| Database | PostgreSQL + Entity Framework Core |
| AI       | LLM API                            |
| Auth     | JWT Bearer + BCrypt                |
| Frontend | Angular (coming soon)              |

---

## Project Structure
```
HowGoodIsMyExcuse/
├── backend/
│   └── HowGoodIsMyExcuse.Api/
│       ├── Controllers/       # HTTP layer
│       ├── Models/            # EF Core entities
│       ├── DTOs/              # Request/response shapes
│       ├── Services/          # Business logic + AI integration
│       ├── Data/              # DbContext + Migrations
│       ├── Middleware/        # Rate limiting
│       ├── Program.cs
│       └── Dockerfile
├── docker-compose.example.yml
└── README.md
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/) (optional)

---

### Running Locally

**1. Clone the repo**
```bash
git clone https://github.com/yourusername/HowGoodIsMyExcuse.git
cd HowGoodIsMyExcuse
```

**2. Set up secrets**

Create `backend/HowGoodIsMyExcuse.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=howgoodismyexcuse;Username=postgres;Password=yourpassword"
  },
  "Groq": {
    "ApiKey": "YOUR_KEY_HERE"
  },
  "Jwt": {
    "Secret": "your-secret-minimum-32-characters!!"
  },
  "AllowedOrigins": "http://localhost:4200"
}
```

**3. Run the API**
```bash
cd backend/HowGoodIsMyExcuse.Api
dotnet run
```

The API will be available at `http://localhost:5000`.  
Swagger UI will be available at `http://localhost:5000/swagger`.

Migrations are applied automatically on startup in development.

---

### Running with Docker

**1. Set up your compose file**
```bash
cp docker-compose.example.yml docker-compose.yml
```

Open `docker-compose.yml` and fill in:
- `YOUR_PASSWORD_HERE` — any Postgres password you want
- `YOUR_API_KEY_HERE` — your AI API key
- `YOUR_JWT_SECRET_MIN_32_CHARS_HERE` — any random string, minimum 32 characters

**2. Run everything**
```bash
docker compose up --build
```

The API will be at `http://localhost:8080`.  
Swagger will be at `http://localhost:8080/swagger`.

**3. Stop**
```bash
docker compose down
```

To also wipe the database volume:
```bash
docker compose down -v
```

---

## API Reference

### Auth

| Method | Endpoint           | Auth     | Description     |
|--------|--------------------|----------|-----------------|
| POST   | /api/auth/register | None     | Register a user |
| POST   | /api/auth/login    | None     | Login, get JWT  |

### Excuses

| Method | Endpoint               | Auth     | Description                   |
|--------|------------------------|----------|-------------------------------|
| POST   | /api/excuses           | Required | Submit and evaluate an excuse |
| GET    | /api/excuses           | Optional | Get leaderboard (paginated)   |
| GET    | /api/excuses/{id}      | Optional | Get a single excuse           |
| POST   | /api/excuses/{id}/vote | Required | Upvote an excuse              |

### Judge Personalities

When submitting an excuse, the `judgePersonality` field must be one of:

- `Strict Corporate Karen`
- `Disappointed Dad`
- `Shakespearean Scholar`

---

### Testing with Swagger

1. Register via `POST /api/auth/register`
2. Copy the `token` from the response
3. Click **Authorize 🔒** in the top right
4. Enter your token
5. All authenticated endpoints are now unlocked

---

## Environment Variables

| Variable                               | Description                       |
|----------------------------------------|-----------------------------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string      |
| `Groq__ApiKey`                         | Your AI API key                   |
| `Jwt__Secret`                          | JWT signing secret (min 32 chars) |
| `AllowedOrigins`                       | CORS allowed origin               |

---

## Rate Limiting

The `POST /api/excuses` endpoint is rate limited to **10 requests per user per hour**. Exceeding the limit returns `429 Too Many Requests` with a `Retry-After` header.

---

## Security Notes

- Use `docker-compose.example.yml` as a template and fill in real values locally
# DocSearch

[![CI](https://github.com/amlsantos/DocSearch/actions/workflows/ci.yml/badge.svg)](https://github.com/amlsantos/DocSearch/actions/workflows/ci.yml)
[![Docker Pulls](https://img.shields.io/docker/pulls/rewind782/docsearch.svg)](https://hub.docker.com/r/rewind782/docsearch)
[![Docker Image Size](https://img.shields.io/docker/image-size/rewind782/docsearch/latest)](https://hub.docker.com/r/rewind782/docsearch)
DocSearch is a .NET Web API that enables **semantic document search** over a collection of Markdown files. It ingests `.md` files from a local folder into a PostgreSQL database (using full-text search vectors), and exposes endpoints to **retrieve** relevant chunks and **ask questions** powered by the Gemini AI model (RAG — Retrieval-Augmented Generation).

---

## Architecture Overview

```
docs/             ← Drop your .md files here
src/
  DocSearch.WebApi/
    Application/  ← CQRS Commands & Queries (MediatR)
    Domain/       ← Entities (Document, DocumentChunk)
    Infrastructure/ ← DB (PostgreSQL/EF Core) & Services
    Presentation/ ← REST Controllers (Swagger)
tests/
  DocSearch.Tests/ ← Unit tests
```

### Tech Stack

| Technology | Purpose |
|---|---|
| .NET 10 | Web API framework |
| PostgreSQL 16 | Database with native Full-Text Search |
| Entity Framework Core 10 | ORM & migrations |
| MediatR | CQRS pattern (Command/Query handlers) |
| Google Gemini 2.5 Flash | LLM for RAG answer generation |
| Docker & Docker Compose | Containerised deployment |
| pgAdmin 4 | Database GUI |
| Swagger / OpenAPI | API explorer |

---

## 🚀 Getting Started

### Prerequisites

- [Docker](https://www.docker.com/) & Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/) (only if running locally outside Docker)

---

### Step 1 — Place the `.env` File

The project requires a **`.env` file in the root of the repository** (next to `compose.yaml`). This file contains the database credentials and the Gemini API key.

> ⚠️ The `.env` file is included in `.gitignore` and is **not committed to the repository**. It will be provided to you separately.

Place the `.env` file at the root:

```
DocSearch/
├── .env              ← Place the provided .env file here
├── compose.yaml
├── docs/
├── src/
└── ...
```

The `.env` file contains the following variables:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=mysecretpassword
POSTGRES_DB=DocSearchDb
GEMINI_API_KEY=<your-gemini-api-key>
PGADMIN_DEFAULT_EMAIL=admin@admin.com
PGADMIN_DEFAULT_PASSWORD=admin

ConnectionStrings__DefaultConnection=Host=localhost;Port=5434;Database=DocSearchDb;Username=postgres;Password=mysecretpassword
Gemini__ApiKey=<your-gemini-api-key>
```

> **Note:** The `.env` file contains two sets of variables. The first set (`POSTGRES_USER`, `GEMINI_API_KEY`, etc.) is used by Docker Compose to configure the containers. The second set (`ConnectionStrings__DefaultConnection`, `Gemini__ApiKey`) is used by the .NET application when running locally via `dotnet run`.

---

### Step 2 — Start the Infrastructure (Docker Compose)

From the **root** of the repository, run:

```bash
docker compose up --build
```

This will build and start three services:

| Service | Description | Port |
|---|---|---|
| `docsearch.webapi` | The .NET REST API | `8080` |
| `db` | PostgreSQL 16 | `5434` (mapped from internal `5432`) |
| `pgadmin` | pgAdmin 4 (DB GUI) | `5050` |

> The API automatically applies EF Core migrations on startup, so the database schema is created for you.

Once you see output like `Now listening on: http://[::]:8080`, the application is ready.

---

### Running Locally (Alternative to Docker)

If you prefer to run the API outside of Docker (e.g. for debugging), you still need the database running. Start **only** the database and pgAdmin:

```bash
docker compose up db pgadmin --build
```

Then, from the **root of the repository**, run:

```bash
dotnet run --project src/DocSearch.WebApi
```

The application will automatically load the `.env` file from the project root (via the `DotNetEnv` library, which traverses parent directories to find it). The connection string in the `.env` points to `localhost:5434`, which is the PostgreSQL port exposed by Docker Compose.

---

## 🌐 Accessing the Services

### Swagger UI (API Explorer)

```
http://localhost:8080/swagger
```

Use Swagger to explore and call all available endpoints interactively.

### pgAdmin (Database UI)

```
http://localhost:5050
```

#### pgAdmin Login Credentials

| Field | Value |
|---|---|
| Email | `admin@admin.com` |
| Password | `admin` |

#### Connecting to the PostgreSQL Server in pgAdmin

After logging in, add a new server with the following settings:

| Field | Value |
|---|---|
| Host | `db` (if accessing from inside Docker) or `localhost` (if from your machine) |
| Port | `5432` (internal) or `5434` (external) |
| Database | `DocSearchDb` |
| Username | `postgres` |
| Password | `mysecretpassword` |

---

## 📋 API Endpoints

All endpoints are available under the base path `/api/documents`.

### 1. Ingest Documents

Scans the `/docs` folder and ingests all **new** `.md` files into the database. Files that have already been ingested (same filename + last modified date) are automatically skipped.

```
POST /api/documents/ingest
```

**Example cURL:**
```bash
curl -X POST http://localhost:8080/api/documents/ingest
```

**Response:**
```json
{
  "message": "Documents ingested successfully.",
  "newDocuments": [
    {
      "id": "...",
      "fileName": "languagewire.md",
      "sourcePath": "...",
      "lastModifiedUtc": "..."
    }
  ]
}
```

> If no new files are found, `newDocuments` will be an empty array `[]`.

---

### 2. Retrieve Document Chunks (Full-Text Search)

Searches the database for document chunks that match the given query using PostgreSQL full-text search. Returns ranked results with relevance scores.

```
GET /api/documents/retrieve?question={your question}
```

**Example cURL:**
```bash
curl "http://localhost:8080/api/documents/retrieve?question=What+is+LanguageWire"
```

**Response:**
```json
[
  {
    "chunkId": "...",
    "documentId": "...",
    "content": "...",
    "orderIndex": 0,
    "rank": 0.123
  }
]
```

---

### 3. Ask a Question (RAG / AI Answer)

Retrieves the most relevant document chunks and passes them to the **Gemini AI model** to generate a grounded, natural language answer. The response includes both the AI-generated answer and the source chunks it was based on.

```
POST /api/documents/ask
Content-Type: application/json
```

**Request body:**
```json
{
  "question": "Where is LanguageWire headquartered?"
}
```

**Example cURL:**
```bash
curl -X POST http://localhost:8080/api/documents/ask \
  -H "Content-Type: application/json" \
  -d '{"question": "Where is LanguageWire headquartered?"}'
```

**Response:**
```json
{
  "answer": "LanguageWire is headquartered in Copenhagen, Denmark.",
  "sources": [
    {
      "chunkId": "...",
      "documentId": "...",
      "content": "...",
      "orderIndex": 0,
      "rank": 0.456
    }
  ]
}
```

---

## 📁 Adding Documents

Simply drop any `.md` file into the `docs/` folder at the root of the repository. The folder is mounted as a volume into the Docker container, so the API can read files immediately without a rebuild.

Then call `POST /api/documents/ingest` to index the new files.

---

## 🛑 Stopping the Stack

```bash
docker compose down
```

To also remove the persistent database volume (this will delete all ingested data):

```bash
docker compose down -v
```

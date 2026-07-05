# DocSearch

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
```

---

## 🚀 Getting Started

### Prerequisites

- [Docker](https://www.docker.com/) & Docker Compose

### 1. Start the stack

From the **root** of the repository, run:

```bash
docker compose up --build
```

This will build and start three services:
| Service | Description | Port |
|---|---|---|
| `docsearch.webapi` | The .NET REST API | `8080` |
| `db` | PostgreSQL 16 | `5434` |
| `pgadmin` | pgAdmin 4 (DB GUI) | `5050` |

> The API automatically applies EF Core migrations on startup, so the database schema is created for you.

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

#### Connecting to the PostgreSQL server in pgAdmin

After logging in, add a new server with the following settings:

| Field | Value |
|---|---|
| Host | `db` |
| Port | `5432` |
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

Searches the database for document chunks that match the given query using PostgreSQL full-text search.

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

Retrieves the most relevant document chunks and passes them to the **Gemini AI model** to generate a grounded, natural language answer.

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

## 🗄️ Database Connection String

Used internally by the API (defined in `compose.yaml`):

```
Host=db;Port=5432;Database=DocSearchDb;Username=postgres;Password=mysecretpassword
```

For connecting from your local machine (e.g. via a SQL client or pgAdmin with `localhost`):

```
Host=localhost;Port=5434;Database=DocSearchDb;Username=postgres;Password=mysecretpassword
```

---

## 🛑 Stopping the Stack

```bash
docker compose down
```

To also remove the persistent database volume:

```bash
docker compose down -v
```

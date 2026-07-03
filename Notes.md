# Tech Notes & Architecture Decisions

This document describes the development journey of the Document Ingestion feature — its domain model, the challenges we encountered, how we solved them, and the architectural decisions we made along the way.

---

## Step 1 — Domain Modelling

The first step was defining the domain entities that represent the core concepts of the system.

### `Document`
Represents a single Markdown file on disk. Key properties:
- `Id` — a `Guid` generated on creation, used as the primary key.
- `FileName` — the name of the file (e.g. `01-getting-started.md`).
- `SourcePath` — the full path to the file on disk, used to locate it.
- `LastModifiedUtc` — the UTC timestamp of the file's last modification on disk (see migration history below).
- `Chunks` — a private collection of `DocumentChunk`, only accessible as a read-only collection. The only way to add a chunk is via the controlled `AddChunk(string content)` method, enforcing business rules.

### `DocumentChunk`
Represents a logical section of a document's text. Key properties:
- `Id` — its own `Guid`.
- `DocumentId` — foreign key linking back to the parent `Document`.
- `Content` — the text of the chunk, with a hard limit of **8000 characters** enforced at the domain level.
- `OrderIndex` — the position of this chunk within the parent document.

The relationship is a **one-to-many cascade**: deleting a `Document` automatically deletes all its `DocumentChunks`.

### Migration History

**`InitialCreate`** — Created the `Documents` and `DocumentChunks` tables with the core schema: `Id`, `FileName`, `SourcePath` for `Document`; `Id`, `DocumentId`, `Content`, `OrderIndex` for `DocumentChunk`.

**`V2`** — Added the `LastModifiedUtc` column to the `Documents` table. This column was not part of the initial design — it became necessary as a result of the incremental ingestion strategy (detailed in Step 3). It replaced an earlier approach of using a SHA-256 `FileHash` to detect file changes.

---

## Step 2 — The First Ingestion Endpoint (and the Bug)

The initial implementation of the ingestion endpoint called `_ingestionService.GetAllDocuments()` and passed the result to `Ok()`. This caused an immediate runtime error:

> `System.InvalidOperationException: The type 'System.Threading.ExecutionContext&' ... is invalid for serialization`

**Root Cause:** The `await` keyword was missing. Without it, the `Task` object itself — rather than its resolved result — was passed to `Ok()`. The JSON serializer then tried to serialize the internal state of the `Task`, which contains non-serializable types like `ExecutionContext&`.

**Fix:** Added `await` to the call. A simple fix, but a good reminder that missing `await` causes confusing, non-obvious errors at the serialization boundary.

---

## Step 3 — The Incremental Ingestion Challenge

The naive approach was to read all documents from disk and persist them on every call. This creates two immediate problems:
1. **Duplicate data** — re-running ingestion would insert the same documents again and again.
2. **Unnecessary work** — with thousands of files, processing unchanged documents is wasteful.

### Strategy: Identity via `FileName + LastModifiedUtc`

We evaluated two approaches to detect whether a file had changed:

| Approach | Pro | Con |
|---|---|---|
| SHA-256 Hash | Detects content changes exactly | Requires reading the entire file, expensive at scale |
| `LastModifiedUtc` | `O(1)` OS metadata call, very cheap | Relies on the OS timestamp being updated on save |

We chose `LastModifiedUtc`. It is the standard approach used by build systems, file watchers, and sync tools. The OS always updates this timestamp when a file is written, making it a reliable change indicator for our use case.

The ingestion logic became a clean, readable 5-step process:
1. Fetch existing documents from the DB and build a `HashSet<string>` of `{FileName}_{LastModifiedUtc}` keys.
2. List all `.md` files on disk and filter out any whose key is already in the `HashSet`.
3. If no new files remain, exit early — no work to do.
4. Pass the list of new file paths to the `IDocumentReader`, which reads each file and splits it into chunks.
5. Persist the resulting documents to the database.

---

## Step 4 — Scaling Challenge: Memory Under Load

The first working version of Step 4 collected all new documents into a `List<Document>` in memory before persisting them. With 6 files this works. With 100,000 files, the server would run out of RAM.

### Solution: Streaming (`IAsyncEnumerable`) + Batch Persistence

We kept the ingestion logic in the service unchanged and only changed the mechanics of how data flows through it:

**`MarkdownDocumentReader`** now implements `IAsyncEnumerable<Document>` using `yield return`. Instead of building an in-memory list, it produces one document at a time as it reads each file from disk.

**`IngestDocumentsCommandHandler`** consumes this stream with `await foreach` and accumulates documents into a temporary `batch` list. When the batch reaches **100 documents**, it is flushed to PostgreSQL and the list is cleared. The application's RAM usage is therefore bounded to approximately 100 documents at any given time — regardless of the total file count.

### Chunking Strategy

During ingestion, each Markdown document is split into `DocumentChunk` entities by splitting on double newlines (`\n\n`). This divides documents by paragraph or section, producing chunks of a natural, human-readable size. The domain enforces a hard limit of **8000 characters per chunk**, which aligns with the context window limits of most embedding models and search APIs used in RAG pipelines.

---

## Step 5 — Architecture: CQRS via MediatR

As the feature matured, having `DocumentIngestionService` injected directly into the controller felt like a coupling concern. We introduced **MediatR** to enforce the **Command Query Responsibility Segregation (CQRS)** pattern cleanly.

The structure is now:

| Layer | Responsibility |
|---|---|
| **Controller** | Receives HTTP request, dispatches Command/Query via `ISender`, maps Domain → DTO, returns HTTP response |
| **Command Handler** | Owns ingestion write logic, returns a list of newly ingested `Document` entities |
| **Query Handler** | Owns read logic, returns a list of existing `Document` entities |
| **DTO (`Presentation/DTOs/`)** | Defines the shape of the API response; never exposes domain internals |

Key principles upheld:
- **Controllers are thin.** `DocumentsController` has zero business logic. It only knows about `ISender`.
- **Domain stays internal.** `Document` entities are never serialized directly. The Controller is the sole responsibility boundary for mapping Domain → `DocumentDto`.
- **Handlers are independently testable.** Each handler has a single, focused responsibility and can be unit tested in isolation with mock repositories.

---

## Step 6 — Retrieval (Full-Text Search)

With documents ingested and chunked, the next challenge was implementing the search functionality: *"given a question, find and rank the most relevant stored chunks"*.

### Strategy: PostgreSQL Native Full-Text Search (FTS)

We evaluated several approaches:
1. **Plain `LIKE '%term%'` SQL Query**: Too slow (full table scans) and too rigid (lexical exact match only, no stemming).
2. **External Vector Database / Embeddings API**: Provides semantic matching, but adds significant architectural complexity, external dependencies, and API costs.
3. **PostgreSQL Full-Text Search (FTS)**: A pragmatical middle-ground. It provides stemming (e.g., matching "configure" when searching "configuration"), handles stopwords, and calculates mathematical relevance scores natively.

We chose **Option 3 (PostgreSQL FTS)** because it perfectly aligns with the project requirement to "start simple". It avoids external dependencies while delivering lightning-fast, ranked results.

### Implementation Details & DDD Purity

To implement FTS without leaking database concerns into our Domain layer, we used Entity Framework Core's **Shadow Properties**:

1. **Clean Domain**: `DocumentChunk.cs` remained pure. We did not add PostgreSQL-specific `tsvector` types to the domain entity.
2. **Infrastructure Configuration**: In `AppDbContext.cs`, we configured a Shadow Property `SearchVector` of type `NpgsqlTsVector` and mapped it as a `GENERATED ALWAYS AS (to_tsvector('english', "Content")) STORED` column.
3. **GIN Index**: We added a Generalized Inverted Index (GIN) on the shadow property, ensuring that searches over millions of chunks remain virtually instant.
4. **Tuple Projections**: To avoid creating unnecessary Read Models in the Application Layer, `IDocumentRepository.SearchChunksAsync` projects the query results directly into a C# Tuple: `IList<(DocumentChunk Chunk, float Rank)>`. The handler returns this tuple, and the Controller maps it to a pristine `SearchResultDto`.
5. **Smart Querying**: We leveraged `EF.Functions.WebSearchToTsQuery`, which automatically parses unformatted user input (including quotes for exact matches and hyphens for exclusions) and translates it into a structured boolean query that PostgreSQL can execute against the GIN index.

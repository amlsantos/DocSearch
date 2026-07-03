# Supported File Formats

Acme Translate processes the following file types.

## Documents

- Microsoft Word (`.docx`)
- PowerPoint (`.pptx`)
- Excel (`.xlsx`)
- PDF (`.pdf`) — text-based only; scanned PDFs require OCR pre-processing
- Plain text (`.txt`), Markdown (`.md`)

## Structured content

- XLIFF 1.2 and 2.0 (`.xlf`, `.xliff`)
- JSON and YAML resource files
- HTML and XML
- CSV (UTF-8 encoded)

## Subtitles

- SubRip (`.srt`)
- WebVTT (`.vtt`)

## Limits

Files up to **100 MB** are accepted per upload. Password-protected files are rejected at upload time. Embedded images are preserved but their contained text is not translated unless OCR is enabled on the project.

## Format-specific notes

For Excel files, hidden sheets are skipped by default; enable **Include hidden content** in project settings to translate them. For HTML, content inside `<code>` and `<pre>` tags is locked from translation automatically.

# Termbases

A termbase is a structured, multilingual terminology database. Unlike a glossary — which is a flat source→target list for one language pair — a termbase entry represents a single *concept* with terms in many languages plus rich metadata.

## Entry structure

Each concept entry can include, per language: one or more terms, definition, part of speech, usage notes, status (**preferred**, **admitted**, or **forbidden**), and example sentences.

Forbidden terms are flagged in the editor and fail automated QA checks if they appear in a translation.

## Import and export

Termbases support TBX import/export and CSV import with a column-mapping step. Existing glossaries can be upgraded to a termbase from the glossary settings page; the reverse (downgrading) is not supported.

## When to use a termbase instead of a glossary

Use a termbase when you translate into more than two languages, need to ban specific terms, or require definitions so translators understand the concept rather than just substituting words. For a simple brand-name list in one language pair, a glossary is lighter and easier to maintain.

## Limits

Termbases have no entry limit, but only one termbase can be attached per project.

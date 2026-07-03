# Working with Glossaries

A glossary is a simple list of preferred translations for specific terms. Glossaries help keep brand names, product names, and key vocabulary consistent across translations.

## Creating a glossary

Go to **Resources → Glossaries → New**. You can add entries manually or import a two-column CSV (source term, target term). Each glossary is tied to one language pair.

## How glossaries are applied

During machine translation, glossary terms are enforced: when a source term appears in the text, the engine uses the specified target term. Human translators see glossary matches highlighted in the editor with the preferred translation suggested.

## Glossary vs. termbase

A glossary is a flat source→target term list per language pair. If you need definitions, part-of-speech information, forbidden terms, or multi-language entries, use a **termbase** instead (see the termbase article). As a rule of thumb: start with a glossary; move to a termbase when terminology needs governance.

## Limits

A glossary may contain up to 10,000 entries. Up to 5 glossaries can be attached to a single project; if two glossaries define the same source term, the project-level glossary order decides which wins.

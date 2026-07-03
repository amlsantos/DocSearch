# Machine Translation Quality

Acme Translate routes content to one of several MT engines and scores the output to decide whether human review is recommended.

## Quality estimation (QE)

Every machine-translated segment receives a QE score from 0 to 100. The score estimates translation quality *without* a reference translation, based on fluency and source-target alignment signals.

- **90–100:** publishable for low-risk content
- **70–89:** light post-editing recommended
- **Below 70:** full human review recommended

QE scores are estimates, not guarantees. Legal, medical, and safety-critical content should always include human review regardless of score.

## Improving MT output

- Attach a glossary or termbase — enforced terminology is the single biggest quality lever.
- Provide translation memory: previously approved translations are reused for exact and fuzzy matches before MT runs.
- Keep source text clean: short sentences, consistent terminology, and no embedded markup improve engine output measurably.

## Engine selection

By default the platform picks an engine per language pair and content type based on historical QE performance. Enterprise plans can pin a specific engine per project.

# Teste de Ingestão

Este é um documento de teste criado para validar se o processo de ingestão de documentos únicos está a funcionar corretamente após a correção do problema de precisão de datas.

## Funcionalidades Esperadas
1. O ficheiro deve ser lido da pasta docs.
2. A data de modificação (`LastModifiedUtc`) deve ser registada corretamente.
3. Se o ingest for executado novamente sem alterações neste ficheiro, o sistema deverá ignorá-lo e não voltar a fazer a ingestão.

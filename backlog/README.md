# Backlog — Edoha API

Plano de débitos técnicos e refatorações pendentes.
Cada arquivo descreve uma tarefa independente (com dependências sinalizadas no topo) e é pensado para ser **executado por uma IA** com tempo/tokens limitados.

## Ordem recomendada de leitura/execução

| # | Arquivo | Tema | Complex. | Depende de |
|---|---|---|---|---|
| 00 | [00-contexto-geral.md](./00-contexto-geral.md) | Contexto do projeto (leitura obrigatória) | — | — |
| 01 | [01-preparar-deploy-aws-ecs-fargate.md](./01-preparar-deploy-aws-ecs-fargate.md) | Dockerfile, healthcheck, CI ECR, task definition | 🟡 | — |
| 02 | [02-refatorar-autenticacao-jwt.md](./02-refatorar-autenticacao-jwt.md) | `[Authorize]`, refresh, logout, hash do refresh | 🔴 | — |
| 03 | [03-organizacao-solid-ddd.md](./03-organizacao-solid-ddd.md) | Camadas, BaseController, remover try/catch | 🔴 | — |
| 04 | [04-padronizar-modelos-request-response-entity.md](./04-padronizar-modelos-request-response-entity.md) | Eliminar DTOs; Requests/Entities/Responses | 🟡 | 03 |
| 05 | [05-objetos-expandidos-relacionamentos.md](./05-objetos-expandidos-relacionamentos.md) | FKs hidratadas em GETs | 🟡 | 04 |
| 06 | [06-value-objects.md](./06-value-objects.md) | Phone, Cpf, Email, Password, Money, Slug | 🟡 | 03 |
| 07 | [07-regras-de-negocio-em-entidades-e-vos.md](./07-regras-de-negocio-em-entidades-e-vos.md) | Onde mora cada regra | 🔴 | 03, 06 |
| 08 | [08-debitos-tecnicos-adicionais.md](./08-debitos-tecnicos-adicionais.md) | Cleanups e bugs pequenos | 🟢/🟡 | parcial |

## Convenções dos arquivos

Todo arquivo começa com:
- **Dependência**: tarefas que precisam ser concluídas antes.
- **Complexidade**: 🟢 Baixa · 🟡 Média · 🔴 Alta.
- **Critérios de aceitação**: lista para a IA validar antes de fechar a task.
- **Notas para a IA executora**: cuidados específicos.

Um critério forte aplicado em todas: **não alterar contratos HTTP** (rotas, verbos e payloads externos) sem necessidade explícita.

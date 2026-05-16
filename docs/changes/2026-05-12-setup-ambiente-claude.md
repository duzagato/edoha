# Setup do ambiente Claude

**Data:** 2026-05-12  
**Branch:** claude-init

## O que foi feito

### CLAUDE.md (criado na raiz)
Guia de contexto do projeto para o Claude. Contém:
- Descrição dos 4 projetos da solution
- Schemas do banco (`edoha` e `lottery`) e suas tabelas
- Padrões de código (BaseRepository reflection, Service<T>, validação via RequestValidationContext)
- Fluxo de autenticação JWT e autorização por permissão
- Middleware de exceções
- CI/CD via GitHub Actions
- Explicação da estrutura `docs/backlog/AI`, `docs/backlog/Human` e `docs/changes`

### .claudeignore (criado na raiz)
Instrui o Claude a ignorar arquivos que não agregam no entendimento do código:
- `src/.vs/` — arquivos internos do Visual Studio (índices, caches, layouts)
- `src/**/bin/` e `src/**/obj/` — outputs de build
- `**/*.user`, `**/*.suo` — preferências locais do VS
- `**/*.db` — arquivos de banco do CopilotIndices
- `edoha.sql` — schema SQL de referência (lido manualmente quando necessário)

### Memórias persistentes (em ~/.claude/projects/.../memory/)
Três arquivos criados para reter contexto entre conversas:
- `project-architecture.md` — stack, padrões, schemas, CI/CD
- `user-profile.md` — perfil do desenvolvedor
- `feedback-docs-convention.md` — regra de criar docs/changes e docs/backlog/Human após cada task

### Estrutura docs/
- `docs/backlog/AI/` — diretório para tasks do Claude
- `docs/backlog/Human/` — diretório para intervenções humanas pós-task
- `docs/changes/` — diretório para registro de mudanças

## Arquivos modificados
| Arquivo | Ação |
|---|---|
| `CLAUDE.md` | Criado |
| `.claudeignore` | Criado |
| `docs/changes/2026-05-12-setup-ambiente-claude.md` | Criado (este arquivo) |
| `~/.claude/.../memory/MEMORY.md` | Criado |
| `~/.claude/.../memory/project-architecture.md` | Criado |
| `~/.claude/.../memory/user-profile.md` | Criado |
| `~/.claude/.../memory/feedback-docs-convention.md` | Criado |

## Nenhum código foi alterado
Esta tarefa foi puramente de configuração e documentação.

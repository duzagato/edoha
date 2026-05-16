# 08 — Outros débitos técnicos (sem categoria nas demais tasks)

> **Dependência:** Cada subtask é independente. Algumas se sobrepõem com a 01, 02 ou 03 — quando isso acontecer está marcado `(ver task XX)`.
> **Complexidade:** 🟢 Baixa-🟡 Média (cada item individualmente é pequeno; o conjunto é grande).
> **Estratégia:** atacar como “lista de cleanups” em commits curtos. A IA pode executar em qualquer ordem.

---

## 8.1 Bug crítico: workflow `3. merge-release.yml` com conflict markers

Arquivo: `.github/workflows/3. merge-release.yml`

Contém:
```
<<<<<<< HEAD
      - name: Fazer merge automático da release na main
        run: |
          gh pr merge "$PR_URL" --merge --delete-branch
        env:
          GH

=======
>>>>>>> 8cc1db33f72817871d4db382bdcaab0670de8819
```

YAML inválido. Ações de release não rodam. **Resolver o conflito**: decidir se o passo extra de auto-merge fica ou não. Provável intenção: manter o `Criar PR release → main` e remover o bloco em conflito.

---

## 8.2 Renomear arquivos de workflow (boa prática)

Os nomes têm espaço e prefixo numérico: `1. pr-to-develop.yml`. Apesar de funcionar, complica scripting. Renomear para `pr-to-develop.yml`, `merge-develop.yml`, `merge-release.yml`, `merge-main.yml`. Manter o atributo `name:` interno.

> Opcional. Se preferir não tocar para preservar histórico de execuções, deixar.

---

## 8.3 `appsettings.Development.json` versionado, mas listado no `.gitignore`

`.gitignore` tem `appsettings.Development.json`, mas o arquivo está commitado em `src/Edoha.Application/appsettings.Development.json`. Decidir:

- **Manter rastreado**: remover do `.gitignore` (linha 41).
- **Não rastrear**: `git rm --cached src/Edoha.Application/appsettings.Development.json` e versionar um `.template`.

Recomendado: **manter rastreado** (já está sendo usado como template; conteúdo só tem `Logging`). Remover do `.gitignore` para não confundir.

---

## 8.4 `try/catch` que vaza `StackTrace` (ver task 03)

Já coberto pela task 03. Reforçando: se a 03 ainda não foi feita e a 08 estiver sendo priorizada, **trocar pelo menos** `StackTrace = ex.StackTrace` por nada (em todos os controllers). É vetor de info disclosure.

Arquivos afetados (busca `ex.StackTrace`):
- `UserController.cs`, `InstitutionController.cs`, `LotteryController.cs`, `TicketbookController.cs`, `TicketController.cs`, etc.

---

## 8.5 Senha de banco em `appsettings.json`

`appsettings.json:10`:
```
"Default": "Server=localhost;Database=edoha;User Id=postgres;Password=12341234;..."
```

Substituir por placeholder vazio e mover senha real para Secrets/User Secrets locais. Revogar no histórico via `git filter-repo` é fora de escopo, mas **comentar** que essa senha já está exposta no histórico e deve ser rotacionada na DB local.

> Coberto também na task 01.

---

## 8.6 Chave JWT hardcoded duplicada

`JwtConfig.cs` e `appsettings.json` têm a mesma chave. Coberto na task 02.

Adicionalmente: no `appsettings.json`, a chave está envolvida em **aspas duplas**: `"\"S3nh4Muit0F0rt3eS3gur4d3NoM1n1mo32\""`. Quando lida via `configuration["Jwt:Key"]`, o valor vai conter aspas como caractere — bug. Remover as aspas internas.

---

## 8.7 `RNGCryptoServiceProvider` está obsoleto (.NET 6+)

`Crypto.cs:20`: `using (var rng = new RNGCryptoServiceProvider())`.

Trocar por `RandomNumberGenerator.Create()` ou usar `RandomNumberGenerator.Fill(buffer)` direto. Funciona idêntico, sem warning de obsolescência.

---

## 8.8 `Console.WriteLine` em código de produção

`Crypto.cs:49-50`:
```
Console.WriteLine(testHash);
Console.WriteLine(hash);
```

Vaza dados sensíveis no log. **Remover**.

---

## 8.9 `IsUnique` com bug de parâmetro nomeado

`BaseRepository.IsUnique`:
```
var query = $@"... WHERE ""{column}"" = @Value";
var count = await _connection.ExecuteScalarAsync<int>(query, new { column = value });
```

O parâmetro nomeado é `column`, mas o SQL referencia `@Value`. Corrigir para `new { Value = value }`.

---

## 8.10 `InsertOrGetId` com `conflictColumns` sobrescrito

`BaseRepository.InsertOrGetId`:
```
var conflictColumns = string.Join(", ", columnNames);
conflictColumns = "phone";
```

Calcula a lista e em seguida joga fora, fixando `"phone"`. Se intencional para `User`, está acoplando a base genérica a uma tabela específica — antipattern. Tornar o método **virtual** ou adicionar parâmetro `string conflictColumn`. Sobrescrever em `UserRepository.InsertOrGetUserInformation` em vez de hardcoded na base.

---

## 8.11 `Dictionary` de erros não thread-safe + `AddError` `async` que nunca aguarda nada

`RequestValidationContext`:
```
public async Task AddError(string errorKey, string errorMessage) {
    if (!Errors.ContainsKey(errorKey)) {
        Errors.Add(errorKey, errorMessage);
    }
}
```

Não tem `await`. Trocar para método sync: `public void AddError(...)`. Atualizar chamadores (vários services usam `await _requestValidationContext.AddError(...)` desnecessariamente).

Concorrência: como `IRequestValidationContext` é Scoped (registrado em `ServiceInjection`), 1 instância por request — `Dictionary` é seguro **se** não houver paralelismo dentro do mesmo request. Atual código não paraleliza, ok. Apenas documentar.

---

## 8.12 Exceção sem sentido: `CannotUnloadAppDomainException`

`AuthService.Autenticate`:
```
throw new CannotUnloadAppDomainException("Usuário não possui acesso a nenhuma instituição");
```

Substituir por `UnauthorizedAccessException` ou `RequestValidationException`. Coberto também na task 02.

---

## 8.13 `Microsoft.Data.SqlClient` instalado sem uso

`Edoha.Application.csproj`:
```
<PackageReference Include="Microsoft.Data.SqlClient" Version="6.0.1" />
```

Projeto usa Postgres (Npgsql). Pacote não é referenciado em código. Remover.

---

## 8.14 `Serilog.Crestron` instalado sem uso

`Edoha.Application.csproj` referencia `Serilog.Crestron` 2.1.0 — biblioteca para hardware Crestron, **não** faz sentido aqui. Remover.

---

## 8.15 `Nullable` desabilitado em alguns projetos

Verificar `Edoha.Domain.csproj`, `Edoha.Infraestructure.csproj`, `Edoha.Shared.csproj`. `Edoha.Application.csproj` tem `<Nullable>enable</Nullable>`. Padronizar **todos** para `enable` e tratar warnings. Há vários `string Property { get; set; }` sem inicialização que vão virar warnings.

> Esperar a tarefa 04 (modelos) antes desta para evitar dobrar trabalho.

---

## 8.16 Annotations vazias / arquivos mortos

- `Edoha.Domain/Annotations/String/MaxLength.cs` (vazio).
- `Edoha.Domain/Annotations/String/RangeLength.cs` (vazio).
- `src/Edoha.Application/Middlewares/RequestContextMiddleware.cs` (vazio).
- `Edoha.Domain/Models/DTOs/User/UpdateUserDTO.cs` (classe vazia herdando de `DTO`).

Remover. (`UpdateUserDTO` precisa primeiro ser **substituído** pela task 04 antes de ser apagado, ou o controller quebra.)

---

## 8.17 `ServoceCollection` typo

Renomear para `ServiceCollectionExtensions`. Coberto na task 03.

---

## 8.18 Dapper `MatchNamesWithUnderscores = true` setado em vários lugares

Aparece em `BaseRepository.SelectById`, `SelectAll`, `UserRepository`. Setar **uma vez** em `Program.cs` (ou em `DapperConfig.Configure()` da task 06). É configuração global.

---

## 8.19 `IDbConnection` Singleton (concorrência)

Coberto na task 03 (item 3.7). Reforçando: bug grave de produção sob carga.

---

## 8.20 `DateTime.Now` em vez de `DateTime.UtcNow`

`TicketbookService.InsertTicketbook`: `WithdrawnDate = DateTime.Now`, `DevolutionDate = DateTime.Now`.

No Lambda a função sempre executa em UTC. `DateTime.Now` pode divergir dependendo da timezone do host de desenvolvimento. **Trocar para `DateTime.UtcNow`** em todo o código (`grep -n "DateTime.Now" src/`).

---

## 8.21 `Task` async sem `await` retornando direto

`UserController.DeleteById`:
```
[HttpDelete("{id}")]
public async Task DeleteById(Guid id) { await _userService.DeleteUserById(id); }
```

Note o retorno é `Task` (não `Task<IActionResult>`). Funciona por convenção do ASP.NET, mas sem status code customizável (sempre 200). Padronizar para retornar `IActionResult`/`NoContent()`. Várias controllers fazem isso (`InstitutionController.DeleteById`, `LotteryController.DeleteById`, `TicketbookController.DeleteById`, `TicketController.DeleteById`).

---

## 8.22 `StaticQueries` (`Edoha.Infraestructure/Constants/StaticQueries.cs`)

Verificar conteúdo (não foi inspecionado nesta análise mas é referenciado por `UserRepository`). Provavelmente strings SQL grandes em `static readonly`. Considerar mover para arquivos `.sql` embutidos como Resource e ler via `Assembly.GetManifestResourceStream`. Mais legível e formatável por IDE de SQL.

> Sub-task **opcional**.

---

## 8.23 `UserInstitution.CreateAt` / `CreateBy` typo

`Edoha.Domain/Entities/UserInstitution.cs`:
```
public DateTime CreateAt { get; set; }
public Guid? CreateBy { get; set; }
```

Tem que ser `CreatedAt` / `CreatedBy` (alinhado com `Entity` base). Verificar se a coluna no banco é `create_at` ou `created_at` antes de renomear para evitar quebrar SQL via `MatchNamesWithUnderscores`.

---

## 8.24 `UserPermission` / `Login` / `UserInstitution` não herdam de `Entity`

São tabelas legítimas com `id`/`created_at`/`created_by`. Padronizar herdando de `Entity`. Atenção: pode mudar nomes de colunas (snake_case) — verificar SQL antes.

---

## 8.25 CORS hardcoded `http://localhost:4200`

Coberto na task 01.

---

## 8.26 Swagger configurado **duas vezes**

`Program.cs:36`: `builder.Services.AddSwaggerGen();`
`JwtInjection.AddJwt`: `services.AddSwaggerGen(c => { ... })`.

A última chamada **substitui** opções, então o cadeado Bearer pode estar funcionando ou não conforme ordem. Unificar em um único lugar. Coberto na task 02.

---

## 8.27 `AuthController` retorna `Ok(response)` mas não tipa o response

Adicionar `[ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]` em todos os endpoints relevantes para Swagger gerar contratos. Aumenta utilidade do front.

---

## 8.28 Falta `EditorConfig` / lint .NET

Adicionar `.editorconfig` na raiz com regras mínimas (4 spaces, UTF-8, LF, `dotnet_diagnostic.CSxxxx.severity = warning` para nullables). Permite que `dotnet format` rode.

> Opcional, mas barato e ajuda muito a IA executar tasks futuras com consistência.

---

## 8.29 README quase vazio

Atualmente `README.md` tem apenas 7 bytes (`# edoha`). Documentar:
- Stack.
- Como rodar localmente (DB Postgres + `dotnet run`).
- Como rodar via Docker (após task 01).
- Estrutura das pastas.
- Onde está o backlog (`backlog/`).

---

## 8.30 `edoha.sql` está praticamente vazio (2 bytes)

Adicionar o DDL real (schemas `edoha`, `lottery`, todas as tabelas usadas). Sem isso é impossível dar setup novo. Idealmente como **migrations** versionadas (FluentMigrator/EF Migrations/Flyway). Sem migrations, ao menos um `init.sql` completo.

> Pode virar uma task própria se for grande. Como subtask aqui está OK.

---

## Critérios de aceitação

Cada item acima é um critério individual. Não precisa concluir todos de uma vez; podem ser entregues em PRs sucessivos.

| # | Severidade | Esforço |
|---|---|---|
| 8.1 conflict markers | 🔴 Crítico | 5 min |
| 8.5 senha em settings | 🔴 Crítico | 10 min |
| 8.6 chave JWT entre aspas | 🔴 Crítico | 5 min |
| 8.8 Console.WriteLine de hash | 🔴 Crítico (info disclosure) | 5 min |
| 8.4 StackTrace em response | 🟠 Alto | 30 min |
| 8.9 IsUnique parâmetro errado | 🟠 Alto | 5 min |
| 8.10 InsertOrGetId hardcode | 🟠 Alto | 20 min |
| 8.19 IDbConnection singleton | 🟠 Alto | 5 min |
| 8.20 DateTime.Now → UtcNow | 🟠 Alto | 10 min |
| 8.12 CannotUnloadAppDomainException | 🟡 Médio | 5 min |
| 8.7, 8.11, 8.13–8.18, 8.21–8.30 | 🟢 Baixo | 5–30 min cada |

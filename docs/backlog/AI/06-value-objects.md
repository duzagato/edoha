# 06 — Value Objects (Phone, Cpf, Email, Password, Money, Slug)

> **Dependência:** Recomenda-se ter feito **task 03** (organização). Não depende de 04/05.
> **Complexidade:** 🟡 Média — bastante criação de classes pequenas com testes; impacto em entidades e mapeamentos.

---

## Quando criar um Value Object?

- Possui **invariantes** (regras que devem ser sempre verdadeiras).
- É **imutável**.
- Tem **igualdade por valor** (não por referência).
- Pode ter comportamento próprio (formatação, comparação, máscara).

Sintomas no código atual que **clamam** por VO:
- `User.Phone` é `string`, sem validação de formato.
- Não existe CPF, mas o domínio (clientes/owners de talão) provavelmente vai querer.
- `Lottery.PriceTicket` é `decimal` puro — sem moeda, sem precisão garantida.
- `Institution.SlugName` é `string` — formato sensível, sem garantia.
- Senha (`User.Password` como `byte[]`) e nickname não têm regra encapsulada — está na `UserService.HashPassword` e `IsUsernameSended`/`IsPasswordSended`.

---

## Itens de trabalho

### 6.0 Estrutura comum

Criar pasta `src/Edoha.Domain/ValueObjects/`.

Criar classe abstrata: `ValueObject` (típica do DDD), com `Equals`/`GetHashCode` baseados em `IEnumerable<object> GetEqualityComponents()`.

```
public abstract class ValueObject {
    protected abstract IEnumerable<object?> GetEqualityComponents();
    public override bool Equals(object? obj) { ... }
    public override int GetHashCode() { ... }
    public static bool operator ==(ValueObject? a, ValueObject? b) => Equals(a, b);
    public static bool operator !=(ValueObject? a, ValueObject? b) => !Equals(a, b);
}
```

### 6.1 `Phone`

Arquivo: `src/Edoha.Domain/ValueObjects/Phone.cs`

Regras:
- Aceita formato BR: `(xx) xxxxx-xxxx`, `xx9xxxxxxxx` (11 dígitos), ou DDI `+55...`.
- Construtor recebe `string raw`, sanitiza (remove `()`, `-`, espaços), valida quantidade de dígitos (10 ou 11).
- Throw `RequestValidationException` (ou criar `DomainException` própria) com chave `"Phone"` em caso de inválido.
- Propriedade `Value` retorna apenas dígitos (canonical). Método `ToFormatted()` retorna com máscara.

### 6.2 `Cpf`

Arquivo: `src/Edoha.Domain/ValueObjects/Cpf.cs`

Regras:
- 11 dígitos, valida dígitos verificadores (algoritmo padrão).
- Rejeita CPFs com todos dígitos iguais (`111.111.111-11`).
- `Value` (só dígitos), `ToFormatted()` com máscara `xxx.xxx.xxx-xx`.

### 6.3 `Email`

Arquivo: `src/Edoha.Domain/ValueObjects/Email.cs`

Regras:
- Trim + lower-case.
- Valida via `MailAddress` ou regex simples (`^[^@\s]+@[^@\s]+\.[^@\s]+$`).

### 6.4 `Password` (input em claro)

Arquivo: `src/Edoha.Domain/ValueObjects/Password.cs`

Regras:
- 8 a 30 caracteres (já é a regra do `UserService.HashPassword`).
- Imutável; expõe `string Value` somente para o serviço de hash. Não logar.
- Método estático `Create(string raw)` que valida.

### 6.5 `HashedPassword` (saída de Crypto)

Arquivo: `src/Edoha.Domain/ValueObjects/HashedPassword.cs`

Encapsula `byte[] Hash`. Construtor privado; factory `FromHash(byte[])`. Método `Verify(string raw, ICrypto crypto)` para comparação.

> Mais higiênico que `User.Password: byte[]?`. Substituir o tipo na entidade.

### 6.6 `Money`

Arquivo: `src/Edoha.Domain/ValueObjects/Money.cs`

Regras:
- `decimal Amount`, `string Currency` (ex.: `"BRL"`).
- Operações: `+`, `-`, `*` por `decimal`. Não permitir misturar moedas.
- Para o domínio atual, todos preços são em BRL — pode-se fixar `Currency = "BRL"`.

Substituir `Lottery.PriceTicket` (decimal) por `Money`. Persistir no banco como `decimal` (ler de coluna `price_ticket`) com Type Handler do Dapper (ver 6.10).

### 6.7 `Slug`

Arquivo: `src/Edoha.Domain/ValueObjects/Slug.cs`

Regras:
- Lower-case, só `[a-z0-9-]`, sem espaços, sem acentos.
- Construtor faz **normalização** de uma string “livre” (remove acentos, troca espaço por `-`, limita comprimento), ou recebe um slug já pronto e valida.

Substituir `Institution.SlugName` por `Slug`.

### 6.8 `Nickname`

Opcional. Centraliza regra `"Digite um nome de usuário"` (`UserAlerts.EmptyUsername`) e formato (sem espaços, min 3 chars). Ajuda a remover lógica de `UserService.IsUsernameSended`.

### 6.9 Substituições nas entidades

| Entidade | Antes | Depois |
|---|---|---|
| `User.Phone` | `string?` | `Phone?` |
| `User.Password` | `byte[]?` | `HashedPassword?` |
| `User.Nickname` | `string?` | `Nickname?` (opcional) |
| `Holder.Phone` / `Owner.Phone` (em `Ticketbook`) | `string` | `Phone` |
| `Institution.SlugName` | `string` | `Slug` |
| `Lottery.PriceTicket` | `decimal` | `Money` |

> `Ticketbook.Owner.Cpf`, `Holder.Cpf` ainda **não existem** — adicionar `Cpf?` nas classes `Owner`/`Holder` (inclusive na coluna do banco; documentar que precisa de migração SQL).

### 6.10 Mapeamento Dapper para VOs

Dapper não sabe materializar `Phone` a partir de uma string. Soluções:

**(a) Type Handlers** (`Dapper.SqlMapper.AddTypeHandler<Phone>(new PhoneHandler())`).
- Registrar todos no `Program.cs` (ou em `Edoha.Infraestructure/Util/DapperTypeHandlers.cs`) **antes** de qualquer query.

**(b) Propriedade auxiliar `string PhoneRaw` que mapeia a coluna**, e `Phone` calculado em getter.

Recomendação: **(a)** Type Handlers para `Phone`, `Cpf`, `Email`, `Slug`, `Money`. Para `HashedPassword`, idem mapeando `byte[]`.

Estrutura de cada handler:

```
public class PhoneHandler : SqlMapper.TypeHandler<Phone> {
    public override Phone? Parse(object value) =>
        value is string s ? new Phone(s) : null;
    public override void SetValue(IDbDataParameter parameter, Phone? value) {
        parameter.Value = value?.Value ?? (object)DBNull.Value;
    }
}
```

Registrar:

```
SqlMapper.AddTypeHandler(new PhoneHandler());
SqlMapper.AddTypeHandler(new CpfHandler());
... (etc.)
```

Local sugerido: `Edoha.Infraestructure/Util/DapperConfig.cs` com método estático `Configure()`. Chamar em `Program.cs`.

### 6.11 Migrar validações e simplificar `UserService`

Após adoção:
- `UserService.IsUsernameSended` / `IsPasswordSended` → desnecessários (VOs já validam).
- `UserService.HashPassword` recebe `Password` (VO) e devolve `HashedPassword`.
- A regra de comprimento da senha (8–30) **muda de lugar**: do `UserService` para o construtor de `Password`.

> Esta migração de regras casa diretamente com a **task 07** (regras nas entidades/VOs).

### 6.12 Atualizar `RequestValidationException`/serialização

Quando um VO lança no construtor, o middleware deve transformá-lo em 400 com mensagem de campo. Definir `DomainException` (em `Edoha.Domain/Exceptions/DomainException.cs`) com `string FieldName`, e tratar no `ExceptionHandlingMiddleware` como 400.

---

## Banco de dados — atenção

- `User.Password BYTEA` continua compatível com `HashedPassword` (que serializa para `byte[]`).
- `Institution.SlugName VARCHAR` continua compatível com `Slug` (string).
- `Lottery.PriceTicket NUMERIC` compatível com `Money` (apenas `decimal Amount`; moeda não é persistida — fixa em BRL).
- `Phone`/`Cpf`: salvar **somente dígitos** (canonical). Atualizar comentário no `edoha.sql` e na seção de Database do README.

---

## Critérios de aceitação

- [ ] Pasta `Edoha.Domain/ValueObjects/` criada com pelo menos: `Phone`, `Cpf`, `Email`, `Password`, `HashedPassword`, `Money`, `Slug`, `ValueObject` base.
- [ ] Cada VO tem testes manuais (smoke) — ou, se houver projeto de teste, testes unitários cobrindo casos válidos e inválidos. **Se não houver projeto de teste**, criar um pequeno `Edoha.Domain.Tests` com xUnit (opcional, marcar como TODO).
- [ ] Entidades migradas (`User`, `Institution`, `Lottery`, `Holder`/`Owner` do `Ticketbook`).
- [ ] Type Handlers do Dapper registrados em um único ponto e chamados no boot.
- [ ] `UserService.HashPassword` recebe `Password` (VO).
- [ ] `dotnet build src/Edoha.sln` passa.
- [ ] Cadastro de usuário (`POST /user`) continua funcionando ponta-a-ponta.

---

## Notas para a IA executora

- Adotar **um VO por vez** (commit por VO + entidade afetada). `Phone` é o mais barato; comece por ele.
- Testar via Swagger após cada VO migrado.
- Não criar VO “só para criar”. Se uma string não tem invariante, **deixar como string**.

# Edoha API — System Reference for AI Integration

This document is the authoritative reference for building or updating frontend API integrations.
Read this before touching any API call in the frontend codebase.

---

## Stack & Conventions

- **Runtime:** .NET 8 ASP.NET Core, PostgreSQL, Dapper (no ORM)
- **Auth:** JWT Bearer, 60-minute expiration
- **Content-Type:** `application/json` on all requests with a body
- **ID format:** UUID v4 strings for all entity IDs, except `StatusTicketbook.id` which is an integer
- **Date format:** ISO 8601 (`2026-06-01T00:00:00Z`)
- **Naming convention:** API fields use camelCase (C# PascalCase serialized to camelCase)
- **Empty list responses:** `204 No Content` (no body) instead of `200 []`
- **Validation errors:** `400` with body `{ is_valid: false, errors: { field: ["message"] } }`

---

## Authentication

### POST /auth

The only public endpoint. All others require `Authorization: Bearer <token>`.

**Request:**
```json
{ "nickname": "john.doe", "password": "secret123" }
```

**Response 200:**
```json
{
  "idUser": "uuid",
  "accessToken": "eyJ...",
  "institutions": [
    {
      "id": "uuid",
      "name": "Instituição X",
      "slugName": "instituicao-x",
      "shortName": "Inst X",
      "description": null,
      "logoDirectory": null,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ]
}
```

**Storage pattern:**
Store `accessToken` and `idUser` in session/localStorage. Attach token to every subsequent request:
```
Authorization: Bearer <accessToken>
```

On `401` response from any endpoint: token expired → redirect to login.

---

## Resource Hierarchy

```
Institution  (tenant/organization)
  └── Lottery  (rifa)
        └── Ticketbook  (talão — a booklet of tickets)
              └── Ticket  (bilhete individual)
```

Routing reflects this hierarchy. You always need parent IDs in the URL path:

| Resource | Route prefix |
|---|---|
| Lottery | `/institution/{idInstitution}/lottery` |
| Ticketbook | `/lottery/{idLottery}/ticketbook` |
| Ticket | `/ticketbook/{idTicketbook}/ticket` |

---

## Permission System

The system uses Page + Action to authorize each endpoint.

- **Page** = controller name (e.g., `UserController` → page name `User`)
- **Action** = HTTP verb (e.g., GET, POST, PUT, DELETE, PATCH)
- **Permission** = named profile (e.g., "Admin") linked to an institution
- **UserPermission** = a tuple of (User, Page, Permission, Action) that grants access

To read a user's permissions: `GET /userpermission/{userId}` → returns permissions grouped by page.

---

## Endpoints Reference

### Auth

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | /auth | No | Login — returns JWT + institutions |

---

### Institution

| Method | Path | Body/Params | Returns |
|---|---|---|---|
| GET | /institution | — | `Institution[]` or 204 |
| GET | /institution/{slug} | slug: string | `Institution` or 204 |
| GET | /institution/{id} | id: uuid | — (used for DELETE only) |
| GET | /institution/institution_by_user/{idUser} | idUser: uuid | `Institution[]` or 204 |
| POST | /institution | `CreateInstitutionDTO` | 200 |
| PUT | /institution | `UpdateInstitutionDTO` | 200 |
| DELETE | /institution/{id} | id: uuid | 200 |

**Institution schema:**
```typescript
interface Institution {
  id: string           // uuid
  createdAt: string    // ISO 8601
  name: string
  slugName: string     // URL-friendly identifier
  shortName: string    // abbreviated name
  description: string | null
  logoDirectory: string | null
}
```

**Create body:**
```typescript
interface CreateInstitutionDTO {
  name: string         // required
  slugName: string     // required
  shortName: string    // required
  description?: string
  logoDirectory?: string
}
```

**Update body:**
```typescript
interface UpdateInstitutionDTO {
  id: string           // required (uuid)
  name?: string
  slugName?: string
  shortName?: string
  description?: string
  logoDirectory?: string
}
```

---

### Lottery

All routes are under `/institution/{idInstitution}/lottery`.

| Method | Path | Body | Returns |
|---|---|---|---|
| GET | /institution/{idInstitution}/lottery | — | `Lottery[]` or 204 |
| GET | /institution/{idInstitution}/lottery/{id} | — | `Lottery` or 204 |
| POST | /institution/{idInstitution}/lottery | `CreateLotteryDTO` | 200 |
| PUT | /institution/{idInstitution}/lottery | `UpdateLotteryDTO` | 200 |
| DELETE | /institution/{idInstitution}/lottery/{id} | — | 200 |

**Lottery schema:**
```typescript
interface Lottery {
  id: string
  createdAt: string
  idInstitution: string
  name: string
  numTicketsTicketbook: number   // tickets per booklet
  numTicketbooks: number         // total booklets
  priceTicket: number            // price per ticket (decimal)
  doubleChance: boolean          // each ticket enters draw twice
}
```

**Create body:**
```typescript
interface CreateLotteryDTO {
  name: string                   // required
  numTicketsTicketbook: number   // required, min 1
  numTicketbooks: number         // required, min 1
  priceTicket: number            // required, min 1
  doubleChance: boolean          // required
  // idInstitution is ignored — taken from URL path
}
```

**Update body:**
```typescript
interface UpdateLotteryDTO {
  id: string                     // required
  name: string                   // required
  numTicketsTicketbook: number   // required, min 1
  numTicketbooks: number         // required, min 1
  priceTicket: number            // required, min 1
  doubleChance: boolean          // required
}
```

---

### Ticketbook

All routes are under `/lottery/{idLottery}/ticketbook`.

| Method | Path | Body/Params | Returns |
|---|---|---|---|
| GET | /lottery/{idLottery}/ticketbook | — | `Ticketbook[]` or 204 |
| GET | /lottery/{idLottery}/ticketbook/returneds | — | devolved ticketbooks (status=2) |
| GET | /lottery/{idLottery}/ticketbook/withdrawns | — | withdrawn ticketbooks (status=1) |
| GET | /lottery/{idLottery}/ticketbook/{id} | — | `Ticketbook` or 204 |
| GET | /lottery/{idLottery}/ticketbook/ticketbook_by_number/{number} | number: int | `Ticketbook` or 204 |
| POST | /lottery/{idLottery}/ticketbook | `PostTicketbookRequest` | 201 + `{ idTicketbook: uuid }` |
| PUT | /lottery/{idLottery}/ticketbook | `UpdateTicketbookDTO` | 200 |
| DELETE | /lottery/{idLottery}/ticketbook/{id} | — | 200 |
| PATCH | /lottery/{idLottery}/ticketbook/{idTicketbook}/status/{idStatus} | — | 204 |
| PATCH | /lottery/{idLottery}/ticketbook/{idTicketbook}/status/returned | — | 204 (set status=2) |
| PATCH | /lottery/{idLottery}/ticketbook/{idTicketbook}/status/withdraw | — | 204 (set status=1) |

**Status values:**
| id | meaning |
|---|---|
| 1 | Retirado (withdrawn/taken) |
| 2 | Devolvido (returned) |

**Ticketbook schema (response):**
```typescript
interface Ticketbook {
  id: string
  createdAt: string
  idLottery: string
  ticketbookOwner: PersonWithId    // person who owns the booklet
  ticketbookHolder: PersonWithId | null  // person currently holding it
  idStatusTicketbook: number       // 1 or 2
  number: number                   // sequential number within the lottery
  withdrawnDate: string | null
  devolutionDate: string | null
  tickets: Ticket[] | null
}

interface PersonWithId {
  id: string | null   // uuid of User record (null if user not found in DB)
  name: string
  phone: string
}
```

**Create body — important: owner/holder identified by name+phone, not by UUID:**
```typescript
interface PostTicketbookRequest {
  ticketbookOwner: Person          // required
  ticketbookHolder?: Person        // optional
  idStatusTicketbook: number       // required: 1 or 2
  number: number                   // required, min 1
  withdrawnDate?: string           // ISO 8601
  devolutionDate?: string          // ISO 8601
}

interface Person {
  name: string
  phone: string
}
```

**Update body — uses UUIDs instead of name/phone:**
```typescript
interface UpdateTicketbookDTO {
  id: string                       // required
  idOwner?: string                 // uuid of new owner User
  idHolder?: string                // uuid of new holder User
  idStatusTicketbook?: number      // min 1
  withdrawnDate?: string
  devolutionDate?: string
}
```

---

### Ticket

All routes are under `/ticketbook/{idTicketbook}/ticket`.

| Method | Path | Body | Returns |
|---|---|---|---|
| GET | /ticketbook/{idTicketbook}/ticket | — | `Ticket[]` or 204 |
| GET | /ticketbook/{idTicketbook}/ticket/{id} | — | `Ticket` or 204 |
| POST | /ticketbook/{idTicketbook}/ticket | `CreateTicketRequest[]` | 201 |
| PUT | /ticketbook/{idTicketbook}/ticket | `UpdateTicketDTO` | 200 |
| DELETE | /ticketbook/{idTicketbook}/ticket/{id} | — | 200 |

**Ticket schema:**
```typescript
interface Ticket {
  idTicketbook: string
  idDonater: string | null   // uuid of buying User
  number: number             // ticket number within the booklet
  soldDate: string | null
}
```

**Create body — array, donater identified by name+phone:**
```typescript
type CreateTicketsRequest = Array<{
  donaterName?: string
  donaterPhone?: string
  number: number             // required, min 1
  soldDate?: string
}>
```

**Update body:**
```typescript
interface UpdateTicketDTO {
  id: string                 // required
  idDonater?: string         // uuid
  soldDate?: string
}
```

---

### User

| Method | Path | Body/Params | Returns |
|---|---|---|---|
| GET | /user | — | `User[]` or 204 |
| GET | /user/{id} | — | `User` or 204 |
| GET | /user/user_information?withTicketbooks=bool | withTicketbooks: boolean | `UserInfo[]` |
| POST | /user | `CreateUserInputModel` | 200 |
| PUT | /user | `UpdateUserDTO` | 200 |
| DELETE | /user/{id} | — | 200 |

**User schema:**
```typescript
interface User {
  id: string
  createdAt: string
  name: string
  phone: string | null
  nickname: string | null
  institutions: Institution[] | null
}
```

**Create body:**
```typescript
interface CreateUserInputModel {
  name: string               // required
  phone?: string
  nickname?: string
  unhashedPassword?: string  // plain text — hashed server-side with bcrypt
}
```

**UserInformationResponse (from /user/user_information):**
```typescript
interface UserInformationResponse {
  id: string
  name: string
  phone: string
}
```

---

### Permission System Entities

#### Action (HTTP verb in permission model)

| Method | Path | Body |
|---|---|---|
| GET | /action | — |
| GET | /action/{id} | — |
| POST | /action | `{ name, description?, withoutOwner?, otherOwner? }` |
| PUT | /action | `{ id, name?, description?, withoutOwner?, otherOwner? }` |
| DELETE | /action/{id} | — |

```typescript
interface Action {
  id: string
  createdAt: string
  name: string
  description: string | null
  withoutOwner: boolean   // can be performed without a booklet owner
  otherOwner: boolean     // can be performed by a different owner
}
```

#### Page (controller in permission model)

Route prefix: `/api/page` (note: different prefix from all other routes)

| Method | Path |
|---|---|
| GET | /api/page |
| GET | /api/page/{id} |
| POST | /api/page |
| PUT | /api/page |
| DELETE | /api/page/{id} |

#### Permission (named profile)

| Method | Path |
|---|---|
| GET | /permission |
| GET | /permission/{id} |
| POST | /permission |
| PUT | /permission |
| DELETE | /permission/{id} |

```typescript
interface Permission {
  id: string
  createdAt: string
  idInstitution: string
  name: string
  description: string | null
}
```

#### UserPermission

| Method | Path | Notes |
|---|---|---|
| GET | /userpermission/{id} | `{id}` = userId — returns permissions grouped by page |
| POST | /userpermission | Grants a permission to a user |
| DELETE | /userpermission/{id} | `{id}` = permission entry ID to remove |

**Read response:**
```typescript
interface UserPermissionPage {
  pageName: string       // controller name
  actions: Action[]     // allowed HTTP verbs on that controller
}
```

**Create body:**
```typescript
interface CreateUserPermissionDTO {
  idUser: string         // required
  idPage: string         // required — Page entity ID
  idPermission: string   // required — Permission profile ID
  idAction: string       // required — Action entity ID
}
```

---

### UserInstitution (user ↔ institution link)

| Method | Path |
|---|---|
| GET | /userinstitution |
| GET | /userinstitution/{id} |
| POST | /userinstitution |
| PUT | /userinstitution |
| DELETE | /userinstitution/{id} |

```typescript
interface UserInstitution {
  idUser: string
  idInstitution: string
  createAt: string        // note: camelCase is createAt (not createdAt)
  createBy: string | null
}
```

---

### UserType

| Method | Path |
|---|---|
| GET | /usertype |
| GET | /usertype/{id} |
| POST | /usertype |
| PUT | /usertype |
| DELETE | /usertype/{id} |

```typescript
interface UserType {
  id: string
  createdAt: string
  nameUserType: string   // note: field is nameUserType, not name
}
```

Create/Update body field is also `nameUserType`.

---

### StatusTicketbook

| Method | Path |
|---|---|
| GET | /statusticketbook |
| GET | /statusticketbook/{id} |
| POST | /statusticketbook |
| PUT | /statusticketbook |
| DELETE | /statusticketbook/{id} |

```typescript
interface StatusTicketbook {
  id: number    // integer, not UUID (1=Retirado, 2=Devolvido)
  name: string
}
```

---

### TableConfiguration (read-only metadata)

| Method | Path | Returns |
|---|---|---|
| GET | /tableconfiguration/{schema}/{tableName} | `TableConfiguration[]` or 204 |

Valid schemas: `edoha`, `lottery`

```typescript
interface TableConfiguration {
  tableSchema: string      // "edoha" or "lottery"
  tableName: string
  columnName: string
  ordinalPosition: number
  columnDefault: string | null
  isNullable: string       // "YES" or "NO"
  dataType: string         // SQL type: "uuid", "text", "integer", "boolean", etc.
}
```

---

## Error Handling Patterns

### Validation Error (400)
```typescript
interface ValidationErrorResponse {
  is_valid: false
  errors: Record<string, string[]>  // field → array of error messages
}
```

### Common HTTP status meanings in this API

| Status | Meaning |
|---|---|
| 200 | Success (most mutations) |
| 201 | Resource created (Ticketbook, Ticket) |
| 204 | Success with no content OR resource not found (context-dependent) |
| 400 | Validation error — check `errors` body |
| 401 | JWT missing, expired, or invalid — redirect to login |
| 404 | Resource not found (used inconsistently — some endpoints return 204 instead) |
| 500 | Unhandled server error |

**Important:** `204` on GET endpoints means "not found / empty result" — treat it as an empty state, not an error.

---

## Common Frontend Patterns

### Initial app load
1. Call `POST /auth` → store `accessToken` + `idUser` + `institutions`
2. Use `institutions[0].id` (or let user pick) as the `idInstitution` for all lottery routes

### Listing ticketbooks for a lottery
```
GET /lottery/{idLottery}/ticketbook          → all
GET /lottery/{idLottery}/ticketbook/withdrawns → status 1 only
GET /lottery/{idLottery}/ticketbook/returneds  → status 2 only
```

### Changing ticketbook status
```
PATCH /lottery/{idLottery}/ticketbook/{idTicketbook}/status/withdraw   → set status 1
PATCH /lottery/{idLottery}/ticketbook/{idTicketbook}/status/returned   → set status 2
PATCH /lottery/{idLottery}/ticketbook/{idTicketbook}/status/{anyId}    → set any status
```

### Creating a ticketbook with its tickets in one flow
1. `POST /lottery/{idLottery}/ticketbook` → get `idTicketbook`
2. `POST /ticketbook/{idTicketbook}/ticket` with array of ticket objects

### Checking user permissions before showing UI
```
GET /userpermission/{userId}
```
Returns `UserPermissionPage[]` — use `pageName` + `actions[].name` to conditionally render buttons.

---

## Field Name Gotchas

These field names differ from what you might expect — get them wrong and the request silently fails or hits a 400:

| Entity | Field | Not |
|---|---|---|
| UserType | `nameUserType` | `name` |
| UserInstitution | `createAt` | `createdAt` |
| Institution | `slugName` | `slug` |
| Ticketbook owner (create) | `ticketbookOwner.name` / `.phone` | IDs |
| Ticketbook owner (update) | `idOwner` (UUID) | name/phone |
| StatusTicketbook | `id` is `number` | not UUID |

# API Prompt — Tarimba III Attendance System

## Context

Build a small REST API for a school attendance system called **Tarimba III**
(Complexo Escolar Tarimba III, Angola). The API will be hosted at
`https://blueviolet-bear-395492.hostingersite.com/` on a shared PHP/MySQL
Hostinger hosting. The primary use case is QR-code-based student attendance:
a teacher scans a student's QR code with any smartphone, which hits the API
and registers that student as present for the current lesson.

---

## Tech Stack

- **Language**: PHP 8.1+ (shared hosting compatible — no Docker, no Node)
- **Database**: MySQL 8 (credentials will be provided)
- **No framework required** — plain PHP with PDO is fine; Laravel/Slim are
  acceptable if available
- **Response format**: JSON for API endpoints; HTML for the browser-facing
  confirmation page
- **CORS**: Allow all origins (`Access-Control-Allow-Origin: *`) so the
  desktop WinForms app can call the API later

---

## Database Schema (already created — do not recreate)

```
professores   (id, nome_completo, email, telefone, ativo, criado_em)
turmas        (id VARCHAR(6), sala, turno, classe, curso, director_turma_id)
disciplinas   (id, nome, classe_minima, classe_maxima, curso)
alunos        (id, numero_processo, nome_completo, sexo, data_nascimento,
               turma_id, classe, ativo, qr_token, criado_em)
atribuicoes_disciplinas (id, turma_id, disciplina_id, professor_id)
horarios      (id, turma_id, disciplina_id, dia_semana TINYINT, aula TINYINT)
presencas     (id, aluno_id, turma_id, disciplina_id, data DATE, aula TINYINT,
               status VARCHAR(12), observacao, registado_em, ip_origem)
```

`horarios.dia_semana`: 1=Monday … 5=Friday  
`horarios.aula`: 1-6 (period slots)  
`turmas.turno`: `'Manha'` or `'Tarde'`  
Period start times — Manhã base 07:00, Tarde base 13:00; offsets in minutes:
`[0, 55, 110, 175, 230, 285]`  (each period lasts 50 min)

---

## Endpoints to Implement

### 1. `GET /api/presenca/{qr_token}`

**The core endpoint — triggered by scanning a student QR code.**

Logic:
1. Look up the student by `qr_token` in the `alunos` table.
   - If not found → return 404 JSON `{"error": "Aluno não encontrado"}`.
   - If `ativo = 0` → return 403 JSON `{"error": "Aluno inactivo"}`.
2. Determine the **current lesson** from the current server datetime:
   - Get `dia_semana` from `DAYOFWEEK()` (MySQL) — convert so Mon=1…Fri=5.
   - Get the current time in minutes since midnight.
   - Determine `turno` from the student's turma: Manhã base=07:00 (420 min),
     Tarde base=13:00 (780 min).
   - Period offsets (minutes): `[0, 55, 110, 175, 230, 285]`, each 50 min long.
   - Match current time to a period slot (1-6). A slot is active while
     `base + offset <= now < base + offset + 50`.
   - If no active period (break, end of day, weekend) → still register
     presence but set `aula = NULL` and `status = 'Presente'`.
3. Look up the discipline for this lesson via `horarios` using
   `(turma_id, dia_semana, aula)`.
4. Insert or update `presencas`:
   - Unique key: `(aluno_id, disciplina_id, data, aula)`.
   - If a record already exists for today with the same key:
     - If `status = 'Presente'` already → return success "already marked".
     - Else update to `Presente`.
   - Set `ip_origem` to the request's remote IP.
5. Return a **friendly HTML page** (not JSON) visible on any smartphone:
   - School name and logo placeholder.
   - Large green checkmark ✓.
   - Student name, class, discipline, current date/time.
   - Message: "Presença registada com sucesso!"
   - If already marked: show the original registration time and message
     "Presença já registada anteriormente."
   - Minimal mobile-first CSS (inline or `<style>`); no external deps.

---

### 2. `GET /api/alunos`

Return all active students as JSON array.

```json
[
  {
    "id": 1,
    "numero_processo": "TP2026001",
    "nome_completo": "Ana Beatriz Silva Cardoso",
    "sexo": "F",
    "data_nascimento": "2016-03-15",
    "turma_id": "S01M",
    "classe": "1ª Classe",
    "qr_token": "TP2026001"
  }
]
```

Optional query params: `?turma_id=S01M`, `?classe=7`.

---

### 3. `GET /api/turmas`

Return all turmas with director name.

```json
[
  {
    "id": "S01M",
    "sala": 1,
    "turno": "Manha",
    "classe": 1,
    "curso": null,
    "director_nome": "Maria José Santos"
  }
]
```

---

### 4. `GET /api/presencas`

Return attendance records. Supports query params:
- `?turma_id=S01M`
- `?aluno_id=5`
- `?data=2026-05-14`
- `?disciplina_id=1`

Returns:
```json
[
  {
    "id": 1,
    "aluno_id": 1,
    "nome_aluno": "Ana Beatriz Silva Cardoso",
    "turma_id": "S01M",
    "disciplina_id": 1,
    "nome_disciplina": "Língua Portuguesa",
    "data": "2026-05-14",
    "aula": 2,
    "status": "Presente",
    "observacao": null,
    "registado_em": "2026-05-14 08:00:00"
  }
]
```

---

### 5. `POST /api/presencas`

Manual attendance registration (for teacher use from the desktop app).
Body (JSON):

```json
{
  "aluno_id": 5,
  "turma_id": "S01M",
  "disciplina_id": 1,
  "data": "2026-05-14",
  "aula": 2,
  "status": "Falta",
  "observacao": "Faltou sem aviso"
}
```

- `status` must be one of: `Presente`, `Falta`, `Justificada`.
- Upsert on `(aluno_id, disciplina_id, data, aula)`.
- Returns `{"success": true, "id": 123}`.

---

### 6. `GET /api/dashboard`

Summary stats for the admin dashboard.

```json
{
  "total_alunos": 56,
  "total_turmas": 24,
  "total_professores": 10,
  "presencas_hoje": 40,
  "faltas_hoje": 8,
  "faltas_criticas": 3
}
```

"Faltas críticas" = students where `(falta_count / total_count) > 0.25`.

---

### 7. `GET /api/horario/{turma_id}`

Return the weekly timetable for a turma.

```json
[
  {
    "dia_semana": 1,
    "aula": 1,
    "disciplina_id": 1,
    "disciplina_nome": "Língua Portuguesa",
    "professor_nome": "Maria José Santos",
    "hora_inicio": "07:00",
    "hora_fim": "07:50"
  }
]
```

---

## Security & Validation

- **No authentication required** for the QR scan endpoint (`GET /api/presenca/{token}`)
  — it must work from any smartphone without login.
- **API key** (simple header `X-API-Key: <secret>`) for all other endpoints.
  The key is defined in a `config.php` file, not committed to the repo.
- Validate all inputs; use PDO prepared statements exclusively — no string
  interpolation in SQL.
- Rate-limit the QR scan endpoint: max 5 requests per `(qr_token, ip)` per
  minute to prevent abuse (implement with a simple `rate_limits` table or
  in-memory file lock).

---

## File Structure (suggested)

```
public_html/
  api/
    presenca/
      index.php        ← handles GET /api/presenca/{token}
    alunos/
      index.php
    turmas/
      index.php
    presencas/
      index.php
    dashboard/
      index.php
    horario/
      index.php
  config.php           ← DB credentials + API key (not in repo)
  db.php               ← PDO singleton
  .htaccess            ← URL rewriting
```

`.htaccess` for URL rewriting (Apache):

```apache
RewriteEngine On
RewriteCond %{REQUEST_FILENAME} !-f
RewriteCond %{REQUEST_FILENAME} !-d
RewriteRule ^api/presenca/([A-Za-z0-9]+)$ api/presenca/index.php?token=$1 [QSA,L]
RewriteRule ^api/(.*)$ api/$1/index.php [QSA,L]
```

---

## Expected Behaviour — QR Scan Flow

1. Student QR code encodes:
   `https://blueviolet-bear-395492.hostingersite.com/api/presenca/TP2026001`

2. Teacher/supervisor opens phone camera → scans QR → browser opens the URL.

3. API:
   - Identifies student `TP2026001` = Ana Beatriz Silva Cardoso, S01M, 1ª Classe.
   - Current time 08:02 → Manhã, slot 2 (08:00–08:50), discipline from horario.
   - Inserts `presencas` record.
   - Returns HTML confirmation page.

4. Phone shows: green checkmark + "Ana Beatriz Silva Cardoso — Presente — Língua Portuguesa — 14/05/2026 08:02"

---

## Deliverables

1. All PHP source files ready to upload via FTP to Hostinger `public_html/`.
2. A `config.example.php` with placeholder credentials.
3. A brief `README.md` explaining setup (upload files, import SQL, set config).
4. The API should be fully functional with the database already populated by
   the provided `tarimba3.sql` seed file.

using Microsoft.Data.Sqlite;
using TarimbaPresence.Models;

namespace TarimbaPresence.Database;

public class DatabaseService
{
    private SqliteConnection Conn() => DatabaseContext.CriarConexao();

    // ══════════════════════════════════════════════════════════════════════
    // PROFESSORES
    // ══════════════════════════════════════════════════════════════════════

    public List<Professor> ObterTodosProfessores()
    {
        var lista = new List<Professor>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(
            "SELECT id,nome_completo,email,telefone,ativo FROM professores", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            lista.Add(new Professor
            {
                Id           = r.GetInt32(0),
                NomeCompleto = r.GetString(1),
                Email        = r.GetString(2),
                Telefone     = r.GetString(3),
                Ativo        = r.GetInt32(4) == 1
            });
        return lista;
    }

    public Professor? ObterProfessorPorId(int id)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(
            "SELECT id,nome_completo,email,telefone,ativo FROM professores WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new Professor
        {
            Id           = r.GetInt32(0),
            NomeCompleto = r.GetString(1),
            Email        = r.GetString(2),
            Telefone     = r.GetString(3),
            Ativo        = r.GetInt32(4) == 1
        };
    }

    public int CriarProfessor(Professor p)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            INSERT INTO professores (nome_completo, email, telefone, ativo)
            VALUES (@nome, @email, @tel, 1);
            SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@nome",  p.NomeCompleto);
        cmd.Parameters.AddWithValue("@email", p.Email);
        cmd.Parameters.AddWithValue("@tel",   p.Telefone);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void AtualizarProfessor(Professor p)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            UPDATE professores
            SET nome_completo=@nome, email=@email, telefone=@tel, ativo=@ativo
            WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@nome",  p.NomeCompleto);
        cmd.Parameters.AddWithValue("@email", p.Email);
        cmd.Parameters.AddWithValue("@tel",   p.Telefone);
        cmd.Parameters.AddWithValue("@ativo", p.Ativo ? 1 : 0);
        cmd.Parameters.AddWithValue("@id",    p.Id);
        cmd.ExecuteNonQuery();
    }

    // ══════════════════════════════════════════════════════════════════════
    // CONTAS DE PROFESSOR
    // ══════════════════════════════════════════════════════════════════════

    public ContaProfessor? ObterContaPorEmail(string email)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,professor_id,email,password_hash,ativo,primeiro_login
            FROM contas_professor WHERE email=@email", conn);
        cmd.Parameters.AddWithValue("@email", email);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new ContaProfessor
        {
            Id            = r.GetInt32(0),
            ProfessorId   = r.GetInt32(1),
            Email         = r.GetString(2),
            PasswordHash  = r.GetString(3),
            Ativo         = r.GetInt32(4) == 1,
            PrimeiroLogin = r.GetInt32(5) == 1
        };
    }

    public int CriarContaProfessor(ContaProfessor conta)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            INSERT INTO contas_professor
                (professor_id, email, password_hash, ativo, primeiro_login)
            VALUES (@profId, @email, @hash, 1, 1);
            SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@profId", conta.ProfessorId);
        cmd.Parameters.AddWithValue("@email",  conta.Email);
        cmd.Parameters.AddWithValue("@hash",   conta.PasswordHash);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void AtualizarSenha(int contaId, string novaHash)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            UPDATE contas_professor
            SET password_hash=@hash, primeiro_login=0
            WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@hash", novaHash);
        cmd.Parameters.AddWithValue("@id",   contaId);
        cmd.ExecuteNonQuery();
    }

    // ══════════════════════════════════════════════════════════════════════
    // ALUNOS
    // ══════════════════════════════════════════════════════════════════════

    public List<Aluno> ObterTodosAlunos()
    {
        var lista = new List<Aluno>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,numero_processo,nome_completo,sexo,
                   data_nascimento,turma_id,classe,ativo
            FROM alunos ORDER BY nome_completo", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            lista.Add(MapAluno(r));
        return lista;
    }

    public List<Aluno> ObterAlunosDaTurma(string turmaId)
    {
        var lista = new List<Aluno>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,numero_processo,nome_completo,sexo,
                   data_nascimento,turma_id,classe,ativo
            FROM alunos WHERE turma_id=@turma AND ativo=1
            ORDER BY nome_completo", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            lista.Add(MapAluno(r));
        return lista;
    }

    public int CriarAluno(Aluno a)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            INSERT INTO alunos
                (numero_processo,nome_completo,sexo,data_nascimento,turma_id,classe,ativo)
            VALUES (@proc,@nome,@sexo,@nasc,@turma,@classe,1);
            SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@proc",   a.NumeroProcesso);
        cmd.Parameters.AddWithValue("@nome",   a.NomeCompleto);
        cmd.Parameters.AddWithValue("@sexo",   a.Sexo);
        cmd.Parameters.AddWithValue("@nasc",   a.DataNascimento.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@turma",  a.TurmaId);
        cmd.Parameters.AddWithValue("@classe", a.Classe);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ══════════════════════════════════════════════════════════════════════
    // PRESENÇAS
    // ══════════════════════════════════════════════════════════════════════

    public List<Presenca> ObterPresencas(string? turmaId = null,
                                          DateTime? de = null,
                                          DateTime? ate = null)
    {
        var lista = new List<Presenca>();
        using var conn = Conn(); conn.Open();

        var where = new List<string>();
        if (turmaId != null) where.Add("turma_id=@turma");
        if (de      != null) where.Add("data>=@de");
        if (ate     != null) where.Add("data<=@ate");

        string filtro = where.Count > 0
            ? "WHERE " + string.Join(" AND ", where)
            : "";

        using var cmd = new SqliteCommand(
            $"SELECT id,aluno_id,turma_id,disciplina_id,data,status,observacao " +
            $"FROM presencas {filtro} ORDER BY data DESC", conn);

        if (turmaId != null) cmd.Parameters.AddWithValue("@turma", turmaId);
        if (de      != null) cmd.Parameters.AddWithValue("@de",    de.Value.ToString("yyyy-MM-dd"));
        if (ate     != null) cmd.Parameters.AddWithValue("@ate",   ate.Value.ToString("yyyy-MM-dd"));

        using var r = cmd.ExecuteReader();
        while (r.Read())
            lista.Add(new Presenca
            {
                Id           = r.GetInt32(0),
                AlunoId      = r.GetInt32(1),
                TurmaId      = r.GetString(2),
                DisciplinaId = r.GetInt32(3),
                Data         = DateTime.Parse(r.GetString(4)),
                Status       = r.GetString(5) switch
                {
                    "Falta"       => StatusPresenca.Falta,
                    "Justificada" => StatusPresenca.Justificada,
                    _             => StatusPresenca.Presente
                },
                Observacao = r.IsDBNull(6) ? null : r.GetString(6)
            });
        return lista;
    }

    public void GuardarPresencas(List<Presenca> presencas)
    {
        using var conn = Conn(); conn.Open();
        using var tx = conn.BeginTransaction();

        foreach (var p in presencas)
        {
            // Apagar o registo anterior do mesmo dia/turma/disciplina/aluno
            using var del = new SqliteCommand(@"
                DELETE FROM presencas
                WHERE aluno_id=@a AND turma_id=@t
                  AND disciplina_id=@d AND data=@data", conn, tx);
            del.Parameters.AddWithValue("@a",    p.AlunoId);
            del.Parameters.AddWithValue("@t",    p.TurmaId);
            del.Parameters.AddWithValue("@d",    p.DisciplinaId);
            del.Parameters.AddWithValue("@data", p.Data.ToString("yyyy-MM-dd"));
            del.ExecuteNonQuery();

            // Inserir o novo registo
            using var ins = new SqliteCommand(@"
                INSERT INTO presencas
                    (aluno_id,turma_id,disciplina_id,data,status,observacao)
                VALUES (@a,@t,@d,@data,@status,@obs)", conn, tx);
            ins.Parameters.AddWithValue("@a",      p.AlunoId);
            ins.Parameters.AddWithValue("@t",      p.TurmaId);
            ins.Parameters.AddWithValue("@d",      p.DisciplinaId);
            ins.Parameters.AddWithValue("@data",   p.Data.ToString("yyyy-MM-dd"));
            ins.Parameters.AddWithValue("@status", p.Status.ToString());
            ins.Parameters.AddWithValue("@obs",
                p.Observacao != null ? p.Observacao : DBNull.Value);
            ins.ExecuteNonQuery();
        }

        tx.Commit();
    }

    // ══════════════════════════════════════════════════════════════════════
    // ESTATÍSTICAS
    // ══════════════════════════════════════════════════════════════════════

    public int ContarAlunos()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(
            "SELECT COUNT(*) FROM alunos WHERE ativo=1", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int ContarProfessores()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(
            "SELECT COUNT(*) FROM professores WHERE ativo=1", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int ContarTurmas()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(
            "SELECT COUNT(*) FROM turmas", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int ContarPresencasHoje()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT COUNT(DISTINCT aluno_id) FROM presencas
            WHERE data=@hoje AND status='Presente'", conn);
        cmd.Parameters.AddWithValue("@hoje", DateTime.Today.ToString("yyyy-MM-dd"));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int ContarFaltasHoje()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT COUNT(DISTINCT aluno_id) FROM presencas
            WHERE data=@hoje AND status='Falta'", conn);
        cmd.Parameters.AddWithValue("@hoje", DateTime.Today.ToString("yyyy-MM-dd"));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public Turma? ObterTurma(string id)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,sala,turno,classe,curso,director_turma_id
            FROM turmas WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapTurma(r) : null;
    }

    public List<Turma> ObterTodasTurmas()
    {
        var lista = new List<Turma>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,sala,turno,classe,curso,director_turma_id
            FROM turmas ORDER BY id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(MapTurma(r));
        return lista;
    }

    public List<Disciplina> ObterTodasDisciplinas()
    {
        var lista = new List<Disciplina>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,nome,classe_minima,classe_maxima,curso
            FROM disciplinas ORDER BY nome", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(MapDisciplina(r));
        return lista;
    }

    public Disciplina? ObterDisciplinaPorId(int id)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,nome,classe_minima,classe_maxima,curso
            FROM disciplinas WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapDisciplina(r) : null;
    }

    public List<Disciplina> ObterDisciplinasDaTurma(Turma turma)
    {
        var lista = new List<Disciplina>();
        using var conn = Conn(); conn.Open();
        string query = @"
            SELECT id,nome,classe_minima,classe_maxima,curso
            FROM disciplinas
            WHERE classe_minima <= @classe
              AND classe_maxima >= @classe
              AND (curso IS NULL" + (turma.Curso.HasValue ? " OR curso=@curso" : "") + @")
            ORDER BY nome";
        using var cmd = new SqliteCommand(query, conn);
        cmd.Parameters.AddWithValue("@classe", turma.Classe);
        if (turma.Curso.HasValue)
            cmd.Parameters.AddWithValue("@curso", turma.Curso.Value.Abreviatura());
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(MapDisciplina(r));
        return lista;
    }

    public Professor? ObterProfessorDaDisciplina(string turmaId, int disciplinaId)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT p.id,p.nome_completo,p.email,p.telefone,p.ativo
            FROM professores p
            INNER JOIN atribuicoes_disciplinas a ON a.professor_id = p.id
            WHERE a.turma_id=@turma AND a.disciplina_id=@disc
            LIMIT 1", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        cmd.Parameters.AddWithValue("@disc", disciplinaId);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new Professor
        {
            Id           = r.GetInt32(0),
            NomeCompleto = r.GetString(1),
            Email        = r.GetString(2),
            Telefone     = r.GetString(3),
            Ativo        = r.GetInt32(4) == 1
        };
    }

    public List<AtribuicaoDisciplina> ObterAtribuicoesPorProfessor(int professorId)
    {
        var lista = new List<AtribuicaoDisciplina>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,turma_id,disciplina_id,professor_id
            FROM atribuicoes_disciplinas
            WHERE professor_id=@prof
            ORDER BY id", conn);
        cmd.Parameters.AddWithValue("@prof", professorId);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(MapAtribuicao(r));
        return lista;
    }

    public List<Turma> ObterTurmasDoProfessor(int professorId)
    {
        var lista = new List<Turma>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT DISTINCT t.id,t.sala,t.turno,t.classe,t.curso,t.director_turma_id
            FROM turmas t
            INNER JOIN atribuicoes_disciplinas a ON a.turma_id = t.id
            WHERE a.professor_id=@prof
            ORDER BY t.id", conn);
        cmd.Parameters.AddWithValue("@prof", professorId);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(MapTurma(r));
        return lista;
    }

    public AtribuicaoDisciplina? ObterAtribuicaoPorTurmaDisciplina(string turmaId, int disciplinaId)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,turma_id,disciplina_id,professor_id
            FROM atribuicoes_disciplinas
            WHERE turma_id=@turma AND disciplina_id=@disc", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        cmd.Parameters.AddWithValue("@disc", disciplinaId);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapAtribuicao(r) : null;
    }

    public int CriarAtribuicao(AtribuicaoDisciplina atrib)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            INSERT INTO atribuicoes_disciplinas (turma_id, disciplina_id, professor_id)
            VALUES (@turma, @disc, @prof);
            SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@turma", atrib.TurmaId);
        cmd.Parameters.AddWithValue("@disc", atrib.DisciplinaId);
        cmd.Parameters.AddWithValue("@prof", atrib.ProfessorId);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void RemoverAtribuicaoPorId(int id)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            DELETE FROM atribuicoes_disciplinas WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void RemoverAtribuicaoPorTurmaDisciplina(string turmaId, int disciplinaId)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            DELETE FROM atribuicoes_disciplinas
            WHERE turma_id=@turma AND disciplina_id=@disc", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        cmd.Parameters.AddWithValue("@disc", disciplinaId);
        cmd.ExecuteNonQuery();
    }

    public Horario? ObterHorario(string turmaId, int dia, int aula)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,turma_id,disciplina_id,dia_semana,aula
            FROM horarios
            WHERE turma_id=@turma AND dia_semana=@dia AND aula=@aula", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        cmd.Parameters.AddWithValue("@dia", dia);
        cmd.Parameters.AddWithValue("@aula", aula);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapHorario(r) : null;
    }

    public List<Horario> ObterHorariosDaTurma(string turmaId)
    {
        var lista = new List<Horario>();
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,turma_id,disciplina_id,dia_semana,aula
            FROM horarios WHERE turma_id=@turma ORDER BY dia_semana,aula", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        using var r = cmd.ExecuteReader();
        while (r.Read()) lista.Add(MapHorario(r));
        return lista;
    }

    public int CriarHorario(Horario horario)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            INSERT INTO horarios (turma_id, disciplina_id, dia_semana, aula)
            VALUES (@turma, @disc, @dia, @aula);
            SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@turma", horario.TurmaId);
        cmd.Parameters.AddWithValue("@disc", horario.DisciplinaId);
        cmd.Parameters.AddWithValue("@dia", horario.DiaDaSemana);
        cmd.Parameters.AddWithValue("@aula", horario.Aula);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void RemoverHorario(string turmaId, int dia, int aula)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            DELETE FROM horarios
            WHERE turma_id=@turma AND dia_semana=@dia AND aula=@aula", conn);
        cmd.Parameters.AddWithValue("@turma", turmaId);
        cmd.Parameters.AddWithValue("@dia", dia);
        cmd.Parameters.AddWithValue("@aula", aula);
        cmd.ExecuteNonQuery();
    }

    public void AtualizarAluno(Aluno a)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            UPDATE alunos
            SET numero_processo=@proc,
                nome_completo=@nome,
                sexo=@sexo,
                data_nascimento=@nasc,
                turma_id=@turma,
                classe=@classe,
                ativo=@ativo
            WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("@proc", a.NumeroProcesso);
        cmd.Parameters.AddWithValue("@nome", a.NomeCompleto);
        cmd.Parameters.AddWithValue("@sexo", a.Sexo);
        cmd.Parameters.AddWithValue("@nasc", a.DataNascimento.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@turma", a.TurmaId);
        cmd.Parameters.AddWithValue("@classe", a.Classe);
        cmd.Parameters.AddWithValue("@ativo", a.Ativo ? 1 : 0);
        cmd.Parameters.AddWithValue("@id", a.Id);
        cmd.ExecuteNonQuery();
    }

    public Aluno? ObterAlunoPorNumeroProcesso(string processo)
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT id,numero_processo,nome_completo,sexo,data_nascimento,turma_id,classe,ativo
            FROM alunos WHERE numero_processo=@proc", conn);
        cmd.Parameters.AddWithValue("@proc", processo);
        using var r = cmd.ExecuteReader();
        return r.Read() ? MapAluno(r) : null;
    }

    public int ContarFaltasCriticas()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT COUNT(*) FROM (
                SELECT aluno_id,
                       SUM(CASE WHEN status = 'Falta' THEN 1 ELSE 0 END) AS faltas,
                       COUNT(*) AS total
                FROM presencas
                GROUP BY aluno_id
                HAVING faltas * 100.0 / total > 25
            )", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int ContarDiasComChamada()
    {
        using var conn = Conn(); conn.Open();
        using var cmd = new SqliteCommand(@"
            SELECT COUNT(DISTINCT data)
            FROM presencas", conn);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ══════════════════════════════════════════════════════════════════════
    // HELPER PRIVADO
    // ══════════════════════════════════════════════════════════════════════

    private static Aluno MapAluno(SqliteDataReader r) => new()
    {
        Id              = r.GetInt32(0),
        NumeroProcesso  = r.GetString(1),
        NomeCompleto    = r.GetString(2),
        Sexo            = r.GetString(3),
        DataNascimento  = DateTime.Parse(r.GetString(4)),
        TurmaId         = r.GetString(5),
        Classe          = r.GetString(6),
        Ativo           = r.GetInt32(7) == 1
    };

    private static Turma MapTurma(SqliteDataReader r) => new()
    {
        Id                = r.GetString(0),
        Sala              = r.GetInt32(1),
        Turno             = ParseTurno(r.GetString(2)),
        Classe            = r.GetInt32(3),
        Curso             = r.IsDBNull(4) ? null : ParseCurso(r.GetString(4)),
        DirectorDeTurmaId = r.IsDBNull(5) ? null : r.GetInt32(5)
    };

    private static Disciplina MapDisciplina(SqliteDataReader r) => new()
    {
        Id           = r.GetInt32(0),
        Nome         = r.GetString(1),
        ClasseMinima = r.GetInt32(2),
        ClasseMaxima = r.GetInt32(3),
        Curso        = r.IsDBNull(4) ? null : ParseCurso(r.GetString(4))
    };

    private static AtribuicaoDisciplina MapAtribuicao(SqliteDataReader r) => new()
    {
        Id           = r.GetInt32(0),
        TurmaId      = r.GetString(1),
        DisciplinaId = r.GetInt32(2),
        ProfessorId  = r.GetInt32(3)
    };

    private static Horario MapHorario(SqliteDataReader r) => new()
    {
        Id           = r.GetInt32(0),
        TurmaId      = r.GetString(1),
        DisciplinaId = r.GetInt32(2),
        DiaDaSemana  = r.GetInt32(3),
        Aula         = r.GetInt32(4)
    };

    private static Turno ParseTurno(string turno) => turno switch
    {
        "Manha" => Turno.Manha,
        "Tarde" => Turno.Tarde,
        _        => Turno.Manha
    };

    private static Curso? ParseCurso(string curso) => curso switch
    {
        "CG"  => Curso.ContabilidadeEGestao,
        "IG"  => Curso.InformaticaDeGestao,
        "CEJ" => Curso.CienciasEconomicasEJuridicas,
        "CFB" => Curso.CienciasFisicasEBiologicas,
        _      => null
    };

    public List<Presenca> ObterPresencasPorAluno(int alunoId)
    {
        var lista = new List<Presenca>();
        using var conn = Conn(); // ← usa o nome correto do teu projeto
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, aluno_id, turma_id, disciplina_id, data, status, observacao
            FROM presencas
            WHERE aluno_id = @id
            ORDER BY data DESC";
        cmd.Parameters.AddWithValue("@id", alunoId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Presenca
            {
                Id           = reader.GetInt32(0),
                AlunoId      = reader.GetInt32(1),
                TurmaId      = reader.GetString(2),
                DisciplinaId = reader.GetInt32(3),
                Data         = DateTime.Parse(reader.GetString(4)),
                Status       = reader.GetString(5) switch
                {
                    "Falta"       => StatusPresenca.Falta,
                    "Justificada" => StatusPresenca.Justificada,
                    _             => StatusPresenca.Presente
                },
                Observacao = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return lista;
    }
}

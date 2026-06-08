using Microsoft.Data.Sqlite;

namespace TarimbaPresence.Database;

public static class DatabaseContext
{
    // O ficheiro .db fica na mesma pasta do programa
    private static readonly string DbPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "tarimba3.db");

    public static SqliteConnection CriarConexao()
    {
        return new SqliteConnection($"Data Source={DbPath}");
    }

    // Chama este método uma vez quando o programa arranca
    public static void InicializarBancoDeDados()
    {
        using var conn = CriarConexao();
        conn.Open();

        CriarTabelas(conn);
        SeedDadosIniciais(conn);
    }

    // ── Criar todas as tabelas ─────────────────────────────────────────────
    private static void CriarTabelas(SqliteConnection conn)
    {
        var sql = @"
        CREATE TABLE IF NOT EXISTS professores (
            id            INTEGER PRIMARY KEY AUTOINCREMENT,
            nome_completo TEXT    NOT NULL,
            email         TEXT    NOT NULL UNIQUE,
            telefone      TEXT    NOT NULL DEFAULT '',
            ativo         INTEGER NOT NULL DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS contas_professor (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            professor_id    INTEGER NOT NULL,
            email           TEXT    NOT NULL UNIQUE,
            password_hash   TEXT    NOT NULL,
            ativo           INTEGER NOT NULL DEFAULT 1,
            primeiro_login  INTEGER NOT NULL DEFAULT 1,
            FOREIGN KEY (professor_id) REFERENCES professores(id)
        );

        CREATE TABLE IF NOT EXISTS turmas (
            id                  TEXT PRIMARY KEY,
            sala                INTEGER NOT NULL,
            turno               TEXT    NOT NULL,
            classe              INTEGER NOT NULL,
            curso               TEXT    NULL,
            director_turma_id   INTEGER NULL,
            FOREIGN KEY (director_turma_id) REFERENCES professores(id)
        );

        CREATE TABLE IF NOT EXISTS disciplinas (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            nome            TEXT    NOT NULL,
            classe_minima   INTEGER NOT NULL DEFAULT 1,
            classe_maxima   INTEGER NOT NULL DEFAULT 13,
            curso           TEXT    NULL
        );

        CREATE TABLE IF NOT EXISTS alunos (
            id               INTEGER PRIMARY KEY AUTOINCREMENT,
            numero_processo  TEXT    NOT NULL UNIQUE,
            nome_completo    TEXT    NOT NULL,
            sexo             TEXT    NOT NULL,
            data_nascimento  TEXT    NOT NULL,
            turma_id         TEXT    NOT NULL,
            classe           TEXT    NOT NULL,
            ativo            INTEGER NOT NULL DEFAULT 1,
            FOREIGN KEY (turma_id) REFERENCES turmas(id)
        );

        CREATE TABLE IF NOT EXISTS atribuicoes_disciplinas (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            turma_id        TEXT    NOT NULL,
            disciplina_id   INTEGER NOT NULL,
            professor_id    INTEGER NOT NULL,
            FOREIGN KEY (turma_id)      REFERENCES turmas(id),
            FOREIGN KEY (disciplina_id) REFERENCES disciplinas(id),
            FOREIGN KEY (professor_id)  REFERENCES professores(id)
        );

        CREATE TABLE IF NOT EXISTS horarios (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            turma_id        TEXT    NOT NULL,
            disciplina_id   INTEGER NOT NULL,
            dia_semana      INTEGER NOT NULL,
            aula            INTEGER NOT NULL,
            FOREIGN KEY (turma_id)      REFERENCES turmas(id),
            FOREIGN KEY (disciplina_id) REFERENCES disciplinas(id)
        );

        CREATE TABLE IF NOT EXISTS presencas (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            aluno_id        INTEGER NOT NULL,
            turma_id        TEXT    NOT NULL,
            disciplina_id   INTEGER NOT NULL,
            data            TEXT    NOT NULL,
            status          TEXT    NOT NULL DEFAULT 'Presente',
            observacao      TEXT    NULL,
            FOREIGN KEY (aluno_id)      REFERENCES alunos(id),
            FOREIGN KEY (turma_id)      REFERENCES turmas(id),
            FOREIGN KEY (disciplina_id) REFERENCES disciplinas(id)
        );
        ";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    // ── Inserir dados iniciais só se as tabelas estiverem vazias ───────────
    private static void SeedDadosIniciais(SqliteConnection conn)
    {
        // Verificar se já há dados
        using var check = new SqliteCommand(
            "SELECT COUNT(*) FROM professores", conn);
        long count = (long)(check.ExecuteScalar() ?? 0L);
        if (count > 0) return; // já tem dados, não repetir

        SeedProfessores(conn);
        SeedContasProfessor(conn);
        SeedTurmas(conn);
        SeedDisciplinas(conn);
        SeedAlunos(conn);
        SeedAtribuicoes(conn);
        SeedHorarios(conn);
    }

    private static void Exec(SqliteConnection conn, string sql)
    {
        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    // ── Professores ────────────────────────────────────────────────────────
    private static void SeedProfessores(SqliteConnection conn)
    {
        var dados = new[]
        {
            (1,  "Maria José Santos",       "mjsantos@tarimba3.ao",    "+244 923 001 001"),
            (2,  "Carlos Alberto Silva",    "casilva@tarimba3.ao",     "+244 923 002 002"),
            (3,  "Ana Paula Rodrigues",     "aprodrigues@tarimba3.ao", "+244 923 003 003"),
            (4,  "João Miguel Pereira",     "jmpereira@tarimba3.ao",   "+244 923 004 004"),
            (5,  "Fernanda Costa Lima",     "fclima@tarimba3.ao",      "+244 923 005 005"),
            (6,  "Pedro Henrique Alves",    "phalves@tarimba3.ao",     "+244 923 006 006"),
            (7,  "Sofia Isabel Nunes",      "sinunes@tarimba3.ao",     "+244 923 007 007"),
            (8,  "Manuel Augusto Correia",  "macorreia@tarimba3.ao",   "+244 923 008 008"),
            (9,  "Rosa Maria Tavares",      "rmtavares@tarimba3.ao",   "+244 923 009 009"),
            (10, "António Manuel Ferreira", "amferreira@tarimba3.ao",  "+244 923 010 010"),
        };

        foreach (var (id, nome, email, tel) in dados)
            Exec(conn, $"INSERT INTO professores (id,nome_completo,email,telefone) " +
                       $"VALUES ({id},'{nome}','{email}','{tel}')");
    }

    // ── Contas de professor (para login) ───────────────────────────────────
    private static void SeedContasProfessor(SqliteConnection conn)
    {
        // Uma conta por professor — senha inicial igual ao primeiro nome em minúsculas
        var contas = new[]
        {
            (1,  1,  "mjsantos@tarimba3.ao",    "maria1234"),
            (2,  2,  "casilva@tarimba3.ao",     "carlos1234"),
            (3,  3,  "aprodrigues@tarimba3.ao", "ana1234"),
            (4,  4,  "jmpereira@tarimba3.ao",   "joao1234"),
            (5,  5,  "fclima@tarimba3.ao",      "fernanda1234"),
            (6,  6,  "phalves@tarimba3.ao",     "pedro1234"),
            (7,  7,  "sinunes@tarimba3.ao",     "sofia1234"),
            (8,  8,  "macorreia@tarimba3.ao",   "manuel1234"),
            (9,  9,  "rmtavares@tarimba3.ao",   "rosa1234"),
            (10, 10, "amferreira@tarimba3.ao",  "antonio1234"),
        };

        foreach (var (id, profId, email, pass) in contas)
            Exec(conn, $"INSERT INTO contas_professor " +
                       $"(id,professor_id,email,password_hash,ativo,primeiro_login) " +
                       $"VALUES ({id},{profId},'{email}','{pass}',1,0)");
    }

    // ── Turmas ─────────────────────────────────────────────────────────────
    private static void SeedTurmas(SqliteConnection conn)
    {
        var turmas = new[]
        {
            ("S01M",1,"Manha",1,"NULL","1"),("S01T",1,"Tarde",1,"NULL","NULL"),
            ("S02M",2,"Manha",2,"NULL","2"),("S02T",2,"Tarde",2,"NULL","NULL"),
            ("S03M",3,"Manha",3,"NULL","3"),("S03T",3,"Tarde",3,"NULL","NULL"),
            ("S04M",4,"Manha",4,"NULL","4"),("S04T",4,"Tarde",4,"NULL","NULL"),
            ("S05M",5,"Manha",5,"NULL","5"),("S05T",5,"Tarde",5,"NULL","NULL"),
            ("S06M",6,"Manha",6,"NULL","6"),("S06T",6,"Tarde",6,"NULL","NULL"),
            ("S07M",7,"Manha",7,"NULL","7"),("S07T",7,"Tarde",7,"NULL","NULL"),
            ("S08M",8,"Manha",8,"NULL","8"),("S08T",8,"Tarde",8,"NULL","NULL"),
            ("S09M",9,"Manha",9,"NULL","9"),("S09T",9,"Tarde",9,"NULL","NULL"),
            ("S10M",10,"Manha",10,"'CG'","10"),("S10T",10,"Tarde",10,"'IG'","NULL"),
            ("S11M",11,"Manha",11,"'CEJ'","NULL"),("S11T",11,"Tarde",11,"'CFB'","NULL"),
            ("S12M",12,"Manha",12,"'CG'","NULL"),("S12T",12,"Tarde",13,"'IG'","NULL"),
        };

        foreach (var (id,sala,turno,classe,curso,dir) in turmas)
            Exec(conn, $"INSERT INTO turmas (id,sala,turno,classe,curso,director_turma_id) " +
                       $"VALUES ('{id}',{sala},'{turno}',{classe},{curso},{dir})");
    }

    // ── Disciplinas ────────────────────────────────────────────────────────
    private static void SeedDisciplinas(SqliteConnection conn)
    {
        var disc = new[]
        {
            (1,"Língua Portuguesa",1,12,"NULL"),
            (2,"Matemática",1,12,"NULL"),
            (3,"Educação Física",1,12,"NULL"),
            (4,"Estudo do Meio",1,4,"NULL"),
            (5,"Educação Manual e Plástica",1,6,"NULL"),
            (6,"Educação Musical",1,6,"NULL"),
            (7,"Ciências da Natureza",5,6,"NULL"),
            (8,"Educação Visual e Plástica",5,9,"NULL"),
            (9,"Educação Moral e Cívica",5,9,"NULL"),
            (10,"História",5,9,"NULL"),
            (11,"Geografia",5,9,"NULL"),
            (12,"Língua Estrangeira",7,12,"NULL"),
            (13,"Física",7,9,"NULL"),
            (14,"Química",7,9,"NULL"),
            (15,"Biologia",7,9,"NULL"),
            (16,"História",10,12,"'CEJ'"),
            (17,"Geografia",10,12,"'CEJ'"),
            (18,"Introdução ao Direito",10,12,"'CEJ'"),
            (19,"Introdução à Economia",10,12,"'CEJ'"),
            (20,"Psicologia",10,12,"'CEJ'"),
            (21,"Sociologia",10,12,"'CEJ'"),
            (22,"Informática",10,12,"'CEJ'"),
            (23,"Filosofia",10,12,"'CEJ'"),
            (24,"Física",10,12,"'CFB'"),
            (25,"Química",10,12,"'CFB'"),
            (26,"Biologia",10,12,"'CFB'"),
            (27,"Geologia",10,12,"'CFB'"),
            (28,"Geometria Descritiva",10,12,"'CFB'"),
            (29,"Informática",10,12,"'CFB'"),
            (30,"Filosofia",10,12,"'CFB'"),
            (31,"Física",10,12,"'IG'"),
            (32,"Empreendedorismo",10,12,"'IG'"),
            (33,"Introdução à Programação",10,10,"'IG'"),
            (34,"Arquitectura de Computadores",10,10,"'IG'"),
            (35,"Sistemas Operativos",10,10,"'IG'"),
            (36,"Organização e Gestão de Empresas",10,11,"'IG'"),
            (37,"Contabilidade Geral",10,10,"'IG'"),
            (38,"Programação",11,11,"'IG'"),
            (39,"Redes de Computadores",11,12,"'IG'"),
            (40,"Tecnologias de Informação e Comunicação",11,11,"'IG'"),
            (41,"Sistemas de Informação / Bases de Dados",11,12,"'IG'"),
            (42,"Contabilidade Analítica",11,11,"'IG'"),
            (43,"Programação Avançada",12,12,"'IG'"),
            (44,"Projecto Tecnológico",12,13,"'IG'"),
            (45,"Práticas Oficinais / FCT",13,13,"'IG'"),
            (46,"Informática",10,12,"'CG'"),
            (47,"Empreendedorismo",10,12,"'CG'"),
            (48,"Organização e Gestão de Empresas",10,11,"'CG'"),
            (49,"Contabilidade Geral",10,10,"'CG'"),
            (50,"Direito Comercial",10,10,"'CG'"),
            (51,"Estatística",10,10,"'CG'"),
            (52,"Contabilidade Analítica",11,11,"'CG'"),
            (53,"Cálculo Financeiro",11,12,"'CG'"),
            (54,"Direito Laboral",11,11,"'CG'"),
            (55,"Fiscalidade",11,12,"'CG'"),
            (56,"Contabilidade Pública / Bancária",12,12,"'CG'"),
            (57,"Projecto Tecnológico",12,13,"'CG'"),
            (58,"Práticas Profissionais / FCT",13,13,"'CG'"),
        };

        foreach (var (id,nome,cmin,cmax,curso) in disc)
            Exec(conn, $"INSERT INTO disciplinas (id,nome,classe_minima,classe_maxima,curso) " +
                       $"VALUES ({id},'{nome.Replace("'","''")}',{cmin},{cmax},{curso})");
    }

    // ── Alunos ─────────────────────────────────────────────────────────────
    private static void SeedAlunos(SqliteConnection conn)
    {
        var alunos = new[]
        {
            (1,"TP2026001","Ana Beatriz Silva Cardoso","F","2016-03-15","S01M","1ª Classe"),
            (2,"TP2026002","Carlos Eduardo Mendes","M","2016-07-22","S01M","1ª Classe"),
            (3,"TP2026003","Fernanda Lopes Neto","F","2016-01-08","S01M","1ª Classe"),
            (4,"TP2026004","Gustavo Almeida Santos","M","2016-09-30","S01M","1ª Classe"),
            (5,"TP2026005","Helena Rodrigues Ferreira","F","2016-05-14","S01M","1ª Classe"),
            (6,"TP2026006","Igor Tavares Pereira","M","2016-11-02","S01M","1ª Classe"),
            (7,"TP2026007","Júlia Nascimento Costa","F","2016-04-19","S01M","1ª Classe"),
            (8,"TP2026008","Kevin Barbosa Lima","M","2016-08-07","S01M","1ª Classe"),
            (9,"TP2026009","Lara Moreira Cunha","F","2016-02-11","S01T","1ª Classe"),
            (10,"TP2026010","Mateus Oliveira Braga","M","2016-06-28","S01T","1ª Classe"),
            (11,"TP2026011","Natália Correia Souza","F","2015-10-01","S02M","2ª Classe"),
            (12,"TP2026012","Otávio Pinto Marques","M","2015-03-28","S02M","2ª Classe"),
            (13,"TP2026013","Patricia Moreira Dias","F","2015-07-16","S02M","2ª Classe"),
            (14,"TP2026014","Rafael Cardoso Nunes","M","2015-12-09","S02M","2ª Classe"),
            (15,"TP2026015","Sabrina Fonseca Torres","F","2015-02-20","S02M","2ª Classe"),
            (16,"TP2026016","Thiago Cunha Rezende","M","2015-05-03","S02T","2ª Classe"),
            (17,"TP2026017","Ursula Freitas Magalhães","F","2015-09-11","S02T","2ª Classe"),
            (18,"TP2026018","Vitor Hugo Campos","M","2014-02-14","S03M","3ª Classe"),
            (19,"TP2026019","Wanda Borges Esteves","F","2014-06-27","S03M","3ª Classe"),
            (20,"TP2026020","Xavier Antunes Melo","M","2014-10-05","S03M","3ª Classe"),
            (21,"TP2026021","Yasmin Carvalho Braga","F","2014-03-17","S03M","3ª Classe"),
            (22,"TP2026022","Zeno Ribeiro Monteiro","M","2014-07-29","S03T","3ª Classe"),
            (23,"TP2026023","Alice Teixeira Barreto","F","2014-11-08","S03T","3ª Classe"),
            (24,"TP2026024","Bruno Azevedo Machado","M","2013-04-21","S04M","4ª Classe"),
            (25,"TP2026025","Camila Nogueira Leite","F","2013-08-06","S04M","4ª Classe"),
            (26,"TP2026026","Daniel Araújo Peixoto","M","2013-01-30","S04M","4ª Classe"),
            (27,"TP2026027","Elisa Batista Rocha","F","2013-05-23","S04M","4ª Classe"),
            (28,"TP2026028","Felipe Cerqueira Duarte","M","2013-09-15","S04T","4ª Classe"),
            (29,"TP2026029","Gabriela Matos Figueiredo","F","2013-02-07","S04T","4ª Classe"),
            (30,"TP2026030","Henrique Vasconcelos Prado","M","2012-06-19","S05M","5ª Classe"),
            (31,"TP2026031","Isabel Queirós Vilas","F","2012-10-03","S05M","5ª Classe"),
            (32,"TP2026032","João Paulo Serra Gomes","M","2012-03-14","S05M","5ª Classe"),
            (33,"TP2026033","Karina Espírito Santo Leal","F","2012-07-25","S05T","5ª Classe"),
            (34,"TP2026034","Lucas Bernardes Falcão","M","2012-11-08","S05T","5ª Classe"),
            (35,"TP2026035","Marina Cavalcante Neves","F","2011-04-02","S06M","6ª Classe"),
            (36,"TP2026036","Nelson Augusto Faria","M","2011-08-16","S06M","6ª Classe"),
            (37,"TP2026037","Olivia Sampaio Loureiro","F","2011-12-27","S06M","6ª Classe"),
            (38,"TP2026038","Pedro Henrique Raposo","M","2011-05-10","S06T","6ª Classe"),
            (39,"TP2026039","Quintina Marcelino Andrade","F","2011-09-21","S06T","6ª Classe"),
            (40,"TP2026040","Ricardo Soares Ventura","M","2011-01-06","S06T","6ª Classe"),
            (41,"TP2026041","Amara Diogo Conceição","F","2010-03-15","S07M","7ª Classe"),
            (42,"TP2026042","Benedito Luvumbo Costa","M","2010-07-22","S07M","7ª Classe"),
            (43,"TP2026043","Cláudia Neto Xavier","F","2010-11-05","S07M","7ª Classe"),
            (44,"TP2026044","David Tiago Mendonça","M","2010-09-30","S07T","7ª Classe"),
            (45,"TP2026045","Emília Paiva Bandeira","F","2010-01-18","S07T","7ª Classe"),
            (46,"TP2026046","Francisco Pinto Ramos","M","2009-05-14","S08M","8ª Classe"),
            (47,"TP2026047","Graça Simões Leitão","F","2009-11-02","S08M","8ª Classe"),
            (48,"TP2026048","Hélio Ferreira Brito","M","2009-04-19","S08T","8ª Classe"),
            (49,"TP2026049","Inês Faria Gonçalves","F","2009-08-07","S08T","8ª Classe"),
            (50,"TP2026050","José António Nkosi","M","2008-02-11","S09M","9ª Classe"),
            (51,"TP2026051","Leonor Bettencourt Silva","F","2008-06-28","S09M","9ª Classe"),
            (52,"TP2026052","Marco Vieira Henriques","M","2008-10-01","S09T","9ª Classe"),
            (53,"TP2026053","Nádia Teixeira Barbosa","F","2007-03-28","S10M","10ª Classe"),
            (54,"TP2026054","Orlando Cunha Baptista","M","2007-07-16","S10M","10ª Classe"),
            (55,"TP2026055","Paula Rocha Moraes","F","2007-12-09","S10T","10ª Classe"),
            (56,"TP2026056","Quirino Fonseca Torres","M","2006-05-03","S11M","11ª Classe"),
        };

        foreach (var (id,proc,nome,sexo,nasc,turma,classe) in alunos)
            Exec(conn, $"INSERT INTO alunos (id,numero_processo,nome_completo,sexo,data_nascimento,turma_id,classe) " +
                       $"VALUES ({id},'{proc}','{nome.Replace("'","''")}','{sexo}','{nasc}','{turma}','{classe}')");
    }

    // ── Atribuições ────────────────────────────────────────────────────────
    private static void SeedAtribuicoes(SqliteConnection conn)
    {
        // Primário (1ª-6ª): um professor por turma trata de todas as disciplinas
        var primario = new[]
        {
            ("S01M",1),("S01T",1),("S02M",2),("S02T",2),
            ("S03M",3),("S03T",3),("S04M",4),("S04T",4),
            ("S05M",5),("S05T",5),("S06M",6),("S06T",6),
        };

        // Disciplinas por classe
        int[][] discPorClasse =
        {
            new[]{1,2,3,4,5,6},       // 1ª-4ª
            new[]{1,2,3,5,6,7,8,9,10,11}, // 5ª-6ª
        };

        int id = 1;
        foreach (var (turmaId, profId) in primario)
        {
            // Descobrir classe da turma
            int classe = int.Parse(turmaId[1..3]);
            var discs = classe <= 4 ? discPorClasse[0] : discPorClasse[1];
            foreach (var discId in discs)
                Exec(conn, $"INSERT INTO atribuicoes_disciplinas (id,turma_id,disciplina_id,professor_id) " +
                           $"VALUES ({id++},'{turmaId}',{discId},{profId})");
        }

        // Secundário: atribuições diretas do SQL original (amostra representativa)
        var secundario = new[]
        {
            ("S07M",1,1),("S07M",2,2),("S07M",3,3),("S07M",12,8),("S07M",13,9),
            ("S07T",1,2),("S07T",2,3),("S07T",3,4),("S07T",12,9),("S07T",13,10),
            ("S08M",1,3),("S08M",2,4),("S08M",3,5),("S08M",12,10),("S08M",13,1),
            ("S08T",1,4),("S08T",2,5),("S08T",3,6),("S08T",12,1), ("S08T",13,2),
            ("S09M",1,5),("S09M",2,6),("S09M",3,7),("S09M",12,2), ("S09M",13,3),
            ("S09T",1,6),("S09T",2,7),("S09T",3,8),("S09T",12,3), ("S09T",13,4),
            ("S10M",1,7),("S10M",2,8),("S10M",3,9),("S10M",46,1), ("S10M",47,2),
            ("S10T",1,8),("S10T",2,9),("S10T",3,10),("S10T",31,2),("S10T",32,3),
            ("S11M",1,9),("S11M",2,10),("S11M",3,1),("S11M",16,3),("S11M",17,4),
            ("S11T",1,10),("S11T",2,1),("S11T",3,2),("S11T",24,4),("S11T",25,5),
            ("S12M",1,1),("S12M",2,2),("S12M",3,3),("S12M",46,5), ("S12M",53,7),
            ("S12T",44,2),("S12T",45,3),
        };

        foreach (var (t,d,p) in secundario)
            Exec(conn, $"INSERT INTO atribuicoes_disciplinas (id,turma_id,disciplina_id,professor_id) " +
                       $"VALUES ({id++},'{t}',{d},{p})");
    }

    // ── Horários ───────────────────────────────────────────────────────────
    private static void SeedHorarios(SqliteConnection conn)
    {
        // Horários simplificados para as turmas principais
        var slots = new[] {
            ("S01M", new[]{1,2,3,4,5,6}),
            ("S02M", new[]{1,2,3,4,5,6}),
            ("S07M", new[]{1,2,3,8,9,10,11,12,13,14,15}),
            ("S10M", new[]{1,2,3,12,46,47,48,49,50,51}),
        };

        int id = 1;
        foreach (var (turmaId, discs) in slots)
        {
            int idx = 0;
            for (int dia = 1; dia <= 5; dia++)
                for (int aula = 1; aula <= 6; aula++)
                {
                    int discId = discs[idx % discs.Length];
                    Exec(conn, $"INSERT INTO horarios (id,turma_id,disciplina_id,dia_semana,aula) " +
                               $"VALUES ({id++},'{turmaId}',{discId},{dia},{aula})");
                    idx++;
                }
        }
    }
}
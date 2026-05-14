using TarimbaPresence.Models;

namespace TarimbaPresence.Data;

public static class MockDataStore
{
    // ── Professores ────────────────────────────────────────────────────────
    public static List<Professor> Professores { get; } = new()
    {
        new() { Id=1,  NomeCompleto="Maria José Santos",       Email="mjsantos@tarimba3.ao",    Telefone="+244 923 001 001" },
        new() { Id=2,  NomeCompleto="Carlos Alberto Silva",    Email="casilva@tarimba3.ao",     Telefone="+244 923 002 002" },
        new() { Id=3,  NomeCompleto="Ana Paula Rodrigues",     Email="aprodrigues@tarimba3.ao", Telefone="+244 923 003 003" },
        new() { Id=4,  NomeCompleto="João Miguel Pereira",     Email="jmpereira@tarimba3.ao",   Telefone="+244 923 004 004" },
        new() { Id=5,  NomeCompleto="Fernanda Costa Lima",     Email="fclima@tarimba3.ao",      Telefone="+244 923 005 005" },
        new() { Id=6,  NomeCompleto="Pedro Henrique Alves",    Email="phalves@tarimba3.ao",     Telefone="+244 923 006 006" },
        new() { Id=7,  NomeCompleto="Sofia Isabel Nunes",      Email="sinunes@tarimba3.ao",     Telefone="+244 923 007 007" },
        new() { Id=8,  NomeCompleto="Manuel Augusto Correia",  Email="macorreia@tarimba3.ao",   Telefone="+244 923 008 008" },
        new() { Id=9,  NomeCompleto="Rosa Maria Tavares",      Email="rmtavares@tarimba3.ao",   Telefone="+244 923 009 009" },
        new() { Id=10, NomeCompleto="António Manuel Ferreira", Email="amferreira@tarimba3.ao",  Telefone="+244 923 010 010" },
    };

    // ── Turmas ─────────────────────────────────────────────────────────────
    public static List<Turma> Turmas { get; } = new()
    {
        // ── Ensino Primário (1ª–6ª Classe) ────────────────────────────────
        new() { Id="S01M", Sala=1,  Turno=Turno.Manha, Classe=1,  DirectorDeTurmaId=1  },
        new() { Id="S01T", Sala=1,  Turno=Turno.Tarde, Classe=1                         },
        new() { Id="S02M", Sala=2,  Turno=Turno.Manha, Classe=2,  DirectorDeTurmaId=2  },
        new() { Id="S02T", Sala=2,  Turno=Turno.Tarde, Classe=2                         },
        new() { Id="S03M", Sala=3,  Turno=Turno.Manha, Classe=3,  DirectorDeTurmaId=3  },
        new() { Id="S03T", Sala=3,  Turno=Turno.Tarde, Classe=3                         },
        new() { Id="S04M", Sala=4,  Turno=Turno.Manha, Classe=4,  DirectorDeTurmaId=4  },
        new() { Id="S04T", Sala=4,  Turno=Turno.Tarde, Classe=4                         },
        new() { Id="S05M", Sala=5,  Turno=Turno.Manha, Classe=5,  DirectorDeTurmaId=5  },
        new() { Id="S05T", Sala=5,  Turno=Turno.Tarde, Classe=5                         },
        new() { Id="S06M", Sala=6,  Turno=Turno.Manha, Classe=6,  DirectorDeTurmaId=6  },
        new() { Id="S06T", Sala=6,  Turno=Turno.Tarde, Classe=6                         },
        // ── Ensino Secundário I (7ª–9ª Classe) ────────────────────────────
        new() { Id="S07M", Sala=7,  Turno=Turno.Manha, Classe=7,  DirectorDeTurmaId=7  },
        new() { Id="S07T", Sala=7,  Turno=Turno.Tarde, Classe=7                         },
        new() { Id="S08M", Sala=8,  Turno=Turno.Manha, Classe=8,  DirectorDeTurmaId=8  },
        new() { Id="S08T", Sala=8,  Turno=Turno.Tarde, Classe=8                         },
        new() { Id="S09M", Sala=9,  Turno=Turno.Manha, Classe=9,  DirectorDeTurmaId=9  },
        new() { Id="S09T", Sala=9,  Turno=Turno.Tarde, Classe=9                         },
        // ── Ensino Secundário II (10ª–13ª Classe, com Curso) ──────────────
        new() { Id="S10M", Sala=10, Turno=Turno.Manha, Classe=10, Curso=Curso.ContabilidadeEGestao,         DirectorDeTurmaId=10 },
        new() { Id="S10T", Sala=10, Turno=Turno.Tarde, Classe=10, Curso=Curso.InformaticaDeGestao                               },
        new() { Id="S11M", Sala=11, Turno=Turno.Manha, Classe=11, Curso=Curso.CienciasEconomicasEJuridicas                      },
        new() { Id="S11T", Sala=11, Turno=Turno.Tarde, Classe=11, Curso=Curso.CienciasFisicasEBiologicas                        },
        new() { Id="S12M", Sala=12, Turno=Turno.Manha, Classe=12, Curso=Curso.ContabilidadeEGestao                               },
        new() { Id="S12T", Sala=12, Turno=Turno.Tarde, Classe=13, Curso=Curso.InformaticaDeGestao                               },
    };

    // ── Disciplinas ────────────────────────────────────────────────────────
    public static List<Disciplina> Disciplinas { get; } = new()
    {
        // ── Transversais — sem restrição de curso ──────────────────────────
        new() { Id= 1, Nome="Língua Portuguesa",                    ClasseMinima= 1, ClasseMaxima=12 },
        new() { Id= 2, Nome="Matemática",                           ClasseMinima= 1, ClasseMaxima=12 },
        new() { Id= 3, Nome="Educação Física",                      ClasseMinima= 1, ClasseMaxima=12 },
        new() { Id= 4, Nome="Estudo do Meio",                       ClasseMinima= 1, ClasseMaxima= 4 },
        new() { Id= 5, Nome="Educação Manual e Plástica",           ClasseMinima= 1, ClasseMaxima= 6 },
        new() { Id= 6, Nome="Educação Musical",                     ClasseMinima= 1, ClasseMaxima= 6 },
        new() { Id= 7, Nome="Ciências da Natureza",                 ClasseMinima= 5, ClasseMaxima= 6 },
        new() { Id= 8, Nome="Educação Visual e Plástica",           ClasseMinima= 5, ClasseMaxima= 9 },
        new() { Id= 9, Nome="Educação Moral e Cívica",              ClasseMinima= 5, ClasseMaxima= 9 },
        new() { Id=10, Nome="História",                             ClasseMinima= 5, ClasseMaxima= 9 },
        new() { Id=11, Nome="Geografia",                            ClasseMinima= 5, ClasseMaxima= 9 },
        new() { Id=12, Nome="Língua Estrangeira",                   ClasseMinima= 7, ClasseMaxima=12 },
        new() { Id=13, Nome="Física",                               ClasseMinima= 7, ClasseMaxima= 9 },
        new() { Id=14, Nome="Química",                              ClasseMinima= 7, ClasseMaxima= 9 },
        new() { Id=15, Nome="Biologia",                             ClasseMinima= 7, ClasseMaxima= 9 },
        // ── Ciências Económicas e Jurídicas — CEJ (10ª–12ª) ──────────────
        new() { Id=16, Nome="História",                             ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=17, Nome="Geografia",                            ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=18, Nome="Introdução ao Direito",                ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=19, Nome="Introdução à Economia",                ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=20, Nome="Psicologia",                           ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=21, Nome="Sociologia",                           ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=22, Nome="Informática",                          ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        new() { Id=23, Nome="Filosofia",                            ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasEconomicasEJuridicas },
        // ── Ciências Físicas e Biológicas — CFB (10ª–12ª) ─────────────────
        new() { Id=24, Nome="Física",                               ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        new() { Id=25, Nome="Química",                              ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        new() { Id=26, Nome="Biologia",                             ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        new() { Id=27, Nome="Geologia",                             ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        new() { Id=28, Nome="Geometria Descritiva",                 ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        new() { Id=29, Nome="Informática",                          ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        new() { Id=30, Nome="Filosofia",                            ClasseMinima=10, ClasseMaxima=12, Curso=Curso.CienciasFisicasEBiologicas },
        // ── Informática de Gestão — IG (10ª–13ª) ─────────────────────────
        new() { Id=31, Nome="Física",                               ClasseMinima=10, ClasseMaxima=12, Curso=Curso.InformaticaDeGestao },
        new() { Id=32, Nome="Empreendedorismo",                     ClasseMinima=10, ClasseMaxima=12, Curso=Curso.InformaticaDeGestao },
        new() { Id=33, Nome="Introdução à Programação",             ClasseMinima=10, ClasseMaxima=10, Curso=Curso.InformaticaDeGestao },
        new() { Id=34, Nome="Arquitectura de Computadores",         ClasseMinima=10, ClasseMaxima=10, Curso=Curso.InformaticaDeGestao },
        new() { Id=35, Nome="Sistemas Operativos",                  ClasseMinima=10, ClasseMaxima=10, Curso=Curso.InformaticaDeGestao },
        new() { Id=36, Nome="Organização e Gestão de Empresas",     ClasseMinima=10, ClasseMaxima=11, Curso=Curso.InformaticaDeGestao },
        new() { Id=37, Nome="Contabilidade Geral",                  ClasseMinima=10, ClasseMaxima=10, Curso=Curso.InformaticaDeGestao },
        new() { Id=38, Nome="Programação",                          ClasseMinima=11, ClasseMaxima=11, Curso=Curso.InformaticaDeGestao },
        new() { Id=39, Nome="Redes de Computadores",                ClasseMinima=11, ClasseMaxima=12, Curso=Curso.InformaticaDeGestao },
        new() { Id=40, Nome="Tecnologias de Informação e Comunicação", ClasseMinima=11, ClasseMaxima=11, Curso=Curso.InformaticaDeGestao },
        new() { Id=41, Nome="Sistemas de Informação / Bases de Dados", ClasseMinima=11, ClasseMaxima=12, Curso=Curso.InformaticaDeGestao },
        new() { Id=42, Nome="Contabilidade Analítica",              ClasseMinima=11, ClasseMaxima=11, Curso=Curso.InformaticaDeGestao },
        new() { Id=43, Nome="Programação Avançada",                 ClasseMinima=12, ClasseMaxima=12, Curso=Curso.InformaticaDeGestao },
        new() { Id=44, Nome="Projecto Tecnológico",                 ClasseMinima=12, ClasseMaxima=13, Curso=Curso.InformaticaDeGestao },
        new() { Id=45, Nome="Práticas Oficinais / FCT",             ClasseMinima=13, ClasseMaxima=13, Curso=Curso.InformaticaDeGestao },
        // ── Contabilidade e Gestão — CG (10ª–13ª) ────────────────────────
        new() { Id=46, Nome="Informática",                          ClasseMinima=10, ClasseMaxima=12, Curso=Curso.ContabilidadeEGestao },
        new() { Id=47, Nome="Empreendedorismo",                     ClasseMinima=10, ClasseMaxima=12, Curso=Curso.ContabilidadeEGestao },
        new() { Id=48, Nome="Organização e Gestão de Empresas",     ClasseMinima=10, ClasseMaxima=11, Curso=Curso.ContabilidadeEGestao },
        new() { Id=49, Nome="Contabilidade Geral",                  ClasseMinima=10, ClasseMaxima=10, Curso=Curso.ContabilidadeEGestao },
        new() { Id=50, Nome="Direito Comercial",                    ClasseMinima=10, ClasseMaxima=10, Curso=Curso.ContabilidadeEGestao },
        new() { Id=51, Nome="Estatística",                          ClasseMinima=10, ClasseMaxima=10, Curso=Curso.ContabilidadeEGestao },
        new() { Id=52, Nome="Contabilidade Analítica",              ClasseMinima=11, ClasseMaxima=11, Curso=Curso.ContabilidadeEGestao },
        new() { Id=53, Nome="Cálculo Financeiro",                   ClasseMinima=11, ClasseMaxima=12, Curso=Curso.ContabilidadeEGestao },
        new() { Id=54, Nome="Direito Laboral",                      ClasseMinima=11, ClasseMaxima=11, Curso=Curso.ContabilidadeEGestao },
        new() { Id=55, Nome="Fiscalidade",                          ClasseMinima=11, ClasseMaxima=12, Curso=Curso.ContabilidadeEGestao },
        new() { Id=56, Nome="Contabilidade Pública / Bancária",     ClasseMinima=12, ClasseMaxima=12, Curso=Curso.ContabilidadeEGestao },
        new() { Id=57, Nome="Projecto Tecnológico",                 ClasseMinima=12, ClasseMaxima=13, Curso=Curso.ContabilidadeEGestao },
        new() { Id=58, Nome="Práticas Profissionais / FCT",         ClasseMinima=13, ClasseMaxima=13, Curso=Curso.ContabilidadeEGestao },
    };

    // ── Alunos ─────────────────────────────────────────────────────────────
    public static List<Aluno> Alunos { get; } = new()
    {
        // ── Sala 1 Manhã — 1ª Classe ───────────────────────────────────────
        new() { Id=1,  NumeroProcesso="TP2026001", NomeCompleto="Ana Beatriz Silva Cardoso",   Sexo="F", DataNascimento=new DateTime(2016,3,15),  TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=2,  NumeroProcesso="TP2026002", NomeCompleto="Carlos Eduardo Mendes",       Sexo="M", DataNascimento=new DateTime(2016,7,22),  TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=3,  NumeroProcesso="TP2026003", NomeCompleto="Fernanda Lopes Neto",         Sexo="F", DataNascimento=new DateTime(2016,1,8),   TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=4,  NumeroProcesso="TP2026004", NomeCompleto="Gustavo Almeida Santos",      Sexo="M", DataNascimento=new DateTime(2016,9,30),  TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=5,  NumeroProcesso="TP2026005", NomeCompleto="Helena Rodrigues Ferreira",   Sexo="F", DataNascimento=new DateTime(2016,5,14),  TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=6,  NumeroProcesso="TP2026006", NomeCompleto="Igor Tavares Pereira",        Sexo="M", DataNascimento=new DateTime(2016,11,2),  TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=7,  NumeroProcesso="TP2026007", NomeCompleto="Júlia Nascimento Costa",      Sexo="F", DataNascimento=new DateTime(2016,4,19),  TurmaId="S01M", Classe="1ª Classe" },
        new() { Id=8,  NumeroProcesso="TP2026008", NomeCompleto="Kevin Barbosa Lima",          Sexo="M", DataNascimento=new DateTime(2016,8,7),   TurmaId="S01M", Classe="1ª Classe" },
        // ── Sala 1 Tarde — 1ª Classe ────────────────────────────────────────
        new() { Id=9,  NumeroProcesso="TP2026009", NomeCompleto="Lara Moreira Cunha",          Sexo="F", DataNascimento=new DateTime(2016,2,11),  TurmaId="S01T", Classe="1ª Classe" },
        new() { Id=10, NumeroProcesso="TP2026010", NomeCompleto="Mateus Oliveira Braga",       Sexo="M", DataNascimento=new DateTime(2016,6,28),  TurmaId="S01T", Classe="1ª Classe" },
        // ── Sala 2 Manhã — 2ª Classe ───────────────────────────────────────
        new() { Id=11, NumeroProcesso="TP2026011", NomeCompleto="Natália Correia Souza",       Sexo="F", DataNascimento=new DateTime(2015,10,1),  TurmaId="S02M", Classe="2ª Classe" },
        new() { Id=12, NumeroProcesso="TP2026012", NomeCompleto="Otávio Pinto Marques",        Sexo="M", DataNascimento=new DateTime(2015,3,28),  TurmaId="S02M", Classe="2ª Classe" },
        new() { Id=13, NumeroProcesso="TP2026013", NomeCompleto="Patricia Moreira Dias",       Sexo="F", DataNascimento=new DateTime(2015,7,16),  TurmaId="S02M", Classe="2ª Classe" },
        new() { Id=14, NumeroProcesso="TP2026014", NomeCompleto="Rafael Cardoso Nunes",        Sexo="M", DataNascimento=new DateTime(2015,12,9),  TurmaId="S02M", Classe="2ª Classe" },
        new() { Id=15, NumeroProcesso="TP2026015", NomeCompleto="Sabrina Fonseca Torres",      Sexo="F", DataNascimento=new DateTime(2015,2,20),  TurmaId="S02M", Classe="2ª Classe" },
        // ── Sala 2 Tarde — 2ª Classe ───────────────────────────────────────
        new() { Id=16, NumeroProcesso="TP2026016", NomeCompleto="Thiago Cunha Rezende",        Sexo="M", DataNascimento=new DateTime(2015,5,3),   TurmaId="S02T", Classe="2ª Classe" },
        new() { Id=17, NumeroProcesso="TP2026017", NomeCompleto="Ursula Freitas Magalhães",    Sexo="F", DataNascimento=new DateTime(2015,9,11),  TurmaId="S02T", Classe="2ª Classe" },
        // ── Sala 3 Manhã — 3ª Classe ───────────────────────────────────────
        new() { Id=18, NumeroProcesso="TP2026018", NomeCompleto="Vitor Hugo Campos",           Sexo="M", DataNascimento=new DateTime(2014,2,14),  TurmaId="S03M", Classe="3ª Classe" },
        new() { Id=19, NumeroProcesso="TP2026019", NomeCompleto="Wanda Borges Esteves",        Sexo="F", DataNascimento=new DateTime(2014,6,27),  TurmaId="S03M", Classe="3ª Classe" },
        new() { Id=20, NumeroProcesso="TP2026020", NomeCompleto="Xavier Antunes Melo",         Sexo="M", DataNascimento=new DateTime(2014,10,5),  TurmaId="S03M", Classe="3ª Classe" },
        new() { Id=21, NumeroProcesso="TP2026021", NomeCompleto="Yasmin Carvalho Braga",       Sexo="F", DataNascimento=new DateTime(2014,3,17),  TurmaId="S03M", Classe="3ª Classe" },
        // ── Sala 3 Tarde — 3ª Classe ───────────────────────────────────────
        new() { Id=22, NumeroProcesso="TP2026022", NomeCompleto="Zeno Ribeiro Monteiro",       Sexo="M", DataNascimento=new DateTime(2014,7,29),  TurmaId="S03T", Classe="3ª Classe" },
        new() { Id=23, NumeroProcesso="TP2026023", NomeCompleto="Alice Teixeira Barreto",      Sexo="F", DataNascimento=new DateTime(2014,11,8),  TurmaId="S03T", Classe="3ª Classe" },
        // ── Sala 4 Manhã — 4ª Classe ───────────────────────────────────────
        new() { Id=24, NumeroProcesso="TP2026024", NomeCompleto="Bruno Azevedo Machado",       Sexo="M", DataNascimento=new DateTime(2013,4,21),  TurmaId="S04M", Classe="4ª Classe" },
        new() { Id=25, NumeroProcesso="TP2026025", NomeCompleto="Camila Nogueira Leite",       Sexo="F", DataNascimento=new DateTime(2013,8,6),   TurmaId="S04M", Classe="4ª Classe" },
        new() { Id=26, NumeroProcesso="TP2026026", NomeCompleto="Daniel Araújo Peixoto",       Sexo="M", DataNascimento=new DateTime(2013,1,30),  TurmaId="S04M", Classe="4ª Classe" },
        new() { Id=27, NumeroProcesso="TP2026027", NomeCompleto="Elisa Batista Rocha",         Sexo="F", DataNascimento=new DateTime(2013,5,23),  TurmaId="S04M", Classe="4ª Classe" },
        // ── Sala 4 Tarde — 4ª Classe ───────────────────────────────────────
        new() { Id=28, NumeroProcesso="TP2026028", NomeCompleto="Felipe Cerqueira Duarte",     Sexo="M", DataNascimento=new DateTime(2013,9,15),  TurmaId="S04T", Classe="4ª Classe" },
        new() { Id=29, NumeroProcesso="TP2026029", NomeCompleto="Gabriela Matos Figueiredo",   Sexo="F", DataNascimento=new DateTime(2013,2,7),   TurmaId="S04T", Classe="4ª Classe" },
        // ── Sala 5 Manhã — 5ª Classe ───────────────────────────────────────
        new() { Id=30, NumeroProcesso="TP2026030", NomeCompleto="Henrique Vasconcelos Prado",  Sexo="M", DataNascimento=new DateTime(2012,6,19),  TurmaId="S05M", Classe="5ª Classe" },
        new() { Id=31, NumeroProcesso="TP2026031", NomeCompleto="Isabel Queirós Vilas",        Sexo="F", DataNascimento=new DateTime(2012,10,3),  TurmaId="S05M", Classe="5ª Classe" },
        new() { Id=32, NumeroProcesso="TP2026032", NomeCompleto="João Paulo Serra Gomes",      Sexo="M", DataNascimento=new DateTime(2012,3,14),  TurmaId="S05M", Classe="5ª Classe" },
        // ── Sala 5 Tarde — 5ª Classe ───────────────────────────────────────
        new() { Id=33, NumeroProcesso="TP2026033", NomeCompleto="Karina Espírito Santo Leal", Sexo="F", DataNascimento=new DateTime(2012,7,25),  TurmaId="S05T", Classe="5ª Classe" },
        new() { Id=34, NumeroProcesso="TP2026034", NomeCompleto="Lucas Bernardes Falcão",      Sexo="M", DataNascimento=new DateTime(2012,11,8),  TurmaId="S05T", Classe="5ª Classe" },
        // ── Sala 6 Manhã — 6ª Classe ───────────────────────────────────────
        new() { Id=35, NumeroProcesso="TP2026035", NomeCompleto="Marina Cavalcante Neves",     Sexo="F", DataNascimento=new DateTime(2011,4,2),   TurmaId="S06M", Classe="6ª Classe" },
        new() { Id=36, NumeroProcesso="TP2026036", NomeCompleto="Nelson Augusto Faria",        Sexo="M", DataNascimento=new DateTime(2011,8,16),  TurmaId="S06M", Classe="6ª Classe" },
        new() { Id=37, NumeroProcesso="TP2026037", NomeCompleto="Olivia Sampaio Loureiro",     Sexo="F", DataNascimento=new DateTime(2011,12,27), TurmaId="S06M", Classe="6ª Classe" },
        // ── Sala 6 Tarde — 6ª Classe ───────────────────────────────────────
        new() { Id=38, NumeroProcesso="TP2026038", NomeCompleto="Pedro Henrique Raposo",       Sexo="M", DataNascimento=new DateTime(2011,5,10),  TurmaId="S06T", Classe="6ª Classe" },
        new() { Id=39, NumeroProcesso="TP2026039", NomeCompleto="Quintina Marcelino Andrade",  Sexo="F", DataNascimento=new DateTime(2011,9,21),  TurmaId="S06T", Classe="6ª Classe" },
        new() { Id=40, NumeroProcesso="TP2026040", NomeCompleto="Ricardo Soares Ventura",      Sexo="M", DataNascimento=new DateTime(2011,1,6),   TurmaId="S06T", Classe="6ª Classe" },
        // ── Sala 7 Manhã — 7ª Classe ───────────────────────────────────────
        new() { Id=41, NumeroProcesso="TP2026041", NomeCompleto="Amara Diogo Conceição",       Sexo="F", DataNascimento=new DateTime(2010,3,15),  TurmaId="S07M", Classe="7ª Classe" },
        new() { Id=42, NumeroProcesso="TP2026042", NomeCompleto="Benedito Luvumbo Costa",      Sexo="M", DataNascimento=new DateTime(2010,7,22),  TurmaId="S07M", Classe="7ª Classe" },
        new() { Id=43, NumeroProcesso="TP2026043", NomeCompleto="Cláudia Neto Xavier",         Sexo="F", DataNascimento=new DateTime(2010,11,5),  TurmaId="S07M", Classe="7ª Classe" },
        // ── Sala 7 Tarde — 7ª Classe ───────────────────────────────────────
        new() { Id=44, NumeroProcesso="TP2026044", NomeCompleto="David Tiago Mendonça",        Sexo="M", DataNascimento=new DateTime(2010,9,30),  TurmaId="S07T", Classe="7ª Classe" },
        new() { Id=45, NumeroProcesso="TP2026045", NomeCompleto="Emília Paiva Bandeira",       Sexo="F", DataNascimento=new DateTime(2010,1,18),  TurmaId="S07T", Classe="7ª Classe" },
        // ── Sala 8 Manhã — 8ª Classe ───────────────────────────────────────
        new() { Id=46, NumeroProcesso="TP2026046", NomeCompleto="Francisco Pinto Ramos",       Sexo="M", DataNascimento=new DateTime(2009,5,14),  TurmaId="S08M", Classe="8ª Classe" },
        new() { Id=47, NumeroProcesso="TP2026047", NomeCompleto="Graça Simões Leitão",         Sexo="F", DataNascimento=new DateTime(2009,11,2),  TurmaId="S08M", Classe="8ª Classe" },
        // ── Sala 8 Tarde — 8ª Classe ───────────────────────────────────────
        new() { Id=48, NumeroProcesso="TP2026048", NomeCompleto="Hélio Ferreira Brito",        Sexo="M", DataNascimento=new DateTime(2009,4,19),  TurmaId="S08T", Classe="8ª Classe" },
        new() { Id=49, NumeroProcesso="TP2026049", NomeCompleto="Inês Faria Gonçalves",        Sexo="F", DataNascimento=new DateTime(2009,8,7),   TurmaId="S08T", Classe="8ª Classe" },
        // ── Sala 9 Manhã — 9ª Classe ───────────────────────────────────────
        new() { Id=50, NumeroProcesso="TP2026050", NomeCompleto="José António Nkosi",          Sexo="M", DataNascimento=new DateTime(2008,2,11),  TurmaId="S09M", Classe="9ª Classe" },
        new() { Id=51, NumeroProcesso="TP2026051", NomeCompleto="Leonor Bettencourt Silva",    Sexo="F", DataNascimento=new DateTime(2008,6,28),  TurmaId="S09M", Classe="9ª Classe" },
        // ── Sala 9 Tarde — 9ª Classe ───────────────────────────────────────
        new() { Id=52, NumeroProcesso="TP2026052", NomeCompleto="Marco Vieira Henriques",      Sexo="M", DataNascimento=new DateTime(2008,10,1),  TurmaId="S09T", Classe="9ª Classe" },
        // ── Sala 10 Manhã — 10ª CG ─────────────────────────────────────────
        new() { Id=53, NumeroProcesso="TP2026053", NomeCompleto="Nádia Teixeira Barbosa",      Sexo="F", DataNascimento=new DateTime(2007,3,28),  TurmaId="S10M", Classe="10ª Classe" },
        new() { Id=54, NumeroProcesso="TP2026054", NomeCompleto="Orlando Cunha Baptista",      Sexo="M", DataNascimento=new DateTime(2007,7,16),  TurmaId="S10M", Classe="10ª Classe" },
        // ── Sala 10 Tarde — 10ª IG ─────────────────────────────────────────
        new() { Id=55, NumeroProcesso="TP2026055", NomeCompleto="Paula Rocha Moraes",          Sexo="F", DataNascimento=new DateTime(2007,12,9),  TurmaId="S10T", Classe="10ª Classe" },
        // ── Sala 11 Manhã — 11ª CEJ ────────────────────────────────────────
        new() { Id=56, NumeroProcesso="TP2026056", NomeCompleto="Quirino Fonseca Torres",      Sexo="M", DataNascimento=new DateTime(2006,5,3),   TurmaId="S11M", Classe="11ª Classe" },
    };

    // ── Atribuições Disciplina-Professor ──────────────────────────────────
    public static List<AtribuicaoDisciplina> AtribuicaoDisciplinas { get; } = GenerateAtribuicaoDisciplinas();

    // ── Presenças ──────────────────────────────────────────────────────────
    public static List<Presenca> Presencas { get; } = GeneratePresencas();

    // ── Horários ───────────────────────────────────────────────────────────
    public static List<Horario> Horarios { get; } = GenerateHorarios();

    // ══════════════════════════════════════════════════════════════════════
    // Discipline helpers
    // ══════════════════════════════════════════════════════════════════════
    public static List<Disciplina> GetDisciplinasDaTurma(Turma t) =>
        Disciplinas.Where(d =>
            d.ClasseMinima <= t.Classe && t.Classe <= d.ClasseMaxima &&
            (d.Curso == null || d.Curso == t.Curso)
        ).ToList();

    public static AtribuicaoDisciplina? GetAtribuicao(string turmaId, int disciplinaId) =>
        AtribuicaoDisciplinas.FirstOrDefault(a => a.TurmaId == turmaId && a.DisciplinaId == disciplinaId);

    public static Horario? GetHorario(string turmaId, int dia, int aula) =>
        Horarios.FirstOrDefault(h => h.TurmaId == turmaId && h.DiaDaSemana == dia && h.Aula == aula);

    public static Professor? GetProfessorDaDisciplina(string turmaId, int disciplinaId)
    {
        var a = GetAtribuicao(turmaId, disciplinaId);
        return a == null ? null : Professores.FirstOrDefault(p => p.Id == a.ProfessorId);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Static generators
    // ══════════════════════════════════════════════════════════════════════
    private static List<AtribuicaoDisciplina> GenerateAtribuicaoDisciplinas()
    {
        var result = new List<AtribuicaoDisciplina>();
        int id = 1;

        foreach (var turma in Turmas)
        {
            var discs = GetDisciplinasDaTurma(turma);

            if (turma.Classe <= 6)
            {
                // Primary: one professor handles all disciplines in the turma
                int profId;
                if (turma.DirectorDeTurmaId.HasValue)
                {
                    profId = turma.DirectorDeTurmaId.Value;
                }
                else
                {
                    // Tarde turma — use the Manhã counterpart's professor
                    var manha = Turmas.FirstOrDefault(t => t.Sala == turma.Sala && t.Turno == Turno.Manha);
                    profId = manha?.DirectorDeTurmaId ?? ((turma.Sala - 1) % Professores.Count + 1);
                }
                foreach (var d in discs)
                    result.Add(new AtribuicaoDisciplina { Id = id++, TurmaId = turma.Id, DisciplinaId = d.Id, ProfessorId = profId });
            }
            else
            {
                // Secondary: round-robin professor assignment per discipline
                int baseIndex = (turma.Sala - 7) * 2 + (turma.Turno == Turno.Tarde ? 1 : 0);
                for (int i = 0; i < discs.Count; i++)
                {
                    int profId = (baseIndex + i) % Professores.Count + 1;
                    result.Add(new AtribuicaoDisciplina { Id = id++, TurmaId = turma.Id, DisciplinaId = discs[i].Id, ProfessorId = profId });
                }
            }
        }
        return result;
    }

    private static List<Presenca> GeneratePresencas()
    {
        var result = new List<Presenca>();
        var rnd    = new Random(2026);
        int id     = 1;

        for (int d = 0; d < 30; d++)
        {
            var date = DateTime.Today.AddDays(-d);
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;

            foreach (var aluno in Alunos)
            {
                var turma = Turmas.FirstOrDefault(t => t.Id == aluno.TurmaId);
                if (turma == null) continue;

                foreach (var disc in GetDisciplinasDaTurma(turma))
                {
                    double r = rnd.NextDouble();
                    var status = r < 0.84 ? StatusPresenca.Presente
                               : r < 0.95 ? StatusPresenca.Falta
                               : StatusPresenca.Justificada;

                    result.Add(new Presenca
                    {
                        Id           = id++,
                        AlunoId      = aluno.Id,
                        TurmaId      = turma.Id,
                        DisciplinaId = disc.Id,
                        Data         = date,
                        Status       = status
                    });
                }
            }
        }
        return result;
    }

    // ══════════════════════════════════════════════════════════════════════
    // Aggregate helpers
    // ══════════════════════════════════════════════════════════════════════
    public static int TotalAlunos      => Alunos.Count(a => a.Ativo);
    public static int TotalTurmas      => Turmas.Count;
    public static int TotalProfessores => Professores.Count(p => p.Ativo);

    // Distinct students with at least one Presente/Falta session today
    public static int PresencasHoje =>
        Presencas.Where(p => p.Data.Date == DateTime.Today && p.Status == StatusPresenca.Presente)
                 .Select(p => p.AlunoId).Distinct().Count();

    public static int FaltasHoje =>
        Presencas.Where(p => p.Data.Date == DateTime.Today && p.Status == StatusPresenca.Falta)
                 .Select(p => p.AlunoId).Distinct().Count();

    public static int FaltasCriticas => Alunos.Count(a => GetPercentualFaltas(a.Id) > 25.0);

    public static double GetPercentualFaltas(int alunoId)
    {
        var reg = Presencas.Where(p => p.AlunoId == alunoId).ToList();
        if (reg.Count == 0) return 0;
        return (double)reg.Count(p => p.Status == StatusPresenca.Falta) / reg.Count * 100.0;
    }

    public static List<Aluno> GetAlunosDaTurma(string turmaId) =>
        Alunos.Where(a => a.TurmaId == turmaId && a.Ativo).OrderBy(a => a.NomeCompleto).ToList();

    public static List<Presenca> GetPresencasHoje(string turmaId) =>
        Presencas.Where(p => p.TurmaId == turmaId && p.Data.Date == DateTime.Today).ToList();

    public static Professor? GetDirectorDeTurma(int? directorId) =>
        directorId.HasValue ? Professores.FirstOrDefault(p => p.Id == directorId.Value) : null;

    // ══════════════════════════════════════════════════════════════════════
    // Horario generator — distributes disciplines evenly across the week
    // ══════════════════════════════════════════════════════════════════════
    private static List<Horario> GenerateHorarios()
    {
        var result = new List<Horario>();
        int id = 1;

        foreach (var turma in Turmas)
        {
            var discs = GetDisciplinasDaTurma(turma);
            if (discs.Count == 0) continue;

            int idx = 0;
            for (int dia = 1; dia <= 5; dia++)
            {
                for (int aula = 1; aula <= 6; aula++)
                {
                    result.Add(new Horario
                    {
                        Id           = id++,
                        TurmaId      = turma.Id,
                        DisciplinaId = discs[idx % discs.Count].Id,
                        DiaDaSemana  = dia,
                        Aula         = aula
                    });
                    idx++;
                }
            }
        }
        return result;
    }
}

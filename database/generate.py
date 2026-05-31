"""
Tarimba III — generates tarimba3.sql and one QR-code PNG per student.
Run: python generate.py
"""

import os, textwrap
import qrcode
from PIL import Image, ImageDraw, ImageFont

BASE_URL  = "https://blueviolet-bear-395492.hostingersite.com/api/presenca/"
OUT_DIR   = os.path.dirname(__file__)
QR_DIR    = os.path.join(OUT_DIR, "qrcodes")
SQL_FILE  = os.path.join(OUT_DIR, "tarimba3.sql")

os.makedirs(QR_DIR, exist_ok=True)

# ── DATA ────────────────────────────────────────────────────────────────────

PROFESSORES = [
    (1,  "Maria José Santos",       "mjsantos@tarimba3.ao",    "+244 923 001 001", 1),
    (2,  "Carlos Alberto Silva",    "casilva@tarimba3.ao",     "+244 923 002 002", 1),
    (3,  "Ana Paula Rodrigues",     "aprodrigues@tarimba3.ao", "+244 923 003 003", 1),
    (4,  "João Miguel Pereira",     "jmpereira@tarimba3.ao",   "+244 923 004 004", 1),
    (5,  "Fernanda Costa Lima",     "fclima@tarimba3.ao",      "+244 923 005 005", 1),
    (6,  "Pedro Henrique Alves",    "phalves@tarimba3.ao",     "+244 923 006 006", 1),
    (7,  "Sofia Isabel Nunes",      "sinunes@tarimba3.ao",     "+244 923 007 007", 1),
    (8,  "Manuel Augusto Correia",  "macorreia@tarimba3.ao",   "+244 923 008 008", 1),
    (9,  "Rosa Maria Tavares",      "rmtavares@tarimba3.ao",   "+244 923 009 009", 1),
    (10, "António Manuel Ferreira", "amferreira@tarimba3.ao",  "+244 923 010 010", 1),
]

# (id, sala, turno, classe, curso, director_id)
TURMAS = [
    ("S01M", 1,  "Manha", 1,  None,  1),
    ("S01T", 1,  "Tarde", 1,  None,  None),
    ("S02M", 2,  "Manha", 2,  None,  2),
    ("S02T", 2,  "Tarde", 2,  None,  None),
    ("S03M", 3,  "Manha", 3,  None,  3),
    ("S03T", 3,  "Tarde", 3,  None,  None),
    ("S04M", 4,  "Manha", 4,  None,  4),
    ("S04T", 4,  "Tarde", 4,  None,  None),
    ("S05M", 5,  "Manha", 5,  None,  5),
    ("S05T", 5,  "Tarde", 5,  None,  None),
    ("S06M", 6,  "Manha", 6,  None,  6),
    ("S06T", 6,  "Tarde", 6,  None,  None),
    ("S07M", 7,  "Manha", 7,  None,  7),
    ("S07T", 7,  "Tarde", 7,  None,  None),
    ("S08M", 8,  "Manha", 8,  None,  8),
    ("S08T", 8,  "Tarde", 8,  None,  None),
    ("S09M", 9,  "Manha", 9,  None,  9),
    ("S09T", 9,  "Tarde", 9,  None,  None),
    ("S10M", 10, "Manha", 10, "CG",  10),
    ("S10T", 10, "Tarde", 10, "IG",  None),
    ("S11M", 11, "Manha", 11, "CEJ", None),
    ("S11T", 11, "Tarde", 11, "CFB", None),
    ("S12M", 12, "Manha", 12, "CG",  None),
    ("S12T", 12, "Tarde", 13, "IG",  None),
]

# (id, nome, classe_min, classe_max, curso)
DISCIPLINAS = [
    (1,  "Língua Portuguesa",                        1,  12, None),
    (2,  "Matemática",                               1,  12, None),
    (3,  "Educação Física",                          1,  12, None),
    (4,  "Estudo do Meio",                           1,   4, None),
    (5,  "Educação Manual e Plástica",               1,   6, None),
    (6,  "Educação Musical",                         1,   6, None),
    (7,  "Ciências da Natureza",                     5,   6, None),
    (8,  "Educação Visual e Plástica",               5,   9, None),
    (9,  "Educação Moral e Cívica",                  5,   9, None),
    (10, "História",                                 5,   9, None),
    (11, "Geografia",                                5,   9, None),
    (12, "Língua Estrangeira",                       7,  12, None),
    (13, "Física",                                   7,   9, None),
    (14, "Química",                                  7,   9, None),
    (15, "Biologia",                                 7,   9, None),
    (16, "História",                                10,  12, "CEJ"),
    (17, "Geografia",                               10,  12, "CEJ"),
    (18, "Introdução ao Direito",                   10,  12, "CEJ"),
    (19, "Introdução à Economia",                   10,  12, "CEJ"),
    (20, "Psicologia",                              10,  12, "CEJ"),
    (21, "Sociologia",                              10,  12, "CEJ"),
    (22, "Informática",                             10,  12, "CEJ"),
    (23, "Filosofia",                               10,  12, "CEJ"),
    (24, "Física",                                  10,  12, "CFB"),
    (25, "Química",                                 10,  12, "CFB"),
    (26, "Biologia",                                10,  12, "CFB"),
    (27, "Geologia",                                10,  12, "CFB"),
    (28, "Geometria Descritiva",                    10,  12, "CFB"),
    (29, "Informática",                             10,  12, "CFB"),
    (30, "Filosofia",                               10,  12, "CFB"),
    (31, "Física",                                  10,  12, "IG"),
    (32, "Empreendedorismo",                        10,  12, "IG"),
    (33, "Introdução à Programação",                10,  10, "IG"),
    (34, "Arquitectura de Computadores",            10,  10, "IG"),
    (35, "Sistemas Operativos",                     10,  10, "IG"),
    (36, "Organização e Gestão de Empresas",        10,  11, "IG"),
    (37, "Contabilidade Geral",                     10,  10, "IG"),
    (38, "Programação",                             11,  11, "IG"),
    (39, "Redes de Computadores",                   11,  12, "IG"),
    (40, "Tecnologias de Informação e Comunicação", 11,  11, "IG"),
    (41, "Sistemas de Informação / Bases de Dados", 11,  12, "IG"),
    (42, "Contabilidade Analítica",                 11,  11, "IG"),
    (43, "Programação Avançada",                    12,  12, "IG"),
    (44, "Projecto Tecnológico",                    12,  13, "IG"),
    (45, "Práticas Oficinais / FCT",                13,  13, "IG"),
    (46, "Informática",                             10,  12, "CG"),
    (47, "Empreendedorismo",                        10,  12, "CG"),
    (48, "Organização e Gestão de Empresas",        10,  11, "CG"),
    (49, "Contabilidade Geral",                     10,  10, "CG"),
    (50, "Direito Comercial",                       10,  10, "CG"),
    (51, "Estatística",                             10,  10, "CG"),
    (52, "Contabilidade Analítica",                 11,  11, "CG"),
    (53, "Cálculo Financeiro",                      11,  12, "CG"),
    (54, "Direito Laboral",                         11,  11, "CG"),
    (55, "Fiscalidade",                             11,  12, "CG"),
    (56, "Contabilidade Pública / Bancária",        12,  12, "CG"),
    (57, "Projecto Tecnológico",                    12,  13, "CG"),
    (58, "Práticas Profissionais / FCT",            13,  13, "CG"),
]

# (id, numero_processo, nome_completo, sexo, data_nasc, turma_id, classe, ativo)
ALUNOS = [
    (1,  "TP2026001", "Ana Beatriz Silva Cardoso",     "F", "2016-03-15", "S01M", "1ª Classe",  1),
    (2,  "TP2026002", "Carlos Eduardo Mendes",         "M", "2016-07-22", "S01M", "1ª Classe",  1),
    (3,  "TP2026003", "Fernanda Lopes Neto",           "F", "2016-01-08", "S01M", "1ª Classe",  1),
    (4,  "TP2026004", "Gustavo Almeida Santos",        "M", "2016-09-30", "S01M", "1ª Classe",  1),
    (5,  "TP2026005", "Helena Rodrigues Ferreira",     "F", "2016-05-14", "S01M", "1ª Classe",  1),
    (6,  "TP2026006", "Igor Tavares Pereira",          "M", "2016-11-02", "S01M", "1ª Classe",  1),
    (7,  "TP2026007", "Júlia Nascimento Costa",        "F", "2016-04-19", "S01M", "1ª Classe",  1),
    (8,  "TP2026008", "Kevin Barbosa Lima",            "M", "2016-08-07", "S01M", "1ª Classe",  1),
    (9,  "TP2026009", "Lara Moreira Cunha",            "F", "2016-02-11", "S01T", "1ª Classe",  1),
    (10, "TP2026010", "Mateus Oliveira Braga",         "M", "2016-06-28", "S01T", "1ª Classe",  1),
    (11, "TP2026011", "Natália Correia Souza",         "F", "2015-10-01", "S02M", "2ª Classe",  1),
    (12, "TP2026012", "Otávio Pinto Marques",          "M", "2015-03-28", "S02M", "2ª Classe",  1),
    (13, "TP2026013", "Patricia Moreira Dias",         "F", "2015-07-16", "S02M", "2ª Classe",  1),
    (14, "TP2026014", "Rafael Cardoso Nunes",          "M", "2015-12-09", "S02M", "2ª Classe",  1),
    (15, "TP2026015", "Sabrina Fonseca Torres",        "F", "2015-02-20", "S02M", "2ª Classe",  1),
    (16, "TP2026016", "Thiago Cunha Rezende",          "M", "2015-05-03", "S02T", "2ª Classe",  1),
    (17, "TP2026017", "Ursula Freitas Magalhães",      "F", "2015-09-11", "S02T", "2ª Classe",  1),
    (18, "TP2026018", "Vitor Hugo Campos",             "M", "2014-02-14", "S03M", "3ª Classe",  1),
    (19, "TP2026019", "Wanda Borges Esteves",          "F", "2014-06-27", "S03M", "3ª Classe",  1),
    (20, "TP2026020", "Xavier Antunes Melo",           "M", "2014-10-05", "S03M", "3ª Classe",  1),
    (21, "TP2026021", "Yasmin Carvalho Braga",         "F", "2014-03-17", "S03M", "3ª Classe",  1),
    (22, "TP2026022", "Zeno Ribeiro Monteiro",         "M", "2014-07-29", "S03T", "3ª Classe",  1),
    (23, "TP2026023", "Alice Teixeira Barreto",        "F", "2014-11-08", "S03T", "3ª Classe",  1),
    (24, "TP2026024", "Bruno Azevedo Machado",         "M", "2013-04-21", "S04M", "4ª Classe",  1),
    (25, "TP2026025", "Camila Nogueira Leite",         "F", "2013-08-06", "S04M", "4ª Classe",  1),
    (26, "TP2026026", "Daniel Araújo Peixoto",         "M", "2013-01-30", "S04M", "4ª Classe",  1),
    (27, "TP2026027", "Elisa Batista Rocha",           "F", "2013-05-23", "S04M", "4ª Classe",  1),
    (28, "TP2026028", "Felipe Cerqueira Duarte",       "M", "2013-09-15", "S04T", "4ª Classe",  1),
    (29, "TP2026029", "Gabriela Matos Figueiredo",     "F", "2013-02-07", "S04T", "4ª Classe",  1),
    (30, "TP2026030", "Henrique Vasconcelos Prado",    "M", "2012-06-19", "S05M", "5ª Classe",  1),
    (31, "TP2026031", "Isabel Queirós Vilas",          "F", "2012-10-03", "S05M", "5ª Classe",  1),
    (32, "TP2026032", "João Paulo Serra Gomes",        "M", "2012-03-14", "S05M", "5ª Classe",  1),
    (33, "TP2026033", "Karina Espírito Santo Leal",    "F", "2012-07-25", "S05T", "5ª Classe",  1),
    (34, "TP2026034", "Lucas Bernardes Falcão",        "M", "2012-11-08", "S05T", "5ª Classe",  1),
    (35, "TP2026035", "Marina Cavalcante Neves",       "F", "2011-04-02", "S06M", "6ª Classe",  1),
    (36, "TP2026036", "Nelson Augusto Faria",          "M", "2011-08-16", "S06M", "6ª Classe",  1),
    (37, "TP2026037", "Olivia Sampaio Loureiro",       "F", "2011-12-27", "S06M", "6ª Classe",  1),
    (38, "TP2026038", "Pedro Henrique Raposo",         "M", "2011-05-10", "S06T", "6ª Classe",  1),
    (39, "TP2026039", "Quintina Marcelino Andrade",    "F", "2011-09-21", "S06T", "6ª Classe",  1),
    (40, "TP2026040", "Ricardo Soares Ventura",        "M", "2011-01-06", "S06T", "6ª Classe",  1),
    (41, "TP2026041", "Amara Diogo Conceição",         "F", "2010-03-15", "S07M", "7ª Classe",  1),
    (42, "TP2026042", "Benedito Luvumbo Costa",        "M", "2010-07-22", "S07M", "7ª Classe",  1),
    (43, "TP2026043", "Cláudia Neto Xavier",           "F", "2010-11-05", "S07M", "7ª Classe",  1),
    (44, "TP2026044", "David Tiago Mendonça",          "M", "2010-09-30", "S07T", "7ª Classe",  1),
    (45, "TP2026045", "Emília Paiva Bandeira",         "F", "2010-01-18", "S07T", "7ª Classe",  1),
    (46, "TP2026046", "Francisco Pinto Ramos",         "M", "2009-05-14", "S08M", "8ª Classe",  1),
    (47, "TP2026047", "Graça Simões Leitão",           "F", "2009-11-02", "S08M", "8ª Classe",  1),
    (48, "TP2026048", "Hélio Ferreira Brito",          "M", "2009-04-19", "S08T", "8ª Classe",  1),
    (49, "TP2026049", "Inês Faria Gonçalves",          "F", "2009-08-07", "S08T", "8ª Classe",  1),
    (50, "TP2026050", "José António Nkosi",            "M", "2008-02-11", "S09M", "9ª Classe",  1),
    (51, "TP2026051", "Leonor Bettencourt Silva",      "F", "2008-06-28", "S09M", "9ª Classe",  1),
    (52, "TP2026052", "Marco Vieira Henriques",        "M", "2008-10-01", "S09T", "9ª Classe",  1),
    (53, "TP2026053", "Nádia Teixeira Barbosa",        "F", "2007-03-28", "S10M", "10ª Classe", 1),
    (54, "TP2026054", "Orlando Cunha Baptista",        "M", "2007-07-16", "S10M", "10ª Classe", 1),
    (55, "TP2026055", "Paula Rocha Moraes",            "F", "2007-12-09", "S10T", "10ª Classe", 1),
    (56, "TP2026056", "Quirino Fonseca Torres",        "M", "2006-05-03", "S11M", "11ª Classe", 1),
]

# ── HELPERS ─────────────────────────────────────────────────────────────────

def get_disciplinas_da_turma(turma):
    tid, sala, turno, classe, curso, _ = turma
    return [d for d in DISCIPLINAS
            if d[2] <= classe <= d[3] and (d[4] is None or d[4] == curso)]

def generate_atribuicoes():
    result, id_ = [], 1
    turma_map = {t[0]: t for t in TURMAS}
    for turma in TURMAS:
        tid, sala, turno, classe, curso, director_id = turma
        discs = get_disciplinas_da_turma(turma)
        if classe <= 6:
            if director_id:
                prof_id = director_id
            else:
                manha = next((t for t in TURMAS if t[1] == sala and t[2] == "Manha"), None)
                prof_id = manha[5] if manha and manha[5] else ((sala - 1) % len(PROFESSORES) + 1)
            for d in discs:
                result.append((id_, tid, d[0], prof_id))
                id_ += 1
        else:
            base_index = (sala - 7) * 2 + (1 if turno == "Tarde" else 0)
            for i, d in enumerate(discs):
                prof_id = (base_index + i) % len(PROFESSORES) + 1
                result.append((id_, tid, d[0], prof_id))
                id_ += 1
    return result

def generate_horarios():
    result, id_ = [], 1
    for turma in TURMAS:
        discs = get_disciplinas_da_turma(turma)
        if not discs:
            continue
        idx = 0
        for dia in range(1, 6):
            for aula in range(1, 7):
                result.append((id_, turma[0], discs[idx % len(discs)][0], dia, aula))
                id_ += 1
                idx += 1
    return result

def esc(s):
    return s.replace("'", "''")

# ── GENERATE SQL ────────────────────────────────────────────────────────────

def build_sql():
    lines = []
    a = lines.append

    a("-- ================================================================")
    a("-- Complexo Escolar Tarimba III — Sistema de Presenças")
    a("-- Database: MySQL 8 / MariaDB 10.6+")
    a("-- Generated automatically — do not edit manually")
    a("-- ================================================================")
    a("")
    a("CREATE DATABASE IF NOT EXISTS tarimba3")
    a("  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;")
    a("USE tarimba3;")
    a("")
    a("SET FOREIGN_KEY_CHECKS = 0;")
    a("")

    # ── professores
    a("-- ── professores ────────────────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS professores (")
    a("  id            INT          NOT NULL AUTO_INCREMENT,")
    a("  nome_completo VARCHAR(120) NOT NULL,")
    a("  email         VARCHAR(120) NOT NULL,")
    a("  telefone      VARCHAR(30)  NOT NULL DEFAULT '',")
    a("  ativo         TINYINT(1)   NOT NULL DEFAULT 1,")
    a("  criado_em     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,")
    a("  PRIMARY KEY (id),")
    a("  UNIQUE KEY uq_email (email)")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")

    # ── turmas
    a("-- ── turmas ─────────────────────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS turmas (")
    a("  id                VARCHAR(6)  NOT NULL,")
    a("  sala              TINYINT     NOT NULL,")
    a("  turno             VARCHAR(5)  NOT NULL COMMENT 'Manha|Tarde',")
    a("  classe            TINYINT     NOT NULL,")
    a("  curso             VARCHAR(3)  NULL     COMMENT 'CG|IG|CEJ|CFB',")
    a("  director_turma_id INT         NULL,")
    a("  PRIMARY KEY (id),")
    a("  FOREIGN KEY (director_turma_id) REFERENCES professores(id) ON DELETE SET NULL")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")

    # ── disciplinas
    a("-- ── disciplinas ────────────────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS disciplinas (")
    a("  id            INT          NOT NULL AUTO_INCREMENT,")
    a("  nome          VARCHAR(100) NOT NULL,")
    a("  classe_minima TINYINT      NOT NULL DEFAULT 1,")
    a("  classe_maxima TINYINT      NOT NULL DEFAULT 13,")
    a("  curso         VARCHAR(3)   NULL     COMMENT 'CG|IG|CEJ|CFB|NULL=transversal',")
    a("  PRIMARY KEY (id)")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")

    # ── alunos
    a("-- ── alunos ─────────────────────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS alunos (")
    a("  id               INT          NOT NULL AUTO_INCREMENT,")
    a("  numero_processo  VARCHAR(20)  NOT NULL,")
    a("  nome_completo    VARCHAR(120) NOT NULL,")
    a("  sexo             CHAR(1)      NOT NULL COMMENT 'M|F',")
    a("  data_nascimento  DATE         NOT NULL,")
    a("  turma_id         VARCHAR(6)   NOT NULL,")
    a("  classe           VARCHAR(15)  NOT NULL,")
    a("  ativo            TINYINT(1)   NOT NULL DEFAULT 1,")
    a("  qr_token         VARCHAR(20)  NOT NULL COMMENT 'equals numero_processo — used in QR URL',")
    a("  criado_em        DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,")
    a("  PRIMARY KEY (id),")
    a("  UNIQUE KEY uq_numero_processo (numero_processo),")
    a("  UNIQUE KEY uq_qr_token (qr_token),")
    a("  FOREIGN KEY (turma_id) REFERENCES turmas(id)")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")

    # ── atribuicoes_disciplinas
    a("-- ── atribuicoes_disciplinas ────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS atribuicoes_disciplinas (")
    a("  id            INT        NOT NULL AUTO_INCREMENT,")
    a("  turma_id      VARCHAR(6) NOT NULL,")
    a("  disciplina_id INT        NOT NULL,")
    a("  professor_id  INT        NOT NULL,")
    a("  PRIMARY KEY (id),")
    a("  UNIQUE KEY uq_atrib (turma_id, disciplina_id),")
    a("  FOREIGN KEY (turma_id)      REFERENCES turmas(id),")
    a("  FOREIGN KEY (disciplina_id) REFERENCES disciplinas(id),")
    a("  FOREIGN KEY (professor_id)  REFERENCES professores(id)")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")

    # ── horarios
    a("-- ── horarios ───────────────────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS horarios (")
    a("  id            INT        NOT NULL AUTO_INCREMENT,")
    a("  turma_id      VARCHAR(6) NOT NULL,")
    a("  disciplina_id INT        NOT NULL,")
    a("  dia_semana    TINYINT    NOT NULL COMMENT '1=Seg 2=Ter 3=Qua 4=Qui 5=Sex',")
    a("  aula          TINYINT    NOT NULL COMMENT '1..6',")
    a("  PRIMARY KEY (id),")
    a("  UNIQUE KEY uq_slot (turma_id, dia_semana, aula),")
    a("  FOREIGN KEY (turma_id)      REFERENCES turmas(id),")
    a("  FOREIGN KEY (disciplina_id) REFERENCES disciplinas(id)")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")

    # ── presencas
    a("-- ── presencas ──────────────────────────────────────────────────")
    a("CREATE TABLE IF NOT EXISTS presencas (")
    a("  id            INT          NOT NULL AUTO_INCREMENT,")
    a("  aluno_id      INT          NOT NULL,")
    a("  turma_id      VARCHAR(6)   NOT NULL,")
    a("  disciplina_id INT          NOT NULL,")
    a("  data          DATE         NOT NULL,")
    a("  aula          TINYINT      NULL COMMENT 'slot 1-6; NULL = unknown',")
    a("  status        VARCHAR(12)  NOT NULL DEFAULT 'Presente' COMMENT 'Presente|Falta|Justificada',")
    a("  observacao    VARCHAR(255) NULL,")
    a("  registado_em  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,")
    a("  ip_origem     VARCHAR(45)  NULL COMMENT 'IP of QR scan',")
    a("  PRIMARY KEY (id),")
    a("  UNIQUE KEY uq_presenca (aluno_id, disciplina_id, data, aula),")
    a("  FOREIGN KEY (aluno_id)      REFERENCES alunos(id),")
    a("  FOREIGN KEY (turma_id)      REFERENCES turmas(id),")
    a("  FOREIGN KEY (disciplina_id) REFERENCES disciplinas(id)")
    a(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;")
    a("")
    a("SET FOREIGN_KEY_CHECKS = 1;")
    a("")

    # ── INSERT professores
    a("-- ── INSERT professores ─────────────────────────────────────────")
    for p in PROFESSORES:
        a(f"INSERT INTO professores (id, nome_completo, email, telefone, ativo) VALUES "
          f"({p[0]}, '{esc(p[1])}', '{esc(p[2])}', '{esc(p[3])}', {p[4]});")
    a("")

    # ── INSERT turmas
    a("-- ── INSERT turmas ──────────────────────────────────────────────")
    for t in TURMAS:
        curso_val = f"'{t[4]}'" if t[4] else "NULL"
        dir_val   = str(t[5])   if t[5] else "NULL"
        a(f"INSERT INTO turmas (id, sala, turno, classe, curso, director_turma_id) VALUES "
          f"('{t[0]}', {t[1]}, '{t[2]}', {t[3]}, {curso_val}, {dir_val});")
    a("")

    # ── INSERT disciplinas
    a("-- ── INSERT disciplinas ─────────────────────────────────────────")
    for d in DISCIPLINAS:
        curso_val = f"'{d[4]}'" if d[4] else "NULL"
        a(f"INSERT INTO disciplinas (id, nome, classe_minima, classe_maxima, curso) VALUES "
          f"({d[0]}, '{esc(d[1])}', {d[2]}, {d[3]}, {curso_val});")
    a("")

    # ── INSERT alunos
    a("-- ── INSERT alunos ──────────────────────────────────────────────")
    for al in ALUNOS:
        a(f"INSERT INTO alunos (id, numero_processo, nome_completo, sexo, data_nascimento, "
          f"turma_id, classe, ativo, qr_token) VALUES "
          f"({al[0]}, '{al[1]}', '{esc(al[2])}', '{al[3]}', '{al[4]}', "
          f"'{al[5]}', '{esc(al[6])}', {al[7]}, '{al[1]}');")
    a("")

    # ── INSERT atribuicoes
    a("-- ── INSERT atribuicoes_disciplinas ─────────────────────────────")
    for r in generate_atribuicoes():
        a(f"INSERT INTO atribuicoes_disciplinas (id, turma_id, disciplina_id, professor_id) VALUES "
          f"({r[0]}, '{r[1]}', {r[2]}, {r[3]});")
    a("")

    # ── INSERT horarios
    a("-- ── INSERT horarios ────────────────────────────────────────────")
    for h in generate_horarios():
        a(f"INSERT INTO horarios (id, turma_id, disciplina_id, dia_semana, aula) VALUES "
          f"({h[0]}, '{h[1]}', {h[2]}, {h[3]}, {h[4]});")
    a("")

    return "\n".join(lines)

# ── GENERATE QR CODES ────────────────────────────────────────────────────────

def make_qr(aluno):
    aid, num_proc, nome, sexo, dn, turma_id, classe, ativo = aluno
    url = BASE_URL + num_proc

    qr = qrcode.QRCode(version=3, error_correction=qrcode.constants.ERROR_CORRECT_M,
                       box_size=10, border=4)
    qr.add_data(url)
    qr.make(fit=True)
    img_qr = qr.make_image(fill_color="black", back_color="white").convert("RGB")

    # Canvas: QR code + label strip at bottom
    qr_w, qr_h = img_qr.size
    label_h = 60
    canvas = Image.new("RGB", (qr_w, qr_h + label_h), "white")
    canvas.paste(img_qr, (0, 0))

    draw = ImageDraw.Draw(canvas)
    # School colour bar
    draw.rectangle([(0, qr_h), (qr_w, qr_h + label_h)], fill="#1E3A8A")

    try:
        font_big   = ImageFont.truetype("arial.ttf", 16)
        font_small = ImageFont.truetype("arial.ttf", 12)
    except:
        font_big   = ImageFont.load_default()
        font_small = font_big

    # Centre the text
    name_short = nome if len(nome) <= 28 else nome[:26] + "…"
    draw.text((qr_w // 2, qr_h + 12), name_short,
              fill="white", font=font_big,  anchor="mt")
    draw.text((qr_w // 2, qr_h + 34), f"{num_proc}  ·  {classe}",
              fill="#93C5FD",  font=font_small, anchor="mt")

    path = os.path.join(QR_DIR, f"{num_proc}.png")
    canvas.save(path)
    return path

# ── MAIN ────────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    print("Generating SQL…")
    sql = build_sql()
    with open(SQL_FILE, "w", encoding="utf-8") as f:
        f.write(sql)
    print(f"  -> {SQL_FILE}  ({len(sql):,} chars)")

    print("Generating QR codes...")
    for aluno in ALUNOS:
        path = make_qr(aluno)
        print(f"  -> {os.path.basename(path)}")

    print(f"\nDone. {len(ALUNOS)} QR codes in {QR_DIR}")
    print(f"QR URL example: {BASE_URL}TP2026001")

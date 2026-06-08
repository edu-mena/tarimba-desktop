using TarimbaPresence.Database;
using TarimbaPresence.Helpers;
using TarimbaPresence.Models;

namespace TarimbaPresence.Forms;

public class AlunoRelatorioForm : Form
{
    private readonly DatabaseService _db = new();
    private readonly Aluno _aluno;
    private DataGridView dgv = null!;
    private Label lblResumo = null!;

    public AlunoRelatorioForm(Aluno aluno)
    {
        _aluno = aluno;
        InicializarJanela();
        CarregarDados();
    }

    private void InicializarJanela()
    {
        Text = $"Relatório Individual — {_aluno.NomeCompleto}";
        Size = new Size(820, 580);
        MinimumSize = new Size(700, 480);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = ThemeHelper.ContentBg;
        Padding = new Padding(24);

        // ── Título ────────────────────────────────────────────────────────
        var lblTitulo = new Label
        {
            Text = $"📋  {_aluno.NomeCompleto}",
            Font = ThemeHelper.FontTitle,
            ForeColor = ThemeHelper.DarkText,
            AutoSize = true,
            Dock = DockStyle.Top,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 0, 0, 4)
        };

        var turma = _db.ObterTurma(_aluno.TurmaId);
        var lblSub = new Label
        {
            Text = $"Turma: {turma?.Nome ?? "—"}   |   Nº Processo: {_aluno.NumeroProcesso}",
            Font = ThemeHelper.FontSmall,
            ForeColor = ThemeHelper.MediumText,
            AutoSize = true,
            Dock = DockStyle.Top,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 0, 0, 12)
        };

        // ── Resumo ────────────────────────────────────────────────────────
        lblResumo = new Label
        {
            Text = "",
            Font = ThemeHelper.FontBody,
            ForeColor = ThemeHelper.DarkText,
            AutoSize = true,
            Dock = DockStyle.Bottom,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 0)
        };

        // ── Botões ────────────────────────────────────────────────────────
        var pnlBotoes = new Panel { Dock = DockStyle.Bottom, Height = 50 };

        var btnExportar = UIHelper.MakeSecondaryButton("↓  Exportar CSV", 150, 36);
        btnExportar.Dock = DockStyle.Right;
        btnExportar.Click += BtnExportar_Click;

        var btnFechar = new Button
        {
            Text = "Fechar",
            Width = 100, Height = 36,
            Dock = DockStyle.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(220, 220, 220),
            ForeColor = ThemeHelper.DarkText,
            Font = ThemeHelper.FontBody,
            Margin = new Padding(0, 0, 8, 0)
        };
        btnFechar.FlatAppearance.BorderSize = 0;
        btnFechar.Click += (_, _) => Close();

        pnlBotoes.Controls.AddRange(new Control[] { btnExportar, btnFechar });

        // ── Tabela ────────────────────────────────────────────────────────
        dgv = new DataGridView { Dock = DockStyle.Fill };
        UIHelper.StyleDataGridView(dgv);

        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Data",        HeaderText = "Data",          Width = 110 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Disciplina",  HeaderText = "Disciplina",    FillWeight = 40 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado",      HeaderText = "Estado",        Width = 130 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Justificacao",HeaderText = "Justificação",  FillWeight = 60 });

        dgv.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == 2 && e.Value != null)
            {
                e.CellStyle.ForeColor = e.Value.ToString() switch
                {
                    "✅ Presente"    => ThemeHelper.SuccessText,
                    "❌ Falta"       => ThemeHelper.DangerText,
                    "📄 Justificada" => ThemeHelper.WarningText,
                    _ => ThemeHelper.DarkText
                };
                e.CellStyle.Font = ThemeHelper.FontBodyBold;
            }
        };

        Controls.Add(dgv);
        Controls.Add(lblResumo);
        Controls.Add(pnlBotoes);
        Controls.Add(lblSub);
        Controls.Add(lblTitulo);
    }

    private void CarregarDados()
    {
        dgv.Rows.Clear();

        var presencas = _db.ObterPresencasPorAluno(_aluno.Id);

        int presentes = 0, faltas = 0, justificadas = 0;

        foreach (var p in presencas)
        {
            string estadoTexto = p.Status switch
            {
                StatusPresenca.Presente    => "✅ Presente",
                StatusPresenca.Falta       => "❌ Falta",
                StatusPresenca.Justificada => "📄 Justificada",
                _ => p.Status.ToString()
            };

            var disciplina = _db.ObterDisciplinaPorId(p.DisciplinaId);

            dgv.Rows.Add(
                p.Data.ToString("dd/MM/yyyy"),
                disciplina?.Nome ?? $"Disciplina {p.DisciplinaId}",
                estadoTexto,
                p.Observacao ?? ""
            );

            if (p.Status == StatusPresenca.Presente)    presentes++;
            else if (p.Status == StatusPresenca.Falta)       faltas++;
            else if (p.Status == StatusPresenca.Justificada) justificadas++;
        }

        int total = presencas.Count;
        double taxa = total > 0 ? (double)presentes / total * 100 : 0;

        lblResumo.Text =
            $"Total: {total}   |   ✅ Presentes: {presentes}   |   " +
            $"❌ Faltas: {faltas}   |   📄 Justificadas: {justificadas}   |   " +
            $"Taxa de presença: {taxa:0.0}%";
    }

    private void BtnExportar_Click(object? sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Title    = "Guardar relatório CSV",
            Filter   = "CSV (*.csv)|*.csv",
            FileName = $"relatorio_{_aluno.NomeCompleto.Replace(" ", "_")}.csv"
        };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            var linhas = new List<string> { "Data,Disciplina,Estado,Justificação" };
            foreach (DataGridViewRow row in dgv.Rows)
            {
                linhas.Add(string.Join(",",
                    row.Cells["Data"].Value,
                    row.Cells["Disciplina"].Value,
                    row.Cells["Estado"].Value,
                    row.Cells["Justificacao"].Value));
            }
            File.WriteAllLines(sfd.FileName, linhas, System.Text.Encoding.UTF8);
            MessageBox.Show("Exportado com sucesso!", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
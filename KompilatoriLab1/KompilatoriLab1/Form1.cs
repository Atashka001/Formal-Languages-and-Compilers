using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KompilatoriLab1
{
    public partial class Form1 : Form
    {
        private readonly PascalEnumScanner _scanner = new PascalEnumScanner();

        private string _currentFilePath = string.Empty;
        private bool _isDirty = false;

        private bool _showAstTable = false;
        private string _lastAstText = "AST не построено.";
        private int _lastTotalErrors = 0;

        private readonly List<ResultRowInfo> _lastErrorRows = new List<ResultRowInfo>();

        private class ResultRowInfo
        {
            public string Fragment { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public object Tag { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeUi();
            WireEvents();
            SetupErrorGrid();
        }

        private void InitializeUi()
        {
            Text = "Текстовый редактор с семантическим анализатором";

            richTextBox1.Font = new Font("Consolas", 11F);

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (деревоASTToolStripMenuItem != null)
                деревоASTToolStripMenuItem.Text = "AST";

            UpdateTitle();
        }

        private void WireEvents()
        {
            button1.Click -= button1_Click;
            button1.Click += button1_Click;

            button2.Click -= button2_Click;
            button2.Click += button2_Click;

            button3.Click -= button3_Click;
            button3.Click += button3_Click;

            button4.Click -= button4_Click_2;
            button4.Click += button4_Click_2;

            button5.Click -= button5_Click;
            button5.Click += button5_Click;

            button6.Click -= button6_Click;
            button6.Click += button6_Click;

            button7.Click -= button7_Click;
            button7.Click += button7_Click;

            button8.Click -= button8_Click;
            button8.Click += button8_Click;

            button9.Click -= button9_Click;
            button9.Click += button9_Click;

            button10.Click -= button10_Click;
            button10.Click += button10_Click;

            button11.Click -= button11_Click;
            button11.Click += button11_Click;

            button12.Click -= button12_Click;
            button12.Click += button12_Click;

            создатьToolStripMenuItem.Click -= CreateNewFileMenu_Click;
            создатьToolStripMenuItem.Click += CreateNewFileMenu_Click;

            открытьToolStripMenuItem.Click -= OpenFileMenu_Click;
            открытьToolStripMenuItem.Click += OpenFileMenu_Click;

            сохранитьToolStripMenuItem.Click -= SaveFileMenu_Click;
            сохранитьToolStripMenuItem.Click += SaveFileMenu_Click;

            сохранитьКакToolStripMenuItem.Click -= SaveFileAsMenu_Click;
            сохранитьКакToolStripMenuItem.Click += SaveFileAsMenu_Click;

            выходToolStripMenuItem.Click -= ExitMenu_Click;
            выходToolStripMenuItem.Click += ExitMenu_Click;

            отменитьToolStripMenuItem.Click -= UndoMenu_Click;
            отменитьToolStripMenuItem.Click += UndoMenu_Click;

            повторитьToolStripMenuItem.Click -= RedoMenu_Click;
            повторитьToolStripMenuItem.Click += RedoMenu_Click;

            вырезатьToolStripMenuItem.Click -= CutMenu_Click;
            вырезатьToolStripMenuItem.Click += CutMenu_Click;

            копироватьToolStripMenuItem.Click -= CopyMenu_Click;
            копироватьToolStripMenuItem.Click += CopyMenu_Click;

            вставитьToolStripMenuItem.Click -= PasteMenu_Click;
            вставитьToolStripMenuItem.Click += PasteMenu_Click;

            удалитьToolStripMenuItem.Click -= DeleteMenu_Click;
            удалитьToolStripMenuItem.Click += DeleteMenu_Click;

            постановкаЗадачиToolStripMenuItem.Click -= InsertTaskTextMenu_Click;
            постановкаЗадачиToolStripMenuItem.Click += InsertTaskTextMenu_Click;

            грамматикаToolStripMenuItem.Click -= InsertGrammarTextMenu_Click;
            грамматикаToolStripMenuItem.Click += InsertGrammarTextMenu_Click;

            классификацияГрамматикиToolStripMenuItem.Click -= InsertGrammarClassTextMenu_Click;
            классификацияГрамматикиToolStripMenuItem.Click += InsertGrammarClassTextMenu_Click;

            методАнализаToolStripMenuItem.Click -= InsertMethodTextMenu_Click;
            методАнализаToolStripMenuItem.Click += InsertMethodTextMenu_Click;

            тестовыйПримерToolStripMenuItem.Click -= InsertTestExampleMenu_Click;
            тестовыйПримерToolStripMenuItem.Click += InsertTestExampleMenu_Click;

            списокЛитературыToolStripMenuItem.Click -= InsertReferencesMenu_Click;
            списокЛитературыToolStripMenuItem.Click += InsertReferencesMenu_Click;

            пускToolStripMenuItem.Click -= button9_Click;
            пускToolStripMenuItem.Click += button9_Click;

            деревоASTToolStripMenuItem.Click -= ToggleAstMenu_Click;
            деревоASTToolStripMenuItem.Click += ToggleAstMenu_Click;

            вызовСправкиToolStripMenuItem.Click -= ShowHelpMenu_Click;
            вызовСправкиToolStripMenuItem.Click += ShowHelpMenu_Click;

            оПрограммеToolStripMenuItem.Click -= ShowAboutMenu_Click;
            оПрограммеToolStripMenuItem.Click += ShowAboutMenu_Click;

            dataGridView1.CellClick -= DataGridView1_CellClick;
            dataGridView1.CellClick += DataGridView1_CellClick;

            richTextBox1.TextChanged -= RichTextBox1_TextChanged;
            richTextBox1.TextChanged += RichTextBox1_TextChanged;

            FormClosing -= Form1_FormClosing;
            FormClosing += Form1_FormClosing;
        }

        private void SetupErrorGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("Fragment", "Неверный фрагмент");
            dataGridView1.Columns.Add("Location", "Местоположение");
            dataGridView1.Columns.Add("Description", "Описание");

            dataGridView1.Columns[0].Width = 240;
            dataGridView1.Columns[1].Width = 220;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void SetupAstGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("Ast", "AST");
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void RunFullAnalysis()
        {
            try
            {
                string text = richTextBox1.Text ?? string.Empty;

                _showAstTable = false;
                _lastAstText = "AST не построено.";
                _lastTotalErrors = 0;
                _lastErrorRows.Clear();

                ClearResultArea();
                SetupErrorGrid();

                var scanResult = _scanner.Scan(text);
                var parser = new PascalEnumParser(scanResult.Tokens, scanResult.Errors);
                var parseResult = parser.Parse();

                _lastAstText = parseResult.AstText;

                int totalErrors = 0;

                foreach (var error in scanResult.Errors)
                {
                    _lastErrorRows.Add(new ResultRowInfo
                    {
                        Fragment = error.Symbol.ToString(),
                        Location = $"строка {error.Line}, позиция {error.Column}",
                        Description = error.Message,
                        Tag = error
                    });

                    totalErrors++;
                }

                foreach (var error in PascalEnumParser.FilterCascadingAgainstLexical(parseResult.Errors, scanResult.Errors))
                {
                    _lastErrorRows.Add(new ResultRowInfo
                    {
                        Fragment = error.Fragment,
                        Location = $"строка {error.Line}, позиция {error.Column}",
                        Description = error.Description,
                        Tag = error
                    });

                    totalErrors++;
                }

                _lastTotalErrors = totalErrors;
                ShowErrorTableFromLastResult();

                if (деревоASTToolStripMenuItem != null)
                    деревоASTToolStripMenuItem.Text = "AST";

                if (totalErrors == 0)
                {
                    MessageBox.Show(
                        "Анализ завершён. Ошибок не обнаружено.",
                        "Анализ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Анализ завершён. Ошибок: {totalErrors}",
                        "Анализ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при анализе: " + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowAstTable()
        {
            ClearResultArea();
            SetupAstGrid();

            string[] lines = (_lastAstText ?? "AST не построено.")
                .Replace("\r\n", "\n")
                .Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                int rowIndex = dataGridView1.Rows.Add(line);
                var row = dataGridView1.Rows[rowIndex];

                row.DefaultCellStyle.BackColor = Color.Honeydew;
                row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                row.DefaultCellStyle.Font = new Font("Consolas", 10F, FontStyle.Regular);
            }
        }

        private void ShowErrorTableFromLastResult()
        {
            ClearResultArea();
            SetupErrorGrid();

            foreach (var rowInfo in _lastErrorRows)
            {
                AddGridErrorRow(
                    rowInfo.Fragment,
                    rowInfo.Location,
                    rowInfo.Description,
                    rowInfo.Tag);
            }

            AddTotalErrorsRow(_lastTotalErrors);
        }

        private void ToggleAstMenu_Click(object sender, EventArgs e)
        {
            if (_showAstTable)
            {
                _showAstTable = false;

                if (деревоASTToolStripMenuItem != null)
                    деревоASTToolStripMenuItem.Text = "AST";

                ShowErrorTableFromLastResult();
            }
            else
            {
                _showAstTable = true;

                if (деревоASTToolStripMenuItem != null)
                    деревоASTToolStripMenuItem.Text = "Ошибки";

                ShowAstTable();
            }
        }

        private void AddGridErrorRow(string fragment, string location, string description, object tag)
        {
            int rowIndex = dataGridView1.Rows.Add(fragment, location, description);
            var row = dataGridView1.Rows[rowIndex];

            row.Tag = tag;
            row.DefaultCellStyle.BackColor = Color.MistyRose;
            row.DefaultCellStyle.ForeColor = Color.DarkRed;
        }

        private void ClearResultArea()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void AddTotalErrorsRow(int count)
        {
            int totalRowIndex = dataGridView1.Rows.Add(
                "Общее количество ошибок:",
                "",
                count.ToString());

            var row = dataGridView1.Rows[totalRowIndex];
            row.DefaultCellStyle.BackColor = Color.Gainsboro;
            row.DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dataGridView1.Rows[e.RowIndex];

            if (row.Tag is PascalEnumParser.SyntaxError syntaxError)
            {
                GoToAbsolutePosition(syntaxError.AbsolutePosition);
                return;
            }

            if (row.Tag is PascalEnumScanner.Error lexicalError)
            {
                GoToAbsolutePosition(lexicalError.AbsolutePosition);
            }
        }

        private void GoToAbsolutePosition(int position)
        {
            if (position < 0)
                position = 0;

            if (position > richTextBox1.TextLength)
                position = richTextBox1.TextLength;

            richTextBox1.Focus();

            if (position < richTextBox1.TextLength)
                richTextBox1.Select(position, 1);
            else
                richTextBox1.Select(position, 0);

            richTextBox1.ScrollToCaret();
        }

        private void CreateNewFile()
        {
            if (!ConfirmSaveChanges())
                return;

            richTextBox1.Clear();
            ClearResultArea();
            SetupErrorGrid();

            _currentFilePath = string.Empty;
            _isDirty = false;
            _showAstTable = false;
            _lastAstText = "AST не построено.";
            _lastTotalErrors = 0;
            _lastErrorRows.Clear();

            if (деревоASTToolStripMenuItem != null)
                деревоASTToolStripMenuItem.Text = "AST";

            UpdateTitle();
        }

        private void OpenFile()
        {
            if (!ConfirmSaveChanges())
                return;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Pascal files (*.pas)|*.pas|Все файлы (*.*)|*.*";
                dialog.Title = "Открыть файл";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.Text = File.ReadAllText(dialog.FileName);
                    _currentFilePath = dialog.FileName;
                    _isDirty = false;
                    _showAstTable = false;
                    _lastAstText = "AST не построено.";
                    _lastTotalErrors = 0;
                    _lastErrorRows.Clear();

                    if (деревоASTToolStripMenuItem != null)
                        деревоASTToolStripMenuItem.Text = "AST";

                    UpdateTitle();
                }
            }
        }

        private void SaveFile()
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath))
            {
                SaveFileAs();
                return;
            }

            File.WriteAllText(_currentFilePath, richTextBox1.Text);
            _isDirty = false;
            UpdateTitle();
        }

        private void SaveFileAs()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Pascal files (*.pas)|*.pas|Все файлы (*.*)|*.*";
                dialog.Title = "Сохранить файл";
                dialog.FileName = string.IsNullOrWhiteSpace(_currentFilePath)
                    ? "program.txt"
                    : Path.GetFileName(_currentFilePath);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentFilePath = dialog.FileName;
                    File.WriteAllText(_currentFilePath, richTextBox1.Text);
                    _isDirty = false;
                    UpdateTitle();
                }
            }
        }

        private bool ConfirmSaveChanges()
        {
            if (!_isDirty)
                return true;

            var answer = MessageBox.Show(
                "Текст был изменён. Сохранить изменения?",
                "Подтверждение",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (answer == DialogResult.Cancel)
                return false;

            if (answer == DialogResult.Yes)
                SaveFile();

            return true;
        }

        private void UpdateTitle()
        {
            string fileName = string.IsNullOrWhiteSpace(_currentFilePath)
                ? "Без имени"
                : Path.GetFileName(_currentFilePath);

            Text = $"{fileName}{(_isDirty ? "*" : "")} - Текстовый редактор с семантическим анализатором";
        }

        private void ShowHelp()
        {
            using (var form = new HelpForm())
            {
                form.ShowDialog(this);
            }
        }

        private void ShowAbout()
        {
            using (var form = new AboutForm())
            {
                form.ShowDialog(this);
            }
        }

        private void InsertTaskText()
        {
            richTextBox1.Text =
@"Постановка задачи

Разработать семантический анализатор для конструкции объявления перечисления Pascal.

Пример конструкции:
type Season = (Winter, Spring, Summer, Autumn);

Программа должна выполнять:
1. Лексический анализ.
2. Синтаксический анализ.
3. Построение AST.
4. Проверку уникальности идентификаторов.
5. Проверку недопустимых элементов перечисления: чисел, true и false.
6. Вывод таблицы ошибок и дерева AST.";
        }

        private void InsertGrammarText()
        {
            richTextBox1.Text =
@"Грамматика

<объявление_перечисления> ::= type <идентификатор> = ( <список_идентификаторов> ) ;

<список_идентификаторов> ::= <идентификтор>
                           | <идентификатор> , <список_идентификаторов>

<идентификатор> ::= id";
        }

        private void InsertGrammarClassText()
        {
            richTextBox1.Text =
@"Классификация грамматики

Данная грамматика является контекстно-свободной.
Она описывает синтаксическую конструкцию объявления перечислимого типа Pascal.";
        }

        private void InsertMethodText()
        {
            richTextBox1.Text =
@"Метод анализа

Используется нисходящий синтаксический анализ.
Сначала выполняется лексический анализ, затем синтаксический и семантический анализ.
В процессе анализа строится абстрактное синтаксическое дерево AST.
При обнаружении ошибки программа продолжает анализ и выводит найденные ошибки в таблицу.";
        }

        private void InsertTestExample()
        {
            richTextBox1.Text = @"type Season = (Winter, Spring, Summer, Autumn);";
        }

        private void InsertReferences()
        {
            richTextBox1.Text =
@"Список литературы

1. Ахо А., Ульман Д. Компиляторы: принципы, технологии и инструменты.
2. Вирт Н. Алгоритмы + структуры данных = программы.
3. Документация Microsoft по Windows Forms.";
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            UpdateTitle();
        }

        private void CreateNewFileMenu_Click(object sender, EventArgs e)
        {
            CreateNewFile();
        }

        private void OpenFileMenu_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveFileMenu_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFileAsMenu_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UndoMenu_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
                richTextBox1.Undo();
        }

        private void RedoMenu_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo)
                richTextBox1.Redo();
        }

        private void CutMenu_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void CopyMenu_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void PasteMenu_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void DeleteMenu_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = string.Empty;
        }

        private void InsertTaskTextMenu_Click(object sender, EventArgs e)
        {
            InsertTaskText();
        }

        private void InsertGrammarTextMenu_Click(object sender, EventArgs e)
        {
            InsertGrammarText();
        }

        private void InsertGrammarClassTextMenu_Click(object sender, EventArgs e)
        {
            InsertGrammarClassText();
        }

        private void InsertMethodTextMenu_Click(object sender, EventArgs e)
        {
            InsertMethodText();
        }

        private void InsertTestExampleMenu_Click(object sender, EventArgs e)
        {
            InsertTestExample();
        }

        private void InsertReferencesMenu_Click(object sender, EventArgs e)
        {
            InsertReferences();
        }

        private void ShowHelpMenu_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void ShowAboutMenu_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmSaveChanges())
                e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateNewFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = string.Empty;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            RunFullAnalysis();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
                richTextBox1.Undo();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo)
                richTextBox1.Redo();
        }
    }
}

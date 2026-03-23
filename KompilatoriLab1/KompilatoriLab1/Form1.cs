using System;
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

        public Form1()
        {
            InitializeComponent();
            InitializeUi();
            WireEvents();
            SetupErrorGrid();
        }

        private void InitializeUi()
        {
            Text = "Текстовый редактор с лексическим анализатором";
            richTextBox1.Font = new Font("Consolas", 11F);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            UpdateTitle();
        }

        private void WireEvents()
        {
            // Не трогаем button4/button12 и другие кнопки, если они уже привязаны в Designer.
            // Подключаем только то, что не ломает существующие обработчики.

            создатьToolStripMenuItem.Click += (s, e) => CreateNewFile();
            открытьToolStripMenuItem.Click += (s, e) => OpenFile();
            сохранитьToolStripMenuItem.Click += (s, e) => SaveFile();
            сохранитьКакToolStripMenuItem.Click += (s, e) => SaveFileAs();
            выходToolStripMenuItem.Click += (s, e) => Close();

            постановкаЗадачиToolStripMenuItem.Click += (s, e) => InsertTaskText();
            грамматикаToolStripMenuItem.Click += (s, e) => InsertGrammarText();
            классификацияГрамматикиToolStripMenuItem.Click += (s, e) => InsertGrammarClassText();
            методАнализаToolStripMenuItem.Click += (s, e) => InsertMethodText();
            тестовыйПримерToolStripMenuItem.Click += (s, e) => InsertTestExample();
            списокЛитературыToolStripMenuItem.Click += (s, e) => InsertReferences();

            пускToolStripMenuItem.Click += button9_Click;
            вызовСправкиToolStripMenuItem.Click += (s, e) => ShowHelp();
            оПрограммеToolStripMenuItem.Click += (s, e) => ShowAbout();

            dataGridView1.CellClick += DataGridView1_CellClick;

            richTextBox1.TextChanged += (s, e) =>
            {
                _isDirty = true;
                UpdateTitle();
            };

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

        private void button9_Click(object sender, EventArgs e)
        {
            RunFullAnalysis();
        }

        private void RunFullAnalysis()
        {
            try
            {
                string text = richTextBox1.Text ?? string.Empty;

                ClearResultArea();

                var scanResult = _scanner.Scan(text);

                if (scanResult.Errors.Count > 0)
                {
                    ShowLexicalErrors(scanResult);

                    MessageBox.Show(
                        $"Обнаружены лексические ошибки: {scanResult.Errors.Count}",
                        "Лексический анализ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                var parser = new PascalEnumParser(scanResult.Tokens);
                var parseResult = parser.Parse();

                if (parseResult.Success)
                {
                    SetupErrorGrid();

                    MessageBox.Show(
                        "Синтаксический анализ завершён. Ошибок не обнаружено.",
                        "Синтаксический анализ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                ShowSyntaxErrors(parseResult);

                MessageBox.Show(
                    $"Синтаксический анализ завершён. Ошибок: {parseResult.Errors.Count}",
                    "Синтаксический анализ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
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

        private void ClearResultArea()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void ShowLexicalErrors(PascalEnumScanner.ScanResult scanResult)
        {
            SetupErrorGrid();

            foreach (var error in scanResult.Errors)
            {
                int rowIndex = dataGridView1.Rows.Add(
                    error.Symbol.ToString(),
                    $"строка {error.Line}, позиция {error.Column}",
                    error.Message);

                var row = dataGridView1.Rows[rowIndex];
                row.Tag = error;
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
            }

            AddTotalErrorsRow(scanResult.Errors.Count);
        }

        private void ShowSyntaxErrors(PascalEnumParser.ParseResult parseResult)
        {
            SetupErrorGrid();

            foreach (var error in parseResult.Errors)
            {
                int rowIndex = dataGridView1.Rows.Add(
                    error.Fragment,
                    $"строка {error.Line}, позиция {error.Column}",
                    error.Description);

                var row = dataGridView1.Rows[rowIndex];
                row.Tag = error;
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
            }

            AddTotalErrorsRow(parseResult.Errors.Count);
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
            _currentFilePath = string.Empty;
            _isDirty = false;
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

            Text = $"{fileName}{(_isDirty ? "*" : "")} - Текстовый редактор с лексическим анализатором";
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

Разработать синтаксический анализатор для конструкции объявления перечисления Pascal.
Анализатор должен запускаться после лексического анализа, обнаруживать синтаксические
ошибки, продолжать анализ после ошибки и выводить таблицу с описанием ошибок.";
        }

        private void InsertGrammarText()
        {
            richTextBox1.Text =
@"Грамматика

<EnumDecl>   ::= type <Identifier> = ( <IdList> ) ;
<IdList>     ::= <Identifier> <IdListTail>
<IdListTail> ::= , <Identifier> <IdListTail> | ε";
        }

        private void InsertGrammarClassText()
        {
            richTextBox1.Text =
@"Классификация грамматики

Данная грамматика является контекстно-свободной.
Она описывает синтаксическую конструкцию объявления перечисления Pascal.";
        }

        private void InsertMethodText()
        {
            richTextBox1.Text =
@"Метод анализа

Используется нисходящий синтаксический анализ.
Сначала выполняется лексический анализ, затем синтаксический.
При обнаружении ошибки применяется восстановление с продолжением анализа
(нейтрализация ошибок методом Айронса).";
        }

        private void InsertTestExample()
        {
            richTextBox1.Text =
@"type Season = (Winter, Spring, Summer, Autumn);";
        }

        private void InsertReferences()
        {
            richTextBox1.Text =
@"Список литературы

1. Ахо А., Ульман Д. Компиляторы: принципы, технологии и инструменты.
2. Вирт Н. Алгоритмы + структуры данных = программы.
3. Документация Microsoft по Windows Forms.";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmSaveChanges())
            {
                e.Cancel = true;
            }
        }

        // Оставлены без изменений
        private void button1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void button3_Click(object sender, EventArgs e) { }
        private void button4_Click(object sender, EventArgs e) { }
        private void button5_Click(object sender, EventArgs e) { }
        private void button6_Click(object sender, EventArgs e) { }
        private void button7_Click(object sender, EventArgs e) { }
        private void button8_Click(object sender, EventArgs e) { }
        private void button10_Click(object sender, EventArgs e) { }
        private void button11_Click(object sender, EventArgs e) { }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click_2(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }
    }
}

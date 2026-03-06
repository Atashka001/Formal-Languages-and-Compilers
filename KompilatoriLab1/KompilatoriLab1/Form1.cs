using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KompilatoriLab1
{
    public partial class Form1 : Form
    {
        private string currentFilePath = null;
        private bool isTextChanged = false;

        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private bool ignoreTextChange = false;

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.Columns.Add("Type", "Тип");
            dataGridView1.Columns.Add("Line", "Строка");
            dataGridView1.Columns.Add("Column", "Колонка");
            dataGridView1.Columns.Add("Message", "Сообщение");

            dataGridView1.Columns[0].Width = 80;
            dataGridView1.Columns[1].Width = 50;
            dataGridView1.Columns[2].Width = 60;

            richTextBox1.Font = new Font("Consolas", 10);
            richTextBox1.WordWrap = false;
            richTextBox1.TextChanged += RichTextBox1_TextChanged;
            richTextBox1.KeyDown += RichTextBox1_KeyDown;
            richTextBox1.MouseUp += RichTextBox1_MouseUp;
            richTextBox1.KeyUp += RichTextBox1_KeyUp;

            SubscribeToEvents();

            UpdateTitle();
            UpdateButtonStates();

            SaveToUndoStack();
        }

        private void SubscribeToEvents()
        {
            try
            {
                this.файлToolStripMenuItem.DropDownItems["создатьToolStripMenuItem"].Click +=
                    new EventHandler(создатьНовыйФайлToolStripMenuItem_Click);
                this.файлToolStripMenuItem.DropDownItems["открытьToolStripMenuItem"].Click +=
                    new EventHandler(открытьФайлToolStripMenuItem_Click);
                this.файлToolStripMenuItem.DropDownItems["сохранитьToolStripMenuItem"].Click +=
                    new EventHandler(сохранитьToolStripMenuItem_Click);
                this.файлToolStripMenuItem.DropDownItems["сохранитьКакToolStripMenuItem"].Click +=
                    new EventHandler(сохранитьКакToolStripMenuItem_Click);
                this.файлToolStripMenuItem.DropDownItems["выходToolStripMenuItem"].Click +=
                    new EventHandler(выходToolStripMenuItem_Click);

                this.правкаToolStripMenuItem.DropDownItems["отменитьToolStripMenuItem"].Click +=
                    new EventHandler(отменитьToolStripMenuItem_Click);
                this.правкаToolStripMenuItem.DropDownItems["повторитьToolStripMenuItem"].Click +=
                    new EventHandler(повторитьToolStripMenuItem_Click);
                this.правкаToolStripMenuItem.DropDownItems["вырезатьToolStripMenuItem"].Click +=
                    new EventHandler(вырезатьToolStripMenuItem_Click);
                this.правкаToolStripMenuItem.DropDownItems["копироватьToolStripMenuItem"].Click +=
                    new EventHandler(копироватьToolStripMenuItem_Click);
                this.правкаToolStripMenuItem.DropDownItems["вставитьToolStripMenuItem"].Click +=
                    new EventHandler(вставитьToolStripMenuItem_Click);
                this.правкаToolStripMenuItem.DropDownItems["удалитьToolStripMenuItem"].Click +=
                    new EventHandler(удалитьToolStripMenuItem_Click);

                this.текстToolStripMenuItem.DropDownItems["постановкаЗадачиToolStripMenuItem"].Click +=
                    new EventHandler(постановкаЗадачиToolStripMenuItem_Click);
                this.текстToolStripMenuItem.DropDownItems["грамматикаToolStripMenuItem"].Click +=
                    new EventHandler(грамматикаToolStripMenuItem_Click);
                this.текстToolStripMenuItem.DropDownItems["классификацияГрамматикиToolStripMenuItem"].Click +=
                    new EventHandler(классификацияГрамматикиToolStripMenuItem_Click);
                this.текстToolStripMenuItem.DropDownItems["методАнализаToolStripMenuItem"].Click +=
                    new EventHandler(методАнализаToolStripMenuItem_Click);
                this.текстToolStripMenuItem.DropDownItems["тестовыйПримерToolStripMenuItem"].Click +=
                    new EventHandler(тестовыйПримерToolStripMenuItem_Click);
                this.текстToolStripMenuItem.DropDownItems["списокЛитературыToolStripMenuItem"].Click +=
                    new EventHandler(списокЛитературыToolStripMenuItem_Click);

                this.справкаToolStripMenuItem.DropDownItems["вызовСправкиToolStripMenuItem"].Click +=
                    new EventHandler(вызовСправкиToolStripMenuItem_Click);
                this.справкаToolStripMenuItem.DropDownItems["оПрограммеToolStripMenuItem"].Click +=
                    new EventHandler(оПрограммеToolStripMenuItem_Click);

                this.button1.Click += new EventHandler(button1_Click);
                this.button2.Click += new EventHandler(button2_Click);
                this.button3.Click += new EventHandler(button3_Click);
                this.button4.Click += new EventHandler(button4_Click);
                this.button5.Click += new EventHandler(button5_Click);
                this.button6.Click += new EventHandler(button6_Click);
                this.button7.Click += new EventHandler(button7_Click);
                this.button8.Click += new EventHandler(button8_Click);
                this.button9.Click += new EventHandler(button9_Click);
                this.button10.Click += new EventHandler(button10_Click);
                this.button11.Click += new EventHandler(button11_Click);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подписке на события: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RichTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateButtonStates();
        }

        private void RichTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateButtonStates();

            if (e.Control && e.KeyCode == Keys.A)
            {
                richTextBox1.SelectAll();
                UpdateButtonStates();
            }
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                отменитьToolStripMenuItem_Click(sender, e);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                повторитьToolStripMenuItem_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreTextChange)
            {
                isTextChanged = true;
                UpdateTitle();
                SaveToUndoStack();
                HighlightSyntax();
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = richTextBox1.SelectionLength > 0;
            bool hasText = !string.IsNullOrEmpty(richTextBox1.Text);

            button6.Enabled = hasSelection;
            button7.Enabled = hasSelection;

            button8.Enabled = Clipboard.ContainsText();

            button4.Enabled = undoStack.Count > 1;
            button5.Enabled = redoStack.Count > 0;

            отменитьToolStripMenuItem.Enabled = button4.Enabled;
            повторитьToolStripMenuItem.Enabled = button5.Enabled;
            копироватьToolStripMenuItem.Enabled = hasSelection;
            вырезатьToolStripMenuItem.Enabled = hasSelection;
            вставитьToolStripMenuItem.Enabled = Clipboard.ContainsText();
            удалитьToolStripMenuItem.Enabled = hasSelection;
        }

        private void SaveToUndoStack()
        {
            undoStack.Push(richTextBox1.Text);

            if (undoStack.Count > 50)
            {
                var tempStack = new Stack<string>();
                for (int i = 0; i < 49; i++)
                {
                    tempStack.Push(undoStack.Pop());
                }
                undoStack.Clear();
                for (int i = 0; i < 49; i++)
                {
                    undoStack.Push(tempStack.Pop());
                }
            }

            redoStack.Clear();
            UpdateButtonStates();
        }

        private void HighlightSyntax()
        {
            int selectionStart = richTextBox1.SelectionStart;
            int selectionLength = richTextBox1.SelectionLength;

            ignoreTextChange = true;

            string[] keywords = { "type", "var", "const", "begin", "end", "if", "then", "else",
                                  "while", "do", "for", "to", "downto", "repeat", "until",
                                  "procedure", "function", "program", "uses", "array", "of",
                                  "integer", "string", "real", "boolean", "char" };

            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);

            foreach (string keyword in keywords)
            {
                int index = 0;
                while (index < richTextBox1.TextLength)
                {
                    index = richTextBox1.Text.IndexOf(keyword, index, StringComparison.OrdinalIgnoreCase);
                    if (index == -1) break;

                    bool isWordStart = (index == 0) || !char.IsLetterOrDigit(richTextBox1.Text[index - 1]);
                    bool isWordEnd = (index + keyword.Length >= richTextBox1.TextLength) ||
                                    !char.IsLetterOrDigit(richTextBox1.Text[index + keyword.Length]);

                    if (isWordStart && isWordEnd)
                    {
                        richTextBox1.Select(index, keyword.Length);
                        richTextBox1.SelectionColor = Color.Blue;
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                    }

                    index += keyword.Length;
                }
            }

            richTextBox1.Select(selectionStart, selectionLength);
            richTextBox1.SelectionColor = Color.Black;

            ignoreTextChange = false;
        }

        private void UpdateTitle()
        {
            string title = "Текстовый редактор";
            if (!string.IsNullOrEmpty(currentFilePath))
                title += " - " + Path.GetFileName(currentFilePath);
            if (isTextChanged)
                title += "*";
            this.Text = title;
        }

        private bool PromptSaveChanges()
        {
            if (!isTextChanged) return true;

            DialogResult result = MessageBox.Show(
                "Сохранить изменения в файле?",
                "Сохранение",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                return SaveFile();
            else if (result == DialogResult.Cancel)
                return false;

            return true;
        }

        private bool SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
                return SaveFileAs();

            try
            {
                File.WriteAllText(currentFilePath, richTextBox1.Text);
                isTextChanged = false;
                UpdateTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SaveFileAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстовые файлы (*.txt)|*.txt|Pascal файлы (*.pas)|*.pas|Все файлы (*.*)|*.*";
                sfd.DefaultExt = "txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = sfd.FileName;
                    return SaveFile();
                }
            }
            return false;
        }

        private void создатьНовыйФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptSaveChanges()) return;

            undoStack.Clear();
            redoStack.Clear();

            richTextBox1.Clear();
            currentFilePath = null;
            isTextChanged = false;
            UpdateTitle();

            SaveToUndoStack();
            UpdateButtonStates();
        }

        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PromptSaveChanges()) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Текстовые файлы (*.txt)|*.txt|Pascal файлы (*.pas)|*.pas|Все файлы (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        undoStack.Clear();
                        redoStack.Clear();

                        richTextBox1.Text = File.ReadAllText(ofd.FileName);
                        currentFilePath = ofd.FileName;
                        isTextChanged = false;
                        UpdateTitle();

                        SaveToUndoStack();
                        UpdateButtonStates();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptSaveChanges())
                Application.Exit();
        }

        // Обработчики меню "Правка"
        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                string currentText = undoStack.Pop();
                redoStack.Push(currentText);

                string previousText = undoStack.Peek();

                ignoreTextChange = true;
                richTextBox1.Text = previousText;
                ignoreTextChange = false;

                isTextChanged = true;
                UpdateTitle();
                HighlightSyntax();
                UpdateButtonStates();
            }
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                string redoText = redoStack.Pop();

                undoStack.Push(redoText);

                ignoreTextChange = true;
                richTextBox1.Text = redoText;
                ignoreTextChange = false;

                isTextChanged = true;
                UpdateTitle();
                HighlightSyntax();
                UpdateButtonStates();
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Cut();
                SaveToUndoStack();
                UpdateButtonStates();
            }
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Copy();
            }
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                int oldLength = richTextBox1.TextLength;
                richTextBox1.Paste();

                if (richTextBox1.TextLength != oldLength)
                {
                    SaveToUndoStack();
                }
                UpdateButtonStates();
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                int selectionStart = richTextBox1.SelectionStart;
                int selectionLength = richTextBox1.SelectionLength;

                richTextBox1.Text = richTextBox1.Text.Remove(selectionStart, selectionLength);
                richTextBox1.SelectionStart = selectionStart;

                SaveToUndoStack();
                UpdateButtonStates();
            }
        }

        private void постановкаЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Постановка задачи будет описана позже.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void грамматикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Грамматика будет описана позже.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void классификацияГрамматикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Классификация грамматики будет описана позже.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void методАнализаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Метод анализа будет описан позже.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void тестовыйПримерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoStack.Clear();
            redoStack.Clear();

            richTextBox1.Text = "type Season = (Winter, Spring, Summer, Autumn);";
            isTextChanged = true;
            UpdateTitle();

            SaveToUndoStack();
            UpdateButtonStates();
        }

        private void списокЛитературыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string literature = "Список литературы:\n\n" +
                "1. Ахо А., Сети Р., Ульман Дж. Компиляторы: принципы, технологии и инструменты.\n" +
                "2. Моженков О.В. Теория формальных языков и компиляторов.\n" +
                "3. Холмс Д. Технология компиляции.";

            MessageBox.Show(literature, "Список литературы",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e) // Создать
        {
            создатьНовыйФайлToolStripMenuItem_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e) // Открыть
        {
            открытьФайлToolStripMenuItem_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e) // Сохранить
        {
            сохранитьToolStripMenuItem_Click(sender, e);
        }

        private void button4_Click(object sender, EventArgs e) // Отмена (Undo)
        {
            отменитьToolStripMenuItem_Click(sender, e);
        }

        private void button5_Click(object sender, EventArgs e) // Повтор (Redo)
        {
            повторитьToolStripMenuItem_Click(sender, e);
        }

        private void button6_Click(object sender, EventArgs e) // Копировать
        {
            копироватьToolStripMenuItem_Click(sender, e);
        }

        private void button7_Click(object sender, EventArgs e) // Вырезать
        {
            вырезатьToolStripMenuItem_Click(sender, e);
        }

        private void button8_Click(object sender, EventArgs e) // Вставить
        {
            вставитьToolStripMenuItem_Click(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            string[] lines = richTextBox1.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells[0].Value = "INFO";
                    dataGridView1.Rows[rowIndex].Cells[1].Value = (i + 1).ToString();
                    dataGridView1.Rows[rowIndex].Cells[2].Value = "1";
                    dataGridView1.Rows[rowIndex].Cells[3].Value = $"Обработана строка: {lines[i]}";
                }
            }

            MessageBox.Show($"Анализ завершен! Обработано строк: {dataGridView1.Rows.Count}",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            вызовСправкиToolStripMenuItem_Click(sender, e);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            оПрограммеToolStripMenuItem_Click(sender, e);
        }
    }
}

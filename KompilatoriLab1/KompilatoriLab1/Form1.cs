using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScintillaNET;

namespace KompilatoriLab1
{
    public partial class Form1 : Form
    {
        private readonly PascalEnumScanner _scanner = new PascalEnumScanner();

        private enum ParserMode
        {
            PascalEnum,
            ArithmeticExpression
        }
        private ParserMode _currentMode = ParserMode.PascalEnum;
        private ArithmeticExpressionParser _arithmeticParser;

        private class EditorTab
        {
            public Scintilla Editor { get; set; }
            public string FilePath { get; set; }
            public bool IsDirty { get; set; }
            public string TabTitle { get; set; }
        }

        private readonly List<EditorTab> _editorTabs = new List<EditorTab>();
        private int _currentEditorIndex = 0;
        private int _nextTabId = 1;

        private DataGridView _resultGridView;

        private bool _showAstTable = false;
        private string _lastAstText = "AST не построено.";
        private int _lastTotalErrors = 0;
        private readonly List<ResultRowInfo> _lastErrorRows = new List<ResultRowInfo>();

        private string _currentLanguage = "ru";
        private readonly Dictionary<string, Dictionary<string, string>> _strings = new Dictionary<string, Dictionary<string, string>>();

        private int _editorFontSize = 11;
        private const int STYLE_DEFAULT = 32;

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
            _arithmeticParser = new ArithmeticExpressionParser();
            InitializeLocalization();
            SetupResultGrid();
            SetupEventHandlers();
            ApplyLanguage(_currentLanguage);
            UpdateStatusBar();
            CreateNewEditorTab();
            режимPascalToolStripMenuItem.Checked = true;
        }

        private void InitializeLocalization()
        {
            var ruStrings = new Dictionary<string, string>
            {
                { "ready", "Готов" },
                { "line_col", "Строка: {0}, Кол: {1}" },
                { "files_count", "Файлов: {0}" },
                { "ast_not_built", "AST не построено." },
                { "analysis_complete", "Анализ завершён. Ошибок не обнаружено." },
                { "analysis_complete_with_errors", "Анализ завершён. Ошибок: {0}" },
                { "analysis_error", "Ошибка при анализе: {0}" },
                { "save_changes_title", "Подтверждение" },
                { "save_changes_message", "Текст был изменён. Сохранить изменения?" },
                { "untitled", "Без имени" },
                { "fragment_column", "Неверный фрагмент" },
                { "location_column", "Местоположение" },
                { "description_column", "Описание" },
                { "ast_column", "AST" },
                { "total_errors", "Общее количество ошибок:" },
                { "output_tab_errors", "Ошибки" },
                { "output_tab_ast", "AST" },
                { "mode_pascal", "Режим: Pascal Enum" },
                { "mode_arithmetic", "Режим: Arithmetic Expression" }
            };

            var enStrings = new Dictionary<string, string>
            {
                { "ready", "Ready" },
                { "line_col", "Line: {0}, Col: {1}" },
                { "files_count", "Files: {0}" },
                { "ast_not_built", "AST not built." },
                { "analysis_complete", "Analysis complete. No errors found." },
                { "analysis_complete_with_errors", "Analysis complete. Errors: {0}" },
                { "analysis_error", "Analysis error: {0}" },
                { "save_changes_title", "Confirm" },
                { "save_changes_message", "Text has been changed. Save changes?" },
                { "untitled", "Untitled" },
                { "fragment_column", "Invalid fragment" },
                { "location_column", "Location" },
                { "description_column", "Description" },
                { "ast_column", "AST" },
                { "total_errors", "Total errors:" },
                { "output_tab_errors", "Errors" },
                { "output_tab_ast", "AST" },
                { "mode_pascal", "Mode: Pascal Enum" },
                { "mode_arithmetic", "Mode: Arithmetic Expression" }
            };

            _strings["ru"] = ruStrings;
            _strings["en"] = enStrings;
        }

        private string GetString(string key)
        {
            if (_strings.ContainsKey(_currentLanguage) && _strings[_currentLanguage].ContainsKey(key))
                return _strings[_currentLanguage][key];
            return key;
        }

        private void ApplyLanguage(string language)
        {
            _currentLanguage = language;

            файлToolStripMenuItem.Text = GetStringFromResource("file_menu", language);
            создатьToolStripMenuItem.Text = GetStringFromResource("new_menu", language);
            открытьToolStripMenuItem.Text = GetStringFromResource("open_menu", language);
            сохранитьToolStripMenuItem.Text = GetStringFromResource("save_menu", language);
            сохранитьКакToolStripMenuItem.Text = GetStringFromResource("save_as_menu", language);
            выходToolStripMenuItem.Text = GetStringFromResource("exit_menu", language);
            правкаToolStripMenuItem.Text = GetStringFromResource("edit_menu", language);
            отменитьToolStripMenuItem.Text = GetStringFromResource("undo_menu", language);
            повторитьToolStripMenuItem.Text = GetStringFromResource("redo_menu", language);
            вырезатьToolStripMenuItem.Text = GetStringFromResource("cut_menu", language);
            копироватьToolStripMenuItem.Text = GetStringFromResource("copy_menu", language);
            вставитьToolStripMenuItem.Text = GetStringFromResource("paste_menu", language);
            удалитьToolStripMenuItem.Text = GetStringFromResource("delete_menu", language);
            текстToolStripMenuItem.Text = GetStringFromResource("text_menu", language);
            пускToolStripMenuItem.Text = GetStringFromResource("run_menu", language);
            справкаToolStripMenuItem.Text = GetStringFromResource("help_menu", language);
            вызовСправкиToolStripMenuItem.Text = GetStringFromResource("help_call_menu", language);
            оПрограммеToolStripMenuItem.Text = GetStringFromResource("about_menu", language);
            режимToolStripMenuItem.Text = GetStringFromResource("mode_menu", language);
            режимPascalToolStripMenuItem.Text = GetStringFromResource("mode_pascal_item", language);
            режимArithmeticToolStripMenuItem.Text = GetStringFromResource("mode_arithmetic_item", language);

            постановкаЗадачиToolStripMenuItem.Text = GetStringFromResource("task_menu", language);
            грамматикаToolStripMenuItem.Text = GetStringFromResource("grammar_menu", language);
            классификацияГрамматикиToolStripMenuItem.Text = GetStringFromResource("grammar_class_menu", language);
            методАнализаToolStripMenuItem.Text = GetStringFromResource("method_menu", language);
            тестовыйПримерToolStripMenuItem.Text = GetStringFromResource("test_menu", language);
            списокЛитературыToolStripMenuItem.Text = GetStringFromResource("references_menu", language);

            языкToolStripMenuItem.Text = GetStringFromResource("language_menu", language);
            русскийToolStripMenuItem.Text = GetStringFromResource("russian_lang", language);
            englishToolStripMenuItem.Text = GetStringFromResource("english_lang", language);

            деревоASTToolStripMenuItem.Text = _showAstTable ? (_currentLanguage == "ru" ? "Ошибки" : "Errors") : "AST";

            UpdateStatusBar();

            if (_showAstTable)
                ShowAstTable();
            else
                ShowErrorTableFromLastResult();
        }

        private string GetStringFromResource(string key, string language)
        {
            var menuStrings = new Dictionary<string, Dictionary<string, string>>
            {
                ["ru"] = new Dictionary<string, string>
                {
                    ["file_menu"] = "Файл",
                    ["new_menu"] = "Создать",
                    ["open_menu"] = "Открыть",
                    ["save_menu"] = "Сохранить",
                    ["save_as_menu"] = "Сохранить как",
                    ["exit_menu"] = "Выход",
                    ["edit_menu"] = "Правка",
                    ["undo_menu"] = "Отменить",
                    ["redo_menu"] = "Повторить",
                    ["cut_menu"] = "Вырезать",
                    ["copy_menu"] = "Копировать",
                    ["paste_menu"] = "Вставить",
                    ["delete_menu"] = "Удалить",
                    ["text_menu"] = "Текст",
                    ["run_menu"] = "Пуск",
                    ["help_menu"] = "Справка",
                    ["help_call_menu"] = "Вызов справки",
                    ["about_menu"] = "О программе",
                    ["language_menu"] = "Язык",
                    ["russian_lang"] = "Русский",
                    ["english_lang"] = "English",
                    ["task_menu"] = "Постановка задачи",
                    ["grammar_menu"] = "Грамматика",
                    ["grammar_class_menu"] = "Классификация грамматики",
                    ["method_menu"] = "Метод анализа",
                    ["test_menu"] = "Тестовый пример",
                    ["references_menu"] = "Список литературы",
                    ["mode_menu"] = "Режим",
                    ["mode_pascal_item"] = "Pascal Enum (Объявление перечисления)",
                    ["mode_arithmetic_item"] = "Arithmetic Expression (Арифметическое выражение)"
                },
                ["en"] = new Dictionary<string, string>
                {
                    ["file_menu"] = "File",
                    ["new_menu"] = "New",
                    ["open_menu"] = "Open",
                    ["save_menu"] = "Save",
                    ["save_as_menu"] = "Save As",
                    ["exit_menu"] = "Exit",
                    ["edit_menu"] = "Edit",
                    ["undo_menu"] = "Undo",
                    ["redo_menu"] = "Redo",
                    ["cut_menu"] = "Cut",
                    ["copy_menu"] = "Copy",
                    ["paste_menu"] = "Paste",
                    ["delete_menu"] = "Delete",
                    ["text_menu"] = "Text",
                    ["run_menu"] = "Run",
                    ["help_menu"] = "Help",
                    ["help_call_menu"] = "Call Help",
                    ["about_menu"] = "About",
                    ["language_menu"] = "Language",
                    ["russian_lang"] = "Russian",
                    ["english_lang"] = "English",
                    ["task_menu"] = "Task Statement",
                    ["grammar_menu"] = "Grammar",
                    ["grammar_class_menu"] = "Grammar Classification",
                    ["method_menu"] = "Analysis Method",
                    ["test_menu"] = "Test Example",
                    ["references_menu"] = "References",
                    ["mode_menu"] = "Mode",
                    ["mode_pascal_item"] = "Pascal Enum",
                    ["mode_arithmetic_item"] = "Arithmetic Expression"
                }
            };

            if (menuStrings.ContainsKey(language) && menuStrings[language].ContainsKey(key))
                return menuStrings[language][key];
            return key;
        }

        private void SwitchToPascalMode()
        {
            _currentMode = ParserMode.PascalEnum;
            режимPascalToolStripMenuItem.Checked = true;
            режимArithmeticToolStripMenuItem.Checked = false;
            statusLabel.Text = _currentLanguage == "ru" ? "Режим: Pascal Enum" : "Mode: Pascal Enum";

            _showAstTable = false;
            деревоASTToolStripMenuItem.Text = "AST";
            _lastAstText = GetString("ast_not_built");
            _lastErrorRows.Clear();
            _lastTotalErrors = 0;
            ShowErrorTableFromLastResult();
        }

        private void SwitchToArithmeticMode()
        {
            _currentMode = ParserMode.ArithmeticExpression;
            режимPascalToolStripMenuItem.Checked = false;
            режимArithmeticToolStripMenuItem.Checked = true;
            statusLabel.Text = _currentLanguage == "ru" ? "Режим: Arithmetic Expression" : "Mode: Arithmetic Expression";

            _showAstTable = false;
            деревоASTToolStripMenuItem.Text = "AST";
            _lastAstText = "";
            _lastErrorRows.Clear();
            _lastTotalErrors = 0;
            ShowErrorTableFromLastResult();
        }

        private void CreateNewEditorTab(string filePath = null, string content = null)
        {
            var editor = new Scintilla();
            editor.Dock = DockStyle.Fill;
            editor.Text = content ?? string.Empty;

            SetupScintillaStyles(editor);

            editor.Margins[0].Width = 40;
            editor.Margins[0].Type = MarginType.Number;

            editor.IndentationGuides = IndentView.LookBoth;
            editor.TabWidth = 4;
            editor.UseTabs = false;
            editor.IndentWidth = 4;

            editor.TextChanged += (s, e) => OnEditorTextChanged(editor);
            editor.UpdateUI += (s, e) => OnEditorUpdateUI(editor);

            string tabTitle = string.IsNullOrEmpty(filePath)
                ? $"{GetString("untitled")}{_nextTabId++}"
                : Path.GetFileName(filePath);

            var tab = new EditorTab
            {
                Editor = editor,
                FilePath = filePath ?? string.Empty,
                IsDirty = !string.IsNullOrEmpty(content),
                TabTitle = tabTitle
            };

            _editorTabs.Add(tab);

            var tabPage = new TabPage(tabTitle);
            tabPage.Controls.Add(editor);
            tabControlEditor.TabPages.Add(tabPage);

            tabControlEditor.SelectedTab = tabPage;
            _currentEditorIndex = _editorTabs.Count - 1;

            UpdateStatusBar();
        }

        private void SetupScintillaStyles(Scintilla editor)
        {
            editor.StyleResetDefault();
            editor.Styles[STYLE_DEFAULT].Font = "Consolas";
            editor.Styles[STYLE_DEFAULT].Size = _editorFontSize;
            editor.Styles[STYLE_DEFAULT].BackColor = Color.White;
            editor.Styles[STYLE_DEFAULT].ForeColor = Color.Black;
            editor.StyleClearAll();

            editor.Lexer = Lexer.Cpp;

            string keywords = "type true false and array begin case const div do downto else end file for function goto if in label mod nil not of or packed procedure program record repeat set then to until var while with";
            editor.SetKeywords(0, keywords);

            editor.Styles[5].ForeColor = Color.Blue;
            editor.Styles[5].Bold = true;
            editor.Styles[4].ForeColor = Color.DarkOrange;
            editor.Styles[10].ForeColor = Color.Red;
            editor.Styles[10].Bold = true;
            editor.Styles[1].ForeColor = Color.Green;
            editor.Styles[1].Italic = true;
            editor.Styles[2].ForeColor = Color.Green;
            editor.Styles[2].Italic = true;
            editor.Styles[6].ForeColor = Color.DarkMagenta;
            editor.Styles[7].ForeColor = Color.DarkMagenta;
            editor.Styles[9].ForeColor = Color.DarkCyan;
            editor.Styles[11].ForeColor = Color.Black;
        }

        private void SetupResultGrid()
        {
            _resultGridView = dataGridView1;
            _resultGridView.AllowUserToAddRows = false;
            _resultGridView.ReadOnly = true;
            _resultGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _resultGridView.MultiSelect = false;
            _resultGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            _resultGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            SetupErrorGrid();
        }

        private void SetupErrorGrid()
        {
            if (_resultGridView == null) return;

            _resultGridView.Columns.Clear();
            _resultGridView.Rows.Clear();

            _resultGridView.Columns.Add("Fragment", GetString("fragment_column"));
            _resultGridView.Columns.Add("Location", GetString("location_column"));
            _resultGridView.Columns.Add("Description", GetString("description_column"));

            if (_resultGridView.Columns.Count > 0)
                _resultGridView.Columns[0].Width = 240;
            if (_resultGridView.Columns.Count > 1)
                _resultGridView.Columns[1].Width = 220;
            if (_resultGridView.Columns.Count > 2)
                _resultGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (DataGridViewColumn column in _resultGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void SetupAstGrid()
        {
            if (_resultGridView == null) return;

            _resultGridView.Columns.Clear();
            _resultGridView.Rows.Clear();

            _resultGridView.Columns.Add("Ast", GetString("ast_column"));
            if (_resultGridView.Columns.Count > 0)
                _resultGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (DataGridViewColumn column in _resultGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void OnEditorTextChanged(Scintilla editor)
        {
            var tab = GetCurrentEditorTab();
            if (tab != null && tab.Editor == editor && !tab.IsDirty)
            {
                tab.IsDirty = true;
                UpdateTabTitle(tab);
                UpdateStatusBar();
            }
        }

        private void OnEditorUpdateUI(Scintilla editor)
        {
            int line = editor.CurrentLine + 1;
            int column = editor.GetColumn(editor.CurrentPosition);
            cursorPositionLabel.Text = string.Format(GetString("line_col"), line, column + 1);
        }

        private EditorTab GetCurrentEditorTab()
        {
            if (tabControlEditor.SelectedIndex >= 0 && tabControlEditor.SelectedIndex < _editorTabs.Count)
                return _editorTabs[tabControlEditor.SelectedIndex];
            return null;
        }

        private Scintilla GetCurrentEditor()
        {
            var tab = GetCurrentEditorTab();
            return tab?.Editor;
        }

        private void UpdateTabTitle(EditorTab tab)
        {
            int index = _editorTabs.IndexOf(tab);
            if (index >= 0 && index < tabControlEditor.TabPages.Count)
            {
                string title = tab.IsDirty ? $"{tab.TabTitle}*" : tab.TabTitle;
                tabControlEditor.TabPages[index].Text = title;
            }
        }

        private void SetupEventHandlers()
        {
            создатьToolStripMenuItem.Click += (s, e) => CreateNewEditorTab();
            открытьToolStripMenuItem.Click += (s, e) => OpenFile();
            сохранитьToolStripMenuItem.Click += (s, e) => SaveFile(GetCurrentEditorTab());
            сохранитьКакToolStripMenuItem.Click += (s, e) => SaveFileAs(GetCurrentEditorTab());
            выходToolStripMenuItem.Click += (s, e) => Close();

            отменитьToolStripMenuItem.Click += (s, e) => GetCurrentEditor()?.Undo();
            повторитьToolStripMenuItem.Click += (s, e) => GetCurrentEditor()?.Redo();
            вырезатьToolStripMenuItem.Click += (s, e) => GetCurrentEditor()?.Cut();
            копироватьToolStripMenuItem.Click += (s, e) => GetCurrentEditor()?.Copy();
            вставитьToolStripMenuItem.Click += (s, e) => GetCurrentEditor()?.Paste();
            удалитьToolStripMenuItem.Click += (s, e) => GetCurrentEditor()?.Clear();

            пускToolStripMenuItem.Click += (s, e) => RunFullAnalysis();
            деревоASTToolStripMenuItem.Click += (s, e) => ToggleAstMenu();

            режимPascalToolStripMenuItem.Click += (s, e) => SwitchToPascalMode();
            режимArithmeticToolStripMenuItem.Click += (s, e) => SwitchToArithmeticMode();

            button1.Click += (s, e) => CreateNewEditorTab();
            button2.Click += (s, e) => OpenFile();
            button3.Click += (s, e) => SaveFile(GetCurrentEditorTab());
            button4.Click += (s, e) => GetCurrentEditor()?.Undo();
            button5.Click += (s, e) => GetCurrentEditor()?.Cut();
            button6.Click += (s, e) => GetCurrentEditor()?.Copy();
            button7.Click += (s, e) => GetCurrentEditor()?.Paste();
            button8.Click += (s, e) => GetCurrentEditor()?.Clear();
            button9.Click += (s, e) => RunFullAnalysis();
            button10.Click += (s, e) => ShowHelp();
            button11.Click += (s, e) => ShowAbout();
            button12.Click += (s, e) => GetCurrentEditor()?.Redo();

            русскийToolStripMenuItem.Click += (s, e) => ApplyLanguage("ru");
            englishToolStripMenuItem.Click += (s, e) => ApplyLanguage("en");

            постановкаЗадачиToolStripMenuItem.Click += (s, e) => InsertTaskText();
            грамматикаToolStripMenuItem.Click += (s, e) => InsertGrammarText();
            классификацияГрамматикиToolStripMenuItem.Click += (s, e) => InsertGrammarClassText();
            методАнализаToolStripMenuItem.Click += (s, e) => InsertMethodText();
            тестовыйПримерToolStripMenuItem.Click += (s, e) => InsertTestExample();
            списокЛитературыToolStripMenuItem.Click += (s, e) => InsertReferences();

            вызовСправкиToolStripMenuItem.Click += (s, e) => ShowHelp();
            оПрограммеToolStripMenuItem.Click += (s, e) => ShowAbout();

            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;

            tabControlEditor.SelectedIndexChanged += (s, e) =>
            {
                _currentEditorIndex = tabControlEditor.SelectedIndex;
                UpdateStatusBar();
            };

            _resultGridView.CellClick += DataGridView1_CellClick;

            this.FormClosing += Form1_FormClosing;
        }

        private void ToggleAstMenu()
        {
            if (_showAstTable)
            {
                _showAstTable = false;
                деревоASTToolStripMenuItem.Text = "AST";
                ShowErrorTableFromLastResult();
            }
            else
            {
                _showAstTable = true;
                деревоASTToolStripMenuItem.Text = _currentLanguage == "ru" ? "Ошибки" : "Errors";
                ShowAstTable();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        string content = File.ReadAllText(file);
                        CreateNewEditorTab(file, content);
                    }
                }
            }
        }

        private void OpenFile()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Pascal files (*.pas)|*.pas|Все файлы (*.*)|*.*";
                dialog.Title = "Открыть файл";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in dialog.FileNames)
                    {
                        string content = File.ReadAllText(fileName);
                        CreateNewEditorTab(fileName, content);
                    }
                }
            }
        }

        private void SaveFile(EditorTab tab)
        {
            if (tab == null) return;

            if (string.IsNullOrWhiteSpace(tab.FilePath))
            {
                SaveFileAs(tab);
                return;
            }

            File.WriteAllText(tab.FilePath, tab.Editor.Text);
            tab.IsDirty = false;
            UpdateTabTitle(tab);
            UpdateStatusBar();
        }

        private void SaveFileAs(EditorTab tab)
        {
            if (tab == null) return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt|Pascal files (*.pas)|*.pas|Все файлы (*.*)|*.*";
                dialog.Title = "Сохранить файл";
                dialog.FileName = string.IsNullOrWhiteSpace(tab.FilePath)
                    ? "program.txt"
                    : Path.GetFileName(tab.FilePath);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tab.FilePath = dialog.FileName;
                    File.WriteAllText(tab.FilePath, tab.Editor.Text);
                    tab.IsDirty = false;
                    tab.TabTitle = Path.GetFileName(tab.FilePath);
                    UpdateTabTitle(tab);
                    UpdateStatusBar();
                }
            }
        }

        private void UpdateStatusBar()
        {
            string modeText = _currentMode == ParserMode.PascalEnum
                ? (_currentLanguage == "ru" ? "Режим: Pascal Enum" : "Mode: Pascal Enum")
                : (_currentLanguage == "ru" ? "Режим: Arithmetic Expression" : "Mode: Arithmetic Expression");
            statusLabel.Text = modeText;
            fileCountLabel.Text = string.Format(GetString("files_count"), _editorTabs.Count);

            var tab = GetCurrentEditorTab();
            if (tab != null)
            {
                string fileName = string.IsNullOrWhiteSpace(tab.FilePath)
                    ? GetString("untitled")
                    : Path.GetFileName(tab.FilePath);

                Text = $"{fileName}{(tab.IsDirty ? "*" : "")} - Семантический анализатор";
            }
        }

        private void RunFullAnalysis()
        {
            try
            {
                var editor = GetCurrentEditor();
                if (editor == null) return;

                string text = editor.Text ?? string.Empty;

                _showAstTable = false;
                _lastErrorRows.Clear();

                if (_currentMode == ParserMode.PascalEnum)
                {
                    _lastAstText = GetString("ast_not_built");
                    _lastTotalErrors = 0;

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

                    if (totalErrors == 0)
                    {
                        MessageBox.Show(
                            GetString("analysis_complete"),
                            GetString("analysis_complete"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            string.Format(GetString("analysis_complete_with_errors"), totalErrors),
                            "Анализ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    _arithmeticParser.ClearVariables();

                    var result = _arithmeticParser.Parse(text);

                    _lastTotalErrors = result.Errors.Count;
                    foreach (var error in result.Errors)
                    {
                        _lastErrorRows.Add(new ResultRowInfo
                        {
                            Fragment = error.Fragment,
                            Location = $"позиция {error.Position}",
                            Description = error.Description,
                            Tag = error
                        });
                    }

                    var astBuilder = new System.Text.StringBuilder();
                    astBuilder.AppendLine(result.AstText);

                    if (result.Success)
                    {
                        astBuilder.AppendLine();
                        astBuilder.AppendLine("=== ТЕТРАДЫ (Quadruples) ===");
                        for (int i = 0; i < result.Tetrads.Count; i++)
                        {
                            astBuilder.AppendLine($"{i + 1}: {result.Tetrads[i]}");
                        }

                        astBuilder.AppendLine();
                        astBuilder.AppendLine("=== ПОЛИЗ (Polish Notation) ===");
                        astBuilder.AppendLine(result.PolishNotation);

                        if (result.ComputedValue.HasValue)
                        {
                            astBuilder.AppendLine();
                            astBuilder.AppendLine($"=== РЕЗУЛЬТАТ ВЫЧИСЛЕНИЯ ===");
                            astBuilder.AppendLine($"Значение: {result.ComputedValue.Value}");
                        }
                    }

                    _lastAstText = astBuilder.ToString();

                    if (result.Success && result.ComputedValue.HasValue)
                    {
                        MessageBox.Show(
                            $"Анализ завершён!\nПОЛИЗ: {result.PolishNotation}\nРезультат: {result.ComputedValue.Value}",
                            "Анализ арифметического выражения",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else if (result.Success && result.Errors.Count == 0)
                    {
                        MessageBox.Show(
                            $"Анализ завершён!\nПОЛИЗ: {result.PolishNotation}",
                            "Анализ арифметического выражения",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            $"Ошибок: {result.Errors.Count}",
                            "Анализ арифметического выражения",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }

                ShowErrorTableFromLastResult();
                деревоASTToolStripMenuItem.Text = "AST";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(GetString("analysis_error"), ex.Message),
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowAstTable()
        {
            SetupAstGrid();

            string[] lines = (_lastAstText ?? GetString("ast_not_built"))
                .Replace("\r\n", "\n")
                .Split('\n');

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                int rowIndex = _resultGridView.Rows.Add(line);
                var row = _resultGridView.Rows[rowIndex];
                row.DefaultCellStyle.BackColor = Color.Honeydew;
                row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                row.DefaultCellStyle.Font = new Font("Consolas", 10F, FontStyle.Regular);
            }
        }

        private void ShowErrorTableFromLastResult()
        {
            SetupErrorGrid();

            foreach (var rowInfo in _lastErrorRows)
            {
                AddGridErrorRow(rowInfo.Fragment, rowInfo.Location, rowInfo.Description, rowInfo.Tag);
            }

            AddTotalErrorsRow(_lastTotalErrors);
        }

        private void AddGridErrorRow(string fragment, string location, string description, object tag)
        {
            if (_resultGridView == null) return;

            int rowIndex = _resultGridView.Rows.Add(fragment, location, description);
            var row = _resultGridView.Rows[rowIndex];
            row.Tag = tag;
            row.DefaultCellStyle.BackColor = Color.MistyRose;
            row.DefaultCellStyle.ForeColor = Color.DarkRed;
        }

        private void AddTotalErrorsRow(int count)
        {
            if (_resultGridView == null) return;

            int totalRowIndex = _resultGridView.Rows.Add(
                GetString("total_errors"),
                "",
                count.ToString());

            var row = _resultGridView.Rows[totalRowIndex];
            row.DefaultCellStyle.BackColor = Color.Gainsboro;
            row.DefaultCellStyle.Font = new Font(_resultGridView.Font, FontStyle.Bold);
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = _resultGridView.Rows[e.RowIndex];

            if (row.Tag is PascalEnumParser.SyntaxError syntaxError)
            {
                GoToAbsolutePosition(syntaxError.AbsolutePosition);
                return;
            }

            if (row.Tag is PascalEnumScanner.Error lexicalError)
            {
                GoToAbsolutePosition(lexicalError.AbsolutePosition);
                return;
            }

            if (row.Tag is ArithmeticExpressionParser.SyntaxError arithmeticError)
            {
                GoToAbsolutePosition(arithmeticError.Position);
                return;
            }
        }

        private void GoToAbsolutePosition(int position)
        {
            var editor = GetCurrentEditor();
            if (editor == null) return;

            if (position < 0) position = 0;
            if (position > editor.TextLength) position = editor.TextLength;

            editor.Focus();
            editor.GotoPosition(position);
        }

        private void InsertTaskText()
        {
            var editor = GetCurrentEditor();
            if (editor != null)
            {
                editor.Text = @"Постановка задачи

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
        }

        private void InsertGrammarText()
        {
            var editor = GetCurrentEditor();
            if (editor != null)
            {
                editor.Text = @"Грамматика

<объявление_перечисления> ::= type <идентификатор> = ( <список_идентификаторов> ) ;

<список_идентификаторов> ::= <идентификтор>
                           | <идентификатор> , <список_идентификаторов>

<идентификатор> ::= id";
            }
        }

        private void InsertGrammarClassText()
        {
            var editor = GetCurrentEditor();
            if (editor != null)
            {
                editor.Text = @"Классификация грамматики

Данная грамматика является контекстно-свободной.
Она описывает синтаксическую конструкцию объявления перечислимого типа Pascal.";
            }
        }

        private void InsertMethodText()
        {
            var editor = GetCurrentEditor();
            if (editor != null)
            {
                editor.Text = @"Метод анализа

Используется нисходящий синтаксический анализ.
Сначала выполняется лексический анализ, затем синтаксический и семантический анализ.
В процессе анализа строится абстрактное синтаксическое дерево AST.
При обнаружении ошибки программа продолжает анализ и выводит найденные ошибки в таблицу.";
            }
        }

        private void InsertTestExample()
        {
            var editor = GetCurrentEditor();
            if (editor != null)
            {
                editor.Text = @"type Season = (Winter, Spring, Summer, Autumn);";
            }
        }

        private void InsertReferences()
        {
            var editor = GetCurrentEditor();
            if (editor != null)
            {
                editor.Text = @"Список литературы

1. Ахо А., Ульман Д. Компиляторы: принципы, технологии и инструменты.
2. Вирт Н. Алгоритмы + структуры данных = программы.
3. Документация Microsoft по Windows Forms.";
            }
        }

        private void ShowHelp()
        {
            string helpText = _currentLanguage == "ru"
                ? @"Горячие клавиши:
Ctrl+N - Новый файл
Ctrl+O - Открыть файл
Ctrl+S - Сохранить файл
Ctrl+Z - Отменить
Ctrl+Y - Повторить
Ctrl+X - Вырезать
Ctrl+C - Копировать
Ctrl+V - Вставить
Delete - Удалить
F5 - Запустить анализ
F1 - Справка

Для изменения размера текста используйте Ctrl+Колесо мыши в окне редактора.

Режимы работы:
- Pascal Enum: анализ объявлений перечислений Pascal
- Arithmetic Expression: анализ арифметических выражений, построение тетрад и ПОЛИЗа"
                : @"Shortcut keys:
Ctrl+N - New file
Ctrl+O - Open file
Ctrl+S - Save file
Ctrl+Z - Undo
Ctrl+Y - Redo
Ctrl+X - Cut
Ctrl+C - Copy
Ctrl+V - Paste
Delete - Delete
F5 - Run analysis
F1 - Help

To change text size, use Ctrl+Mouse wheel in the editor window.

Modes:
- Pascal Enum: analysis of Pascal enumeration declarations
- Arithmetic Expression: arithmetic expression analysis, quadruples and POLISH notation";

            MessageBox.Show(helpText, _currentLanguage == "ru" ? "Справка" : "Help",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            string aboutText = _currentLanguage == "ru"
                ? "Текстовый редактор с семантическим анализатором\nВерсия 2.0\n\nРазработано для анализа:\n- Объявлений перечисления Pascal\n- Арифметических выражений (тетрады, ПОЛИЗ, вычисление)"
                : "Text editor with semantic analyzer\nVersion 2.0\n\nDeveloped for analysis:\n- Pascal enumeration declarations\n- Arithmetic expressions (quadruples, POLISH notation, evaluation)";

            MessageBox.Show(aboutText, _currentLanguage == "ru" ? "О программе" : "About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var tab in _editorTabs)
            {
                if (tab.IsDirty)
                {
                    var result = MessageBox.Show(
                        $"Файл '{tab.TabTitle}' был изменён. Сохранить изменения?",
                        GetString("save_changes_title"),
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        SaveFile(tab);
                    }
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                var editor = GetCurrentEditor();
                if (editor != null)
                {
                    if (e.Delta > 0 && _editorFontSize < 30)
                        _editorFontSize++;
                    else if (e.Delta < 0 && _editorFontSize > 6)
                        _editorFontSize--;

                    editor.Styles[STYLE_DEFAULT].Size = _editorFontSize;
                }
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }
    }
}

namespace KompilatoriLab1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem создатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьКакToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem правкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem отменитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem повторитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вырезатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem копироватьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вставитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem текстToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem постановкаЗадачиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem грамматикаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem классификацияГрамматикиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem методАнализаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem тестовыйПримерToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem списокЛитературыToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem пускToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem деревоASTToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вызовСправкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;

        // Элементы меню для интернационализации и режимов
        private System.Windows.Forms.ToolStripMenuItem языкToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem русскийToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem режимToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem режимPascalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem режимArithmeticToolStripMenuItem;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControlEditor;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel cursorPositionLabel;
        private System.Windows.Forms.ToolStripStatusLabel fileCountLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьКакToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.правкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.отменитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.повторитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.вырезатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.копироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.вставитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.текстToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.постановкаЗадачиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.грамматикаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.классификацияГрамматикиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.методАнализаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.тестовыйПримерToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.списокЛитературыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.пускToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.деревоASTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.режимToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.режимPascalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.режимArithmeticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.языкToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.русскийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.вызовСправкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop = new System.Windows.Forms.Panel();
            this.button12 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControlEditor = new System.Windows.Forms.TabControl();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.cursorPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fileCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.правкаToolStripMenuItem,
            this.текстToolStripMenuItem,
            this.пускToolStripMenuItem,
            this.деревоASTToolStripMenuItem,
            this.режимToolStripMenuItem,
            this.языкToolStripMenuItem,
            this.справкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1219, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";

            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьToolStripMenuItem,
            this.открытьToolStripMenuItem,
            this.сохранитьToolStripMenuItem,
            this.сохранитьКакToolStripMenuItem,
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(59, 26);
            this.файлToolStripMenuItem.Text = "Файл";

            this.создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            this.создатьToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.создатьToolStripMenuItem.Text = "Создать";
            this.создатьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N;

            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.открытьToolStripMenuItem.Text = "Открыть";
            this.открытьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;

            this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.сохранитьToolStripMenuItem.Text = "Сохранить";
            this.сохранитьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;

            this.сохранитьКакToolStripMenuItem.Name = "сохранитьКакToolStripMenuItem";
            this.сохранитьКакToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.сохранитьКакToolStripMenuItem.Text = "Сохранить как";

            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(192, 26);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4;

            this.правкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.отменитьToolStripMenuItem,
            this.повторитьToolStripMenuItem,
            this.вырезатьToolStripMenuItem,
            this.копироватьToolStripMenuItem,
            this.вставитьToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.правкаToolStripMenuItem.Name = "правкаToolStripMenuItem";
            this.правкаToolStripMenuItem.Size = new System.Drawing.Size(74, 26);
            this.правкаToolStripMenuItem.Text = "Правка";

            this.отменитьToolStripMenuItem.Name = "отменитьToolStripMenuItem";
            this.отменитьToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.отменитьToolStripMenuItem.Text = "Отменить";
            this.отменитьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z;

            this.повторитьToolStripMenuItem.Name = "повторитьToolStripMenuItem";
            this.повторитьToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.повторитьToolStripMenuItem.Text = "Повторить";
            this.повторитьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y;

            this.вырезатьToolStripMenuItem.Name = "вырезатьToolStripMenuItem";
            this.вырезатьToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.вырезатьToolStripMenuItem.Text = "Вырезать";
            this.вырезатьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X;

            this.копироватьToolStripMenuItem.Name = "копироватьToolStripMenuItem";
            this.копироватьToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.копироватьToolStripMenuItem.Text = "Копировать";
            this.копироватьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;

            this.вставитьToolStripMenuItem.Name = "вставитьToolStripMenuItem";
            this.вставитьToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.вставитьToolStripMenuItem.Text = "Вставить";
            this.вставитьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V;

            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;

            this.текстToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.постановкаЗадачиToolStripMenuItem,
            this.грамматикаToolStripMenuItem,
            this.классификацияГрамматикиToolStripMenuItem,
            this.методАнализаToolStripMenuItem,
            this.тестовыйПримерToolStripMenuItem,
            this.списокЛитературыToolStripMenuItem});
            this.текстToolStripMenuItem.Name = "текстToolStripMenuItem";
            this.текстToolStripMenuItem.Size = new System.Drawing.Size(59, 26);
            this.текстToolStripMenuItem.Text = "Текст";

            this.постановкаЗадачиToolStripMenuItem.Name = "постановкаЗадачиToolStripMenuItem";
            this.постановкаЗадачиToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.постановкаЗадачиToolStripMenuItem.Text = "Постановка задачи";

            this.грамматикаToolStripMenuItem.Name = "грамматикаToolStripMenuItem";
            this.грамматикаToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.грамматикаToolStripMenuItem.Text = "Грамматика";

            this.классификацияГрамматикиToolStripMenuItem.Name = "классификацияГрамматикиToolStripMenuItem";
            this.классификацияГрамматикиToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.классификацияГрамматикиToolStripMenuItem.Text = "Классификация грамматики";

            this.методАнализаToolStripMenuItem.Name = "методАнализаToolStripMenuItem";
            this.методАнализаToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.методАнализаToolStripMenuItem.Text = "Метод анализа";

            this.тестовыйПримерToolStripMenuItem.Name = "тестовыйПримерToolStripMenuItem";
            this.тестовыйПримерToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.тестовыйПримерToolStripMenuItem.Text = "Тестовый пример";

            this.списокЛитературыToolStripMenuItem.Name = "списокЛитературыToolStripMenuItem";
            this.списокЛитературыToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.списокЛитературыToolStripMenuItem.Text = "Список литературы";

            this.пускToolStripMenuItem.Name = "пускToolStripMenuItem";
            this.пускToolStripMenuItem.Size = new System.Drawing.Size(55, 26);
            this.пускToolStripMenuItem.Text = "Пуск";
            this.пускToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;

            this.деревоASTToolStripMenuItem.Name = "деревоASTToolStripMenuItem";
            this.деревоASTToolStripMenuItem.Size = new System.Drawing.Size(49, 26);
            this.деревоASTToolStripMenuItem.Text = "AST";

            this.режимToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.режимPascalToolStripMenuItem,
            this.режимArithmeticToolStripMenuItem});
            this.режимToolStripMenuItem.Name = "режимToolStripMenuItem";
            this.режимToolStripMenuItem.Size = new System.Drawing.Size(65, 26);
            this.режимToolStripMenuItem.Text = "Режим";

            this.режимPascalToolStripMenuItem.Name = "режимPascalToolStripMenuItem";
            this.режимPascalToolStripMenuItem.Size = new System.Drawing.Size(280, 26);
            this.режимPascalToolStripMenuItem.Text = "Pascal Enum (Объявление перечисления)";
            this.режимPascalToolStripMenuItem.Tag = "pascal";
            this.режимPascalToolStripMenuItem.Checked = true;

            this.режимArithmeticToolStripMenuItem.Name = "режимArithmeticToolStripMenuItem";
            this.режимArithmeticToolStripMenuItem.Size = new System.Drawing.Size(280, 26);
            this.режимArithmeticToolStripMenuItem.Text = "Arithmetic Expression (Арифметическое выражение)";
            this.режимArithmeticToolStripMenuItem.Tag = "arithmetic";

            this.языкToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.русскийToolStripMenuItem,
            this.englishToolStripMenuItem});
            this.языкToolStripMenuItem.Name = "языкToolStripMenuItem";
            this.языкToolStripMenuItem.Size = new System.Drawing.Size(63, 26);
            this.языкToolStripMenuItem.Text = "Язык";

            this.русскийToolStripMenuItem.Name = "русскийToolStripMenuItem";
            this.русскийToolStripMenuItem.Size = new System.Drawing.Size(150, 26);
            this.русскийToolStripMenuItem.Text = "Русский";
            this.русскийToolStripMenuItem.Tag = "ru";

            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(150, 26);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Tag = "en";

            this.справкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.вызовСправкиToolStripMenuItem,
            this.оПрограммеToolStripMenuItem});
            this.справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            this.справкаToolStripMenuItem.Size = new System.Drawing.Size(81, 26);
            this.справкаToolStripMenuItem.Text = "Справка";

            this.вызовСправкиToolStripMenuItem.Name = "вызовСправкиToolStripMenuItem";
            this.вызовСправкиToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.вызовСправкиToolStripMenuItem.Text = "Вызов справки";
            this.вызовСправкиToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;

            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            this.оПрограммеToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.оПрограммеToolStripMenuItem.Text = "О программе";

            this.panelTop.Controls.Add(this.button12);
            this.panelTop.Controls.Add(this.button11);
            this.panelTop.Controls.Add(this.button10);
            this.panelTop.Controls.Add(this.button9);
            this.panelTop.Controls.Add(this.button8);
            this.panelTop.Controls.Add(this.button7);
            this.panelTop.Controls.Add(this.button6);
            this.panelTop.Controls.Add(this.button5);
            this.panelTop.Controls.Add(this.button4);
            this.panelTop.Controls.Add(this.button3);
            this.panelTop.Controls.Add(this.button2);
            this.panelTop.Controls.Add(this.button1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 30);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1219, 76);
            this.panelTop.TabIndex = 1;

            this.button1.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_создать_50;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Location = new System.Drawing.Point(8, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 62);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;

            this.button2.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_добавить_файл_50__1_;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button2.Location = new System.Drawing.Point(109, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 62);
            this.button2.TabIndex = 1;
            this.button2.UseVisualStyleBackColor = true;

            this.button3.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_сохранить_файл_99;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button3.Location = new System.Drawing.Point(210, 8);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(95, 62);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = true;

            this.button4.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_назад_100;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button4.Location = new System.Drawing.Point(311, 8);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(95, 62);
            this.button4.TabIndex = 3;
            this.button4.UseVisualStyleBackColor = true;

            this.button5.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_вырезать_файл_64;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button5.Location = new System.Drawing.Point(493, 11);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(95, 62);
            this.button5.TabIndex = 4;
            this.button5.UseVisualStyleBackColor = true;

            this.button6.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_копировать_файл_48;
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button6.Location = new System.Drawing.Point(594, 11);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(95, 62);
            this.button6.TabIndex = 5;
            this.button6.UseVisualStyleBackColor = true;

            this.button7.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_вставить_50__1_;
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button7.Location = new System.Drawing.Point(695, 14);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(95, 62);
            this.button7.TabIndex = 6;
            this.button7.UseVisualStyleBackColor = true;

            this.button8.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_удалить_файл_64;
            this.button8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button8.Location = new System.Drawing.Point(796, 11);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(95, 62);
            this.button8.TabIndex = 7;
            this.button8.UseVisualStyleBackColor = true;

            this.button9.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_начало_50;
            this.button9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button9.Location = new System.Drawing.Point(897, 11);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(95, 62);
            this.button9.TabIndex = 8;
            this.button9.UseVisualStyleBackColor = true;

            this.button10.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_резюме_80;
            this.button10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button10.Location = new System.Drawing.Point(998, 14);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(95, 62);
            this.button10.TabIndex = 9;
            this.button10.UseVisualStyleBackColor = true;

            this.button11.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_лупа_100;
            this.button11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button11.Location = new System.Drawing.Point(1099, 14);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(110, 62);
            this.button11.TabIndex = 10;
            this.button11.UseVisualStyleBackColor = true;

            this.button12.BackgroundImage = global::KompilatoriLab1.Properties.Resources.icons8_вперед_80;
            this.button12.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button12.Location = new System.Drawing.Point(412, 11);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 59);
            this.button12.TabIndex = 11;
            this.button12.UseVisualStyleBackColor = true;

            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 106);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;

            this.splitContainer1.Panel1.Controls.Add(this.tabControlEditor);

            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1219, 594);
            this.splitContainer1.SplitterDistance = 307;
            this.splitContainer1.TabIndex = 2;

            this.tabControlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEditor.Location = new System.Drawing.Point(0, 0);
            this.tabControlEditor.Name = "tabControlEditor";
            this.tabControlEditor.Size = new System.Drawing.Size(1219, 307);
            this.tabControlEditor.TabIndex = 0;

            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1219, 283);
            this.dataGridView1.TabIndex = 0;

            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.cursorPositionLabel,
            this.fileCountLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 700);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1219, 26);
            this.statusStrip.TabIndex = 3;

            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(100, 20);
            this.statusLabel.Text = "Режим: Pascal Enum";

            this.cursorPositionLabel.Name = "cursorPositionLabel";
            this.cursorPositionLabel.Size = new System.Drawing.Size(100, 20);
            this.cursorPositionLabel.Text = "Строка: 1, Кол: 1";

            this.fileCountLabel.Name = "fileCountLabel";
            this.fileCountLabel.Size = new System.Drawing.Size(70, 20);
            this.fileCountLabel.Text = "Файлов: 1";

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1219, 726);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Текстовый редактор с семантическим анализатором";
            this.AllowDrop = true;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

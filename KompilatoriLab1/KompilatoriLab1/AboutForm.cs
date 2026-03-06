using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KompilatoriLab1
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            this.Text = "О программе";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label titleLabel = new Label();
            titleLabel.Text = "Текстовый редактор";
            titleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 50;

            Label infoLabel = new Label();
            infoLabel.Text = "Версия: 1.0.0\n\n" +
                "Разработчик: Студент\n" +
                "Дисциплина: Теория формальных языков и компиляторов\n\n" +
                "© 2024";
            infoLabel.Font = new Font("Segoe UI", 10);
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Dock = DockStyle.Fill;

            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.Size = new Size(80, 30);
            okButton.Location = new Point(160, 180);
            okButton.Click += (s, e) => this.Close();

            this.Controls.Add(infoLabel);
            this.Controls.Add(titleLabel);
            this.Controls.Add(okButton);
        }
    }
}

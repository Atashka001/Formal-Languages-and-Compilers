using System;
using System.Windows.Forms;

namespace KompilatoriLab1
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            okButton.Click += (s, e) => Close();
        }
    }
}

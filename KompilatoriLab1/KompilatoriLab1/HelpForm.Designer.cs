namespace KompilatoriLab1
{
    partial class HelpForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RichTextBox helpContent;

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
            this.helpContent = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            // helpContent
            this.helpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpContent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.helpContent.Location = new System.Drawing.Point(0, 0);
            this.helpContent.Name = "helpContent";
            this.helpContent.ReadOnly = true;
            this.helpContent.Size = new System.Drawing.Size(600, 500);
            this.helpContent.TabIndex = 0;
            this.helpContent.Text = "";

            // HelpForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.helpContent);
            this.Name = "HelpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Справка - Руководство пользователя";
            this.ResumeLayout(false);
        }
    }
}
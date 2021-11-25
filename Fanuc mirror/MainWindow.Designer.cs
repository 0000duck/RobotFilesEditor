namespace Fanuc_mirro
{
    partial class MainWindow
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectButton = new System.Windows.Forms.Button();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.mirrorWorkbook_checkbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(12, 12);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(642, 38);
            this.SelectButton.TabIndex = 0;
            this.SelectButton.Text = "Select directory";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Enabled = false;
            this.pathTextBox.Location = new System.Drawing.Point(12, 56);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(642, 22);
            this.pathTextBox.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(12, 112);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(642, 41);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // mirrorWorkbook_checkbox
            // 
            this.mirrorWorkbook_checkbox.AutoSize = true;
            this.mirrorWorkbook_checkbox.Checked = true;
            this.mirrorWorkbook_checkbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mirrorWorkbook_checkbox.Location = new System.Drawing.Point(12, 85);
            this.mirrorWorkbook_checkbox.Name = "mirrorWorkbook_checkbox";
            this.mirrorWorkbook_checkbox.Size = new System.Drawing.Size(139, 21);
            this.mirrorWorkbook_checkbox.TabIndex = 3;
            this.mirrorWorkbook_checkbox.Text = "Mirror workbook?";
            this.mirrorWorkbook_checkbox.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 166);
            this.Controls.Add(this.mirrorWorkbook_checkbox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.SelectButton);
            this.Name = "MainWindow";
            this.Text = "Fanuc mirror";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox mirrorWorkbook_checkbox;
    }
}


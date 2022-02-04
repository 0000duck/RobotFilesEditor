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
            this.TextBox_ToolsToMirror = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(9, 10);
            this.SelectButton.Margin = new System.Windows.Forms.Padding(2);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(482, 31);
            this.SelectButton.TabIndex = 0;
            this.SelectButton.Text = "Select directory";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Enabled = false;
            this.pathTextBox.Location = new System.Drawing.Point(9, 46);
            this.pathTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(482, 20);
            this.pathTextBox.TabIndex = 1;
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(9, 179);
            this.saveButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(482, 33);
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
            this.mirrorWorkbook_checkbox.Location = new System.Drawing.Point(9, 157);
            this.mirrorWorkbook_checkbox.Margin = new System.Windows.Forms.Padding(2);
            this.mirrorWorkbook_checkbox.Name = "mirrorWorkbook_checkbox";
            this.mirrorWorkbook_checkbox.Size = new System.Drawing.Size(108, 17);
            this.mirrorWorkbook_checkbox.TabIndex = 3;
            this.mirrorWorkbook_checkbox.Text = "Mirror workbook?";
            this.mirrorWorkbook_checkbox.UseVisualStyleBackColor = true;
            this.mirrorWorkbook_checkbox.CheckedChanged += new System.EventHandler(this.mirrorWorkbook_checkbox_CheckedChanged);
            // 
            // TextBox_ToolsToMirror
            // 
            this.TextBox_ToolsToMirror.Enabled = false;
            this.TextBox_ToolsToMirror.Location = new System.Drawing.Point(9, 100);
            this.TextBox_ToolsToMirror.Margin = new System.Windows.Forms.Padding(2);
            this.TextBox_ToolsToMirror.Name = "TextBox_ToolsToMirror";
            this.TextBox_ToolsToMirror.Size = new System.Drawing.Size(482, 20);
            this.TextBox_ToolsToMirror.TabIndex = 4;
            this.TextBox_ToolsToMirror.TextChanged += new System.EventHandler(this.TextBox_ToolsToMirror_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Tools to be mirrored:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(9, 132);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(152, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Renumber process points?";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Renumber factor:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(336, 129);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(56, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "1";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(244, 154);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(168, 17);
            this.checkBox2.TabIndex = 9;
            this.checkBox2.Text = "Remove header and Spaces?";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 223);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBox_ToolsToMirror);
            this.Controls.Add(this.mirrorWorkbook_checkbox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.SelectButton);
            this.Margin = new System.Windows.Forms.Padding(2);
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
        private System.Windows.Forms.TextBox TextBox_ToolsToMirror;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}


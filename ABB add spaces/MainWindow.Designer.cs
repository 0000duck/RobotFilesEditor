namespace ABB_add_spaces
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_dir = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox_Header = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.getorgsfrombackup_button = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(776, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select directory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_dir
            // 
            this.textBox_dir.Location = new System.Drawing.Point(13, 46);
            this.textBox_dir.Name = "textBox_dir";
            this.textBox_dir.Size = new System.Drawing.Size(775, 20);
            this.textBox_dir.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 96);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(776, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "Add spaces";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox_Header
            // 
            this.checkBox_Header.AutoSize = true;
            this.checkBox_Header.Location = new System.Drawing.Point(12, 73);
            this.checkBox_Header.Name = "checkBox_Header";
            this.checkBox_Header.Size = new System.Drawing.Size(89, 17);
            this.checkBox_Header.TabIndex = 3;
            this.checkBox_Header.Text = "Use Header?";
            this.checkBox_Header.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 165);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(776, 29);
            this.button3.TabIndex = 4;
            this.button3.Text = "Add comments and translate orgs";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button4.Location = new System.Drawing.Point(13, 199);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(776, 29);
            this.button4.TabIndex = 5;
            this.button4.Text = "Get tooldatas and loaddatas";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // getorgsfrombackup_button
            // 
            this.getorgsfrombackup_button.Location = new System.Drawing.Point(13, 233);
            this.getorgsfrombackup_button.Name = "getorgsfrombackup_button";
            this.getorgsfrombackup_button.Size = new System.Drawing.Size(776, 29);
            this.getorgsfrombackup_button.TabIndex = 6;
            this.getorgsfrombackup_button.Text = "Get orgs from backups ";
            this.getorgsfrombackup_button.UseVisualStyleBackColor = true;
            this.getorgsfrombackup_button.Click += new System.EventHandler(this.getorgsfrombackup_button_Click);
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(12, 131);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(776, 29);
            this.button5.TabIndex = 7;
            this.button5.Text = "Mirror Paths";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 291);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.getorgsfrombackup_button);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox_Header);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox_dir);
            this.Controls.Add(this.button1);
            this.Name = "MainWindow";
            this.Text = "ABB Add spaces";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_dir;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox_Header;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button getorgsfrombackup_button;
        private System.Windows.Forms.Button button5;
    }
}


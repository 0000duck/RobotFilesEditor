namespace RobotSafetyGenerator
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
            this.textBox_ModFile = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_ReadXML = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button_ReadFromBackup = new System.Windows.Forms.Button();
            this.comboBox_RobotTypes = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_AddZone32 = new System.Windows.Forms.CheckBox();
            this.textBox_XML_Fanuc = new System.Windows.Forms.TextBox();
            this.WriteXVR = new System.Windows.Forms.Button();
            this.ReadXVR = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.textbox_KUKAxml = new System.Windows.Forms.TextBox();
            this.button_saveKUKAfiles = new System.Windows.Forms.Button();
            this.button_readWVxml = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button1.Location = new System.Drawing.Point(5, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(291, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = ".mod from PS -> RobKalDat .xml";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_ModFile
            // 
            this.textBox_ModFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox_ModFile.Location = new System.Drawing.Point(6, 53);
            this.textBox_ModFile.Name = "textBox_ModFile";
            this.textBox_ModFile.Size = new System.Drawing.Size(590, 20);
            this.textBox_ModFile.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button2.Location = new System.Drawing.Point(5, 119);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(591, 26);
            this.button2.TabIndex = 2;
            this.button2.Text = "Save RobKalDat .xml file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(6, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Choose Robot Type:";
            // 
            // button_ReadXML
            // 
            this.button_ReadXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_ReadXML.Location = new System.Drawing.Point(313, 21);
            this.button_ReadXML.Name = "button_ReadXML";
            this.button_ReadXML.Size = new System.Drawing.Size(283, 26);
            this.button_ReadXML.TabIndex = 5;
            this.button_ReadXML.Text = "SafetyRobot.xml -> RobKalDat .xml";
            this.button_ReadXML.UseVisualStyleBackColor = true;
            this.button_ReadXML.Click += new System.EventHandler(this.button_ReadXML_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button_ReadFromBackup);
            this.groupBox1.Controls.Add(this.button_ReadXML);
            this.groupBox1.Controls.Add(this.comboBox_RobotTypes);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_ModFile);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(606, 216);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ABB";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Location = new System.Drawing.Point(6, 182);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(590, 26);
            this.button3.TabIndex = 7;
            this.button3.Text = "SafetyRobot.xml -> PS .mod";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button_ReadFromBackup
            // 
            this.button_ReadFromBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_ReadFromBackup.Location = new System.Drawing.Point(6, 151);
            this.button_ReadFromBackup.Name = "button_ReadFromBackup";
            this.button_ReadFromBackup.Size = new System.Drawing.Size(591, 26);
            this.button_ReadFromBackup.TabIndex = 6;
            this.button_ReadFromBackup.Text = "Backup or sc_cfg.xml (RW6) or psc_user_1.sxml (RW5)  -> PS .mod (RW6) or PSC_Defi" +
    "ne_Safety_Zones (RW5)";
            this.button_ReadFromBackup.UseVisualStyleBackColor = true;
            this.button_ReadFromBackup.Click += new System.EventHandler(this.button_ReadFromBackup_Click);
            // 
            // comboBox_RobotTypes
            // 
            this.comboBox_RobotTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBox_RobotTypes.FormattingEnabled = true;
            this.comboBox_RobotTypes.Location = new System.Drawing.Point(6, 92);
            this.comboBox_RobotTypes.Name = "comboBox_RobotTypes";
            this.comboBox_RobotTypes.Size = new System.Drawing.Size(590, 21);
            this.comboBox_RobotTypes.TabIndex = 4;
            this.comboBox_RobotTypes.SelectedIndexChanged += new System.EventHandler(this.comboBox_RobotTypes_SelectedIndexChanged);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.button4.Location = new System.Drawing.Point(5, 182);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(594, 26);
            this.button4.TabIndex = 8;
            this.button4.Text = "Create simplified .xml";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_AddZone32);
            this.groupBox2.Controls.Add(this.textBox_XML_Fanuc);
            this.groupBox2.Controls.Add(this.WriteXVR);
            this.groupBox2.Controls.Add(this.ReadXVR);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox2.Location = new System.Drawing.Point(12, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(606, 140);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FANUC";
            // 
            // checkBox_AddZone32
            // 
            this.checkBox_AddZone32.AutoSize = true;
            this.checkBox_AddZone32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBox_AddZone32.Location = new System.Drawing.Point(6, 83);
            this.checkBox_AddZone32.Name = "checkBox_AddZone32";
            this.checkBox_AddZone32.Size = new System.Drawing.Size(89, 17);
            this.checkBox_AddZone32.TabIndex = 8;
            this.checkBox_AddZone32.Text = "Add zone32?";
            this.checkBox_AddZone32.UseVisualStyleBackColor = true;
            this.checkBox_AddZone32.CheckedChanged += new System.EventHandler(this.checkBox_AddZone32_CheckedChanged);
            // 
            // textBox_XML_Fanuc
            // 
            this.textBox_XML_Fanuc.Enabled = false;
            this.textBox_XML_Fanuc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox_XML_Fanuc.Location = new System.Drawing.Point(6, 57);
            this.textBox_XML_Fanuc.Name = "textBox_XML_Fanuc";
            this.textBox_XML_Fanuc.Size = new System.Drawing.Size(597, 20);
            this.textBox_XML_Fanuc.TabIndex = 6;
            // 
            // WriteXVR
            // 
            this.WriteXVR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WriteXVR.Location = new System.Drawing.Point(5, 106);
            this.WriteXVR.Name = "WriteXVR";
            this.WriteXVR.Size = new System.Drawing.Size(597, 26);
            this.WriteXVR.TabIndex = 7;
            this.WriteXVR.Text = "Save .xvr file";
            this.WriteXVR.UseVisualStyleBackColor = true;
            this.WriteXVR.Click += new System.EventHandler(this.WriteXVR_Click);
            // 
            // ReadXVR
            // 
            this.ReadXVR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ReadXVR.Location = new System.Drawing.Point(6, 25);
            this.ReadXVR.Name = "ReadXVR";
            this.ReadXVR.Size = new System.Drawing.Size(597, 26);
            this.ReadXVR.TabIndex = 6;
            this.ReadXVR.Text = "Read SafetyRobot.xml for Fanuc";
            this.ReadXVR.UseVisualStyleBackColor = true;
            this.ReadXVR.Click += new System.EventHandler(this.ReadXml_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Controls.Add(this.textbox_KUKAxml);
            this.groupBox3.Controls.Add(this.button_saveKUKAfiles);
            this.groupBox3.Controls.Add(this.button_readWVxml);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox3.Location = new System.Drawing.Point(17, 380);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(606, 151);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "KUKA";
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button6.Location = new System.Drawing.Point(6, 115);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(597, 26);
            this.button6.TabIndex = 8;
            this.button6.Text = "KRC2 safety from backup -> PS path";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // textbox_KUKAxml
            // 
            this.textbox_KUKAxml.Enabled = false;
            this.textbox_KUKAxml.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textbox_KUKAxml.Location = new System.Drawing.Point(6, 57);
            this.textbox_KUKAxml.Name = "textbox_KUKAxml";
            this.textbox_KUKAxml.Size = new System.Drawing.Size(597, 20);
            this.textbox_KUKAxml.TabIndex = 6;
            // 
            // button_saveKUKAfiles
            // 
            this.button_saveKUKAfiles.Enabled = false;
            this.button_saveKUKAfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_saveKUKAfiles.Location = new System.Drawing.Point(6, 83);
            this.button_saveKUKAfiles.Name = "button_saveKUKAfiles";
            this.button_saveKUKAfiles.Size = new System.Drawing.Size(597, 26);
            this.button_saveKUKAfiles.TabIndex = 7;
            this.button_saveKUKAfiles.Text = "Save .src and .dat files";
            this.button_saveKUKAfiles.UseVisualStyleBackColor = true;
            this.button_saveKUKAfiles.Click += new System.EventHandler(this.button_saveKUKAfiles_Click);
            // 
            // button_readWVxml
            // 
            this.button_readWVxml.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button_readWVxml.Location = new System.Drawing.Point(6, 25);
            this.button_readWVxml.Name = "button_readWVxml";
            this.button_readWVxml.Size = new System.Drawing.Size(597, 26);
            this.button_readWVxml.TabIndex = 6;
            this.button_readWVxml.Text = "Read safety.xml exported from WorkVisual project";
            this.button_readWVxml.UseVisualStyleBackColor = true;
            this.button_readWVxml.Click += new System.EventHandler(this.button_readWVxml_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox4.Location = new System.Drawing.Point(18, 537);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(602, 62);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Robots checksums";
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button5.Location = new System.Drawing.Point(-1, 25);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(597, 26);
            this.button5.TabIndex = 8;
            this.button5.Text = "Get robot checksums";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(627, 611);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainWindow";
            this.Text = "Robot Safety Tools";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_ModFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ReadXML;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button WriteXVR;
        private System.Windows.Forms.Button ReadXVR;
        private System.Windows.Forms.TextBox textBox_XML_Fanuc;
        private System.Windows.Forms.ComboBox comboBox_RobotTypes;
        private System.Windows.Forms.CheckBox checkBox_AddZone32;
        private System.Windows.Forms.Button button_ReadFromBackup;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textbox_KUKAxml;
        private System.Windows.Forms.Button button_saveKUKAfiles;
        private System.Windows.Forms.Button button_readWVxml;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}


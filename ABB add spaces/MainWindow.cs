using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace ABB_add_spaces
{
    public partial class MainWindow : Form
    {
        public string[] files;
        public string mocFile;
        private bool isSuccess;
        public MainWindow()
        {
            InitializeComponent();
            this.TopMost= true;
            checkBox_Header.Checked = true;
            textBox_dir.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                this.TopMost = false;
                files = null;
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.InitialDirectory = textBox_dir.Text;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    textBox_dir.Text = dialog.FileName;
                    //Properties.Settings.Default.LastFolder = textBox_dir.Text;
                    Properties.Settings.Default.Save();
                }
                this.TopMost = true;
            }
            if (textBox_dir.Text != "")
            {
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                if (Directory.Exists(textBox_dir.Text + "\\RAPID\\TASK1\\PROGMOD"))
                    files = Directory.GetFiles(textBox_dir.Text + "\\RAPID\\TASK1\\PROGMOD", "*.mod", SearchOption.TopDirectoryOnly);
                else
                    files = Directory.GetFiles(textBox_dir.Text, "*.mod", SearchOption.TopDirectoryOnly);
                mocFile = textBox_dir.Text + "\\SYSPAR\\MOC.cfg";
                if (files != null)
                    Methods.FindComments(files);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool addHeader;
            if (checkBox_Header.Checked)
                addHeader = true;
            else
                addHeader = false;
            try
            {                    
                isSuccess = Methods.AddSpacesToAll(files, addHeader);
                if (isSuccess)
                    MessageBox.Show("Spaces added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("No valid files found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("No valid files found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try {
                isSuccess = Methods.TranslateOrgsAndAddComments(files);
                if (isSuccess)
                    MessageBox.Show("Orgs translated and comments added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("No valid files found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("No valid files found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                isSuccess = Methods.FindLoads(files, textBox_dir.Text);
                if (isSuccess)
                { } //MessageBox.Show("Loads found succesfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getorgsfrombackup_button_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            GetOrgsMethods.Execute();
            this.TopMost = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Methods.MirrorPaths(textBox_dir.Text);
        }
    }
}

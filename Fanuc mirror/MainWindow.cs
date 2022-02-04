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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Fanuc_mirro
{

    public partial class MainWindow : Form
    {
        public CompleteMirror resultFiles;

        public MainWindow()
        {
            this.TopMost = true;
            InitializeComponent();
            TextBox_ToolsToMirror.Enabled = true;
            TextBox_ToolsToMirror.Text = "51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70";
            Methods.toolsToMirrorString = TextBox_ToolsToMirror.Text;
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            resultFiles = null;
            var dialog = new CommonOpenFileDialog();
            //dialog.Filters.Add(new CommonFileDialogFilter("Mod file (*.ls)", ".ls"));
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            dialog.InitialDirectory = pathTextBox.Text;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathTextBox.Text = dialog.FileName;
                Properties.Settings.Default.Save();
            }
            if (Directory.Exists(pathTextBox.Text))
                if (Directory.GetFiles(pathTextBox.Text,"*.ls",SearchOption.TopDirectoryOnly).Any() || mirrorWorkbook_checkbox.Checked && Directory.GetFiles(pathTextBox.Text, "*.xvr", SearchOption.TopDirectoryOnly).Any())
                    saveButton.Enabled = true;
                else
                    saveButton.Enabled = false;
            this.TopMost = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool mirrorWorkbook = false;
            if (mirrorWorkbook_checkbox.Checked)
                mirrorWorkbook = true;
            resultFiles = Methods.ReadLSFile(pathTextBox.Text, mirrorWorkbook, checkBox1.Checked ? int.Parse(textBox1.Text) : 0);

            SaveFileDialog sf = new SaveFileDialog();
            sf.FileName = "Save here";
            List<string> savedFiles = new List<string>();
            if (sf.ShowDialog() == DialogResult.OK)
            {
                string savePath = Path.GetDirectoryName(sf.FileName);
                foreach (FileAndPath file in resultFiles.Paths)
                {
                    bool saveFile = true;
                    if (File.Exists(savePath + "\\" + file.FileName.Name))
                    {
                        DialogResult dr = MessageBox.Show("File " + file.FileName.Name + " already exists\nOverwrite?",
                        "Overwrite?", MessageBoxButtons.YesNo);
                        switch (dr)
                        {
                            case DialogResult.Yes:
                                saveFile = true;
                                break;
                            case DialogResult.No:
                                saveFile = false;
                                break;
                        }
                    }
                    if (saveFile)
                    {
                        File.WriteAllText(savePath + "\\" + file.FileName.Name, file.Path);
                        savedFiles.Add(file.FileName.Name);
                    }
                }
                string outputString = "The following paths have been mirrored:\n";
                foreach (string file in savedFiles)
                    outputString = outputString + file + "\n";
                if (resultFiles.Workbook != null && (resultFiles.Workbook.FileName != null | resultFiles.Workbook.Path != null))
                {
                    bool saveFile = true;
                    if (File.Exists(savePath + "\\workbook.xvr"))
                    {
                        DialogResult dr = MessageBox.Show("File " + "workbook.xvr" + " already exists\nOverwrite?",
                        "Overwrite?", MessageBoxButtons.YesNo);
                        switch (dr)
                        {
                            case DialogResult.Yes:
                                saveFile = true;
                                break;
                            case DialogResult.No:
                                saveFile = false;
                                break;
                        }
                    }
                    if (saveFile)
                    {
                        {
                            File.WriteAllText(savePath + "\\workbook.xvr", resultFiles.Workbook.Path);
                            outputString = outputString + "\nWorkbook.xvr has been mirrored";
                        }
                    }
                }
                if (savedFiles.Count == 0)
                    MessageBox.Show("No paths were mirrored");
                else
                    MessageBox.Show(outputString);
            }

        }

        private void mirrorWorkbook_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mirrorWorkbook_checkbox.Checked)
                TextBox_ToolsToMirror.Enabled = true;
            else
                TextBox_ToolsToMirror.Enabled = false;
        }

        private void TextBox_ToolsToMirror_TextChanged(object sender, EventArgs e)
        {
            Methods.toolsToMirrorString = TextBox_ToolsToMirror.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox1.Enabled = true;
            else
                textBox1.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);

              
            }
        }
    }
}
    


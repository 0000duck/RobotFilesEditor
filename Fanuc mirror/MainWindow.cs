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
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            bool mirrorWorkbook = false;
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
                if (mirrorWorkbook_checkbox.Checked)
                    mirrorWorkbook = true;
                resultFiles = Methods.ReadLSFile(pathTextBox.Text, mirrorWorkbook);

            }
            if (resultFiles != null)
                if (resultFiles.Paths.Count != 0)
                    saveButton.Enabled = true;
                else
                    saveButton.Enabled = false;
            this.TopMost = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
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
                if (resultFiles.Workbook.FileName != null | resultFiles.Workbook.Path != null)
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
                    if (savedFiles.Count == 0)
                        MessageBox.Show("No paths were mirrored");
                    else
                        MessageBox.Show(outputString);
                }
            }

        }
    }
}
    


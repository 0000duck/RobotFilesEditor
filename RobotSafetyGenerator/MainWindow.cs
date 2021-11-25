using CommonLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;
using RobotSafetyGenerator;
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

namespace RobotSafetyGenerator
{
    public partial class MainWindow : Form
    {
        public SafetyConfigABB configABB;
        public SafetyConfigFanuc configFanuc;
        public SafetyConfigABB configABBfromBackup;
        public SafetyConfigKukaKRC4 configKukaKRC4;

        public MainWindow()
        {
            //this.TopMost = true;
            InitializeComponent();
            textBox_ModFile.Enabled = false;
            FillComboBox(RobotList.RobotsElbow);
            button2.Enabled = false;
            button3.Enabled = false;
            WriteXVR.Enabled = false;
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        }

        private void FillComboBox(List<Robot> robotsElbow)
        {
            foreach (Robot robotType in robotsElbow)
                comboBox_RobotTypes.Items.Add(robotType.RobotName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            configABB = null;
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Mod file (*.mod)", ".mod"));
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.InitialDirectory = textBox_ModFile.Text;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox_ModFile.Text = dialog.FileName;
                Properties.Settings.Default.Save();
                configABB = Methods.ReadModFile(textBox_ModFile.Text);
                if (comboBox_RobotTypes.SelectedItem != null)
                    if (comboBox_RobotTypes.SelectedItem.ToString() != "" & textBox_ModFile.Text != "")
                        button2.Enabled = true;
            }
            this.TopMost = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string resultString = Methods.BuildXMLABB(configABB, comboBox_RobotTypes.SelectedItem.ToString());
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save safety configuration";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, resultString);
                MessageBox.Show("Safety configuration generated succesfully");
            }
        }

        private void comboBox_RobotTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_RobotTypes.SelectedItem.ToString() != "" & textBox_ModFile.Text != "")
            {
                button2.Enabled = true;
            }
            button3.Enabled = true;
        }

        private void button_ReadXML_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            configABB = null;
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("XML file (*.xml)", ".xml"));
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.InitialDirectory = textBox_ModFile.Text;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox_ModFile.Text = dialog.FileName;
                Properties.Settings.Default.Save();
                configABB = Methods.ReadXmlFile(textBox_ModFile.Text);
                if (comboBox_RobotTypes.SelectedItem != null)
                {
                    if (comboBox_RobotTypes.SelectedItem.ToString() != "" & textBox_ModFile.Text != "" & configABB != null)
                        button2.Enabled = true;
                    else
                        button2.Enabled = false;
                }
            }
            this.TopMost = true;
        }

        private void ReadXml_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            configFanuc = null;
            bool addZone32 = false;
            if (checkBox_AddZone32.Checked)
                addZone32 = true;
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("xml file (*.xml)", ".xml"));
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.InitialDirectory = textBox_XML_Fanuc.Text;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox_XML_Fanuc.Text = dialog.FileName;
                Properties.Settings.Default.Save();
                configFanuc = Methods.ReadXMLFileFanuc(textBox_XML_Fanuc.Text, addZone32);
            }
            else
                textBox_XML_Fanuc.Text = "";
            if (configFanuc != null & textBox_XML_Fanuc.Text != "")
                WriteXVR.Enabled = true;
            else
                WriteXVR.Enabled = false;
            this.TopMost = false;
        }

        private void WriteXVR_Click(object sender, EventArgs e)
        {
            string resultString = Methods.BuildXMLFanuc(configFanuc);
            if (resultString != "" & resultString != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "XVR File|*.xvr";
                saveFileDialog1.Title = "Save safety configuration";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    File.WriteAllText(saveFileDialog1.FileName, resultString);
                    MessageBox.Show("Safety configuration generated succesfully");
                }
            }
        }

        private void checkBox_AddZone32_CheckedChanged(object sender, EventArgs e)
        {
            if (configFanuc != null)
                configFanuc = Methods.ReadXMLFileFanuc(textBox_XML_Fanuc.Text, checkBox_AddZone32.Checked);
        }

        private void button_ReadFromBackup_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            configABBfromBackup = null;
            DialogResult dialogResult = MessageBox.Show("Backup - Yes\r\nFile - No","Backup or file?", MessageBoxButtons.YesNo);
            var dialog = new CommonOpenFileDialog();
            if (dialogResult == DialogResult.Yes)
                dialog.IsFolderPicker = true;
            else
            {
                dialog.IsFolderPicker = false;
                dialog.Filters.Add(new CommonFileDialogFilter("XML (*.xml)", ".xml"));
            }
            dialog.EnsurePathExists = true;
            bool oldConfig = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {              
                Properties.Settings.Default.Save();
                if (File.Exists(dialog.FileName) && dialog.FileName.ToLower().Contains(".xml"))
                    configABBfromBackup = Methods.ReadXmlFileFromBackup(dialog.FileName, oldConfig);
                else if (File.Exists(dialog.FileName + "\\BACKINFO\\sc_cfg.xml"))
                    configABBfromBackup = Methods.ReadXmlFileFromBackup(dialog.FileName + "\\BACKINFO\\sc_cfg.xml", oldConfig);
                else if (File.Exists(dialog.FileName + "\\BACKINFO\\psc_user_1.sxml"))
                {
                    oldConfig = true;
                    configABBfromBackup = Methods.ReadXmlFileFromBackup(dialog.FileName + "\\BACKINFO\\psc_user_1.sxml", oldConfig);
                }
                else if (File.Exists(dialog.FileName + "\\sc_cfg.xml"))
                    configABBfromBackup = Methods.ReadXmlFileFromBackup(dialog.FileName + "\\sc_cfg.xml", oldConfig);
                else if (File.Exists(dialog.FileName + "\\psc_user_1.sxml"))
                {
                    oldConfig = true;
                    configABBfromBackup = Methods.ReadXmlFileFromBackup(dialog.FileName + "\\psc_user_1.sxml", oldConfig);
                }
                else
                    MessageBox.Show("No sc_cfg.xml nor psc_user_1.sxml found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string resultModFile = Methods.BuildModFileFromBackup(configABBfromBackup, oldConfig);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "MOD File|*.mod";
                saveFileDialog1.Title = "Save safety path configuration";
                if (oldConfig)
                    saveFileDialog1.FileName = "PSC_Define_Safety_Zones";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    File.WriteAllText(saveFileDialog1.FileName, resultModFile);
                    MessageBox.Show("Safety configuration generated succesfully");
                }
            }
            this.TopMost = true;
        }

        private void button_readWVxml_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            configKukaKRC4 = null;
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("xml file (*.xml)", ".xml"));
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.InitialDirectory = textbox_KUKAxml.Text;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textbox_KUKAxml.Text = dialog.FileName;
                Properties.Settings.Default.Save();
                configKukaKRC4 = Methods.ReadXmlFileKUKA(textbox_KUKAxml.Text);
                if (textbox_KUKAxml.Text != "" & configKukaKRC4 != null)
                    button_saveKUKAfiles.Enabled = true;
                else
                    button_saveKUKAfiles.Enabled = false;
            }
            this.TopMost = true;
        }

        private void button_saveKUKAfiles_Click(object sender, EventArgs e)
        {
            IDictionary<string, string> resultString = Methods.BuildSrcDat(configKukaKRC4);
            IDictionary<string, string> mirroredResult = new Dictionary<string,string>();
            var dialog = MessageBox.Show("Mirror safety?", " Mirror?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                mirroredResult = Methods.MirrorSafety(resultString);
            }
            if (resultString["src"] != "" & resultString["dat"] != "" & resultString != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                //saveFileDialog1.Filter = "XVR File|*.xvr";
                saveFileDialog1.Title = "Save safety configuration";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    File.WriteAllText(saveFileDialog1.FileName + ".src", resultString["src"]);
                    File.WriteAllText(saveFileDialog1.FileName + ".dat", resultString["dat"]);
                    if (mirroredResult.Count > 0)
                    {
                        File.WriteAllText(saveFileDialog1.FileName + "_mirrored.src", mirroredResult["src"]);
                        File.WriteAllText(saveFileDialog1.FileName + "_mirrored.dat", mirroredResult["dat"]);
                    }
                    MessageBox.Show("Safety configuration generated succesfully");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            configABB = null;
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("XML file (*.xml)", ".xml"));
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.InitialDirectory = textBox_ModFile.Text;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox_ModFile.Text = dialog.FileName;
                Properties.Settings.Default.Save();
                configABB = Methods.ReadXmlFile(dialog.FileName, isSafetyRobotXML:true);
            }
            //configABB = Methods.ConvertMilimetersInTools(configABB);
            string resultModFile = Methods.BuildModFileFromBackup(configABB, false);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "MOD File|*.mod";
            saveFileDialog1.Title = "Save safety path configuration";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, resultModFile);
                MessageBox.Show("Safety configuration generated succesfully");
            }
            //configABBfromBackup = null;
            //var dialog = new CommonOpenFileDialog();
            //dialog.IsFolderPicker = false;
            //dialog.EnsurePathExists = true;
            // dialog.Filters.Add(new CommonFileDialogFilter("xml file (*.xml)", ".xml"));
            //bool oldConfig = false;
            //if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            //{
            //    //textBox_ModFile.Text = dialog.FileName;
            //    Properties.Settings.Default.Save();
            //    if (File.Exists(dialog.FileName))
            //        configABBfromBackup = Methods.ReadXmlFileFromBackup(dialog.FileName, oldConfig, comboBox_RobotTypes.SelectedItem.ToString());
            //    else
            //        MessageBox.Show("No safety configuration found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    string resultModFile = Methods.BuildModFileFromBackup(configABBfromBackup, oldConfig);

            //    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //    saveFileDialog1.Filter = "MOD File|*.mod";
            //    saveFileDialog1.Title = "Save safety path configuration";
            //    if (oldConfig)
            //        saveFileDialog1.FileName = "PSC_Define_Safety_Zones";
            //    saveFileDialog1.ShowDialog();
            //    if (saveFileDialog1.FileName != "")
            //    {
            //        File.WriteAllText(saveFileDialog1.FileName, resultModFile);
            //        MessageBox.Show("Safety configuration generated succesfully");
            //    }
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string resultString = "";
            configABBfromBackup = null;
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            dialog.EnsurePathExists = true;
            dialog.Filters.Add(new CommonFileDialogFilter("xml file (*.xml)", ".xml"));
            bool oldConfig = false;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //textBox_ModFile.Text = dialog.FileName;
                Properties.Settings.Default.Save();
                if (File.Exists(dialog.FileName))
                {
                    resultString = Methods.ReadCompleteData(dialog.FileName);
                }
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save simplified safety configuration";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, resultString);
                MessageBox.Show("Safety configuration generated succesfully");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            RobotChecksumMethods.Execute();
            this.TopMost = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            string selectedDir = CommonMethods.SelectDirOrFile(true);
            CommonLibrary.SafetyConfig safetyConfig = CommonLibrary.SafetyKUKAMethods.GetSafetyConfig(selectedDir);
            configKukaKRC4 = Methods.ConvertKukaConfigurations(safetyConfig);
            IDictionary<string, string> resultString = Methods.BuildSrcDat(configKukaKRC4);
            if (resultString["src"] != "" & resultString["dat"] != "" & resultString != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                //saveFileDialog1.Filter = "XVR File|*.xvr";
                saveFileDialog1.Title = "Save safety configuration";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    File.WriteAllText(saveFileDialog1.FileName + ".src", resultString["src"]);
                    File.WriteAllText(saveFileDialog1.FileName + ".dat", resultString["dat"]);
                    MessageBox.Show("Safety configuration generated succesfully");
                }
            }
            this.TopMost = true;
        }
    }
}

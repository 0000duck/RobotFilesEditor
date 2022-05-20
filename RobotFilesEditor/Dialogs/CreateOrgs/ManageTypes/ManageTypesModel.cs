using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace RobotFilesEditor.Dialogs.CreateOrgs.ManageTypes
{
    public class ManageTypesModel
    {
        public ManageTypesModel()
        {
            Messenger.Default.Register<List<ManageTypesData>> (this, "exportXMLOrgs", message => ExportXMLOrgs(message));
            Messenger.Default.Register<string>(this, "importXMLOrgs", message => ImportXMLOrgs());
            Messenger.Default.Register<List<ManageTypesData>>(this, "applyXMLOrgs", message => ApplyXMLOrgs(message));

            //if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester", "OrgsXML", "OrgsData.xml")))
            //    BuildInitialXML();
            List<ManageTypesData> linesAndTypes = CreateTypesFromXML();
            ManageTypesVM vm = new ManageTypesVM(linesAndTypes);
            var window = new ManageTypesWindow(vm);
            var view = window.ShowDialog();
        }

        public ManageTypesModel(bool isCalledFromMainWindow)
        {

        }

        private void BuildInitialXML()
        {
            List<ManageTypesData> data = new List<ManageTypesData>();
            List<string> lines = ConfigurationManager.AppSettings["Lines"].Split(',').Select(s => s.Trim()).ToList();
            foreach (var line in lines)
            {
                List<string> typesStrings = ConfigurationManager.AppSettings["Line_" + line].Split(',').Select(s => s.Trim()).ToList();
                List<TypeAndNum> typAndNum = GetTypeAndNum(typesStrings);
                List<string> plcs = ConfigurationManager.AppSettings["LinePLC_" + line].Split(',').Select(s => s.Trim()).ToList();
                List<int> intPlcs = new List<int>();
                plcs.ForEach(x => intPlcs.Add(int.Parse(x)));
                data.Add(new ManageTypesData(line, typAndNum, intPlcs));
            }
            BuildXML(data, true);
        }


        public List<ManageTypesData> CreateTypesFromXML()
        {
            List<ManageTypesData> result = new List<ManageTypesData>();
            string loadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester", "OrgsXML", "OrgsData.xml");
            if (!File.Exists(loadPath))
            {
                MessageBox.Show("Source file does not exist. Default file will be created", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                BuildInitialXML();
            }
            XDocument document = XDocument.Load(loadPath);
            foreach(var line in document.Element("Lines").Elements("Line"))
            {
                List<TypeAndNum> types = new List<TypeAndNum>();
                List<int> plcs = new List<int>();
                foreach (var type in line.Element("Types").Elements())
                {
                    types.Add(new TypeAndNum(int.Parse(type.Attribute("Number").Value),type.Attribute("Description").Value));
                }
                foreach (var plc in line.Element("PLCs").Elements())
                {
                    string tempstring = plc.Name.ToString().Replace("plc_", "");
                    plcs.Add(int.Parse(tempstring));
                }

                result.Add(new ManageTypesData(line.Attribute("Name").Value.Replace("_", " "), types, plcs));
            }
            return result;
        }

        private void ExportXMLOrgs(List<ManageTypesData> listAndTypes)
        {
            BuildXML(listAndTypes, false);
        }

        private void ApplyXMLOrgs(List<ManageTypesData> listAndTypes)
        {
            BuildXML(listAndTypes, true);
        }


        private void ImportXMLOrgs()
        {
            string savePath = CreateDefaultDir();
            string inputFile = CommonLibrary.CommonMethods.SelectDirOrFile(false, "xml file", "*.xml");
            if (File.Exists(savePath))
                File.Delete(savePath);
            File.Copy(inputFile, savePath);
            Messenger.Default.Send(CreateTypesFromXML(), "importXMLOrgsFeedback");
        }

        private void BuildXML(List<ManageTypesData> data, bool isInitial)
        {
            XDocument document = new XDocument(
               new XComment("XML file containing Lines and Types for OrgCreator"),
               new XElement("Lines")
               );
            foreach (var line in data)
            {
                XElement lineElement = new XElement("Line", new XAttribute("Name", line.LineName));
                XElement typesElement = new XElement("Types");
                foreach (var type in line.Types)
                {
                    typesElement.Add(new XElement("Type", new XAttribute("Number",type.Number), new XAttribute("Description",type.Description)));
                }
                XElement plcElement = new XElement("PLCs");
                foreach (var plc in line.PLCs)
                {
                    string tempString = plc.ToString();
                    if (new Regex(@"\d").IsMatch(tempString.Substring(0, 1)))
                        tempString = "plc_" + tempString;
                    plcElement.Add(new XElement(tempString));
                }
                lineElement.Add(typesElement);
                lineElement.Add(plcElement);
                document.Element("Lines").Add(lineElement);
            }
            string savePath;
            if (isInitial)
            {
                savePath = CreateDefaultDir();
                if (File.Exists(savePath))
                    File.Delete(savePath);
            }
            else
                savePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, "xml file", "*.xml");
            if (Path.GetExtension(savePath).ToLower() != ".xml")
                savePath += ".xml";
            document.Save(savePath);
        }

        private string CreateDefaultDir()
        {
            string result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester", "OrgsXML", "OrgsData.xml");
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester"));
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester", "OrgsXML")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RobotFilesHarvester", "OrgsXML"));
            return result;
        }

        private List<TypeAndNum> GetTypeAndNum(List<string> typesStrings)
        {
            List<TypeAndNum> result = new List<TypeAndNum>();
            int counter = 1;
            foreach (var typ in typesStrings)
            {
                if (!string.IsNullOrEmpty(typ))
                    result.Add(new TypeAndNum(counter, typ));
                counter++;
            }
            return result;
        }
    }
}

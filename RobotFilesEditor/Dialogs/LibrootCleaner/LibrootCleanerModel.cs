using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace RobotFilesEditor.Dialogs.LibrootCleaner
{
    public class LibrootCleanerModel
    {
        #region fields
        private string pszFilePath;
        private string librootPath;
        private List<CojtPair> cojts;
        #endregion

        #region properties
        public ObservableCollection<CojtPair> MissingInPSZ { get; set; }
        public ObservableCollection<CojtPair> MissingInLibroot { get; set; }
        public ObservableCollection<CojtPair> OkPairs { get; set; }
        #endregion

        #region ctor
        public LibrootCleanerModel(string pszFilePath, string librootPath)
        {
            this.pszFilePath = pszFilePath;
            this.librootPath = librootPath;
            ExecuteScan();
        }
        #endregion

        #region methods
        private void ExecuteScan()
        {
            if (ScanPSZ());
                ScanLibroot();
        }

        private bool ScanPSZ()
        {
            string errorstring = string.Empty;
            string instanceString = string.Empty;
            try
            {
                Regex instanceRegex = new Regex(@"Pm.*Instance", RegexOptions.IgnoreCase);
                cojts = new List<CojtPair>();
                string copyFile = Path.Combine(Path.GetDirectoryName(pszFilePath), Path.GetFileNameWithoutExtension(pszFilePath) + "_copy.psz");
                if (!File.Exists(copyFile))
                    File.Copy(pszFilePath, copyFile);
                using (FileStream zipToOpen = new FileStream(copyFile, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        var psState = archive.GetEntry("StandaloneStudy_PsState.xml");
                        StreamReader reader = new StreamReader(psState.Open());
                        XDocument dokument = XDocument.Parse(reader.ReadToEnd());
                        reader.Close();

                        var pmInstances = dokument.Element("Data").Element("Objects").Elements().Where(x => instanceRegex.IsMatch(x.Name.ToString()));
                        foreach (var instance in pmInstances)
                        {
                            if (instance.Attribute("ExternalId").Value == "E2E28D7B-9317-40AF-9E21-F41673EF2C9F")
                            { }
                            instanceString = instance.Attribute("ExternalId").Value;
                            List<string> prototypes = new List<string>();
                            string objtype = instance.Name.LocalName;
                            if (instance.Element("masterObj") != null && instance.Element("masterObj").Value.ToLower() != "null")
                            {
                                var masterObj = instance.Element("masterObj").Value.ToLower();
                                var tempComponent = dokument.Element("Data").Element("Objects").Elements().First(x => x.Attribute("ExternalId").Value.ToLower() == masterObj);
                                if (tempComponent.Element("threeDRep") != null)
                                    prototypes.Add(masterObj);
                                else
                                {
                                    prototypes.Add(tempComponent.Element("prototype").Value.ToLower());
                                    if (tempComponent.Element("parentPrototype") != null)
                                        prototypes.Add(tempComponent.Element("parentPrototype").Value.ToLower());
                                }
                            }
                            else
                                prototypes.Add(instance.Element("prototype").Value.ToLower());
                            foreach (var prototype in prototypes)
                            {
                                var component = dokument.Element("Data").Element("Objects").Elements().First(x => x.Attribute("ExternalId").Value.ToLower() == prototype);
                                var threeDRep = component.Element("threeDRep").Value.ToLower();
                                var pm3DRep = dokument.Element("Data").Element("Objects").Elements("Pm3DRep").FirstOrDefault(x => x.Attribute("ExternalId").Value.ToLower() == threeDRep);
                                if (pm3DRep != null)
                                {
                                    var file = pm3DRep.Element("file").Value.ToLower();
                                    var pmReferenceFile = dokument.Element("Data").Element("Objects").Elements("PmReferenceFile").First(x => x.Attribute("ExternalId").Value.ToLower() == file);
                                    var filename = pmReferenceFile.Element("fileName").Value;
                                    if (!cojts.Any(x => x.CojtInPSZ.ToLower() == filename.ToLower()))
                                        cojts.Add(new CojtPair(objtype, filename, pmReferenceFile.Attribute("ExternalId").Value));
                                }
                                else
                                    errorstring += "Error: missing pm3DRep file for instance " + instanceString + "\r\n";

                            }
                        }
                    }
                    zipToOpen.Close();
                }
                if (!string.IsNullOrEmpty(errorstring))
                    MessageBox.Show(errorstring, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + instanceString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void ScanLibroot()
        {

            List<string> cojtDirs = Directory.GetDirectories(librootPath, "*.cojt", SearchOption.AllDirectories).ToList();
            List<string> clonedCojtList = CommonLibrary.CommonMethods.CloneList(cojtDirs);
            foreach (var cojt in cojtDirs)
            {
                if (cojts.Any(x => cojt.ToLower().Contains(x.CojtInPSZ.Replace("#","").ToLower())))
                {
                    var item = cojts.First(x => cojt.ToLower().Contains(x.CojtInPSZ.Replace("#", "").ToLower()));
                    item.CojtInLibroot = cojt;
                    clonedCojtList.Remove(cojt);
                }
            }
            OkPairs = CommonLibrary.CommonMethods.ToObservableCollection(cojts.Where(x => !string.IsNullOrEmpty(x.CojtInLibroot) && !string.IsNullOrEmpty(x.CojtInPSZ)).ToList());
            MissingInLibroot = CommonLibrary.CommonMethods.ToObservableCollection(cojts.Where(x => string.IsNullOrEmpty(x.CojtInLibroot) && !string.IsNullOrEmpty(x.CojtInPSZ)).ToList());
            //MissingInPSZ = ;
            ObservableCollection<CojtPair> missingInPSZ = new ObservableCollection<CojtPair>();
            clonedCojtList.ForEach(x => missingInPSZ.Add(new CojtPair("None", "None", "None") { CojtInPSZ = x }));
            MissingInPSZ = missingInPSZ;
        }
        #endregion  

    }

    public class CojtPair 
    {
        public string ObjectType { get; set; }
        public string CojtInPSZ { get; set; }
        public string CojtInLibroot { get; set; }
        public string ExternalID { get; set; }

        public CojtPair(string objectType, string cojtInPsz, string externalID)
        {
            ObjectType = objectType;
            CojtInPSZ = cojtInPsz;
            ExternalID = externalID;
        }
    }
}

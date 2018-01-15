using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RobotFilesEditor.Serializer
{
    public class FilesSerialization
    {

        public FilesSerialization()
        {}

        public Controler GetControlerConfigration(string controlerType)
        {
            Controler controler = new Controler();

            FileStream fs = File.OpenRead(GlobalData.ConfigurationFileName);
            ControlersConfiguration cc = (ControlersConfiguration)new XmlSerializer(typeof(ControlersConfiguration)).Deserialize(fs);

            foreach (var c in cc.Controler)
            {
                if (c.ControlerType.Equals(controlerType))
                {
                    foreach (var ftc in c.FilesToCopy)
                    {
                        switch (ftc.ProgramType.ToLower())
                        {
                            case "program":
                                {
                                    controler._productionCopiedFiles.Extension = ftc?.FilesFilter.FilesExtension.ToList();
                                    controler._productionCopiedFiles.Containing = ftc?.FilesFilter.ContainName.ToList();
                                    controler._productionCopiedFiles.Destination = ftc?.DestinationFolder;
                                }
                                break;
                            case "service":
                                {
                                    controler._serviceCopiedFiles.Extension = ftc?.FilesFilter.FilesExtension.ToList();
                                    controler._serviceCopiedFiles.Containing = ftc?.FilesFilter.ContainName.ToList();
                                    controler._serviceCopiedFiles.Destination = ftc?.DestinationFolder;
                                }
                                break;
                        }
                    }

                    foreach (var dtc in c.DataToCopy)
                    {
                        switch (dtc.FileType.ToLower())
                        {
                            case "olp":
                                {
                                    controler._copiedOlpDataFiles.Extension = dtc?.FilesFilter.FilesExtension.ToList();
                                    controler._copiedOlpDataFiles.Containing = dtc?.FilesFilter.ContainName.ToList();
                                    controler._copiedOlpDataFiles.Destination = dtc?.DestinationFolder;
                                }
                                break;
                            case "global": {
                                    controler._copiedGlobalDataFiles.Extension = dtc?.FilesFilter.FilesExtension.ToList();
                                    controler._copiedGlobalDataFiles.Containing = dtc?.FilesFilter.ContainName.ToList();
                                    controler._copiedGlobalDataFiles.Destination = dtc?.DestinationFolder;
                                } break;
                        }
                    }

                    foreach (var ftr in c.FilesToRemove)
                    {
                        controler._removingDataFiles.Extension = ftr?.FilesFilter.FilesExtension.ToList();
                    }
                }
            }               
           
            return controler;
        }


    }
}

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
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

            try {

                XmlSerializer serializer = new XmlSerializer(typeof(ControlersConfiguration));

                // A FileStream is needed to read the XML document.
                FileStream fs = new FileStream(GlobalData.ConfigurationFileName, FileMode.Open);
                XmlReader reader = XmlReader.Create(fs);

                // Declare an object variable of the type to be deserialized.
                ControlersConfiguration cc;

                // Use the Deserialize method to restore the object's state.
                cc = (ControlersConfiguration)serializer.Deserialize(reader);
                fs.Close();

                foreach (var c in cc.Controlers)
                {
                    if (c.ControlerType.Equals(controlerType))
                    {
                        foreach (var ftc in c.FilesToCopy)
                        {
                            switch (ftc?.Type?.ToLower())
                            {
                                case "program":
                                    {
                                        controler._productionCopiedFiles.Extension = ftc?.FilesExtension.ToList();
                                        controler._productionCopiedFiles.Containing = ftc?.ContainNames.ToList();
                                        controler._productionCopiedFiles.Destination = ftc?.DestinationFolder;
                                    }
                                    break;
                                case "service":
                                    {
                                        controler._serviceCopiedFiles.Extension = ftc?.FilesExtension.ToList();
                                        controler._serviceCopiedFiles.Containing = ftc?.ContainNames.ToList();
                                        controler._serviceCopiedFiles.Destination = ftc?.DestinationFolder;
                                    }
                                    break;
                            }
                        }

                        foreach (var dtc in c.DataToCopy)
                        {
                            switch (dtc?.Type?.ToLower())
                            {
                                case "olp":
                                    {
                                        controler._copiedOlpDataFiles.Extension = dtc?.FilesExtension.ToList();
                                        controler._copiedOlpDataFiles.Containing = dtc?.ContainNames.ToList();
                                        controler._copiedOlpDataFiles.Destination = dtc?.DestinationFolder;
                                    }
                                    break;
                                case "global":
                                    {
                                        controler._copiedGlobalDataFiles.Extension = dtc?.FilesExtension.ToList();
                                        controler._copiedGlobalDataFiles.Containing = dtc?.ContainNames.ToList();
                                        controler._copiedGlobalDataFiles.Destination = dtc?.DestinationFolder;
                                    }
                                    break;
                            }
                        }

                        foreach (var ftr in c.FilesToRemove)
                        {
                            controler._removingDataFiles.Extension = ftr?.FilesExtension.ToList();
                        }

                        return controler;
                    }
                }
            } catch(Exception ex) {

                MessageBoxResult result = MessageBox.Show("Problem with read a configration file.Error: "+ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);                
            }

            return null;
        }


    }
}

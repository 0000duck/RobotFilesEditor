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
        XmlSerializer serializer;
        FileStream fileStream;
        XmlReader reader;
        ControlersConfiguration controlerConfiguration;

        public FilesSerialization()
        {}

        public ControlersConfiguration ReadAplicationConfiguration()
        {
            if (String.IsNullOrEmpty(GlobalData.ConfigurationFileName))
            {
                throw new ArgumentNullException();
            }
            try
            {
                serializer = new XmlSerializer(typeof(ControlersConfiguration));
                fileStream = new FileStream(GlobalData.ConfigurationFileName, FileMode.Open);
                reader= XmlReader.Create(fileStream); 
                controlerConfiguration = (ControlersConfiguration)serializer.Deserialize(reader);
                fileStream.Close();                
            }catch(Exception ex)
            {
                throw ex;
            }
            return controlerConfiguration;
        }

        //public Controler GetControlerConfigration(string controlerType)
        //{
        //    Controler controler = new Controler();

        //    try {

        //        XmlSerializer serializer = new XmlSerializer(typeof(ControlersConfiguration));

        //        // A FileStream is needed to read the XML document.
        //        FileStream fs = new FileStream(GlobalData.ConfigurationFileName, FileMode.Open);
        //        XmlReader reader = XmlReader.Create(fs);

        //        // Declare an object variable of the type to be deserialized.
        //        ControlersConfiguration cc;



        //        // Use the Deserialize method to restore the object's state.
        //        cc = (ControlersConfiguration)serializer.Deserialize(reader);
        //        fs.Close();

        //        foreach (var c in cc.Controlers)
        //        {
        //            if (c.ControlerType.Equals(controlerType))
        //            {
        //                foreach (var ftc in c.FilesToCopy)
        //                {
        //                    switch (ftc?.Type?.ToLower())
        //                    {
        //                        case "program":
        //                            {
        //                                controler._productionCopiedFiles.FileExtensions = ftc?.FilesExtension.ToList();
        //                                controler._productionCopiedFiles.ContainsAtName = ftc?.ContainNames.ToList();
        //                                controler._productionCopiedFiles.DestinationFolder = ftc?.DestinationFolder;
        //                            }
        //                            break;
        //                        case "service":
        //                            {
        //                                controler._serviceCopiedFiles.FileExtensions = ftc?.FilesExtension.ToList();
        //                                controler._serviceCopiedFiles.ContainsAtName = ftc?.ContainNames.ToList();
        //                                controler._serviceCopiedFiles.DestinationFolder = ftc?.DestinationFolder;
        //                            }
        //                            break;
        //                    }
        //                }

        //                foreach (var dtc in c.DataToCopy)
        //                {
        //                    switch (dtc?.Type?.ToLower())
        //                    {
        //                        case "olp":
        //                            {
        //                                controler._copiedOlpDataFiles.FileExtensions = dtc?.FilesExtension.ToList();
        //                                controler._copiedOlpDataFiles.ContainsAtName = dtc?.ContainNames.ToList();
        //                                controler._copiedOlpDataFiles.DestinationFolder = dtc?.DestinationFolder;
        //                            }
        //                            break;
        //                        case "global":
        //                            {
        //                                controler._copiedGlobalDataFiles.FileExtensions = dtc?.FilesExtension.ToList();
        //                                controler._copiedGlobalDataFiles.ContainsAtName = dtc?.ContainNames.ToList();
        //                                controler._copiedGlobalDataFiles.DestinationFolder = dtc?.DestinationFolder;
        //                            }
        //                            break;
        //                    }
        //                }

        //                foreach (var ftr in c.FilesToRemove)
        //                {
        //                    controler._removingDataFiles.FileExtensions = ftr?.FilesExtension.ToList();
        //                }

        //                return controler;
        //            }
        //        }
        //    } catch(Exception ex) {

        //        MessageBoxResult result = MessageBox.Show("Problem with read a configration file.Error: "+ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);                
        //    }

        //    return null;
        //}


    }
}

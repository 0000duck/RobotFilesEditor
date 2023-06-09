﻿using RobotFilesEditor.Model.Operations;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RobotFilesEditor
{
    public class Serialization
    {
        public XmlControlersConfiguration ReadAplicationConfiguration()
        {
            XmlControlersConfiguration controlerConfiguration;

            if (String.IsNullOrEmpty(CommonLibrary.CommonGlobalData.ConfigurationFileName))
            {
                throw new ArgumentNullException();
            }

            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(XmlControlersConfiguration));
                FileStream fileStream = new FileStream(CommonLibrary.CommonGlobalData.ConfigurationFileName, FileMode.Open);
                XmlReader reader = XmlReader.Create(fileStream);
                controlerConfiguration = (XmlControlersConfiguration)deserializer.Deserialize(reader);
                fileStream.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
            return controlerConfiguration;
        }

        public void SaveAplicationConfiguration(XmlControlersConfiguration xmlControlersConfiguration)
        {
            if (String.IsNullOrEmpty(CommonLibrary.CommonGlobalData.ConfigurationFileName))
            {
                throw new ArgumentNullException();
            }

            try
            {
                XmlSerializer serializer= new XmlSerializer(typeof(XmlControlersConfiguration));
                FileStream fileStream = new FileStream(CommonLibrary.CommonGlobalData.ConfigurationFileName, FileMode.Create);

                XmlWriter writer = new XmlTextWriter(fileStream, Encoding.Default);
                serializer.Serialize(fileStream, xmlControlersConfiguration);
                writer.Close();  
                fileStream.Close();      
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }
    }
}

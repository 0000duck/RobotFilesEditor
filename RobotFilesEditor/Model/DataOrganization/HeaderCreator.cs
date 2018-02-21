using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class HeaderCreator
    {
        public List<string>GlobalFileHeader(string sourcePath)
        {
            List<string> header = new List<string>();
            List<string>paths= FilesTool.GetAllFilesFromDirectory(sourcePath);
            List<string> file = new List<string>();
            string headerPattern = @"^;\*+.*(Programm|Beschreibung|Roboter|Firma|Ersteller|Datum|Aenderungsverlauf|\*{20})";


            foreach (string path in paths)
            {
                file = FilesTool.GetSourceFileText(path);

                int i=file.FindIndex(x => Regex.IsMatch(x, headerPattern));

                if (i>=0)
                {
                    string line = file[i];

                    while (Regex.IsMatch(line, headerPattern))
                    {
                        header.Add(line);
                        i++;
                        line = file[i];
                    }

                    if (header?.Count > 0)
                    {
                        return header;
                    }
                }               
            }           

            return header;
        }

        public List<FileLineProperties>GroupsHeaders()
        {
            List<FileLineProperties> result = new List<FileLineProperties>();



        }


    }
}

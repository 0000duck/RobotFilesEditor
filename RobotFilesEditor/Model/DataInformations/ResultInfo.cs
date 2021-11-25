using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using RobotFilesEditor.Model.Operations;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

namespace RobotFilesEditor
{
    public class ResultInfo
    {
        public string Path { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public string Color
        {
            get
            {
                if (string.IsNullOrEmpty(Description) == false)
                {
                    return Colors.Tomato.ToString();
                }
                else
                {
                    return Colors.Black.ToString();
                }
            }        
        }
        public ICommand OpenInOtherProgramCommand { get; set; }
        public string FontWeight
        {
            get
            {
                if (Bold)
                {
                    return "Bold";
                }
                else
                {
                    return "Normal";
                }
            }
        }
        public bool HasError
        {
            get { return !string.IsNullOrEmpty(Description); }
        }
        public bool Bold { get; set; }
        public ResultInfo()
        {
            OpenInOtherProgramCommand = new RelayCommand(OpenInOtherProgramCommandExecute);
        }         

        public static ResultInfo CreateResultInfo(FileLineProperties fileLineProperies)
        {
            ResultInfo resultInfo = new ResultInfo();

            resultInfo.Content = fileLineProperies.LineContent;
            resultInfo.Path = fileLineProperies.FileLinePath;

            if (fileLineProperies.HasExeption == true)
            {
                resultInfo.Description = $"Error in file: {System.IO.Path.GetFileName(fileLineProperies.FileLinePath)}\n" +
                                         $"at line: {fileLineProperies.LineNumber}\n" +
                                         $"for varible: \"{fileLineProperies.Variable}";
            }
            return resultInfo;
        }

        public static ResultInfo CreateResultInfo(string fileLine)
        {
            ResultInfo resultInfo = new ResultInfo();
            resultInfo.Content = fileLine;
            return resultInfo;
        }

        public static ResultInfo CreateResultInfoHeder(string header, string filePath)
        {
            ResultInfo resultInfo = new ResultInfo();
            resultInfo.Content = header;
            resultInfo.Path = filePath;
            return resultInfo;
        }

        public void OpenInOtherProgramCommandExecute()
        {
            if(String.IsNullOrEmpty(Path)==false)
            {
                if(File.Exists(Path))
                {
                    try
                    {
                       
                        //string args = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
                        //args += ",OpenAs_RunDLL " + Path;                        
                        //var temp = Process.Start("rundll32.exe", args);

                        Process.Start(Path);
                        if (HasError)
                        {
                            string test = System.IO.Path.Combine(GlobalData.DestPath, System.IO.Path.GetFileName(Path));
                            if (File.Exists(System.IO.Path.Combine(GlobalData.DestPath, System.IO.Path.GetFileName(Path))))
                            Process.Start(System.IO.Path.Combine(GlobalData.DestPath,System.IO.Path.GetFileName(Path)));
                        }
                        
                    }                    
                    catch (Exception ex)
                    {
                        SrcValidator.GetExceptionLine(ex);
                        throw ex;
                    }                                                     
                }                
            }            
        }
    }   
}

﻿using GalaSoft.MvvmLight.Command;
using System;
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
        public ICommand OpenInNotepadCommand { get; set; }

        public ResultInfo()
        {
            OpenInNotepadCommand = new RelayCommand(OpenInNotepadCommandExecute);
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

        private void OpenInNotepadCommandExecute()
        {
            System.Diagnostics.Process.Start("notepad++.exe", Path);
        }
    }   
}

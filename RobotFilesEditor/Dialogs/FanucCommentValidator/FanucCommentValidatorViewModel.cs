﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.FanucCommentValidator
{
    public class FanucCommentValidatorViewModel : ViewModelBase
    {
        #region fields
        int lengthLimitInternal;
        #endregion
        #region ctor
        public FanucCommentValidatorViewModel(string line, string path, int lengthLimit)
        {
            lengthLimitInternal = lengthLimit;
            Regex lineContentRegex = new Regex(@"(?<=^\s*\d+\s*:\s*(!|PR_CALL CMN.*'))[^']+");
            string lineContent = lineContentRegex.Match(line).ToString();
            InputLine = new TextForCommenValidator(path, lengthLimit, line);
            //OutputLine = lineContent.Substring(0, lengthLimit);
            OutputLine = lineContent;
            SetCommands();
        }
        #endregion

        #region props
        private TextForCommenValidator inputLine;
        public TextForCommenValidator InputLine
        {
            get { return inputLine; }
            set { inputLine = value; }
        }

        private string outputLine;
        public string OutputLine
        {
            get { return outputLine; }
            set
            {                
                //if (value != outputLine && value.Length > lengthLimitInternal)
                //{
                //    value = value.Substring(0, lengthLimitInternal);
                //}
                Set(ref outputLine, value);
                CharCounter = "";
                if (outputLine.Length > 32)
                    EnableOK = false;
                else
                    EnableOK = true;
            }
        }

        private string charCounter;
        public string CharCounter
        {
            get { return charCounter; }
            set
            {
                if (value != charCounter)
                {
                    value = "Character counter: " + OutputLine.Length + "/" + lengthLimitInternal;
                    Set(ref charCounter, value);
                }
            }
        }

        private bool enableOK;
        public bool EnableOK
        {
            get { return enableOK; }
            set {
                Set(ref enableOK, value);
            }
        }


        #endregion

        #region commands
        public ICommand OkCommand { get; set; }

        #endregion

        #region methods
        private void SetCommands()
        {
            OkCommand = new RelayCommand(OkExecute);
        }

        private void OkExecute()
        {
            var window = Application.Current.Windows.Cast<Window>().Single(w => w.DataContext == this);
            window.DialogResult = false;
            window.Close();
        }
        #endregion
    }

    public class TextForCommenValidator
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }

        public TextForCommenValidator(string path, int limit, string line)
        {
            Line1 = path;
            Line2 = "Comment is longer than " + limit + " characters!";
            Line3 = line;
        }

    }
}

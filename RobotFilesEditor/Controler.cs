using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RobotFilesEditor
{
    public class Controler : INotifyPropertyChanged
    {
        #region Public

        public string ContolerType
        {
            get { return _contolerType; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(ContolerType));
                }

                if (_contolerType != value)
                {
                    _contolerType = value;
                    OnPropertyChanged(nameof(_contolerType));
                }
            }
        }
        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(DestinationPath));
                }

                if (_destinationPath != value)
                {
                    if (Directory.Exists(value))
                    {
                        _destinationPath = value;
                        OnPropertyChanged(nameof(DestinationPath));
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory \'{value} \'not exist!");
                    }
                }
            }
        }
        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(DestinationPath));
                }

                if (_sourcePath != value)
                {
                    if (Directory.Exists(value))
                    {
                        _sourcePath = value;
                        OnPropertyChanged(nameof(SourcePath));
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory: \'{value} \'not exist!");
                    }
                }
            }
        }
        public Operations Operations
        {
            get { return _operations; }
            set
            {
                if(_operations!=value)
                {
                    _operations = value;
                }
            }
        }
        #endregion Public

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private 
        private string _destinationPath;
        private string _sourcePath;
        private string _contolerType;
        private Operations _operations;
        #endregion Private 

        [NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Controler()
        {
            Operations = new Operations();
        }       
    }
}

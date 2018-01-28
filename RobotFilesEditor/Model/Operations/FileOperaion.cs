using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor
{
    public class FileOperaion: Operation, IFileOperations
    {
        #region Public
        public Filter Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                }
            }
        }
        public List<string> FileExtensions
        {
            get { return _fileExtensions; }
            set
            {
                if (_fileExtensions != value)
                {
                    _fileExtensions = value;
                }
            }
        }
        #endregion Public

        #region Private
        public List<string> _fileExtensions;
        public Filter _filter;
        #endregion Private

        public new void FollowOperation()
        {
            switch (Action)
            {
                case GlobalData.Action.Copy:
                    {
                        CopyFile();
                    }
                    break;
                case GlobalData.Action.Move:
                    {
                        MoveFile();
                    }
                    break;
                case GlobalData.Action.Remove:
                    {
                        RemoveFile();
                    }
                    break;
            }
        }

        public bool CopyFile()
        {
            throw new NotImplementedException();
        }

        public bool MoveFile()
        {
            throw new NotImplementedException();
        }

        public bool RemoveFile()
        {
            throw new NotImplementedException();
        }
       


    }
}

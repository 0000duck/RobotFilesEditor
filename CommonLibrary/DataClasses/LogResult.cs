using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.DataClasses
{
    public enum LogResultTypes { OK, Warning, Error, Information };
    public class LogResult : INotifyPropertyChanged
    {
        public string ContentToDisplay { get; set; }
        public LogResultTypes InfoType { get; set; }

        public bool Visible { get { return m_Visible; } set { m_Visible = value; OnPropertyChanged(nameof(Visible)); } }
        private bool m_Visible;

        public string DateTimeString { get; set; }

        public LogResult(string contentToDisplay, LogResultTypes infoType)
        {
            ContentToDisplay = contentToDisplay;
            InfoType = infoType;
            DateTimeString = DateTime.Now.ToString("G");
            Visible = true;
        }

        internal void SwitchVisibility(bool vis)
        {
            Visible = vis;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return DateTimeString + " - " + InfoType.ToString() + " - " + ContentToDisplay;
        }
    }
}

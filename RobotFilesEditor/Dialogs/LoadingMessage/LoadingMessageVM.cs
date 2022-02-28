using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor.Dialogs.LoadingMessage
{
    public class LoadingMessageVM : ViewModelBase, INotifyPropertyChanged
    { 
        private readonly ICommand instigateWorkCommand;

        public LoadingMessageVM()
        {
            CloseCommand = new RelayCommand(CloseExecute);
        }

        public ICommand InstigateWorkCommand
        {
            get { return this.instigateWorkCommand; }
        }

        private void CloseExecute()
        {
            var window = Application.Current.Windows
            .Cast<Window>()
            .Single(w => w.DataContext == this);
            window.Close();
        }

        private Visibility setVisible;
        public Visibility SetVisible
        {
            get { return setVisible; }
            set
            {
                Set(ref setVisible, value);
                OnPropertyChanged();
            }
        }


        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                Set(ref text, value);
                OnPropertyChanged();
            }
        }

        private int progress;
        public int Progress
        {
            get { return progress; }
            set
            {
                Set(ref progress, value);
                OnPropertyChanged();
            }
        }

        private int progressFromTopApp;
        public int ProgressFromTopApp
        {
            get { return progressFromTopApp; }
            set
            {
                Set(ref progressFromTopApp, value);
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; set; }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

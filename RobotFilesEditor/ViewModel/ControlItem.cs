using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RobotFilesEditor
{
    public class ControlItem: ViewModelBase
    {       
        #region events

        public event EventHandler<ControlItem> ControlItemSelected;

        #endregion

        #region Property

        public string Title { get; set; }
        public ObservableCollection<ResultInfo> OperationResult { get; set; }
        public ICommand ClickedCommand { get; set; }
        public ICommand ExecuteOperationCommand { get; set; }
        public ICommand PreviewOperationCommand { get; set; }
        public List<IOperation> Operations { get; set; }
        public string ViewWindowVisibility
        {
            get
            {
                if(OperationResult.Count>0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }
        public string ExecuteAviable{ get; set;}

        #endregion

        #region Constructors

        public ControlItem(string title)
        {
            Title = title;
            Operations = new List<IOperation>();
            OperationResult = new ObservableCollection<ResultInfo>();
            ClickedCommand = new RelayCommand(ClickedCommandExecute);
            ExecuteOperationCommand = new RelayCommand(ExecuteOperationCommandExecute);
            PreviewOperationCommand = new RelayCommand(PreviewOperationCommandExecute);
        }

        #endregion

        #region Medthods

        private void ClickedCommandExecute()
        {
            OnControlItemSelected();
        }

        protected void OnControlItemSelected()
        {
            ControlItemSelected?.Invoke(this, this);
        }

        private void ExecuteOperationCommandExecute()
        {
            if(DetectExceptions()==false)
            {
                IOperation activeOperation;
                List<string> exeptions = new List<string>();
                List<ResultInfo> result = new List<ResultInfo>();
                OperationResult.Clear();

                Operations.OrderBy(y => y.Priority).ToList();
                foreach (var operation in Operations)
                {
                    try
                    {
                        activeOperation = operation;
                        activeOperation.ExecuteOperation();
                        result = activeOperation.GetOperationResult();
                        if (result?.Count > 0)
                        {
                            result.ForEach(x => OperationResult.Add(x));
                        }
                        else
                        {
                            OperationResult.Add(new ResultInfo() { Content = "No result to show" });
                        }
                        RaisePropertyChanged(nameof(ViewWindowVisibility));

                        if (activeOperation != null)
                        {
                            activeOperation?.ClearMemory();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }else
            {
                MessageBoxResult result = MessageBox.Show($"Refresh preview?", "Error!", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    PreviewOperationCommandExecute();
                }
            }          
        }
        private void PreviewOperationCommandExecute()
        {
            IOperation activeOperation;
            List<string> exeptions = new List<string>();
            List<ResultInfo> result = new List<ResultInfo>();
            OperationResult.Clear();

            Operations.OrderBy(y => y.Priority).ToList();
            foreach (var operation in Operations)
            {
                try
                {
                    activeOperation = operation;
                    activeOperation.PrepareOperation();
                    result = activeOperation.GetOperationResult();
                    if (result?.Count > 0)
                    {
                        result.ForEach(x => OperationResult.Add(x));
                    }
                    else
                    {
                        OperationResult.Add(new ResultInfo() { Content = "No result to show" });
                    }

                    RaisePropertyChanged(nameof(ViewWindowVisibility));

                    if (activeOperation != null)
                    {
                        activeOperation?.ClearMemory();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private bool DetectExceptions()
        {
            List<ResultInfo> Exceptions = new List<ResultInfo>();

            Exceptions = OperationResult.Where(x => string.IsNullOrEmpty(x.Description)==false).ToList();

            foreach(var exeption in Exceptions)
            {
                MessageBoxResult result = MessageBox.Show($"Error: {exeption.Description}.\nOpen file?", "Error!", MessageBoxButton.YesNo);

                if (result==MessageBoxResult.Yes)
                {
                    exeption.OpenInNotepadCommandExecute();
                }
            }

            if(Exceptions?.Count>0)
            {
                return true;
            }else
            {
                return false;
            }
        }
        #endregion        
    }
}

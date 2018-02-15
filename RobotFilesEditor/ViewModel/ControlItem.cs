using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            try
            {
                if (DetectExceptions() == false)
                {
                    IOperation activeOperation;
                    List<string> exeptions = new List<string>();
                    List<ResultInfo> result = new List<ResultInfo>();
                    OperationResult.Clear();

                    Operations.OrderBy(y => y.Priority).ToList();
                    foreach (var operation in Operations)
                    {
                        activeOperation = operation;
                        activeOperation.ExecuteOperation();
                        result = activeOperation.GetOperationResult();
                        if (result?.Count > 0)
                        {
                            result.ForEach(x => OperationResult.Add(x));
                        }
                       
                        RaisePropertyChanged(nameof(ViewWindowVisibility));

                        if (activeOperation != null)
                        {
                            activeOperation?.ClearMemory();
                        }
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"Refresh preview?", "Error!", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        PreviewOperationCommandExecute();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK);
            }            
        }
        private void PreviewOperationCommandExecute()
        {
            IOperation activeOperation;
            List<string> exeptions = new List<string>();
            List<ResultInfo> result = new List<ResultInfo>();

            try
            {
                OperationResult.Clear();

                Operations.OrderBy(y => y.Priority).ToList();
                foreach (var operation in Operations)
                {
                    activeOperation = operation;
                    activeOperation.PreviewOperation();
                    result = activeOperation.GetOperationResult();
                    if (result?.Count > 0)
                    {
                        result.ForEach(x => OperationResult.Add(x));
                    }

                    RaisePropertyChanged(nameof(ViewWindowVisibility));

                    if (activeOperation != null)
                    {
                        activeOperation?.ClearMemory();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK);
            }          
        }

        private bool DetectExceptions()
        {
            List<ResultInfo> Exceptions = new List<ResultInfo>();

            Exceptions = OperationResult.Where(x => string.IsNullOrEmpty(x.Description)==false).ToList();

            foreach(var exeption in Exceptions)
            {
                MessageBoxResult result = MessageBox.Show($"Error: {exeption.Description}.\nOpen file?", $"Error on {Title}", MessageBoxButton.YesNo);

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

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
        public bool ExecuteOperationButtonIsEnabled { get; set; }
        public bool PreviewOperationButtonIsEnabled { get; set; }
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
            ExecuteOperationButtonIsEnabled = true;
            PreviewOperationButtonIsEnabled = true;
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

        public void ExecuteOperationCommandExecute()
        {
            try
            {
                ExecuteOperationButtonIsEnabled = false;
                PreviewOperationCommandExecute();
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
                    if(DetectExceptions()==false)
                    {
                        MessageBox.Show($"Finish operation \"{Title}\" with success!", "Successed!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }                 
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK);
            }
            finally
            {
                ExecuteOperationButtonIsEnabled = true;
            }            
        }
        public void PreviewOperationCommandExecute()
        {
            IOperation activeOperation;
            List<string> exeptions = new List<string>();
            List<ResultInfo> result = new List<ResultInfo>();

            try
            {
                PreviewOperationButtonIsEnabled = false;
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
            finally
            {
                PreviewOperationButtonIsEnabled = true;
            }      
        }

        private bool DetectExceptions()
        {
            List<ResultInfo> exceptions = new List<ResultInfo>();
            string message = "";

            exceptions = OperationResult.Where(x => string.IsNullOrEmpty(x.Description)==false).ToList();

            foreach(var exeption in exceptions)
            {
                message+= $"\nError: {exeption.Description}.\n";
            }            

            if(exceptions?.Count>0)
            {
                MessageBox.Show(message, $"Errors in operation:\"{Title}\"!", MessageBoxButton.OK);
                return true;
            }else
            {
                return false;
            }
        }
        #endregion        
    }
}

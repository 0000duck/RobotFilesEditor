using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RobotFilesEditor.Model.Operations;
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
        //public ICommand ClickedCommand { get; set; }
        public ICommand ExecuteOperationCommand { get; set; }
        public ICommand PreviewOperationCommand { get; set; }
        public List<IOperation> Operations { get; set; }
        
        private bool _checked;

        public bool Checked
        {
            get { return _checked; }
            set {
                if (value != _checked)
                {
                    GlobalData.RobotType = "";
                    _checked = value;
                    //ClickedCommandExecuteTest(_checked);
                    RaisePropertyChanged(nameof(Checked));
                }
            }
        }

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
        public int OrderNumber { get; set; }
        ResultInfo _selectedItemFromList;
        public ResultInfo SelectedItemFromList
        {
            get { return _selectedItemFromList; }
            set
            {
                if (_selectedItemFromList != value)
                {
                    _selectedItemFromList = value;
                    RaisePropertyChanged<ResultInfo>(() => SelectedItemFromList);
                    if (_selectedItemFromList !=null && !string.IsNullOrEmpty(_selectedItemFromList.Path))
                    {
                        _selectedItemFromList.OpenInOtherProgramCommandExecute();
                    }
                }
            }
        }
        #endregion

        #region Constructors

        public ControlItem(string title)
        {
            Title = title;
            Operations = new List<IOperation>();
            OperationResult = new ObservableCollection<ResultInfo>();
            //ClickedCommand = new RelayCommand(ClickedCommandExecute);
            ExecuteOperationCommand = new RelayCommand(ExecuteOperationCommandExecute);
            PreviewOperationCommand = new RelayCommand(PreviewOperationCommandExecute);
            ExecuteOperationButtonIsEnabled = true;
            PreviewOperationButtonIsEnabled = true;
        }


        #endregion

        #region Medthods

        //private void ClickedCommandExecute()
        //{
        //    OnControlItemSelected();
        //}

        public void ClickedCommandExecuteTest(bool _checked)
        {
            if (_checked)
            {
                OperationResult.Clear();
                GlobalData.ControllerType = this.Title;
                SrcValidator.GlobalFiles = null;
                ControlItemSelected?.Invoke(this, this);                
            }
            //Checked = false;
        }

        //protected void OnControlItemSelected()
        //{
        //    OperationResult.Clear();
        //    GlobalData.ControllerType = this.Title;
        //    SrcValidator.GlobalFiles = null;
        //    ControlItemSelected?.Invoke(this, this);
        //    Checked = false;
        //}

        public void ExecuteOperationCommandExecute()
        {
            try
            {
                ExecuteOperationButtonIsEnabled = false;

                //PreviewOperation();

                //if (DetectExceptions() == false)
                //{
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
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK);
            }
            finally
            {
                ExecuteOperationButtonIsEnabled = true;
            }            
        }
        public void PreviewOperationCommandExecute()
        {
            try
            {
                PreviewOperationButtonIsEnabled = false;
                PreviewOperation();
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
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

            if(exceptions.Any() & !SrcValidator.CopiedFiles)
            {
                MessageBox.Show(message, $"Errors in operation:\"{Title}\"!", MessageBoxButton.OK);
                return true;
            }else
            {
                return false;
            }
            
        }

        private void PreviewOperation()
        {
            IOperation activeOperation;
            List<string> exeptions = new List<string>();
            List<ResultInfo> result = new List<ResultInfo>();

            try
            {               
                OperationResult.Clear();
                //if (this.Title == "Move program files")
                //    OperationResult.Clear;
                Operations.OrderBy(y => y.Priority).ToList();
                foreach (var operation in Operations)
                {
                    if (operation.OperationName == "Copy global data to GlobalBase")
                    { }

                    activeOperation = operation;
                    activeOperation.PreviewOperation();
                    result = activeOperation.GetOperationResult();
                    if (activeOperation.OperationName == "Copy OLP files data")
                        SrcValidator.GetInputDataString(result);
                    if (activeOperation.OperationName == "Copy global data to GlobalBase")
                        SrcValidator.GetInputGlobalDataString(result);
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
            }catch(Exception ex)
            {
                SrcValidator.GetExceptionLine(ex);
                throw ex;
            }
        }
        #endregion
    }
}

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            IOperation activeOperation;
            List<string> exeptions = new List<string>();
            List<ResultInfo> result=new List<ResultInfo>();           

            Operations.OrderBy(y => y.Priority).ToList();
            foreach (var operation in Operations)
            {
                try
                {
                    //if (activeOperation != null)
                    //{
                    //    activeOperation?.ClearMemory();
                    //}                 

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
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private void PreviewOperationCommandExecute()
        {

        }

        #endregion        
    }
}

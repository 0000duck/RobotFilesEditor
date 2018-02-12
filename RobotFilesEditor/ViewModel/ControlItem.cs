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
    public class ControlItem
    {       
        #region events

        public event EventHandler<ControlItem> ControlItemSelected;

        #endregion

        #region Property

        public string Content { get; set; }
        public IObservable<ResultInfo> OperationResult { get; set; }

        public ICommand ClickedCommand { get; set; }
        public ICommand ExecuteOperationCommand { get; set; }
        public ICommand PreviewOperationCommand { get; set; }
        public List<IOperation> Operations { get; set; }

        #endregion

        #region Constructors

        public ControlItem(string title)
        {
            Content = title;
            Operations = new List<IOperation>();
            OperationResult = new IObservable<ResultInfo>();
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
                    OperationResult = new ObservableCollection(activeOperation.GetOperationResult());
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

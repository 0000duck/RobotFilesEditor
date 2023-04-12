using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ProgramTextFormat.Messaging;
using ProgramTextFormat.Model.RobotInstructions;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProgramTextFormat.ViewModel
{
    public partial class InstructionsViewModel : ObservableRecipient, IRecipient<InstructionsMessage>, IRecipient<EditValidMessage>
    {
        #region fields    
        int currentlyEditedRow;
        RobotInstructionBase lastInstruction;
        bool addInstructionActive;
        #endregion fields

        #region properties
        public ObservableCollection<string> RobotTypes { get => new ObservableCollection<string>() { "KUKA" }; }

        [ObservableProperty]
        ObservableCollection<RobotInstructionBase>? instructionsCollection;

        [ObservableProperty]
        bool buttonsEnabled;

        [ObservableProperty]
        bool editVisibility;

        [ObservableProperty]
        int selectedInstruction;
        #endregion properties

        [ObservableProperty]
        bool editValid;

        #region commands
        [RelayCommand]
        private void AddInstruction()
        {
            var instruction = new KukaInstruction();
            InstructionsCollection?.Add(instruction);
            SelectedInstruction = InstructionsCollection.Count - 1;
            EditInstruction();
            addInstructionActive = true;
        }

        [RelayCommand]
        private void EditInstruction()
        {
            if (SelectedInstruction < 0)
                return;
            EditValid = EditValidCheck();
            WeakReferenceMessenger.Default.Send<OKEnablerMessage>(new OKEnablerMessage(false));
            currentlyEditedRow = SelectedInstruction;
            lastInstruction = (RobotInstructionBase)InstructionsCollection[SelectedInstruction].Clone();
            EditVisibility = true;
            ButtonsEnabled = false;
            InstructionsCollection[SelectedInstruction].SetEditability(true);

        }

        [RelayCommand]
        private void RemoveInstruction()
        {
            if (SelectedInstruction >= 0 && SelectedInstruction < InstructionsCollection.Count - 1)
            {
                WeakReferenceMessenger.Default.Send<RemoveInstructionMessage>(new RemoveInstructionMessage(InstructionsCollection[SelectedInstruction]));
                InstructionsCollection.RemoveAt(SelectedInstruction);
            } 
        }

        [RelayCommand]
        private void EditOK()
        {
            WeakReferenceMessenger.Default.Send<OKEnablerMessage>(new OKEnablerMessage(true));
            EditVisibility = false;
            ButtonsEnabled = true;
            InstructionsCollection[currentlyEditedRow].SetEditability(false);
            if (!addInstructionActive && !InstructionsCollection[currentlyEditedRow].Name.Equals(lastInstruction.Name))
                WeakReferenceMessenger.Default.Send<UpdateInstructionMessage>(new UpdateInstructionMessage(new KeyValuePair<string, string>(lastInstruction.Name, InstructionsCollection[currentlyEditedRow].Name)));
            if (addInstructionActive)
                WeakReferenceMessenger.Default.Send<AddInstructionMessage>(new AddInstructionMessage(InstructionsCollection[currentlyEditedRow]));
            addInstructionActive = false;
        }

        [RelayCommand]
        private void CancelEdit()
        {
            WeakReferenceMessenger.Default.Send<OKEnablerMessage>(new OKEnablerMessage(true));
            EditVisibility = false;
            ButtonsEnabled = true;
            InstructionsCollection[currentlyEditedRow] = lastInstruction;
            InstructionsCollection[currentlyEditedRow].SetEditability(false);
            addInstructionActive = false;
        }
        #endregion commands

        #region constructor

        #endregion constructor
        public InstructionsViewModel()
        {
            WeakReferenceMessenger.Default.Register<InstructionsMessage>(this);
            WeakReferenceMessenger.Default.Register<EditValidMessage>(this);
            SelectedInstruction = -1;
            ButtonsEnabled = true;
            EditVisibility = false;
            addInstructionActive = false;
        }
        #region methods
        public void Receive(InstructionsMessage message)
        {
            List<RobotInstructionBase> tempInstructions = new List<RobotInstructionBase>();
            tempInstructions.AddRange(message.Value.KukaInstructions);
            InstructionsCollection = CommonLibrary.CommonMethods.ToObservableCollection(tempInstructions);
        }
        private bool EditValidCheck()
        {
            if (InstructionsCollection is null)
                return false;
            if (SelectedInstruction >= 0)
            {
                var instruction = InstructionsCollection[SelectedInstruction];
                if (instruction != null)
                    if (!string.IsNullOrEmpty(instruction.Name))
                        if (!string.IsNullOrEmpty(instruction.KeyWordsString))
                            return true;
            }
            return false;
        }

        public void Receive(EditValidMessage message)
        {
            EditValid = EditValidCheck();
        }
        #endregion methods

    }
}

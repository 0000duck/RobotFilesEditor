using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ProgramTextFormat.Messaging;
using ProgramTextFormat.Model.Helpers;
using ProgramTextFormat.Model.RobotInstructions;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramTextFormat.ViewModel
{
    public partial class RulesViewModel : ObservableRecipient, IRecipient<RulesMessage>, IRecipient<EditValidMessage>
    {

        #region fields    
        int currentlyEditedRow;
        ProgramFormatRule lastRule;
        bool addRuleActive;
        #endregion fields


        #region properties
        public RulesMessage ReceivedRules { get; set; }

        [ObservableProperty]
        bool buttonsEnabled;

        [ObservableProperty]
        bool editVisibility;

        [ObservableProperty]
        int selectedRule;

        [ObservableProperty]
        ObservableCollection<ProgramFormatRule>? rulesCollection;

        [ObservableProperty]
        ObservableCollection<RobotInstructionBase>? instructionBases;

        [ObservableProperty]
        bool editValid;
        public List<string> Actions { get => FillActions(); }
        #endregion properties


        #region commands
        [RelayCommand]
        private void DeleteRule()
        {
            if (SelectedRule >= 0 && SelectedRule <= RulesCollection?.Count - 1)
            {
                WeakReferenceMessenger.Default.Send<RemoveRuleMessage>(new RemoveRuleMessage(RulesCollection[SelectedRule]));
                RulesCollection?.Remove(RulesCollection[SelectedRule]);
            }
        }

        [RelayCommand]
        private void AddRule()
        {
            addRuleActive = true;
            int number = Statics.GetHighest(RulesCollection) + 1;
            var rule = new ProgramFormatRule(number.ToString(), "UnknownInstruction", Actions.FirstOrDefault().ToString(), false);
            RulesCollection.Add(rule);
            SelectedRule = RulesCollection.Count - 1;
            EditRule();
        }

        [RelayCommand]
        private void EditRule()
        {
            if (SelectedRule < 0)
                return;
            EditValid = EditValidCheck();
            WeakReferenceMessenger.Default.Send<OKEnablerMessage>(new OKEnablerMessage(false));
            currentlyEditedRow = SelectedRule;
            lastRule = (ProgramFormatRule)RulesCollection[SelectedRule].Clone();
            EditVisibility = true;
            ButtonsEnabled = false;
            RulesCollection[SelectedRule].SetEditability(true);
        }

        [RelayCommand]
        private void EditOK()
        {
            WeakReferenceMessenger.Default.Send<OKEnablerMessage>(new OKEnablerMessage(true));
            EditVisibility = false;
            ButtonsEnabled = true;
            RulesCollection[currentlyEditedRow].SetEditability(false);
            if (addRuleActive)
                WeakReferenceMessenger.Default.Send<AddRuleMessage>(new AddRuleMessage(RulesCollection[currentlyEditedRow]));
            addRuleActive = false;
        }

        [RelayCommand]
        private void CancelEdit()
        {
            WeakReferenceMessenger.Default.Send<OKEnablerMessage>(new OKEnablerMessage(true));
            EditVisibility = false;
            ButtonsEnabled = true;
            RulesCollection[currentlyEditedRow] = lastRule;
            RulesCollection[currentlyEditedRow].SetEditability(false);
            addRuleActive= false;
        }
        #endregion commands


        #region constuctor
        public RulesViewModel()
        {
            WeakReferenceMessenger.Default.Register<RulesMessage>(this);
            WeakReferenceMessenger.Default.Send<RulesLoaded>(new RulesLoaded(true));
            WeakReferenceMessenger.Default.Register<EditValidMessage>(this);
            SelectedRule = -1;
            ButtonsEnabled = true;
            EditVisibility = false;
            addRuleActive = false;
        }
        #endregion constuctor

        #region private methods
        private List<string> FillActions()
        {
            var tempactions = new List<string>();
            var actionsArray = Enum.GetValues(typeof(Actions));
            foreach (Actions action in actionsArray) { tempactions.Add(action.ToString()); }
            return tempactions;
        }

        private bool EditValidCheck()
        {
            if (RulesCollection is null)
                return false;
            if (SelectedRule >= 0 && SelectedRule < RulesCollection.Count)
            {
                var rule = RulesCollection[SelectedRule];
                if (rule != null)
                    if (!string.IsNullOrEmpty(rule.Number))
                        if (!string.IsNullOrEmpty(rule.Instruction) && !rule.Instruction.Equals("UnknownInstruction"))
                            return true;
            }
            return false;
        }
        #endregion private methods

        #region messaging
        public void Receive(RulesMessage message)
        {
            RulesCollection = CommonLibrary.CommonMethods.ToObservableCollection(message.Value.Rules.ProgramFormatRule.ToList());
            InstructionBases = new ObservableCollection<RobotInstructionBase>();
            message.Value.Instructions.KukaInstructions.ForEach(x => InstructionBases.Add(x));           
        }

        public void Receive(EditValidMessage message)
        {
            EditValid = EditValidCheck();
        }
        #endregion messaging

    }
}

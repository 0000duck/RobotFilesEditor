using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProgramTextFormat.Model;
using ProgramTextFormat.Model.Helpers;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace ProgramTextFormat.ViewModel
{
    public partial class ProgramTextViewModel : ObservableObject
    {

        #region fields
        ProgramFormatter xmlDeserialized;
        int currentlyEditedRow;
        ProgramFormatRule lastRule;
        #endregion fields

        #region properties
        [ObservableProperty]
        bool buttonsEnabled;

        [ObservableProperty]       
        bool editVisibility;

        [ObservableProperty]
        ObservableCollection<ProgramFormatRule>? rules;

        [ObservableProperty]
        bool groupItems;

        [ObservableProperty]
        int selectedRule;

        public List<string> Actions { get => FillActions(); }

        #endregion properties

        #region commands
        [RelayCommand]
        private void AddInstruction()
        { }

        [RelayCommand]
        private void ShowInstructions()
        { }

        [RelayCommand]
        private void DeleteRule()
        {
            if (selectedRule > 0 && selectedRule <= rules.Count )
                rules.Remove(rules[selectedRule]);
        }

        [RelayCommand]
        private void AddRule()
        {
            int number = Statics.GetHighest(rules) + 1;
            var rule = new ProgramFormatRule(number.ToString(), "UnknownInstruction", Actions.FirstOrDefault().ToString(), false);
            rules.Add(rule);
        }

        [RelayCommand]
        private void EditRule()
        {
            if (SelectedRule < 0)
                return;
            currentlyEditedRow= SelectedRule;
            lastRule = (ProgramFormatRule)Rules[SelectedRule].Clone();
            EditVisibility = true;
            ButtonsEnabled = false;
            Rules[SelectedRule].SetEditability(true);
        }

        [RelayCommand]
        private void EditOK()
        {
            EditVisibility = false;
            ButtonsEnabled = true;
            Rules[currentlyEditedRow].SetEditability(false);
        }

        [RelayCommand]
        private void CancelEdit()
        {
            EditVisibility = false;
            ButtonsEnabled = true;
            Rules[currentlyEditedRow] = lastRule;
            Rules[currentlyEditedRow].SetEditability(false);
        }
        #endregion commands

        #region constructor
        public ProgramTextViewModel()
        {
            SelectedRule = -1;
            ButtonsEnabled = true;
            EditVisibility = false;
            string path = @"C:\Projekty\inne\Harvester\ProgramFormatter.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(ProgramFormatter));           
            using (Stream reader = new FileStream(path, FileMode.Open))
                xmlDeserialized = (ProgramFormatter)serializer.Deserialize(reader);
            Rules = CommonLibrary.CommonMethods.ToObservableCollection(xmlDeserialized?.Rules.ProgramFormatRule);
        }
        #endregion constructor

        #region private methods
        private List<string> FillActions()
        {
            var tempactions = new List<string>();
            var actionsArray = Enum.GetValues(typeof(Actions));
            foreach (Actions action in actionsArray) { tempactions.Add(action.ToString()); }
            return tempactions;
        }
        #endregion private methods
    }
}

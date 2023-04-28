using CommonLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ProgramTextFormat.Messaging;
using ProgramTextFormat.Model;
using ProgramTextFormat.Model.Helpers;
using ProgramTextFormat.Model.RobotInstructions;
using ProgramTextFormat.Model.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ProgramTextFormat.ViewModel
{
    public partial class MainViewModel : MessagingBase
    {
         
        #region fields
        ProgramFormatter xmlDeserialized;
        string path;
        #endregion fields

        #region properties
        [ObservableProperty]
        bool instructionsActive;

        [ObservableProperty]
        bool rulesActive;

        [ObservableProperty]
        bool okEnabled;
        #endregion properties

        #region commands

        [RelayCommand]
        private void OK(Window window)
        {
            var serializer = new XmlSerializer(typeof(ProgramFormatter));
            System.IO.File.WriteAllText(path, string.Empty);
            using (Stream fs = new FileStream(path, FileMode.Open))
            {
                serializer.Serialize(fs, xmlDeserialized);
            }
            if (window != null)
                window.Close();
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            if (window != null)
                window.Close();
        }

        [RelayCommand]
        private void SetRules()
        {
            InstructionsActive = false;
            RulesActive = true;
            WeakReferenceMessenger.Default.Send<RulesMessage>(new RulesMessage(xmlDeserialized));
        }

        [RelayCommand]
        private void SetInstructions()
        {
            RulesActive = false;
            InstructionsActive = true;
            WeakReferenceMessenger.Default.Send<InstructionsMessage>(new InstructionsMessage(xmlDeserialized.Instructions));
        }
        #endregion commands

        #region constructor
        public MainViewModel()
        {
            RegisterMessages();
            path = CommonMethods.GetFilePath("ProgramFormatter.xml");
            RulesActive = true; InstructionsActive = false; OkEnabled = true;
            XmlSerializer serializer = new XmlSerializer(typeof(ProgramFormatter));   
            
            using (Stream reader = new FileStream(path, FileMode.Open))
                xmlDeserialized = (ProgramFormatter)serializer.Deserialize(reader);
            xmlDeserialized.Initialize();
        }
        #endregion constructor


        #region private methods
        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<RulesLoaded>(this);
            WeakReferenceMessenger.Default.Register<UpdateInstructionMessage>(this);
            WeakReferenceMessenger.Default.Register<OKEnablerMessage>(this);
            WeakReferenceMessenger.Default.Register<AddInstructionMessage>(this);
            WeakReferenceMessenger.Default.Register<AddRuleMessage>(this);
            WeakReferenceMessenger.Default.Register<RemoveRuleMessage>(this);
            WeakReferenceMessenger.Default.Register<RemoveInstructionMessage>(this);
        }
        #endregion private methods


        #region messaging

        public override void Receive(AddInstructionMessage message)
        {
            if (message.Value is KukaInstruction kukaInstruction)
                xmlDeserialized.Instructions.KukaInstructions.Add(kukaInstruction);
        }

        public override void Receive(AddRuleMessage message)
        {
            xmlDeserialized.Rules.ProgramFormatRule.Add(message.Value);
        }
        public override void Receive(RulesLoaded message)
        {
            SetRules();
        }

        public override void Receive(UpdateInstructionMessage message)
        {
            if (xmlDeserialized.Rules.ProgramFormatRule.Any(x => x.Instruction.Equals(message.Value.Key, StringComparison.OrdinalIgnoreCase)))
                xmlDeserialized.Rules.ProgramFormatRule.FirstOrDefault(x => x.Instruction.Equals(message.Value.Key, StringComparison.OrdinalIgnoreCase)).Instruction = message.Value.Value;
        }

        public override void Receive(OKEnablerMessage message)
        {
            OkEnabled = message.Value;
        }

        public override void Receive(RemoveInstructionMessage message)
        {
            if (message.Value is KukaInstruction kukaInstruction)
                xmlDeserialized.Instructions.KukaInstructions.Remove(kukaInstruction);
        }

        public override void Receive(RemoveRuleMessage message)
        {
            xmlDeserialized.Rules.ProgramFormatRule.Remove(message.Value);
        }
        #endregion messaging



    }
}

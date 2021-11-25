using RobotFilesEditor.Model.Operations.DataClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RobotFilesEditor.Model.DataInformations
{
    public class FileValidationData : INotifyPropertyChanged
    {      

        private int _collNumber;
        public int CollNumber
        {
            get
            {return _collNumber; }

            set
            {
                if (value != this._collNumber)
                {
                    this._collNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private List<string> _collDescriptions;
        public List<string> CollDescriptions
        {
            get
            { return _collDescriptions; }

            set
            {
                if (value != this._collDescriptions)
                {
                    this._collDescriptions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _selectedDescription;
        public string SelectedDescription
        {
            get
            { return _selectedDescription; }

            set
            {
                if (value != this._selectedDescription)
                {
                    this._selectedDescription = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class OperationsPriority
        {
            public int Priority { get; set; }
            public string Command { get; set; }

            public OperationsPriority ()
            { }
            public OperationsPriority(int priority, string command)
            {
                Priority = priority;
                Command = command;
            }
        }

        public class FilesWithPriorities
        {
            public string Filename { get; set; }
            public List<OperationsPriority> Commands { get; set; }

            public FilesWithPriorities(string filename, List<OperationsPriority> commands)
            {
                Filename = filename;
                Commands = commands;
            }
        }

        public class CollisionWithDescr
        {
            public int Number { get; set; }
            public List<string> Description { get; set; }

            public CollisionWithDescr(int number, List<string> description)
            {
                Number = number;
                Description = description;
            } 

            public CollisionWithDescr ()
            { }
        }

        public class CollisionWithoutDescr
        {
            public int Number { get; set; }
            public string Type { get; set; }

            public CollisionWithoutDescr(int number, string type)
            {
                Number = number;
                Type = type;
            }

            public CollisionWithoutDescr()
            { }
        }

        public class ToolAndBase
        {
            public int Tool { get; set; }
            public int Base { get; set; }

            public ToolAndBase()
            {

            }
        }

        public class InconsistentCollDescriptions
        {
            public int Number { get; set; }
            public List<string> Descriptions { get; set; }

            public InconsistentCollDescriptions(int number, List<string> descriptions)
            {
                Number = number;
                Descriptions = descriptions;
            }

            public InconsistentCollDescriptions()
            { }
        }

        public class OpenAndCloseCommand
        {
            public string File { get; set; }
            public string Operation { get; set; }
            public int Number { get; set; }
            public int Line { get; set; }
            public bool IsStart { get; set; }
            public string Description { get; set; }

            public OpenAndCloseCommand(string file, string operation, int number, int line, bool isStart, string description)
            {
                File = file;
                Operation = operation;
                Number = number;
                Line = line;
                IsStart = isStart;
                Description = description;
            }

            public OpenAndCloseCommand()
            { }

        }

        public class Operation
        {
            public int Number { get; set; }
            public string Type { get; set; }
            public List<int> StartLines { get; set; }
            public List<int> EndLines { get; set; }

            public Operation(int number, string type, List<int> startLines, List<int> endLines)
            {
                Number = number;
                Type = type;
                StartLines = startLines;
                EndLines = endLines;
            }

            public Operation()
            { }

        }

        public class ValidationData
        {
            public IDictionary<string, string> SrcFiles { get; set; }
            public string OpereationName { get; set; }
            public List<string> DatFiles { get; set; }

            public ValidationData(IDictionary<string, string> srcFiles, string opereationName, List<string> datFiles)
            {
                SrcFiles = srcFiles;
                OpereationName = opereationName;
                DatFiles = datFiles;
            }
            public ValidationData()
            {

            }

        }

        public class Point
        {
            public string Name { get; set; }
            public bool IsHome { get; set; }
            public string FDAT { get; set; }
            public string PDAT { get; set; }
            public int ToolNr { get; set; }
            public string ToolName { get; set; }
            public int BaseNr { get; set; }
            public string BaseName { get; set; }
            public string LDAT { get; set; }

            public Point()
            {}

            public Point(string name, bool isHome, string fdat, string pdat, string ldat, int toolNr, string toolName, int baseNr, string baseName)
            {
                Name = name;
                IsHome = isHome;
                LDAT = ldat;
                FDAT = fdat;
                PDAT = pdat;
                ToolNr = toolNr;
                ToolName = toolName;
                BaseNr = baseNr;
                BaseName = baseName;
            }


        }

        public class Dats
        {
            public List<string> E6POS { get; set; }
            public List<string> E6AXIS { get; set; }
            public List<string> FDAT { get; set; }
            public List<string> PDAT { get; set; }
            public List<string> LDAT { get; set; }
            //public List<string> Line { get; set; }

            public Dats()
            {
                E6POS = new List<string>();
                E6AXIS = new List<string>();
                FDAT = new List<string>();
                PDAT = new List<string>();
                LDAT = new List<string>();
               // Line = new List<string>();
            }

            public Dats(List<string> e6pos,List<string> e6axis,List<string> fdat, List<string> pdat, List<string> ldat)
            {
                E6POS = e6pos;
                E6AXIS = e6axis;
                FDAT = fdat;
                PDAT = pdat;
                LDAT = ldat;
            }
        } 
        
        public class Roboter
        {
            public int Payload { get; set; }
            public int Range { get; set; }
            public int Generation { get; set; }
            public bool IsWeldingRobot { get; set; }

            public Roboter (int payload, int range, int generation, bool isWeldingRobot)
            {
                Payload = payload;
                Range = range;
                Generation = generation;
                IsWeldingRobot = isWeldingRobot;
            }
        }

        public class Userbits : IGripperConfig
        {
            public SortedDictionary<int,List<string>> Inputs { get; set; }
            public SortedDictionary<int, List<string>> Outputs { get; set; }

            public Userbits()
            {

            }
            public Userbits(SortedDictionary<int,List<string>> inputs, SortedDictionary<int, List<string>> outputs)
            {
                Inputs = inputs;
                Outputs = outputs;
            }
        }
    }
}



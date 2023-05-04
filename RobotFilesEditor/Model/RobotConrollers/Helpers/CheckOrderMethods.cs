using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;

namespace RobotFilesEditor.Model.RobotConrollers.Helpers
{
    public static class CheckOrderMethods
    {
        public static IDictionary<string, string> CheckOrder(IDictionary<string, List<string>> filteredFiles)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            IDictionary<int, string> orderOfOperations = new Dictionary<int, string>();
            List<FilesWithPriorities> resultList = new List<FilesWithPriorities>();
            string[] commandsOrder = ConfigurationManager.AppSettings["OrderOfOperations" + GlobalData.ControllerType.Replace(" ", "_")].Split(',').Select(s => s.Trim()).ToArray();
            for (int i = 0; i < commandsOrder.Length; i++)
                orderOfOperations.Add(i, commandsOrder[i]);

            foreach (var file in filteredFiles)
            {
                int jobNr = FindJob(file);
                List<OperationsPriority> orderOfOperationsFromFile = new List<OperationsPriority>();
                KeyValuePair<string, List<string>> switchGrops = FindSwitchGroups(file);
                bool isFirstPTPpointFound = false;
                bool isHomeAlreadyFound = false;
                foreach (string operation in switchGrops.Value)
                {
                    if (operation.Contains(" PTP ") && !operation.Contains(ConfigurationManager.AppSettings["PTPHome" + GlobalData.ControllerType.Replace(" ", "_")]))
                        isFirstPTPpointFound = true;
                    OperationsPriority operationFromFile = new OperationsPriority();
                    foreach (var op in orderOfOperations)
                    {
                        string tempOp = operation.ToLower().Replace(" ", "");
                        if (tempOp.Contains((ConfigurationManager.AppSettings[op.Value + GlobalData.ControllerType.Replace(" ", "_")]).Replace("USERBITNR", (33 - jobNr).ToString()).ToLower()))
                        {
                            operationFromFile = new OperationsPriority(op.Key, operation);
                            break;
                        }

                        if (operation.ToLower().Contains("endswitch"))
                        {
                            operationFromFile = new OperationsPriority(99, operation);
                            break;
                        }
                        if (operation.Contains(ConfigurationManager.AppSettings[op.Value + GlobalData.ControllerType.Replace(" ", "_")]))
                        {
                            if (operation.Contains(ConfigurationManager.AppSettings["PTPHome" + GlobalData.ControllerType.Replace(" ", "_")]) && isHomeAlreadyFound)
                                operationFromFile = new OperationsPriority(99, operation);
                            else
                                operationFromFile = new OperationsPriority(op.Key, operation);

                            if (operation.Contains(ConfigurationManager.AppSettings["PTPHome" + GlobalData.ControllerType.Replace(" ", "_")]))
                                isHomeAlreadyFound = true;
                        }
                    }
                    if (operationFromFile.Command == null || isFirstPTPpointFound)
                        operationFromFile = new OperationsPriority(99, operation);
                    orderOfOperationsFromFile.Add(operationFromFile);
                }
                resultList.Add(new FilesWithPriorities(file.Key, orderOfOperationsFromFile));
            }

            result = SortSrcFile(resultList);
            IDictionary<string, string> resultDividedToString = new Dictionary<string, string>();
            foreach (var file in result)
            {
                string resultString = "";
                foreach (string item in file.Value)
                {
                    resultString += item + "\r\n";
                }
                resultDividedToString.Add(file.Key, resultString);
            }
            return resultDividedToString;
        }

        private static KeyValuePair<string, List<string>> FindSwitchGroups(KeyValuePair<string, List<string>> file)
        {
            bool addLine = false;
            Regex switchRegex = new Regex(@"switch\s+[a-zA-Z0-9_\$]*", RegexOptions.IgnoreCase);
            List<string> switchString = new List<string>();
            //List<List<string>> list = new List<List<string>>();
            List<string> copyoffile = new List<string>();
            foreach (string command in file.Value)
            {
                if (switchRegex.IsMatch(command.ToLower()) && !command.ToLower().Contains("endswitch"))
                {
                    addLine = true;
                }
                if (addLine)
                    switchString.Add(command);
                else
                    copyoffile.Add(command);
                if (command.ToLower().Contains("endswitch"))
                {
                    addLine = false;
                    string currentString = "";
                    foreach (string item in switchString)
                    {
                        currentString += item;

                    }
                    copyoffile.Add(currentString);
                    //list.Add(switchString);
                    switchString = new List<string>();
                }
            }
            KeyValuePair<string, List<string>> switchStrings = new KeyValuePair<string, List<string>>(file.Key, copyoffile);

            return switchStrings;
        }

        private static IDictionary<string, List<string>> SortSrcFile(List<FilesWithPriorities> inputList)
        {
            IDictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var file in inputList)
            {
                IDictionary<int, OperationsPriority> positionAndCommand = new Dictionary<int, OperationsPriority>();
                IDictionary<int, OperationsPriority> positionAndCommandWithPriority = new Dictionary<int, OperationsPriority>();
                IDictionary<int, OperationsPriority> positionAndCommandWithoutPriority = new Dictionary<int, OperationsPriority>();
                int counter = 0;
                foreach (var item in file.Commands)
                {
                    positionAndCommand.Add(counter, item);
                    counter++;
                }
                foreach (var item in positionAndCommand.Where(x => x.Value.Priority == 99))
                {
                    positionAndCommandWithoutPriority.Add(item);
                }
                foreach (var item in positionAndCommand.Where(x => x.Value.Priority != 99))
                {
                    positionAndCommandWithPriority.Add(item);
                }
                IDictionary<int, OperationsPriority> commandsWithCorrectPosition = new Dictionary<int, OperationsPriority>();
                for (int priority = 0; priority < 20; priority++)
                {
                    foreach (var item in positionAndCommandWithPriority)
                    {
                        if (item.Value.Priority == priority)
                            commandsWithCorrectPosition.Add(item);
                    }
                }
                List<int> positions = new List<int>();
                foreach (var item in commandsWithCorrectPosition)
                    positions.Add(item.Key);
                List<int> positionsNotSorted = new List<int>(positions);
                positions.Sort();

                IDictionary<int, string> sortedCommands = new Dictionary<int, string>();
                int j = 0;
                foreach (int item in positions)
                {
                    sortedCommands.Add(item, commandsWithCorrectPosition[positionsNotSorted[j]].Command);
                    j++;
                }
                foreach (var item in positionAndCommandWithoutPriority)
                {
                    sortedCommands.Add(item.Key, item.Value.Command);
                }
                List<string> resultList = new List<string>();
                for (int i = 0; i < sortedCommands.Count; i++)
                {
                    resultList.Add(sortedCommands[i]);
                }
                result.Add(file.Filename, resultList);
            }

            return result;
        }

        private static int FindJob(KeyValuePair<string, List<string>> file)
        {

            foreach (string command in file.Value)
            {
                if (command.ToLower().Replace(" ", "").Contains(ConfigurationManager.AppSettings["JobReq" + GlobalData.ControllerType.Replace(" ", "_")]))
                {
                    Regex jobNrRegex = new Regex(@"(?<=(Job_req[a-zA-Z]*|Plc_Job)\s*\(\s*)\d+", RegexOptions.IgnoreCase);
                    return int.Parse(jobNrRegex.Match(command).ToString());
                }
            }
            return 0;
        }
    }
}

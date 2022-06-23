using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations
{
    public static class KukaAddSpaces
    {
        enum CheckCriterion { CollisionGroup, CollisionGroupFinished, GripperGroup, Comment, MotionGroup, EndLine, HomeOrCentralSection, MovementBeforeGripper, InitGun, Job, Area, SwpPositioning , PLCCom};

        public static IDictionary<string, string> AddSpaces(IDictionary<string, List<string>> filesAndContent)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            IDictionary<string, List<DataClass.RobotCommand>> filesWithSpaces = CreateLinesWithSpaces(filesAndContent);
            foreach (var file in filesWithSpaces)
            {
                string resultString = string.Empty;
                foreach (var command in file.Value)
                {
                    if (!command.AddSpaceBefore)
                        resultString += command.Content;
                    else
                        resultString += "\r\n" + command.Content;
                }
                result.Add(file.Key, resultString);
            }
            return result;
        }

        private static IDictionary<string, List<DataClass.RobotCommand>> CreateLinesWithSpaces(IDictionary<string, List<string>> filesAndContent)
        {
            IDictionary<string, List<DataClass.RobotCommand>> result = new Dictionary<string, List<DataClass.RobotCommand>>();
            foreach (var file in filesAndContent)
            {
                List<DataClass.RobotCommand> commands = new List<DataClass.RobotCommand>();
                file.Value.ForEach(x => commands.Add(new DataClass.RobotCommand(x.TrimEnd() + "\r\n")));
                result.Add(file.Key, commands);
            }
            result = AddSpacesToRobotCommands(result);
            return result;
        }

        private static IDictionary<string, List<DataClass.RobotCommand>> AddSpacesToRobotCommands(IDictionary<string, List<DataClass.RobotCommand>> input)
        {
            IDictionary<string, List<DataClass.RobotCommand>> result = new Dictionary<string, List<DataClass.RobotCommand>>();
            result = input.ToDictionary(entry => entry.Key, entry => entry.Value);
            foreach (var file in input)
            {

                List<DataClass.RobotCommand> commands = file.Value;
                foreach (var criterion in (CheckCriterion[])Enum.GetValues(typeof(CheckCriterion)))
                {
                    int i = 0;
                    foreach (var command in file.Value)
                    {
                        if (!command.AddSpaceBefore && i > 0)
                        {
                            DataClass.RobotCommand previousCommand = result[file.Key][i - 1];
                            DataClass.RobotCommand nextCommand = i < file.Value.Count-2 ? result[file.Key][i + 1] : null; 
                            switch (criterion)
                            {
                                case CheckCriterion.CollisionGroup:
                                    {
                                        if (command.CommandType.IsCollisionReqRel && !previousCommand.CommandType.IsCollisionReqRel)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.CollisionGroupFinished:
                                    {
                                        if (!command.CommandType.IsCollisionReqRel && previousCommand.CommandType.IsCollisionReqRel)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.Comment:
                                    {
                                        if (command.CommandType.IsComment && previousCommand.CommandType.IsMeaningfulFold)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.EndLine:
                                    {
                                        if (command.CommandType.IsEnd)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.GripperGroup:
                                    {
                                        if (command.CommandType.IsGripperGroup && !previousCommand.CommandType.IsGripperGroup && (!previousCommand.CommandType.IsMotionFoldFold || previousCommand.CommandType.IsTriggeredAction))
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.MotionGroup:
                                    {
                                        if ((previousCommand.CommandType.IsSingleInstruction || previousCommand.CommandType.IsMeaningfulFold) && (!previousCommand.CommandType.IsMotionFoldFold || previousCommand.CommandType.IsTriggeredAction) && command.CommandType.IsMotionFoldFold)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.HomeOrCentralSection:
                                    {
                                        if (command.CommandType.IsHomeSection && !previousCommand.CommandType.IsHomeSection || command.CommandType.IsCentralPosSection && !previousCommand.CommandType.IsCentralPosSection)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.MovementBeforeGripper:
                                    {
                                        if (command.CommandType.IsMotionFoldFold && nextCommand !=null && nextCommand.CommandType.IsGripperGroup)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.InitGun:
                                    {
                                        if (command.CommandType.IsGunInit)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.Job:
                                    {
                                        if (command.CommandType.IsJob)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.Area:
                                    {
                                        if (command.CommandType.IsAreaReq && !previousCommand.CommandType.IsAreaReq)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.SwpPositioning:
                                    {
                                        if (command.CommandType.IsSwpPositioning)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                case CheckCriterion.PLCCom:
                                    {
                                        if (command.CommandType.IsPLCCom && !previousCommand.CommandType.IsPLCCom)
                                            result[file.Key][i].AddSpaceBefore = true;
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        i++;

                    }
                }
            }
            return result;
        }

    }
}

using CommonLibrary.DataClasses;
using GalaSoft.MvvmLight.Messaging;
using RobotFilesEditor.Model.DataInformations;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RobotFilesEditor.Model.DataInformations.FileValidationData;

namespace RobotFilesEditor.Model.RobotConrollers.Helpers
{
    public static class UnusedDatsMethods
    {
        public static void FindUnusedDataInDatFiles(IDictionary<string, List<string>> srcFiles, List<string> datFiles)
        {
            string currentline = "", currentfile = "";
            string errorLine = "";
            try
            {
                IDictionary<string, FileValidationData.Dats> usedDats = new Dictionary<string, FileValidationData.Dats>();
                IDictionary<string, FileValidationData.Dats> unusedDats = new Dictionary<string, FileValidationData.Dats>();
                IDictionary<string, FileValidationData.Dats> usedDatsClone = new Dictionary<string, FileValidationData.Dats>();
                IDictionary<string, FileValidationData.Dats> unusedDatsClone = new Dictionary<string, FileValidationData.Dats>();
                SrcValidator.UnusedDats = new Dictionary<string, Dats>();
                SrcValidator.UsedDats = new Dictionary<string, Dats>();
                IDictionary<string, List<Point>> pointsInSrcs = new Dictionary<string, List<Point>>();
                IDictionary<string, Dats> pointsInDats = new Dictionary<string, Dats>();
                foreach (var file in srcFiles)
                {
                    currentfile = file.Key;

                    List<Point> pointsInSrc = new List<Point>();

                    foreach (string command in file.Value)
                    {
                        currentline = command;
                        if (command.ToLower().Contains("startpos"))
                        { }
                        if (command.ToLower().Contains("ptp ") || command.ToLower().Contains("lin ") || command.ToLower().Contains("search_way") || command.ToLower().Contains("swr_reloadpos"))
                        {
                            Point currentPoint = new Point();
                            if (command.ToLower().Contains("home"))
                            {
                                currentPoint.IsHome = true;
                                currentPoint.ToolNr = 999;
                                currentPoint.ToolName = "ToolNotFound";
                                currentPoint.BaseNr = 999;
                                currentPoint.BaseName = "BaseNotFound";
                            }
                            else
                            {
                                currentPoint.IsHome = false;
                                if (command.ToLower().Contains("job.") || command.ToLower().Contains("collision") || command.ToLower().Contains("spot."))
                                {
                                    Regex regexToolNr = new Regex(@"(?<=Tool\s*(\[|=)\s*)\d*", RegexOptions.IgnoreCase);
                                    currentPoint.ToolNr = int.Parse((regexToolNr.Match(command)).ToString());
                                    Regex regexBaseNr = new Regex(@"(?<=Base\s*(\[|=)\s*)\d*", RegexOptions.IgnoreCase);
                                    currentPoint.BaseNr = int.Parse((regexBaseNr.Match(command)).ToString());
                                    currentPoint.BaseName = "";
                                    currentPoint.ToolName = "";
                                }
                                else
                                {
                                    if (command.ToLower().Contains("tool") && !command.Contains(";#"))
                                    {
                                        int toolNr = 999;
                                        errorLine = command;
                                        Regex regexToolNr = new Regex(@"(?<=Tool\s*(\[|\=)\s*)\d*", RegexOptions.IgnoreCase);
                                        if (int.TryParse(regexToolNr.Match(command).ToString(), out toolNr))
                                        {
                                            currentPoint.ToolNr = toolNr;
                                            Regex regexToolName = new Regex(@"(?<=Tool\s*\[\s*[0-9]*\s*\]\s*:\s*)[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                                            currentPoint.ToolName = (regexToolName.Match(command)).ToString();
                                        }
                                    }
                                    else
                                    {
                                        currentPoint.ToolNr = 999;
                                        currentPoint.ToolName = "ToolNotFound";
                                    }
                                    if (command.ToLower().Contains("base") && !command.ToLower().Contains("search_") && !command.Contains(";#"))
                                    {
                                        Regex regexBaseNr = new Regex(@"(?<=Base\s*(\[|\=)\s*)\d*", RegexOptions.IgnoreCase);
                                        if (!command.ToLower().Replace(" ", "").Contains("palposrel"))
                                        {
                                            if (regexBaseNr.IsMatch(command))
                                            {
                                                if (!string.IsNullOrEmpty(regexBaseNr.Match(command).ToString()))
                                                {
                                                    currentPoint.BaseNr = int.Parse((regexBaseNr.Match(command)).ToString());
                                                    Regex regexBaseName = new Regex(@"(?<=Base\s*\[\s*\d*\s*\]\s*:\s*).*(?=\s*;)", RegexOptions.IgnoreCase);
                                                    currentPoint.BaseName = (regexBaseName.Match(command)).ToString();
                                                }
                                            }
                                            else
                                            { }
                                        }
                                    }
                                    else
                                    {
                                        currentPoint.BaseNr = 999;
                                        currentPoint.BaseName = "BaseNotFound";
                                    }
                                }

                            }
                            //Regex regexName = new Regex(@"(?<=(PTP|LIN)\s+)X[a-zA-Z0-9_]*", RegexOptions.IgnoreCase);
                            Regex regexName = new Regex(@"(?<=PTP\s*|LIN\s*|Search\s*\(\s*\d+\s*,\s*|Search\s*\(\s*\d+\s*,\s*[$a-zA-Z0-9_]*\s*,\s*|Reload_(PTP|LIN)\s*,\s*|Pal_PosRel(Ptp|Lin)\s+\(\s*\d+\s*,\s*#\w+\s*,\s*\d+\s*,(|-)\d+\s*,\s*|Pal_StartPos\s+\(\s*\d+\s*,\s*#\w+\s*,\s*)X[a-zA-Z0-9_\-]*", RegexOptions.IgnoreCase);
                            currentPoint.Name = (regexName.Match(command)).ToString();
                            Regex regexFDAT = new Regex(@"(?<=FDAT_ACT\s*\=\s*)[a-zA-Z0-9_]+", RegexOptions.IgnoreCase);
                            currentPoint.FDAT = (regexFDAT.Match(command)).ToString();
                            Regex regexLDAT = new Regex(@"(?<=LDAT_ACT\s*\=\s*)[a-zA-Z0-9_]+", RegexOptions.IgnoreCase);
                            currentPoint.LDAT = (regexLDAT.Match(command)).ToString();
                            Regex regexPDAT = new Regex(@"(?<=PDAT_ACT\s*\=\s*)[a-zA-Z0-9_]+", RegexOptions.IgnoreCase);
                            currentPoint.PDAT = (regexPDAT.Match(command)).ToString();

                            pointsInSrc.Add(currentPoint);
                        }
                    }
                    pointsInSrcs.Add(file.Key, pointsInSrc);
                }
                foreach (var file in datFiles)
                {
                    Dats pointsInDat = new Dats();
                    var reader = new StreamReader(file);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToLower().Contains("decl fdat"))
                        {
                            Regex matchFDAT = new Regex(@"(?<=FDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.FDAT.Add((matchFDAT.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl pdat"))
                        {
                            Regex matchPDAT = new Regex(@"(?<=PDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.PDAT.Add((matchPDAT.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl ldat"))
                        {
                            Regex matchLDAT = new Regex(@"(?<=LDAT\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.LDAT.Add((matchLDAT.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl e6pos"))
                        {
                            Regex matchE6POS = new Regex(@"(?<=E6POS\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.E6POS.Add((matchE6POS.Match(line)).ToString());
                        }
                        if (line.ToLower().Contains("decl e6axis"))
                        {
                            Regex matchE6AXIS = new Regex(@"(?<=E6AXIS\s+).*(?=\s*\=)", RegexOptions.IgnoreCase);
                            pointsInDat.E6AXIS.Add((matchE6AXIS.Match(line)).ToString());
                        }
                    }

                    pointsInDats.Add(file, pointsInDat);
                }
                foreach (var item in pointsInDats)
                    unusedDats.Add(item);
                foreach (var item in pointsInDats)
                    usedDats.Add(item.Key, new Dats());
                //foreach (var item in unusedDats)
                unusedDatsClone = new Dictionary<string, FileValidationData.Dats>(unusedDats);
                //foreach (var item in usedDats)
                usedDatsClone = new Dictionary<string, FileValidationData.Dats>(usedDats);
                foreach (var dat in pointsInDats)
                {
                    foreach (var src in pointsInSrcs.Where(x => x.Key.Replace(".src", "").Contains(dat.Key.Replace(".dat", ""))))
                    {
                        foreach (Point point in src.Value)
                        {
                            if (point.Name.ToLower().Contains("xservice"))
                            { }
                            //foreach(var item in dat.Value.E6POS.Where(x=>x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.E6POS.Contains(point.Name))
                            if (dat.Value.E6POS.Any(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].E6POS.Add(point.Name);
                                var itemToRemove = unusedDats[dat.Key].E6POS.Where(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].E6POS.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.E6AXIS.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.E6AXIS.Contains(point.Name))
                            if (dat.Value.E6AXIS.Any(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].E6AXIS.Add(point.Name);
                                var itemToRemove = unusedDats[dat.Key].E6AXIS.Where(s => s.Equals(point.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].E6AXIS.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.FDAT.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.FDAT.Contains(point.FDAT))
                            if (dat.Value.FDAT.Any(s => s.Equals(point.FDAT, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].FDAT.Add(point.FDAT);
                                var itemToRemove = unusedDats[dat.Key].FDAT.Where(s => s.Equals(point.FDAT, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].FDAT.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.PDAT.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.PDAT.Contains(point.PDAT))
                            if (dat.Value.PDAT.Any(s => s.Equals(point.PDAT, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].PDAT.Add(point.PDAT);
                                var itemToRemove = unusedDats[dat.Key].PDAT.Where(s => s.Equals(point.PDAT, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].PDAT.Remove(itemToRemove[0]);
                            }
                            //foreach (var item in dat.Value.LDAT.Where(x => x.ToLower().Contains(point.Name.ToLower())))
                            //if (dat.Value.LDAT.Contains(point.LDAT))
                            if (dat.Value.LDAT.Any(s => s.Equals(point.LDAT, StringComparison.OrdinalIgnoreCase)))
                            {
                                usedDats[dat.Key].LDAT.Add(point.LDAT);
                                var itemToRemove = unusedDats[dat.Key].LDAT.Where(s => s.Equals(point.LDAT, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (itemToRemove.Count > 0)
                                    unusedDats[dat.Key].LDAT.Remove(itemToRemove[0]);
                            }
                        }
                    }
                }
                bool unusedDataFound = false;

                CreateUnusedDatsLog(unusedDats);

                if (unusedDats != null)
                    SrcValidator.UnusedDats = unusedDats;
                if (usedDats != null)
                    SrcValidator.UsedDats = usedDats;
            }
            catch (Exception ex)
            {
                SrcValidator.GetExceptionLine(ex, errorLine);
            }
        }

        private static void CreateUnusedDatsLog(IDictionary<string, Dats> unusedDats)
        {
            string unusedDatsString = "";

            foreach (var unusedElement in unusedDats.Where(x => ((x.Value.E6AXIS.Count + x.Value.E6POS.Count + x.Value.FDAT.Count + x.Value.LDAT.Count + x.Value.PDAT.Count) > 0)))
            {
                foreach (string element in unusedElement.Value.E6AXIS)
                {
                    unusedDatsString = "File: " + Path.GetFileName(unusedElement.Key) + ": ";
                    unusedDatsString += "Not used E6AXIS: " + element;
                    Messenger.Default.Send<LogResult>(new LogResult(unusedDatsString, LogResultTypes.Information), "AddLog");
                }
                foreach (string element in unusedElement.Value.E6POS)
                {
                    unusedDatsString = "File: " + Path.GetFileName(unusedElement.Key) + ": ";
                    unusedDatsString += "Not used E6POS: " + element;
                    Messenger.Default.Send<LogResult>(new LogResult(unusedDatsString, LogResultTypes.Information), "AddLog");
                }
                foreach (string element in unusedElement.Value.FDAT)
                {
                    unusedDatsString = "File: " + Path.GetFileName(unusedElement.Key) + ": ";
                    unusedDatsString += "Not used FDAT: " + element;
                    Messenger.Default.Send<LogResult>(new LogResult(unusedDatsString, LogResultTypes.Information), "AddLog");
                }
                foreach (string element in unusedElement.Value.PDAT)
                {
                    unusedDatsString = "File: " + Path.GetFileName(unusedElement.Key) + ": ";
                    unusedDatsString += "Not used PDAT: " + element;
                    Messenger.Default.Send<LogResult>(new LogResult(unusedDatsString, LogResultTypes.Information), "AddLog");
                }
                foreach (string element in unusedElement.Value.LDAT)
                {
                    unusedDatsString = "File: " + Path.GetFileName(unusedElement.Key) + ": ";
                    unusedDatsString += "Not used LDAT: " + element;
                    Messenger.Default.Send<LogResult>(new LogResult(unusedDatsString, LogResultTypes.Information), "AddLog");
                }
            }
        }

    }
}

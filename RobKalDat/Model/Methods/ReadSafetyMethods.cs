using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using MessageLib = System.Windows;

namespace RobKalDat.Model.Methods
{
    public static class ReadSafetyMethods
    {
        public static readonly string[] itemsInExcel = { "paths&locations", "x", "y", "z","rx","ry","rz"};

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static Excel.Application safetyXlApp;
        private static Excel.Workbooks safetyXlWorkbooks;
        private static Excel.Workbook safetyXlWorkbook;

        internal static void ReadSafetyFromXML(string file)
        {
            Dictionary<string, Coords> foundElements = ReadExcel(file);
            ObservableCollection<ItemInSafety> itemsInSafety = CreateSafetyData(foundElements);
            if (itemsInSafety == null)
                itemsInSafety = new ObservableCollection<ItemInSafety>();
            Messenger.Default.Send(itemsInSafety, "foundSafety");
        }

        private static Dictionary<string, Coords> ReadExcel(string file)
        { 
            try
            {
                Dictionary<string, Coords> foundElements = new Dictionary<string, Coords>();
                int[] columns = new int[7];
                int maxColumn = 10, itemCounter = 0;
                safetyXlApp = new Excel.Application();
                safetyXlWorkbooks = safetyXlApp.Workbooks;
                Thread.Sleep(200);
                safetyXlWorkbook = safetyXlWorkbooks.Open(file);
                Thread.Sleep(200);
                Excel._Worksheet safetyxlWorksheet = safetyXlWorkbook.Sheets[1];
                Excel.Range safetyxlRange = safetyxlWorksheet.UsedRange;
                foreach (string item in itemsInExcel)
                {
                    for (int i = 1; i <= maxColumn; i++)
                    {
                        if (safetyxlWorksheet.Cells[1, i].FormulaLocal.ToLower().Replace(" ", "") == itemsInExcel[itemCounter])
                        {
                            columns[itemCounter] = i;
                            break;
                        }
                    }
                    itemCounter++;
                }

                for (int i = 1; i <= safetyxlRange.Rows.Count; i++)
                {
                    string currentElement = safetyxlWorksheet.Cells[i, columns[0]].FormulaLocal;
                    if (currentElement.Trim().Length > 2 && (currentElement.Trim().ToLower().Substring(0, 2) == "cs" || currentElement.Trim().ToLower().Substring(0, 2) == "ts" || currentElement.Trim().ToLower().Substring(0, 2) == "sp"))
                    {
                        bool success1 = false, success2 = false, success3 = false, success4 = false, success5 = false, success6 = false;
                        double x, y, z, rx, ry, rz;
                        success1 = double.TryParse(safetyxlWorksheet.Cells[i, columns[1]].FormulaLocal, out x);
                        success2 = double.TryParse(safetyxlWorksheet.Cells[i, columns[2]].FormulaLocal, out y);
                        success3 = double.TryParse(safetyxlWorksheet.Cells[i, columns[3]].FormulaLocal, out z);
                        success4 = double.TryParse(safetyxlWorksheet.Cells[i, columns[4]].FormulaLocal, out rx);
                        success5 = double.TryParse(safetyxlWorksheet.Cells[i, columns[5]].FormulaLocal, out ry);
                        success6 = double.TryParse(safetyxlWorksheet.Cells[i, columns[6]].FormulaLocal, out rz);
                        if (success1 && success2 && success3 && success4 && success5 && success6)
                            foundElements.Add(safetyxlWorksheet.Cells[i, columns[0]].FormulaLocal, new Coords(x, y, z, rx, ry, rz));
                        else
                            foundElements.Add(safetyxlWorksheet.Cells[i, columns[0]].FormulaLocal, null);
                    }
                }
                CleanUpExcel();
                    return foundElements;
                }
                catch (Exception ex)
                {
                    CleanUpExcel();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
        }

        private static ObservableCollection<ItemInSafety> CreateSafetyData(Dictionary<string, Coords> foundElements)
        {
            try
            {
                Regex getToolNr = new Regex(@"(?<=ts)\d+", RegexOptions.IgnoreCase);
                Regex getSPNr = new Regex(@"(?<=sp)\d+", RegexOptions.IgnoreCase);
                Regex sphereNr = new Regex(@"(?<=sphere)\d+", RegexOptions.IgnoreCase);
                Regex radius = new Regex(@"(?<=sphere\d+_)\d+", RegexOptions.IgnoreCase);
                ObservableCollection<ItemInSafety> result = new ObservableCollection<ItemInSafety>();
                foreach (var element in foundElements)
                {
                    if (element.Key.ToLower().Trim().Substring(0, 2) == "ts")
                    {
                        int toolNr = int.Parse(getToolNr.Match(element.Key).ToString());
                        if (!result.Any(x => x.Type == "Tool"))
                            result.Add(new ItemInSafety() { Type = "Tool", Tool = new KukaSafeTool() { Number = toolNr, Spheres = new List<Sphere>() } });
                        if (!result.Where(y=>y.Tool != null).Any(x => x.Tool.Number == toolNr))
                            result.Add(new ItemInSafety() { Type = "Tool", Tool = new KukaSafeTool() { Number = toolNr, Spheres = new List<Sphere>() } });
                        if (element.Key.ToLower().Contains("tcp"))
                            result.Where(x => x.Tool !=null && x.Tool.Number == toolNr).FirstOrDefault().Tool.TCP = new Coords(element.Value.X, element.Value.Y, element.Value.Z, element.Value.RX, element.Value.RY, element.Value.RZ);
                        if (element.Key.ToLower().Contains("sphere"))
                            result.Where(x => x.Tool!=null && x.Tool.Number==toolNr).FirstOrDefault().Tool.Spheres.Add(new Sphere() { Radius = int.Parse(radius.Match(element.Key).ToString()), Coordinates = new Coords(element.Value.X, element.Value.Y, element.Value.Z, element.Value.RX, element.Value.RY, element.Value.RZ) });
                    }
                    if (element.Key.ToLower().Trim().Substring(0, 2) == "cs")
                    {
                        if (!result.Any(x => x.Type == "Cellspace"))
                            result.Add(new ItemInSafety() { Type = "Cellspace", CellSpaceSoll = new List<Coords>(), CellSpaceIst = new List<Coords>() });
                        if (!element.Key.ToLower().Contains("_max"))
                            result.Where(x => x.Type == "Cellspace").FirstOrDefault().CellSpaceSoll.Add(new Coords(element.Value.X, element.Value.Y, element.Value.Z, element.Value.RX, element.Value.RY, element.Value.RZ));
                        else
                            result.Where(x => x.Type == "Cellspace").FirstOrDefault().CellSpaceHeight = element.Value.Z - result.Where(x => x.Type == "Cellspace").FirstOrDefault().CellSpaceSoll[0].Z;
                    }
                    if (element.Key.ToLower().Trim().Substring(0, 2) == "sp")
                    {
                        int spaceNr = int.Parse(getSPNr.Match(element.Key).ToString());
                        if (!result.Any(x=>x.Type == "Safespaces"))
                            result.Add(new ItemInSafety() { Type = "Safespaces", SafeSpaces = new SafeSpace() { Number = spaceNr } });
                        //if (result.Where(x=>x.SafeSpaces != null).Any(y=>y.SafeSpaces.Number==spaceNr))
                        if (!(result.Where(x => x.SafeSpaces != null).Any(y => y.SafeSpaces.Number == spaceNr)))
                            result.Add(new ItemInSafety() { Type = "Safespaces", SafeSpaces = new SafeSpace() { Number = spaceNr } });
                        if (element.Value == null)
                            result.Where(x => x.SafeSpaces != null && x.SafeSpaces.Number == spaceNr).FirstOrDefault().SafeSpaces.Name = element.Key;
                        if (element.Key.ToLower().Contains("_origin"))
                        {
                            var currentItem = result.Where(x => x.SafeSpaces != null && x.SafeSpaces.Number == spaceNr).FirstOrDefault();
                            currentItem.SafeSpaces.OriginSoll = new Coords(element.Value.X, element.Value.Y, element.Value.Z, element.Value.RX, element.Value.RY, element.Value.RZ);
                            if (currentItem.SafeSpaces.Max != null)
                            {
                                currentItem.SafeSpaces.Dimensions = Methods.CalculateDimensions(currentItem.SafeSpaces.OriginSoll, currentItem.SafeSpaces.Max);
                                if (currentItem.SafeSpaces.Dimensions.X < 0 || currentItem.SafeSpaces.Dimensions.Y < 0 || currentItem.SafeSpaces.Dimensions.Z < 0)
                                {
                                    MessageBox.Show("Dimensions on axes XYZ for safe space " + currentItem.SafeSpaces.Name + " are not increasing from origin to max. Correct safety data!","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                                    return null;
                                }
                            }
                        }
                        if (element.Key.ToLower().Contains("_max"))
                        {
                            var currentItem = result.Where(x => x.SafeSpaces != null && x.SafeSpaces.Number == spaceNr).FirstOrDefault();
                            currentItem.SafeSpaces.Max = new Coords(element.Value.X, element.Value.Y, element.Value.Z, element.Value.RX, element.Value.RY, element.Value.RZ);
                            if (currentItem.SafeSpaces.OriginSoll != null)
                            {
                                currentItem.SafeSpaces.Dimensions = Methods.CalculateDimensions(currentItem.SafeSpaces.OriginSoll, currentItem.SafeSpaces.Max);
                                if (currentItem.SafeSpaces.Dimensions.X < 0 || currentItem.SafeSpaces.Dimensions.Y < 0 || currentItem.SafeSpaces.Dimensions.Z < 0)
                                {
                                    MessageBox.Show("Dimensions on axes XYZ for safe space " + currentItem.SafeSpaces.Name + " are not increasing from origin to max. Correct safety data!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return null;
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch  (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        internal static void SaveSafety(KeyValuePair<Measurement, ObservableCollection<ItemInSafety>> robotWithSafety)
        {
            try
            {
                string savePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "XML file", filter1: "*.xml");
                if (string.IsNullOrEmpty(savePath))
                    return;
                XDocument doc = XDocument.Parse(RobKalResources.templateSafetyKuka);
                //Cellspace
                var cellspace = robotWithSafety.Value.Where(x => x.CellSpaceIst != null && x.Type == "Cellspace").FirstOrDefault();
                var cellspaceElement = new XElement("CellSpace",
                    new XElement("ZMin", cellspace.GetMinCellspace()),
                    new XElement("Zmax", cellspace.GetMaxCellspace()));
                doc.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("RangeMonitoring").Add(cellspaceElement);
                List<Coords> currentPointList = new List<Coords>();
                if (cellspace.CellSpaceIst.Count == cellspace.CellSpaceSoll.Count)
                    currentPointList = cellspace.CellSpaceIst;
                else
                    currentPointList = cellspace.CellSpaceSoll;
                for (int i = 1; i <= 10; i++)
                {
                    XElement currentElement = new XElement("Polygon", new XAttribute("Number", i),
                        new XElement("X", currentPointList.Count > (i - 1) ? currentPointList[i - 1].X : 0),
                        new XElement("Y", currentPointList.Count > (i - 1) ? currentPointList[i - 1].Y : 0),
                        new XElement("IsPolygonNodeActive", currentPointList.Count > (i - 1) ? 1 : 0));
                    doc.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("RangeMonitoring").Element("CellSpace").Add(currentElement);
                }

                //Safesapces
                List<ItemInSafety> safeSpaces = new List<ItemInSafety>();
                robotWithSafety.Value.Where(y => y.SafeSpaces != null && y.Type == "Safespaces").ToList().ForEach(x => safeSpaces.Add(x));
                foreach (var safeSpace in safeSpaces)
                {
                    var currentOrigin = safeSpace.SafeSpaces.OriginIst != null ? safeSpace.SafeSpaces.OriginIst : safeSpace.SafeSpaces.OriginSoll;
                    XElement currentElement = new XElement("WorkspaceMonitoring", new XAttribute("Number", safeSpace.SafeSpaces.Number), new XAttribute("Name", safeSpace.SafeSpaces.Name),
                        new XElement("Activation", 50),
                        new XElement("RobotStops", 1),
                        new XElement("RangeType", 1),
                        new XElement("IsProtectedSpace", 1),
                        new XElement("MonitoringStop", 0),
                        new XElement("CheckCartesianSpeedInProtectedArea", 0),
                        new XElement("CartVel", 30000),
                        new XElement("ReferenceRobRoot", 0),
                        new XElement("CartesianRange", new XElement("X", currentOrigin.X), new XElement("Y", currentOrigin.Y), new XElement("Z", currentOrigin.Z), new XElement("A", currentOrigin.RZ), new XElement("B", currentOrigin.RY), new XElement("C", currentOrigin.RX),
                        new XElement("X1", 0.000000), new XElement("X2", safeSpace.SafeSpaces.Dimensions.X), new XElement("Y1", 0.000000), new XElement("Y2", safeSpace.SafeSpaces.Dimensions.Y), new XElement("Z1", 0.000000), new XElement("Z2", safeSpace.SafeSpaces.Dimensions.Z))
                        );
                    doc.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("RangeMonitoring").Element("CellSpace").Add(currentElement);
                }

                //Tools
                List<ItemInSafety> tools = new List<ItemInSafety>();
                robotWithSafety.Value.Where(y => y.Tool != null && y.Type == "Tool").ToList().ForEach(x => tools.Add(x));
                foreach (var tool in tools)
                {
                    XElement toolElement = new XElement("Tool", new XAttribute("Number", tool.Tool.Number), new XAttribute("Name", "Tool" + tool.Tool.Number),
                    new XElement("ToolEnabled", 1),
                    new XElement("TCPVector_X", tool.Tool.TCP.X),
                    new XElement("TCPVector_Y", tool.Tool.TCP.Y),
                    new XElement("TCPVector_Z", tool.Tool.TCP.Z));
                    for (int i = 1; i <= 12; i++)
                    {
                        var currentSphere = new XElement("Sphere", new XAttribute("Number", i),
                           new XElement("SphereEnabled", tool.Tool.Spheres.Count > (i - 1) ? 1 : 0),
                           new XElement("X", tool.Tool.Spheres.Count > (i - 1) ? tool.Tool.Spheres[i - 1].Coordinates.X : 0.000000),
                           new XElement("Y", tool.Tool.Spheres.Count > (i - 1) ? tool.Tool.Spheres[i - 1].Coordinates.Y : 0.000000),
                           new XElement("Z", tool.Tool.Spheres.Count > (i - 1) ? tool.Tool.Spheres[i - 1].Coordinates.Z : 0.000000),
                           new XElement("Radius", tool.Tool.Spheres.Count > (i - 1) ? tool.Tool.Spheres[i - 1].Radius : 0.000000));
                        toolElement.Add(currentSphere);
                    }
                    doc.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("Tools").Add(toolElement);
                }
                string fullSavePath = Path.GetExtension(savePath) == ".xml" ? savePath : savePath + ".xml";
                if (File.Exists(fullSavePath))
                {
                    MessageLib.Forms.DialogResult dialogResult = MessageLib.Forms.MessageBox.Show("File " + fullSavePath + " already exists. Overwrite?", "Overwrite files?", MessageLib.Forms.MessageBoxButtons.YesNo, MessageLib.Forms.MessageBoxIcon.Question);
                    if (dialogResult == MessageLib.Forms.DialogResult.Yes)
                    {
                        doc.Save(fullSavePath);
                        MessageLib.MessageBox.Show("Successfully saved at " + fullSavePath, "Success", MessageLib.MessageBoxButton.OK, MessageLib.MessageBoxImage.Information);
                    }
                }
                else
                {
                    doc.Save(fullSavePath);
                    MessageLib.MessageBox.Show("Successfully saved at " + fullSavePath, "Success", MessageLib.MessageBoxButton.OK, MessageLib.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageLib.MessageBox.Show(ex.Message, "Error", MessageLib.MessageBoxButton.OK, MessageLib.MessageBoxImage.Error);
            }
        }

        private static void CleanUpExcel()
        {
            if (safetyXlApp != null)
            {
                int hWndDest = safetyXlApp.Application.Hwnd;

                uint processID;

                GetWindowThreadProcessId((IntPtr)hWndDest, out processID);
                Process.GetProcessById((int)processID).Kill();
            }
            safetyXlWorkbook = null;
            safetyXlWorkbooks = null;
            safetyXlApp = null;
          
        }
    }
}

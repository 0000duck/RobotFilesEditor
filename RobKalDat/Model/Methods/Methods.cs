using GalaSoft.MvvmLight.Messaging;
using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommonLibrary.RobKalDatCommon;
using MessageLib = System.Windows;
using Common = CommonLibrary.CommonMethods;

namespace RobKalDat.Model.Methods
{
    public static class Methods
    {
        internal static ObservableCollection<Measurement> LoadProject()
        {
            string selectedProject = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "XML File", filter1: "*.xml");
            if (string.IsNullOrEmpty(selectedProject))
                return null;
            ObservableCollection<Measurement> foundMeasurements = FindMeasurememntsInProject(selectedProject);
            return foundMeasurements;
        }

        private static ObservableCollection<Measurement> FindMeasurememntsInProject(string selectedProject)
        {
            ObservableCollection<Measurement> result = new ObservableCollection<Measurement>();
            XElement doc = XElement.Load(selectedProject);
            var bases = doc.Elements("Bases").Elements("Base");
            var objects = doc.Elements("Objects").Elements("Object");
            var safetys = doc.Elements("SafetySets").Elements("SafeRob");
            foreach (var foundobject in objects)
            {
                Measurement currentObject = new Measurement();
                double currentNumber = 0.0;
                currentObject.Name = foundobject.Attribute("Name").Value;
                currentObject.HasRealValues = foundobject.Element("HasRealValues").Value;
                if (foundobject.Element("RobotType") != null)
                {
                    currentObject.RobotType = foundobject.Element("RobotType").Value;
                    currentObject.BaseIDs = new List<string>();
                    foreach (var item in foundobject.Element("ObjectBases").Elements("item"))
                    {
                        currentObject.BaseIDs.Add(item.Value);
                        foreach (var baseId in bases.Where(x=>x.Attribute("ID").Value.ToLower() == item.Value.ToLower()))
                        {
                            //string baseType = baseId.Element("Ext_Base").Value.ToLower() == "false" ? "Obj2" : "Adjusted_TCP";
                            //string baseType = "Obj2";
                            Coords tcp = new Coords(), measSoll = new Coords();
                            Coords robot = GetCoorinatesFromXML(baseId.Element("Obj1"));
                            Coords meas = GetCoorinatesFromXML(baseId.Element("Obj2"));
                            if (baseId.Element("Ext_Base").Value.ToLower() == "true")
                            {
                                var currentTCP = doc.Elements("Objects").Elements("Object").Where(x => x.Attribute("Name").Value.ToLower() == baseId.Element("Obj3").Attribute("Name").Value.ToLower()).FirstOrDefault();
                                tcp = GetCoorinatesFromXML(currentTCP.Element("nominal"));
                                var currentMeas = doc.Elements("Objects").Elements("Object").Where(x => x.Attribute("Name").Value.ToLower() == meas.Name.ToLower()).FirstOrDefault();
                                measSoll = GetCoorinatesFromXML(currentMeas.Element("nominal"));
                            }
                            Point calcBase = baseId.Element("Ext_Base").Value.ToLower() == "false" ? CommonLibrary.CommonMethods.CalculateBases(CoordsToPoint(robot), CoordsToPoint(meas)) : AddBaseExecute(1,new Measurement() { XIst = robot.X, YIst = robot.Y, ZIst = robot.Z, RXIst = robot.RX, RYIst = robot.RY, RZIst = robot.RZ }, new Measurement() { XIst = meas.X, YIst = meas.Y, ZIst = meas.Z, RXIst = meas.RX, RYIst = meas.RY, RZIst = meas.RZ, XSoll = measSoll.X, YSoll = measSoll.Y, ZSoll = measSoll.Z, RXSoll = measSoll.RX, RYSoll = measSoll.RY, RZSoll = measSoll.RZ}, new Measurement() { XSoll = tcp.X, YSoll = tcp.Y, ZSoll = tcp.Z, RXSoll = tcp.RX, RYSoll = tcp.RY, RZSoll = tcp.RZ});
                            if (currentObject.Bases == null)
                                currentObject.Bases = new ObservableCollection<CalculatedBase>();
                            currentObject.Bases.Add(new CalculatedBase(baseId.Element("Base_Nr").Value, baseId.Element("Base_name").Value, Math.Round(calcBase.XPos,6), Math.Round(calcBase.YPos,6), Math.Round(calcBase.ZPos,6), Math.Round(calcBase.RX,6), Math.Round(calcBase.RY,6), Math.Round(calcBase.RZ,6), item.Value, baseId.Element("Ext_Base").Value.ToLower() == "false" ? false : true,robot,meas, baseId.Element("Ext_Base").Value.ToLower() == "false" ? null : GetCoorinatesFromXML(baseId.Element("Obj3")), baseId.Element("Ext_Base").Value.ToLower() == "false" ? null : GetCoorinatesFromXML(baseId.Element("Tool")), baseId.Element("Ext_Base").Value.ToLower() == "false" ? null : GetCoorinatesFromXML(baseId.Element("Adjusted_TCP"))));
                        }
                    }
                    if (currentObject.BaseIDs.Count == 0)
                        currentObject.BaseIDs = null;
                }
                if (double.TryParse(foundobject.Element("nominal").Attribute("X").Value,NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                    currentObject.XSoll = currentNumber;
                if (double.TryParse(foundobject.Element("nominal").Attribute("Y").Value,NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                    currentObject.YSoll = currentNumber;
                if (double.TryParse(foundobject.Element("nominal").Attribute("Z").Value,NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                    currentObject.ZSoll = currentNumber;
                if (double.TryParse(foundobject.Element("nominal").Attribute("RX").Value,NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                    currentObject.RXSoll = currentNumber;
                if (double.TryParse(foundobject.Element("nominal").Attribute("RY").Value,NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                    currentObject.RYSoll = currentNumber;
                if (double.TryParse(foundobject.Element("nominal").Attribute("RZ").Value,NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                    currentObject.RZSoll = currentNumber;

                if (foundobject.Element("HasRealValues").Value.ToLower().Contains("true"))
                {
                    if (double.TryParse(foundobject.Element("real").Attribute("X").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                        currentObject.XIst = currentNumber;
                    if (double.TryParse(foundobject.Element("real").Attribute("Y").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                        currentObject.YIst = currentNumber;
                    if (double.TryParse(foundobject.Element("real").Attribute("Z").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                        currentObject.ZIst = currentNumber;
                    if (double.TryParse(foundobject.Element("real").Attribute("RX").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                        currentObject.RXIst = currentNumber;
                    if (double.TryParse(foundobject.Element("real").Attribute("RY").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                        currentObject.RYIst = currentNumber;
                    if (double.TryParse(foundobject.Element("real").Attribute("RZ").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out currentNumber))
                        currentObject.RZIst = currentNumber;
                }
                result.Add(currentObject);            
            }
            foreach (var safety in safetys)
            {
                ObservableCollection<ItemInSafety> currentSafety = new ObservableCollection<ItemInSafety>();
                //TOOL
                var toolsInXML = safety.Elements("Tool");
                foreach (var tool in toolsInXML)
                {
                    KukaSafeTool currentTool = new KukaSafeTool() { Name = tool.Attribute("Name").Value, Number = int.Parse(tool.Attribute("Nr").Value) + 1, TCP = new Coords(double.Parse(tool.Element("Matrix").Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(tool.Element("Matrix").Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(tool.Element("Matrix").Attribute("Z").Value, CultureInfo.InvariantCulture), Common.ConvertToDegrees(double.Parse(tool.Element("Matrix").Attribute("RX").Value, CultureInfo.InvariantCulture)), Common.ConvertToDegrees(double.Parse(tool.Element("Matrix").Attribute("RY").Value, CultureInfo.InvariantCulture)), Common.ConvertToDegrees(double.Parse(tool.Element("Matrix").Attribute("RZ").Value, CultureInfo.InvariantCulture))) };
                    List<Sphere> spheres = new List<Sphere>();
                    foreach (var sphere in tool.Elements("Point"))
                    {
                        spheres.Add(new Sphere() { Radius = double.Parse(sphere.Attribute("DIAM").Value, CultureInfo.InvariantCulture), Coordinates = new Coords(double.Parse(sphere.Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Attribute("Z").Value, CultureInfo.InvariantCulture), 0, 0, 0) });
                    }
                    currentTool.Spheres = spheres;
                    currentSafety.Add(new ItemInSafety() { Type = "Tool", Tool = currentTool });
                }
                // CELLSPACE
                var cellspaceElement = safety.Element("CSP");
                double height = double.Parse(cellspaceElement.Attribute("Z_MAX").Value, CultureInfo.InvariantCulture) - double.Parse(cellspaceElement.Attribute("Z_MIN").Value, CultureInfo.InvariantCulture);
                var cellspaceSoll = new List<Coords>();
                var cellspaceIst = new List<Coords>();
                foreach (var csSoll in cellspaceElement.Elements("Point").Where(x => x.Attribute("Type").Value == "nominal"))
                {
                    cellspaceSoll.Add(new Coords(double.Parse(csSoll.Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(csSoll.Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(csSoll.Attribute("Z").Value, CultureInfo.InvariantCulture), 0, 0, 0));
                }
                if (cellspaceElement.Attribute("Calculated").Value.ToLower() == "true")
                {
                    foreach (var csIst in cellspaceElement.Elements("Point").Where(x => x.Attribute("Type").Value == "real"))
                    {
                        cellspaceSoll.Add(new Coords(double.Parse(csIst.Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(csIst.Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(csIst.Attribute("Z").Value, CultureInfo.InvariantCulture), 0, 0, 0));
                    }
                }
                currentSafety.Add(new ItemInSafety() { Type = "Cellspace", CellSpaceHeight = height, CellSpaceIst = cellspaceIst, CellSpaceSoll = cellspaceSoll });

                // SAFESPACES
                foreach (var safeSpaceElement in safety.Elements("SP"))
                {
                    SafeSpace currentSafeSpace = new SafeSpace() { Number = int.Parse(safeSpaceElement.Attribute("Nr").Value) + 1, Name = safeSpaceElement.Attribute("Name").Value, RefObj =  safeSpaceElement.Element("ReferenceObj") == null ? string.Empty : safeSpaceElement.Element("ReferenceObj").Value };
                    var originSoll = safeSpaceElement.Elements("Matrix").Where(x => x.Attribute("Type").Value == "nominal").FirstOrDefault();
                    currentSafeSpace.OriginSoll = new Coords(double.Parse(originSoll.Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(originSoll.Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(originSoll.Attribute("Z").Value, CultureInfo.InvariantCulture), Common.ConvertToDegrees(double.Parse(originSoll.Attribute("RX").Value, CultureInfo.InvariantCulture)), Common.ConvertToDegrees(double.Parse(originSoll.Attribute("RY").Value, CultureInfo.InvariantCulture)), Common.ConvertToDegrees(double.Parse(originSoll.Attribute("RZ").Value, CultureInfo.InvariantCulture)));
                    if (safeSpaceElement.Attribute("Calculated").Value.ToLower() == "true")
                    {
                        var originIst = safeSpaceElement.Elements("Matrix").Where(x => x.Attribute("Type").Value == "real").FirstOrDefault();
                        currentSafeSpace.OriginIst = new Coords(double.Parse(originIst.Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(originIst.Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(originIst.Attribute("Z").Value, CultureInfo.InvariantCulture), Common.ConvertToDegrees(double.Parse(originIst.Attribute("RX").Value, CultureInfo.InvariantCulture)), Common.ConvertToDegrees(double.Parse(originIst.Attribute("RY").Value, CultureInfo.InvariantCulture)), Common.ConvertToDegrees(double.Parse(originIst.Attribute("RZ").Value, CultureInfo.InvariantCulture)));
                    }
                    var dimensions = safeSpaceElement.Elements("Point").Where(x => x.Attribute("Type").Value == "maximum").FirstOrDefault();
                    currentSafeSpace.Dimensions = new Coords(double.Parse(dimensions.Attribute("X").Value, CultureInfo.InvariantCulture), double.Parse(dimensions.Attribute("Y").Value, CultureInfo.InvariantCulture), double.Parse(dimensions.Attribute("Z").Value, CultureInfo.InvariantCulture), 0, 0, 0);
                    currentSafety.Add(new ItemInSafety() { Type = "Safespaces", SafeSpaces = currentSafeSpace });
                }
                result.Where(x => x.Name == safety.Attribute("ID").Value).FirstOrDefault().Safety = currentSafety;
            }

            return result;
        }

        internal static void WriteInputData(ObservableCollection<CalculatedBase> bases)
        {
            string filename = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1: "*.src", filter1Descr: "src file");
            if (string.IsNullOrEmpty(filename))
                return;
            string filecontent = "DEF " + Path.GetFileNameWithoutExtension(filename) + " ()\r\n\r\n";
            foreach (var _base in bases)
            {
                filecontent += String.Format(";**** BASE-Bezeichnung: {0}_{1}",_base.Robot.Name,_base.Obj2.Name) + (!_base.ExtTCP ? "" : "_"+_base.Obj3.Name) + "\r\n";
                filecontent += String.Format("BASE_DATA[{0}]={{X {1},Y {2},Z {3},A {4},B {5},C {6}}}",_base.Number,_base.X,_base.Y,_base.Z,_base.RX,_base.RY,_base.RZ) + "\r\n";
                filecontent += String.Format("BASE_TYPE[{0}]=#{1}\r\n", _base.Number, _base.ExtTCP ? "TCP" : "BASE");
                filecontent += String.Format("BASE_NAME[{0},]=\"{1}\"\r\n\r\n",_base.Number,_base.Name);
            }
            filecontent += "END";
            if (string.IsNullOrEmpty(Path.GetExtension(filename)))
                filename += ".src";
            try
            {
                File.WriteAllText(filename, filecontent);
                MessageLib.MessageBox.Show("Succesfully saved at " + filename, "Succes", MessageLib.MessageBoxButton.OK, MessageLib.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageLib.MessageBox.Show("Save failed" + filename, "Error", MessageLib.MessageBoxButton.OK, MessageLib.MessageBoxImage.Error);
            }
        }

        internal static ObservableCollection<Coords> CalculateSafety(ItemInSafety itemInSafety, Measurement robot, Measurement selectedMeas)
        {
            ObservableCollection<Coords> result = new ObservableCollection<Coords>();
            Point calculatedPoint = new Point();
            Point robotOffset = Common.CalculateBases(new Point(selectedMeas.XSoll, selectedMeas.YSoll, selectedMeas.ZSoll, selectedMeas.RXSoll, selectedMeas.RYSoll, selectedMeas.RZSoll), new Point(robot.XIst, robot.YIst, robot.ZIst, robot.RXIst, robot.RYIst, robot.RZIst), isSafety: true);
            if (itemInSafety.Type == "Safespaces")
            {
                calculatedPoint = Common.CalculateBases(robotOffset, CoordsToPoint(itemInSafety.SafeSpaces.OriginSoll), isSafety: true);
                result.Add(PointtoCoords(calculatedPoint));
            }
            if (itemInSafety.Type == "Cellspace")
            {
                foreach (var point in itemInSafety.CellSpaceSoll)
                {
                    calculatedPoint = Common.CalculateBases(robotOffset, new Point(point.X, point.Y, point.Z, point.RX, point.RY, point.RZ), isSafety: true);
                    result.Add(PointtoCoords(calculatedPoint));
                }
            }
            return result;
        }

        private static Coords GetCoorinatesFromXML(XElement xElement)
        {
            int success = 0;
            double x, y, z, rx, ry, rz;
            string name = xElement.Attribute("Name") == null ? string.Empty : xElement.Attribute("Name").Value;
            if (double.TryParse(xElement.Attribute("X").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                success++;
            if (double.TryParse(xElement.Attribute("Y").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                success++;
            if (double.TryParse(xElement.Attribute("Z").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out z))
                success++;
            if (double.TryParse(xElement.Attribute("RX").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out rx))
                success++;
            if (double.TryParse(xElement.Attribute("RY").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out ry))
                success++;
            if (double.TryParse(xElement.Attribute("RZ").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out rz))
                success++;
            if (success!=6)
            {
                MessageLib.MessageBox.Show("Error while reading file", "Error", MessageLib.MessageBoxButton.OK, MessageLib.MessageBoxImage.Error);
                return null;
            }
            return new Coords(x,y,z,rx,ry,rz,name);
        }

        internal static Coords CalculateDimensions(Coords origin, Coords max)
        {
            Point dimensions = Common.CalculateBases( CoordsToPoint(origin), CoordsToPoint(max));
            return PointtoCoords(dimensions);
        }

        private static Point CoordsToPoint(Coords input)
        { return new Point(input.X, input.Y, input.Z, input.RX, input.RY, input.RZ); }

        private static Coords PointtoCoords(Point input)
        { return new Coords(input.XPos, input.YPos, input.ZPos, input.RX, input.RY, input.RZ); }

        internal static Point AddBaseExecute(int selectedBaseType, Measurement robot, Measurement meas, Measurement tcp)
        {
            Point calcBase = new Point();
            if (selectedBaseType == 0)
            {
                calcBase = Common.CalculateBases(new Point(robot.XIst, robot.YIst, robot.ZIst, robot.RXIst, robot.RYIst, robot.RZIst), new Point(meas.XIst, meas.YIst, meas.ZIst, meas.RXIst, meas.RYIst, meas.RZIst));
            }
            else
            {
                // wyliczenie offsetu na podstawie wartości Soll
                Point offeset = CommonLibrary.CommonMethods.CalculateBases(new Point(meas.XSoll, meas.YSoll, meas.ZSoll, meas.RXSoll, meas.RYSoll, meas.RZSoll), new Point(tcp.XSoll, tcp.YSoll, tcp.ZSoll, tcp.RXSoll, tcp.RYSoll, tcp.RZSoll));
                Messenger.Default.Send(new Coords(offeset.XPos, offeset.YPos, offeset.ZPos, offeset.RX, offeset.RY, offeset.RZ), "extOffsetCalculated");
                // wyznaczenie pozycji XYZ TCP
                Point translationAnRotation = GetTCPXYZ(meas,offeset);
                //wyliczenie bazy
                calcBase = Common.CalculateBases(new Point(robot.XIst, robot.YIst, robot.ZIst, robot.RXIst, robot.RYIst, robot.RZIst), translationAnRotation);

            }
            return calcBase;
        }

        private static Point GetTCPXYZ(Measurement meas, Point offeset)
        {
            //wyznaczenie macierzy obrotu
            double[,] rotationByZMeas = new double[,] { { Math.Cos(Common.ConvertToRadians(meas.RZIst)), -Math.Sin(Common.ConvertToRadians(meas.RZIst)), 0 }, { Math.Sin(Common.ConvertToRadians(meas.RZIst)), Math.Cos(Common.ConvertToRadians(meas.RZIst)), 0 }, { 0, 0, 1 } };
            double[,] rotationByYMeas = MatrixOperations.MultiplyMatrix3x3(rotationByZMeas, new double[,] { { Math.Cos(Common.ConvertToRadians(meas.RYIst)), 0, Math.Sin(Common.ConvertToRadians(meas.RYIst)) }, { 0, 1, 0 }, { -Math.Sin(Common.ConvertToRadians(meas.RYIst)), 0, Math.Cos(Common.ConvertToRadians(meas.RYIst)) } });
            double [,] rotationByXMeas = MatrixOperations.MultiplyMatrix3x3(rotationByYMeas, new double[,] { { 1, 0, 0 }, { 0, Math.Cos(Common.ConvertToRadians(meas.RXIst)), -Math.Sin(Common.ConvertToRadians(meas.RXIst)) }, { 0, Math.Sin(Common.ConvertToRadians(meas.RXIst)), Math.Cos(Common.ConvertToRadians(meas.RXIst)) } });
            //macierz obrotu razy translacja
            double[,] xyzTemp = MatrixOperations.MultiplyMatrix(rotationByXMeas, new double[,] { { offeset.XPos },{ offeset.YPos },{ offeset.ZPos } });
            Point translation = new Point(meas.XIst + xyzTemp[0, 0], meas.YIst + xyzTemp[1, 0], meas.ZIst + xyzTemp[2, 0], 0, 0, 0);

            double[,] rotationByZOffset = MatrixOperations.MultiplyMatrix3x3(rotationByXMeas, new double[,] { { Math.Cos(Common.ConvertToRadians(offeset.RZ)), -Math.Sin(Common.ConvertToRadians(offeset.RZ)), 0 }, { Math.Sin(Common.ConvertToRadians(offeset.RZ)), Math.Cos(Common.ConvertToRadians(offeset.RZ)), 0 }, { 0, 0, 1 } });
            double[,] rotationByYOffset = MatrixOperations.MultiplyMatrix3x3(rotationByZOffset, new double[,] { { Math.Cos(Common.ConvertToRadians(offeset.RY)), 0, Math.Sin(Common.ConvertToRadians(offeset.RY)) }, { 0, 1, 0 }, { -Math.Sin(Common.ConvertToRadians(offeset.RY)), 0, Math.Cos(Common.ConvertToRadians(offeset.RY)) } });
            double[,] rotationByXOffset = MatrixOperations.MultiplyMatrix3x3(rotationByYOffset, new double[,] { { 1, 0, 0 }, { 0, Math.Cos(Common.ConvertToRadians(offeset.RX)), -Math.Sin(Common.ConvertToRadians(offeset.RX)) }, { 0, Math.Sin(Common.ConvertToRadians(offeset.RX)), Math.Cos(Common.ConvertToRadians(offeset.RX)) } });
            double rx = Common.ConvertToDegrees(Math.Atan2(rotationByXOffset[2, 1], rotationByXOffset[2, 2]));
            double ry = Common.ConvertToDegrees(Math.Atan2((rotationByXOffset[2, 0]*-1), Math.Sqrt((Math.Pow(rotationByXOffset[2, 1], 2) + Math.Pow(rotationByXOffset[2, 2], 2)))));
            double rz = Common.ConvertToDegrees(Math.Atan2(rotationByXOffset[1, 0], rotationByXOffset[0, 0]));
            Messenger.Default.Send(new Coords(translation.XPos, translation.YPos, translation.ZPos, rx, ry, rz), "adjTCPCalculated");

            return new Point(translation.XPos,translation.YPos,translation.ZPos,rx,ry,rz);
        }

        internal static void SaveProject(ObservableCollection<Measurement> foundMeasurements)
        {
            try
            {
                string savePath = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "XML file", filter1: "*.xml");
                if (string.IsNullOrEmpty(savePath))
                    return;
                XDocument doc = XDocument.Parse(RobKalResources.templateBase);
                foreach (var meas in foundMeasurements)
                {
                    //string format ="dd/MM/YYYY HH:mm:ss " 
                    var measContent = new XElement("Object",
                        new XAttribute("Name", meas.Name),
                        (meas.HasRealValues.ToLower() == "true" ? new XElement("real",
                            new XAttribute("X", meas.XIst),
                            new XAttribute("Y", meas.YIst),
                            new XAttribute("Z", meas.ZIst),
                            new XAttribute("RX", meas.RXIst),
                            new XAttribute("RY", meas.RYIst),
                            new XAttribute("RZ", meas.RZIst))
                            : null),
                        new XElement("nominal",
                            new XAttribute("X", meas.XSoll),
                            new XAttribute("Y", meas.YSoll),
                            new XAttribute("Z", meas.ZSoll),
                            new XAttribute("RX", meas.RXSoll),
                            new XAttribute("RY", meas.RYSoll),
                            new XAttribute("RZ", meas.RZSoll)),
                        (!string.IsNullOrEmpty(meas.RobotType) ? new XElement("RobotType", meas.RobotType) : null),
                        new XElement("HasRealValues", meas.HasRealValues),
                        new XElement("IsZeroOffsetManualy", "false"),
                        new XElement("RobotZeroOffset",
                            new XAttribute("X", "0.000000"),
                            new XAttribute("Y", "0.000000"),
                            new XAttribute("Z", "0.000000")),
                        new XElement("AddedDate", DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))),
                        new XElement("ChangedUserNominal", "AIUT"),
                        new XElement("ChangedDateReal", DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("en-US"))),
                        new XElement("ChangedUserReal", "AIUT"),
                        (meas.BaseIDs == null || meas.BaseIDs.Count == 0 ? new XElement("ObjectBases") : GetBasesIDs(meas.BaseIDs, "ObjectBases", "item")));
                    doc.Element("Project").Element("Objects").Add(measContent);
                }
                foreach (var meas in foundMeasurements.Where(x => !string.IsNullOrEmpty(x.RobotType) && x.Bases!=null && x.Bases.Any()))
                {
                    foreach (var calcBase in meas.Bases)
                    {
                        if (!calcBase.ExtTCP)
                        {
                            var baseContent = new XElement("Base", new XAttribute("ID", calcBase.BaseID), new XAttribute("RobotType", meas.RobotType),
                                new XElement("Base_name", calcBase.Name),
                                new XElement("Base_Nr", calcBase.Number),
                                new XElement("Ext_Base", "False"),
                                new XElement("Obj1", new XAttribute("Name", calcBase.Robot.Name), new XAttribute("X", calcBase.Robot.X), new XAttribute("Y", calcBase.Robot.Y), new XAttribute("Z", calcBase.Robot.Z), new XAttribute("RX", calcBase.Robot.RX), new XAttribute("RY", calcBase.Robot.RY), new XAttribute("RZ", calcBase.Robot.RZ)),
                                new XElement("Obj2", new XAttribute("Name", calcBase.Obj2.Name), new XAttribute("X", calcBase.Obj2.X), new XAttribute("Y", calcBase.Obj2.Y), new XAttribute("Z", calcBase.Obj2.Z), new XAttribute("RX", calcBase.Obj2.RX), new XAttribute("RY", calcBase.Obj2.RY), new XAttribute("RZ", calcBase.Obj2.RZ))
                                );
                            doc.Element("Project").Element("Bases").Add(baseContent);
                        }
                        else
                        {
                            var baseContent = new XElement("Base", new XAttribute("ID", calcBase.BaseID), new XAttribute("RobotType", meas.RobotType),
                            new XElement("Base_name", calcBase.Name),
                            new XElement("Base_Nr", calcBase.Number),
                            new XElement("Ext_Base", "True"),
                            new XElement("Obj1", new XAttribute("Name", calcBase.Robot.Name), new XAttribute("X", calcBase.Robot.X), new XAttribute("Y", calcBase.Robot.Y), new XAttribute("Z", calcBase.Robot.Z), new XAttribute("RX", calcBase.Robot.RX), new XAttribute("RY", calcBase.Robot.RY), new XAttribute("RZ", calcBase.Robot.RZ)),
                            new XElement("Obj2", new XAttribute("Name", calcBase.Obj2.Name), new XAttribute("X", calcBase.Obj2.X), new XAttribute("Y", calcBase.Obj2.Y), new XAttribute("Z", calcBase.Obj2.Z), new XAttribute("RX", calcBase.Obj2.RX), new XAttribute("RY", calcBase.Obj2.RY), new XAttribute("RZ", calcBase.Obj2.RZ)),
                            new XElement("Obj3", new XAttribute("Name", calcBase.Obj3.Name), new XAttribute("X", calcBase.Obj3.X), new XAttribute("Y", calcBase.Obj3.Y), new XAttribute("Z", calcBase.Obj3.Z), new XAttribute("RX", calcBase.Obj3.RX), new XAttribute("RY", calcBase.Obj3.RY), new XAttribute("RZ", calcBase.Obj3.RZ)),
                            new XElement("Tool", new XAttribute("Name", calcBase.Tool.Name), new XAttribute("X", calcBase.Tool.X.ToString("F2", CultureInfo.InvariantCulture)), new XAttribute("Y", calcBase.Tool.Y.ToString("F2", CultureInfo.InvariantCulture)), new XAttribute("Z", calcBase.Tool.Z.ToString("F2", CultureInfo.InvariantCulture)), new XAttribute("RX", calcBase.Tool.RX.ToString("F4", CultureInfo.InvariantCulture)), new XAttribute("RY", calcBase.Tool.RY.ToString("F4", CultureInfo.InvariantCulture)), new XAttribute("RZ", calcBase.Tool.RZ.ToString("F4", CultureInfo.InvariantCulture))),
                            new XElement("Adjusted_TCP", new XAttribute("Name", calcBase.AdjustedTCP.Name), new XAttribute("X", calcBase.AdjustedTCP.X.ToString("F2", CultureInfo.InvariantCulture)), new XAttribute("Y", calcBase.AdjustedTCP.Y.ToString("F2", CultureInfo.InvariantCulture)), new XAttribute("Z", calcBase.AdjustedTCP.Z.ToString("F2", CultureInfo.InvariantCulture)), new XAttribute("RX", calcBase.AdjustedTCP.RX.ToString("F4", CultureInfo.InvariantCulture)), new XAttribute("RY", calcBase.AdjustedTCP.RY.ToString("F4",CultureInfo.InvariantCulture)), new XAttribute("RZ", calcBase.AdjustedTCP.RZ.ToString("F4", CultureInfo.InvariantCulture)))
                            );
                            doc.Element("Project").Element("Bases").Add(baseContent);
                        }
                    }
                }
                foreach (var meas in foundMeasurements.Where(x=>x.Safety!=null))
                {
                    XElement safetyContent = new XElement("SafeRob", new XAttribute("ID",meas.Name), new XAttribute("Type",meas.RobotType),
                        new XElement("RobotName",meas.Name),
                        new XElement("Calculated","true"));
                    foreach (var safeTool in meas.Safety.Where(x=>x.Type=="Tool"))
                    {
                        XElement toolElement = new XElement("Tool", new XAttribute("Nr", safeTool.Tool.Number - 1), new XAttribute("ID", "ts_" + safeTool.Tool.Number), new XAttribute("Name", "[" + safeTool.Tool.Number + "] Tool" + safeTool.Tool.Number),
                            new XElement("Matrix", new XAttribute("Type", "TCP"),new XAttribute("Nr","0"), new XAttribute("ID", "TCP"),new XAttribute("X",safeTool.Tool.TCP.X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", safeTool.Tool.TCP.Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", safeTool.Tool.TCP.Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RX", Common.ConvertToRadians(safeTool.Tool.TCP.RX).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RY", Common.ConvertToRadians(safeTool.Tool.TCP.RY).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RZ", Common.ConvertToRadians(safeTool.Tool.TCP.RZ).ToString("F6", CultureInfo.CreateSpecificCulture("en-US")))));
                        int counter = 0;
                        foreach (var sphere in safeTool.Tool.Spheres)
                        {
                            XElement sphereElement = new XElement("Point", new XAttribute("Type", "tool"), new XAttribute("Nr", counter), new XAttribute("ID", "ts" + safeTool.Tool.Number + "_sphere" + (counter+1) + "_" + sphere.Radius), new XAttribute("X", sphere.Coordinates.X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", sphere.Coordinates.Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", sphere.Coordinates.Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("DIAM",Convert.ToDouble(sphere.Radius).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))));
                            toolElement.Add(sphereElement);
                            counter++;
                        }
                        safetyContent.Add(toolElement);
                    }
                    var cellspace = meas.Safety.Where(x => x.Type == "Cellspace").FirstOrDefault();
                    XElement cellElement = new XElement("CSP", new XAttribute("ID", "Cellspace"), new XAttribute("Z_MIN", cellspace.GetMinCellspace().ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z_MAX", cellspace.GetMaxCellspace().ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))),new XAttribute("Calculated",cellspace.CellSpaceIst.Count==0 ? "False" : "True"));
                    int counter2 = 0;
                    foreach (var cellspaceSollPoint in cellspace.CellSpaceSoll)
                    {
                        cellElement.Add(new XElement("Point", new XAttribute("Nr", counter2), new XAttribute("Type", "nominal"), new XAttribute("ID", "CS_" + (counter2 + 1)), new XAttribute("X", cellspaceSollPoint.X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", cellspaceSollPoint.Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", cellspaceSollPoint.Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US")))));
                        if (cellspace.CellSpaceIst.Count == cellspace.CellSpaceSoll.Count)
                            cellElement.Add(new XElement("Point", new XAttribute("Nr", counter2), new XAttribute("Type", "real"), new XAttribute("X", cellspace.CellSpaceIst[counter2].X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", cellspace.CellSpaceIst[counter2].Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", cellspace.CellSpaceIst[counter2].Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US")))));
                        counter2++;
                    }
                    safetyContent.Add(cellElement);

                    foreach (var safeSpace in meas.Safety.Where(x=>x.Type == "Safespaces"))
                    {
                        XElement safeSpaceElement = new XElement("SP", new XAttribute("Nr", safeSpace.SafeSpaces.Number - 1), new XAttribute("ID", "sp" + safeSpace.SafeSpaces.Number), new XAttribute("Name", "[" + safeSpace.SafeSpaces.Number + "] " + safeSpace.SafeSpaces.Name), new XAttribute("Calculated", safeSpace.SafeSpaces.OriginIst == null ? "False" : "True"), new XAttribute("ProtectedSpace", "1"),
                            new XElement("ReferenceObj", safeSpace.SafeSpaces.RefObj),
                            new XElement("Matrix",new XAttribute("Type", "nominal"), new XAttribute("X",safeSpace.SafeSpaces.OriginSoll.X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", safeSpace.SafeSpaces.OriginSoll.Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", safeSpace.SafeSpaces.OriginSoll.Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RX", Common.ConvertToRadians(safeSpace.SafeSpaces.OriginSoll.RX).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RY", Common.ConvertToRadians(safeSpace.SafeSpaces.OriginSoll.RY).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RZ", Common.ConvertToRadians(safeSpace.SafeSpaces.OriginSoll.RZ).ToString("F6", CultureInfo.CreateSpecificCulture("en-US")))));
                        if (safeSpace.SafeSpaces.OriginIst!=null)
                            safeSpaceElement.Add(new XElement("Matrix", new XAttribute("Type", "real"), new XAttribute("X", safeSpace.SafeSpaces.OriginIst.X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", safeSpace.SafeSpaces.OriginIst.Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", safeSpace.SafeSpaces.OriginIst.Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RX", Common.ConvertToRadians(safeSpace.SafeSpaces.OriginIst.RX).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RY", Common.ConvertToRadians(safeSpace.SafeSpaces.OriginIst.RY).ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("RZ", Common.ConvertToRadians(safeSpace.SafeSpaces.OriginIst.RZ).ToString("F6", CultureInfo.CreateSpecificCulture("en-US")))));
                        safeSpaceElement.Add(new XElement("Point", new XAttribute("Type", "minimum"), new XAttribute("ID", "min"), new XAttribute("X", "0.000000"), new XAttribute("Y", "0.000000"), new XAttribute("Z", "0.000000")));
                        safeSpaceElement.Add(new XElement("Point", new XAttribute("Type", "maximum"), new XAttribute("ID", "max"), new XAttribute("X", safeSpace.SafeSpaces.Dimensions.X.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Y", safeSpace.SafeSpaces.Dimensions.Y.ToString("F6", CultureInfo.CreateSpecificCulture("en-US"))), new XAttribute("Z", safeSpace.SafeSpaces.Dimensions.Z.ToString("F6", CultureInfo.CreateSpecificCulture("en-US")))));
                        safetyContent.Add(safeSpaceElement);
                    }
                    doc.Element("Project").Element("SafetySets").Add(safetyContent);
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

        private static XElement GetBasesIDs(List<string> baseIDs, string nameElement, string nameAtribute)
        {
            XElement result = new XElement(nameElement);
            foreach (var id in baseIDs)
            {
                result.Add(new XElement(nameAtribute, id));
            }
            return result;
        }

        internal static void LoadReal(ObservableCollection<Measurement> meas)
        {
            string selectedMeasurement = CommonLibrary.CommonMethods.SelectDirOrFile(false, filter1Descr: "CSV file", filter1: "*.csv");
            if (string.IsNullOrEmpty(selectedMeasurement))
                return;
            Views.MeasurementWindow window = new Views.MeasurementWindow();
            Coords coordinates = GetCoords(selectedMeasurement);
            Messenger.Default.Send(meas, "measToModify");
            Messenger.Default.Send(coordinates, "selectedCSV");
            Messenger.Default.Send(Path.GetFileNameWithoutExtension(selectedMeasurement), "selectedCSVName");
            window.ShowDialog();            
            
        }

        private static Coords GetCoords(string selectedMeasurement)
        {
            Regex getNumbers = new Regex(@"-\d+\.\d+|-\d|\d+\.\d+|d\+", RegexOptions.IgnoreCase);
            double x=0, y=0, z=0, rx=0, ry=0, rz=0;
            StreamReader reader = new StreamReader(selectedMeasurement);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.ToLower().Contains("translation "))
                {
                    double.TryParse(getNumbers.Matches(line)[0].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out x);
                    double.TryParse(getNumbers.Matches(line)[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out y);
                    double.TryParse(getNumbers.Matches(line)[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out z);
                }
                if (line.ToLower().Contains("fixed xyz rotation"))
                {
                    double.TryParse(getNumbers.Matches(line)[0].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out rx);
                    double.TryParse(getNumbers.Matches(line)[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out ry);
                    double.TryParse(getNumbers.Matches(line)[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out rz);

                }
            }
            reader.Close();
            return new Coords(x, y, z, rx, ry, rz);
        }

        public static string IDGenerator()
        {
            int maxNr1 = 65535, maxNr2 = 65535, maxNr3 = 65535, maxNr4 = 65535, maxNr5= 65535, maxNr6 = 16777215, maxNr7 = 16777215;
            Random rnd = new Random();
            string num1 = rnd.Next(0, maxNr1).ToString("X4");
            string num2 = rnd.Next(0, maxNr2).ToString("X4");
            string num3 = rnd.Next(0, maxNr3).ToString("X4");
            string num4 = rnd.Next(0, maxNr4).ToString("X4");
            string num5 = rnd.Next(0, maxNr5).ToString("X4");
            string num6 = rnd.Next(0, maxNr6).ToString("X6");
            string num7 = rnd.Next(0, maxNr7).ToString("X6");

            return String.Format("{0}{1}-{2}-{3}-{4}-{5}{6}",num1, num2, num3, num4, num5, num6, num7).ToLower();
        }
    }
}

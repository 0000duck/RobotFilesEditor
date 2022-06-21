using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using RobotFilesEditor.Model.Operations.DataClass;
using PdfSharp.Pdf;
//using PdfSharp.Pdf.IO;
//using PdfSharp.Drawing;
using MigraDoc;
using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace RobotFilesEditor.Model.Operations
{
    public class ReadSafetyXMLModel
    {
        Dictionary<int, XPen> zoneColours;
        const int heightPixels = 850;
        const int widthPixels = 600;
        const int marginPixels = 0;
        int scalingfactor;

        public ReadSafetyXMLModel()
        {
            FillZoneColours();
            Execute();
        }

        private void Execute()
        {
            MessageBox.Show("Select xml file.", "Select file", MessageBoxButton.OK, MessageBoxImage.Information);
            string file = CommonLibrary.CommonMethods.SelectDirOrFile(false, "XML file", "*.xml");
            if (!File.Exists(file))
                return;
            try
            {
                XDocument doku = XDocument.Load(file);

                var usedworkspaces = doku.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("RangeMonitoring").Elements("WorkspaceMonitoring").Where(x => x.Element("Activation").Value != "255");
                List<DataClass.SafetyZone> safeZones = GetSafeZones(usedworkspaces);
                var usedtools = doku.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("Tools").Elements("Tool").Where(x => x.Element("ToolEnabled").Value != "0");
                List<DataClass.SafetyTool> safeTools = GetSafeTools(usedtools);
                var cellspacexml = doku.Element("configuration").Element("KUKARoboter.SafeRobot.Parameters").Element("encryptedData").Element("SafetyParameters").Element("RangeMonitoring").Element("CellSpace");
                Cellspace cellspace = GetCellspace(cellspacexml);
                DataClass.SafetyElementsClass safetyConfig = new DataClass.SafetyElementsClass(safeTools, safeZones, cellspace);
                WriteSafety(safetyConfig);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while reading xml file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Cellspace GetCellspace(XElement cellspacexml)
        {
            double minHeight = double.Parse(cellspacexml.Element("Zmin").Value, CultureInfo.InvariantCulture);
            double maxHeight = double.Parse(cellspacexml.Element("Zmax").Value, CultureInfo.InvariantCulture);
            List<CommonLibrary.PointXYZABC> cellspacepoints = new List<CommonLibrary.PointXYZABC>();
            foreach (var polygon in cellspacexml.Elements("Polygon").Where(x => x.Element("IsPolygonNodeActive").Value == "1"))
            {
                double x = double.Parse(polygon.Element("X").Value, CultureInfo.InvariantCulture);
                double y = double.Parse(polygon.Element("Y").Value, CultureInfo.InvariantCulture);
                cellspacepoints.Add(new CommonLibrary.PointXYZABC(x, y, minHeight, 0, 0, 0));
            }
            return new DataClass.Cellspace(minHeight, maxHeight, cellspacepoints);
        }

        private List<SafetyTool> GetSafeTools(IEnumerable<XElement> usedtools)
        {
            List<DataClass.SafetyTool> result = new List<SafetyTool>();
            foreach (var tool in usedtools)
            {
                List<DataClass.Sphere> spheres = new List<DataClass.Sphere>();
                int number = int.Parse(tool.Attribute("Number").Value);
                string name = tool.Attribute("Name").Value;
                double xTCP = double.Parse(tool.Element("TCPVector_X").Value, CultureInfo.InvariantCulture);
                double yTCP = double.Parse(tool.Element("TCPVector_Y").Value, CultureInfo.InvariantCulture);
                double zTCP = double.Parse(tool.Element("TCPVector_Z").Value, CultureInfo.InvariantCulture);
                foreach (var sphere in tool.Elements("Sphere").Where(x => x.Element("SphereEnabled").Value == "1"))
                {
                    int spherenumber = int.Parse(sphere.Attribute("Number").Value);
                    double radius = double.Parse(sphere.Element("Radius").Value, CultureInfo.InvariantCulture);
                    CommonLibrary.PointXYZABC center = new CommonLibrary.PointXYZABC(double.Parse(sphere.Element("X").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Element("Y").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Element("Z").Value, CultureInfo.InvariantCulture), 0, 0, 0);
                    spheres.Add(new DataClass.Sphere(spherenumber, radius, center));
                }
                result.Add(new DataClass.SafetyTool(number, name, new CommonLibrary.PointXYZABC(xTCP, yTCP, zTCP, 0, 0, 0), spheres));
            }
            return result;
        }

        private List<SafetyZone> GetSafeZones(IEnumerable<XElement> usedworkspaces)
        {
            List<SafetyZone> result = new List<SafetyZone>();
            foreach (var zone in usedworkspaces)
            {
                int number = int.Parse(zone.Attribute("Number").Value);
                string name = zone.Attribute("Name").Value;
                bool ispermanent = zone.Element("Activation").Value == "0" ? true : false;
                double xOrigin = double.Parse(zone.Element("CartesianRange").Element("X").Value, CultureInfo.InvariantCulture);
                double yOrigin = double.Parse(zone.Element("CartesianRange").Element("Y").Value, CultureInfo.InvariantCulture);
                double zOrigin = double.Parse(zone.Element("CartesianRange").Element("Z").Value, CultureInfo.InvariantCulture);
                double aOrigin = double.Parse(zone.Element("CartesianRange").Element("A").Value, CultureInfo.InvariantCulture);
                double bOrigin = double.Parse(zone.Element("CartesianRange").Element("B").Value, CultureInfo.InvariantCulture);
                double cOrigin = double.Parse(zone.Element("CartesianRange").Element("C").Value, CultureInfo.InvariantCulture);
                double x1 = double.Parse(zone.Element("CartesianRange").Element("X1").Value, CultureInfo.InvariantCulture);
                double y1 = double.Parse(zone.Element("CartesianRange").Element("Y1").Value, CultureInfo.InvariantCulture);
                double z1 = double.Parse(zone.Element("CartesianRange").Element("Z1").Value, CultureInfo.InvariantCulture);
                double x2 = double.Parse(zone.Element("CartesianRange").Element("X2").Value, CultureInfo.InvariantCulture);
                double y2 = double.Parse(zone.Element("CartesianRange").Element("Y2").Value, CultureInfo.InvariantCulture);
                double z2 = double.Parse(zone.Element("CartesianRange").Element("Z2").Value, CultureInfo.InvariantCulture);

                result.Add(new DataClass.SafetyZone(number, ispermanent, name, new CommonLibrary.PointXYZABC(xOrigin, yOrigin, zOrigin, aOrigin, bOrigin, cOrigin), new CommonLibrary.PointXYZABC(x1, y1, z1, 0, 0, 0), new CommonLibrary.PointXYZABC(x2, y2, z2, 0, 0, 0)));
            }
            return result;
        }

        private void WriteSafety(SafetyElementsClass safetyConfig)
        {
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\RobotFilesHarvester\SafetyConfig.pdf";
            if (File.Exists(filename) && CommonLibrary.CommonMethods.IsFileLocked(filename))
            {
                MessageBox.Show("File " + filename + " is used by another process. Please close the file and run functionality again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Document document = new Document();
            document.Info.Title = "Safety overview";
            Section section = document.AddSection();
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 24;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddFormattedText("Safety configuration report",TextFormat.Bold);

            //CELLSPACE
            paragraph = section.AddParagraph();
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddFormattedText("Cellspace", TextFormat.Bold);
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.AddFormattedText("Min. Height: " + safetyConfig.Cellspace.Min);
            paragraph.Format.Font.Color = Colors.Black;
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.AddFormattedText("Max. Height: " + safetyConfig.Cellspace.Max);
            document.LastSection.AddParagraph();
            Table table = new Table();
            table.Borders.Width = 0.75;
            Column column1 = table.AddColumn(Unit.FromCentimeter(2));
            column1.Format.Alignment = ParagraphAlignment.Center;
            Column column2 = table.AddColumn(Unit.FromCentimeter(13));
            column2.Format.Alignment = ParagraphAlignment.Center;
            int rowCounter = 1;
            foreach (var cellpoint in safetyConfig.Cellspace.Points)
            {
                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                cell.AddParagraph("Point " + rowCounter);
                cell = row.Cells[1];
                cell.AddParagraph("X: "+ cellpoint.X + "; Y: " + cellpoint.Y + "; Z: " + cellpoint.Z);
                rowCounter++;
            }
            table.SetEdge(0, 0, 2, rowCounter - 1, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
            document.LastSection.Add(table);
            rowCounter = 1;

            //SAFE ZONES
            paragraph = section.AddParagraph();
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddFormattedText("SafeZones:", TextFormat.Bold);

            foreach (var zone in safetyConfig.SafeZones)
            {
                paragraph = section.AddParagraph();
                table = new Table();
                table.Borders.Width = 0.75;
                column1 = table.AddColumn(Unit.FromCentimeter(15));
                column1.Format.Alignment = ParagraphAlignment.Left;

                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                cell.AddParagraph("Zone " + zone.Number + ", Name: " + zone.Name);
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("Activation: " + (zone.IsPermanent ? "Permanent" : "By Input"));
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("Origin: X: " + zone.Origin.X + "; Y: " + zone.Origin.Y + "; Z: " + zone.Origin.Z + "; A: " + zone.Origin.A + "; B: " + zone.Origin.B + "; C: " + zone.Origin.C);
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("P1: X: " + zone.P1.X + "; Y: " + zone.P1.Y + "; Z: " + zone.P1.Z);
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("P2: X: " + zone.P2.X + "; Y: " + zone.P2.Y + "; Z: " + zone.P2.Z);
                table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                document.LastSection.Add(table);
            }

            //TOOLS
            paragraph = section.AddParagraph();
            paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Colors.Black;
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddFormattedText("Safety Tools:", TextFormat.Bold);

            foreach (var tool in safetyConfig.Tools)
            {
                paragraph = section.AddParagraph();
                table = new Table();
                table.Borders.Width = 0.75;
                column1 = table.AddColumn(Unit.FromCentimeter(15));
                column1.Format.Alignment = ParagraphAlignment.Left;

                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                cell.AddParagraph("Tool " + tool.Number + ", Name: " + tool.Name);
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("TCP: X: " + tool.TCP.X + "; Y: " + tool.TCP.Y + "; Z: " + tool.TCP.Z);
                table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                document.LastSection.Add(table);

                table = new Table();
                table.Borders.Width = 0.75;
                column1 = table.AddColumn(Unit.FromCentimeter(3));
                column2 = table.AddColumn(Unit.FromCentimeter(9));
                Column column3 = table.AddColumn(Unit.FromCentimeter(3));
                rowCounter = 1;
                foreach (var sphere in tool.Spheres)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph("Sphere " + rowCounter);
                    cell = row.Cells[1];
                    cell.AddParagraph("X: " + sphere.Center.X + "; Y: " + +sphere.Center.Y + "; Z: " + +sphere.Center.Z);
                    cell = row.Cells[2];
                    cell.AddParagraph("Radius " + sphere.Radius);
                    rowCounter++;
                }
                table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                document.LastSection.Add(table);
            }

            document.UseCmykColor = true;
            document.LastSection.AddPageBreak();
            List<ShapesToDraw> shapes = GetShapesToDraw(safetyConfig);

            table = new Table();
            table.Borders.Width = 0.75;
            Column columnNumber = table.AddColumn(Unit.FromCentimeter(3)); // NUMBER
            Column columnName = table.AddColumn(Unit.FromCentimeter(6)); // NAME
            Column columnTypr = table.AddColumn(Unit.FromCentimeter(6)); // TYPE
            foreach (var shape in shapes)
            {
                Row row = table.AddRow();
                Cell cell = row.Cells[0];
                cell.AddParagraph(shape.Number.ToString());
                cell.Format.Font.Color = ConvertXColorToColor(zoneColours[shape.Number].Color);
                cell = row.Cells[1];
                cell.AddParagraph(shape.Name.ToString());
                cell.Format.Font.Color = ConvertXColorToColor(zoneColours[shape.Number].Color);
                cell = row.Cells[2];
                string type = shape.Number == 0 ? "Cellspace" : "SafeZone";
                if (shape.Number > 0)
                    type = type + (shape.IsPermanent ? " permanent" : " switchable");
                cell.AddParagraph(type);
                cell.Format.Font.Color = ConvertXColorToColor(zoneColours[shape.Number].Color);
            }
            table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
            document.LastSection.Add(table);

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = document;
            pdfRenderer.PrepareRenderPages();
            //var page = pdfRenderer.PdfDocument.AddPage(new PdfPage());
            pdfRenderer.RenderDocument();
            int pages = pdfRenderer.DocumentRenderer.FormattedDocument.PageCount;
            var page = pdfRenderer.PdfDocument.Pages[pages-1];

            //RYSOWANIE STREF

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                //Point2d centerPoint = new Point2d(105, 297 / 2);
                Point2d centerPoint = CalculateCenterPoint(shapes) ;
                //int scalingfactor = CalculateScalingFactor(shapes);
                gfx.MUH = pdfRenderer.Unicode ? PdfFontEncoding.Unicode : PdfFontEncoding.WinAnsi;
                //gfx.DrawLine(XPens.Black, 0, 0, GetPixelsFromMM(210), GetPixelsFromMM(297));
                Image img = Image.FromFile(@"..\..\Resources\coordinate_system.png");
                gfx.DrawImage(XImage.FromFile(@"..\..\Resources\coordinate_system.png"), CalculateImgStartPoint(centerPoint.XPos,img.Width/img.HorizontalResolution), CalculateImgStartPoint(centerPoint.YPos, img.Height/ img.VerticalResolution));
                foreach (var shape in shapes)
                {
                    for (int i = 1; i <= shape.Points.Count; i++)
                    {
                        Point2d ptStart, ptEnd;
                        if (i < shape.Points.Count)
                        {
                            ptStart = shape.Points[i - 1];
                            ptEnd = shape.Points[i];
                        }
                        else
                        {
                            ptStart = shape.Points[i - 1];
                            ptEnd = shape.Points[0];
                        }
                        var x1 = GetPixelsFromMM((centerPoint.XPos) + (ptStart.XPos / scalingfactor));
                        var y1 = GetPixelsFromMM((centerPoint.YPos) - (ptStart.YPos / scalingfactor));
                        var x2 = GetPixelsFromMM((centerPoint.XPos) + (ptEnd.XPos / scalingfactor));
                        var y2 = GetPixelsFromMM((centerPoint.YPos) - (ptEnd.YPos / scalingfactor));
                        gfx.DrawLine(zoneColours[shape.Number], x1, y1, x2, y2);
                    }
                }
                pdfRenderer.DocumentRenderer.RenderPage(gfx, pages);
            }
            //pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(filename);
            Process.Start(filename);

        }

        private Point2d CalculateCenterPoint(List<ShapesToDraw> shapes)
        {
            Point2d result = new Point2d();
            int pixelsPerRow = (int)Math.Ceiling(GetPixelsFromMM(45) + shapes.Count * 2);
            //int pixelsPerRow = 0;
            double maxX = GetLimit("X", "high", shapes);
            double maxY = GetLimit("Y", "high", shapes);
            double minX = GetLimit("X", "low", shapes);
            double minY = GetLimit("Y", "low", shapes);

            double centerXinMM = (Math.Abs(minX) + Math.Abs(maxX))/2;
            double centerYinMM = (Math.Abs(minY) + Math.Abs(maxY))/2 + pixelsPerRow * shapes.Count;

            scalingfactor = CalculateScalingFactor(centerXinMM, centerYinMM, shapes.Count);

            double totalX = Math.Abs(maxX - minX);
            double totalY = Math.Abs(maxY - minY);

            double xRatio = 0;
            double yRatio = 0;

            //if (Math.Abs(maxX) > Math.Abs(minX))
            if (centerXinMM - maxX > centerXinMM - minX)
                xRatio = Math.Abs(maxX) / totalX;
            else
                xRatio = Math.Abs(minX) / totalX;

            if (centerYinMM - maxY < centerYinMM - minY)
                yRatio = (Math.Abs(maxY) + pixelsPerRow * shapes.Count) / totalY;
            else
                yRatio = (Math.Abs(minY) + pixelsPerRow * shapes.Count) / totalY;

            result.XPos = xRatio * GetMMFromPixels(widthPixels);
            result.YPos = yRatio * GetMMFromPixels(heightPixels);

            return result;
        }

        private double GetLimit(string axis, string limit, List<ShapesToDraw> shapes)
        {
            double result = 0;
            bool firstCycle = true;
            foreach (var shape in shapes)
            {
                foreach (var point in shape.Points)
                {
                    
                    if (axis == "X")
                    {
                        if (firstCycle)
                        {
                            result = point.XPos;
                            firstCycle = false;
                        }
                        if (limit == "high")
                        {
                            if (point.XPos > result)
                                result = point.XPos;
                        }
                        else
                        {
                            if (point.XPos < result)
                                result = point.XPos;
                        }
                    }
                    else
                    {
                        if (firstCycle)
                        {
                            result = point.YPos;
                            firstCycle = false;
                        }
                        if (limit == "high")
                        {
                            if (point.YPos > result)
                                result = point.YPos;
                        }
                        else
                        {
                            if (point.YPos < result)
                                result = point.YPos;
                        }
                    }
                }
            }
            return result;
        }

        private MigraDoc.DocumentObjectModel.Color ConvertXColorToColor(XColor color)
        {

            return new MigraDoc.DocumentObjectModel.Color(color.R, color.G, color.B);
        }

        private double CalculateImgStartPoint(double maxSize, double imageSize)
        {
            return GetPixelsFromMM(maxSize - (25.4 * imageSize) / 2);
        }

        private double GetPixelsFromMM(double v)
        {
            double result = v * 2.857142857142;
            return result;
        }

        private double GetMMFromPixels(double v)
        {
            double result = v / 2.857142857142;
            return result;
        }

        private List<ShapesToDraw> GetShapesToDraw(SafetyElementsClass safetyConfig)
        {
            List<ShapesToDraw> result = new List<ShapesToDraw>();
            //Cellspace
            List<Point2d> pointsOnCellspace = new List<Point2d>();
            foreach (var pt in safetyConfig.Cellspace.Points)
                pointsOnCellspace.Add(new Point2d(pt.X, pt.Y));
            result.Add(new ShapesToDraw(pointsOnCellspace, "Cellspace", 0, true));

            //SafeZones
            foreach (var zone in safetyConfig.SafeZones)
            {
                Matrix<double> originMatrix = CommonLibrary.MatrixOperationsMethods.BuildHTMMatrix(zone.Origin);
                Matrix<double> p1Matrix = CommonLibrary.MatrixOperationsMethods.BuildHTMMatrix(zone.P1);
                Matrix<double> p2Matrix = CommonLibrary.MatrixOperationsMethods.BuildHTMMatrix(zone.P2);

                Matrix<double> p1Shifted = originMatrix.Multiply(p1Matrix);
                Matrix<double> p2Shifted = originMatrix.Multiply(p2Matrix);

                Matrix<double> xPoint = p2Matrix.Clone();
                xPoint[1, 3] = 0;
                Matrix<double> xPointCalculated = originMatrix.Multiply(xPoint);

                Matrix<double> yPoint = p2Matrix.Clone();
                yPoint[0, 3] = 0;
                Matrix<double> yPointCalculated = originMatrix.Multiply(yPoint);

                Point2d p1 = new Point2d(p1Shifted[0, 3], p1Shifted[1, 3]);
                Point2d p2 = new Point2d(xPointCalculated[0, 3], xPointCalculated[1, 3]);
                Point2d p3 = new Point2d(p2Shifted[0, 3], p2Shifted[1, 3]);
                Point2d p4 = new Point2d(yPointCalculated[0, 3], yPointCalculated[1, 3]);

                List<Point2d> points = new List<Point2d>() { p1, p2, p3, p4 };

                result.Add(new ShapesToDraw(points, zone.Name, zone.Number, zone.IsPermanent));
            }

            return result;
        }

        private int CalculateScalingFactor(List<ShapesToDraw> shapes)
        {
            int result = 1;
            double maxPixelsX = (widthPixels/2) - marginPixels;
            double maxPixelsY = (heightPixels / 2) - marginPixels;

            double maxX = GetMaxValue("X",shapes);
            double maxY = GetMaxValue("Y", shapes);

            double pixelsX = GetPixelsFromMM(maxX);
            double pixelsY = GetPixelsFromMM(maxY);

            int scalingFactorX = (int)Math.Ceiling(Math.Abs(pixelsX)/maxPixelsX);
            int scalingFactorY = (int)Math.Ceiling(Math.Abs(pixelsY) / maxPixelsY);

            result = scalingFactorX > scalingFactorY ? scalingFactorX : scalingFactorY;
            return result;
        }

        private int CalculateScalingFactor(double centerXinMM, double centerYinMM, int shapesCount)
        {
            int result = 1;
            double maxPixelsX = (widthPixels / 2) - marginPixels;
            double maxPixelsY = (heightPixels / 2) - marginPixels;

            double maxX = centerXinMM;
            double maxY = centerYinMM;

            double pixelsX = GetPixelsFromMM(maxX);
            double pixelsY = GetPixelsFromMM(maxY);

            int scalingFactorX = (int)Math.Ceiling(Math.Abs(pixelsX) / maxPixelsX * 1.1);
            int scalingFactorY = (int)Math.Ceiling(Math.Abs(pixelsY) / maxPixelsY * 1.1 );

            result = scalingFactorX > scalingFactorY ? scalingFactorX : scalingFactorY;
            return result;
        }

        private double GetMaxValue(string axis, List<ShapesToDraw> shapes)
        {
            double result = 0;
            foreach (var shape in shapes)
            {
                foreach (var point in shape.Points)
                {
                    if (axis == "X")
                    {
                        if (Math.Abs(point.XPos) > Math.Abs(result))
                            result = point.XPos;
                    }
                    else
                        if (Math.Abs(point.YPos) > Math.Abs(result))
                            result = point.YPos;
                }
            }
            return result;
        }

        private void FillZoneColours()
        {
            zoneColours = new Dictionary<int, XPen>();
            zoneColours.Add(0, XPens.Black);
            zoneColours.Add(1, XPens.DarkRed);
            zoneColours.Add(2, XPens.DeepPink);
            zoneColours.Add(3, XPens.BlueViolet);
            zoneColours.Add(4, XPens.DarkGreen);
            zoneColours.Add(5, XPens.YellowGreen);
            zoneColours.Add(6, XPens.RosyBrown);
            zoneColours.Add(7, XPens.Crimson);
            zoneColours.Add(8, XPens.Tomato);
            zoneColours.Add(9, XPens.Violet);
            zoneColours.Add(10, XPens.SeaGreen);
            zoneColours.Add(11, XPens.Navy);
            zoneColours.Add(12, XPens.Magenta);
            zoneColours.Add(13, XPens.DarkCyan);
            zoneColours.Add(14, XPens.DarkGray);
            zoneColours.Add(15, XPens.Lavender);
            zoneColours.Add(16, XPens.LawnGreen);
        }
    }

    public class ShapesToDraw
    {
        public List<Point2d> Points { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool IsPermanent { get; set; }

        public ShapesToDraw(List<Point2d> points, string name, int number, bool isPermanent)
        {
            Points = points;
            Name = name;
            Number = number;
            IsPermanent = isPermanent;
        }
    }
}

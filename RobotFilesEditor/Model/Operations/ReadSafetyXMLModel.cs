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

namespace RobotFilesEditor.Model.Operations
{
    public class ReadSafetyXMLModel
    {

        public ReadSafetyXMLModel()
        {
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
            List<DataClass.PointKUKA> cellspacepoints = new List<DataClass.PointKUKA>();
            foreach (var polygon in cellspacexml.Elements("Polygon").Where(x => x.Element("IsPolygonNodeActive").Value == "1"))
            {
                double x = double.Parse(polygon.Element("X").Value, CultureInfo.InvariantCulture);
                double y = double.Parse(polygon.Element("Y").Value, CultureInfo.InvariantCulture);
                cellspacepoints.Add(new DataClass.PointKUKA(x, y, minHeight, 0, 0, 0));
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
                    DataClass.PointKUKA center = new DataClass.PointKUKA(double.Parse(sphere.Element("X").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Element("Y").Value, CultureInfo.InvariantCulture), double.Parse(sphere.Element("Z").Value, CultureInfo.InvariantCulture), 0, 0, 0);
                    spheres.Add(new DataClass.Sphere(spherenumber, radius, center));
                }
                result.Add(new DataClass.SafetyTool(number, name, new DataClass.PointKUKA(xTCP, yTCP, zTCP, 0, 0, 0), spheres));
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

                result.Add(new DataClass.SafetyZone(number, ispermanent, name, new DataClass.PointKUKA(xOrigin, yOrigin, zOrigin, aOrigin, bOrigin, cOrigin), new DataClass.PointKUKA(x1, y1, z1, 0, 0, 0), new DataClass.PointKUKA(x2, y2, z2, 0, 0, 0)));
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
                cell.AddParagraph("X: "+ cellpoint.Xpos + "; Y: " + cellpoint.Ypos + "; Z: " + cellpoint.Zpos);
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
                cell.AddParagraph("Origin: X: " + zone.Origin.Xpos + "; Y: " + zone.Origin.Ypos + "; Z: " + zone.Origin.Zpos + "; A: " + zone.Origin.A + "; B: " + zone.Origin.B + "; C: " + zone.Origin.C);
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("P1: X: " + zone.P1.Xpos + "; Y: " + zone.P1.Ypos + "; Z: " + zone.P1.Zpos);
                row = table.AddRow();
                cell = row.Cells[0];
                cell.AddParagraph("P2: X: " + zone.P2.Xpos + "; Y: " + zone.P2.Ypos + "; Z: " + zone.P2.Zpos);
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
                cell.AddParagraph("TCP: X: " + tool.TCP.Xpos + "; Y: " + tool.TCP.Ypos + "; Z: " + tool.TCP.Zpos);
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
                    cell.AddParagraph("X: " + sphere.Center.Xpos + "; Y: " + +sphere.Center.Ypos + "; Z: " + +sphere.Center.Zpos);
                    cell = row.Cells[2];
                    cell.AddParagraph("Radius " + sphere.Radius);
                    rowCounter++;
                }
                table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
                document.LastSection.Add(table);
            }

            document.UseCmykColor = true;
            //const bool unicode = false;
            //const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(filename);
            Process.Start(filename);

        }
    }
}

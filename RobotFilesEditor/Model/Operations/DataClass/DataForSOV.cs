using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Model.Operations.DataClass
{
    public class DataForSOV
    {
        public IDictionary<int, ToolDataSOV> ToolDatasSOV { get; set; }
        public IDictionary<int, BaseDataSOV> BaseDatasSOV { get; set; }
        public IDictionary<int, HomeSOV> HomesSOV { get; set; }
        public IDictionary<string, GlobalPointsSOV> GlobalPointsSOV { get; set; }

        public DataForSOV(IDictionary<int, ToolDataSOV> toolDatasSOV, IDictionary<int, BaseDataSOV> baseDatasSOV, IDictionary<int, HomeSOV> homesSOV, IDictionary<string, GlobalPointsSOV> globalPointsSOV)
        {
            ToolDatasSOV = toolDatasSOV;
            BaseDatasSOV = baseDatasSOV;
            HomesSOV = homesSOV;
            GlobalPointsSOV = globalPointsSOV;
        }
    }

    public class ToolDataSOV
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }

        public ToolDataSOV()
        {
        }
    }
    public class BaseDataSOV
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }

        public BaseDataSOV()
        {
        }
    }

    public class HomeSOV
    {
        public int Number { get; set; }
        public string Content { get; set; }

        public HomeSOV(int number, string content)
        {
            Number = number;
            Content = content;
        }
    }

    public class GlobalPointsSOV
    {
        public string Name { get; set; }
        public string Content { get; set; }

        public GlobalPointsSOV(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }

}

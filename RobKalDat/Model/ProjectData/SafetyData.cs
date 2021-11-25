using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobKalDat.Model.ProjectData
{
    public class SafetyData
    {
        //wszystkie
        public int Index { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } //tool, cellspace, safespace
        public Coords SollPoint { get; set; }

        //Cellspace
        public Coords IstPoint { get; set; }
        public double CellSpaceHeight { get; set; }
        //Tool
        public double Radius { get; set; }
        //SafeSpace
        public int Number { get; set; }
        public Coords MaxSoll { get; set; }
    }

    public class ItemInSafety
    {
        public string Type { get; set; } //tool, cellspace, safespace
        public KukaSafeTool Tool { get; set; }
        public List<Coords> CellSpaceSoll { get; set; }
        public double CellSpaceHeight { get; set; }
        public List<Coords> CellSpaceIst { get; set; }
        public SafeSpace SafeSpaces { get; set; }

        public double GetMinCellspace ()
        {
            double result = 100000;
            List<Coords> currentList = this.CellSpaceSoll;
            if (this.CellSpaceIst.Count == this.CellSpaceSoll.Count)
                currentList = this.CellSpaceIst;
            foreach (var item in currentList.Where(x => x.Z < result))
                result = item.Z;
            return result;
        }

        public double GetMaxCellspace()
        {
            double min = this.GetMinCellspace();
            return min+this.CellSpaceHeight;
        }
    }

    public class Sphere
    {
        public Coords Coordinates { get; set; }
        public double Radius { get; set; }
    }

    public class SafeSpace
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string RefObj { get; set; }
        public Coords OriginSoll { get; set; }
        public Coords OriginIst { get; set; }
        public Coords Max { get; set; }
        public Coords Dimensions { get; set; }

    }

    public class KukaSafeTool
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public Coords TCP { get; set; }
        public List<Sphere> Spheres { get; set; }
    }


}

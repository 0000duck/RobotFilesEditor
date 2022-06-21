using RobotFilesEditor.Model.DataInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static RobotFilesEditor.Model.Operations.DataClass.UserBitKUKA;

namespace RobotFilesEditor.Model.Operations.DataClass
{

    public class RobotData
    {
        //public FileValidationData.Userbits GripperConfig { get; set; }
        public IGripperConfig GripperConfig { get; set; }
        public CommonLibrary.SafetyConfig SafetyConfig { get; set; }
        public List<IToolData> ToolDatas { get; set; }
        public List<ToolName> ToolNames { get; set; }
        public List<ILoadData> LoadDatas { get; set; }
        public List<ILoadData> LoadVars { get; set; }
        public List<ToolType> ToolTypes { get; set; }
        public List<CommonLibrary.IRobotPoint> BaseDatas { get; set; }
        public List<BaseName> BaseNames { get; set; }
        public List<BaseType> BaseTypes { get; set; }
        public List<HomePos> HomePoses { get; set; }
        public List<Program> Programs { get; set; }
        public List<HomeNameLong> HomeNameLongs { get; set; }
        public List<HomeNameShort> HomeNameShorts { get; set; }
        public List<CollZone> CollZones { get; set; }
        public List<CollZone> CollZonesFromCommnad { get; set; }
        public List<CollZone> CollZonesWithoutDescr { get; set; }
        public List<CommonLibrary.IRobotPoint> BaseDatasInputData { get; set; }
        public List<BaseName> BaseNamesInputData { get; set; }
        public List<int> BaseNumbersInputData { get; set; }
        public List<IUserBit> UserbitsOut { get; set; }
        public List<IUserBit> UserbitsIn { get; set; }
        public List<IUserBit> TypBits { get; set; }
        public List<IUserBit> JobEnables { get; set; }
        public IDictionary<int, string> LoadVarNames { get; set; }

    }
    public class Program
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartHome { get; set; }

        public Program(string name, string description, string startHome = "0")
        {
            Name = name;
            Description = description;
            StartHome = startHome;
        }
    }


    public class ToolDataKuka : IToolData
    {
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Zpos { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }

        public ToolDataKuka(float xpos, float ypos, float zpos, float a, float b, float c)
        {
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            A = a;
            B = b;
            C = c;
        }

        public ToolDataKuka()
        { }
    }

    public class ToolName
    {
        public string Name { get; set; }

        public ToolName(string name)
        {
            Name = name;
        }
        public ToolName()
        { }
    }

    public class LoadDataKuka : ILoadData
    {
        public float Mass {  get; set; }
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Zpos { get; set; }
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }
        public float JXpos { get; set; }
        public float JYpos { get; set; }
        public float JZpos { get; set; }

        public LoadDataKuka(float mass, float xpos, float ypos, float zpos, float a, float b, float c, float jxpos, float jypos, float jzpos)
        {
            Mass = mass;
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            A = a;
            B = b;
            C = c;
            JXpos = jxpos;
            JYpos = jypos;
            JZpos = jzpos;
        }

        public LoadDataKuka()
        { }
    }

    public interface ILoadData
    {
        float Mass { get; set; }
        float Xpos { get; set; }
        float Ypos { get; set; }
        float Zpos { get; set; }
        float JXpos { get; set; }
        float JYpos { get; set; }
        float JZpos { get; set; }
    }

    public class ToolType
    {
        public string Type;

        public ToolType(string type)
        {
            Type = type;
        }

        public ToolType()
        { }
    }

    public class BaseDataKUKA : CommonLibrary.IRobotPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public BaseDataKUKA(double xpos, double ypos, double zpos, double a, double b, double c)
        {
            X = xpos;
            Y = ypos;
            Z = zpos;
            A = a;
            B = b;
            C = c;
        }

        public BaseDataKUKA()
        { }
    }

    //public interface IBaseData
    //{
    //    float Xpos { get; set; }
    //    float Ypos { get; set; }
    //    float Zpos { get; set; }
    //}

    public class BaseName
    {
        public string Name { get; set; }

        public BaseName(string name)
        {
            Name = name;
        }
        public BaseName()
        { }
    }

    public class BaseType
    {
        public string Type;

        public BaseType(string type)
        {
            Type = type;
        }

        public BaseType()
        { }
    }

    public class HomePos
    {
        public float A1 { get; set; }
        public float A2 { get; set; }
        public float A3 { get; set; }
        public float A4 { get; set; }
        public float A5 { get; set; }
        public float A6 { get; set; }
        public float E1 { get; set; }

        public HomePos(float a1, float a2, float a3, float a4, float a5, float a6, float e1)
        {
            A1 = a1;
            A2 = a2;
            A3 = a3;
            A4 = a4;
            A5 = a5;
            A6 = a6;
            E1 = e1;
        }

        public HomePos()
        { }
    }

    public class HomeNameLong
    {
        public string Name;

        public HomeNameLong(string name)
        {
            Name = name;
        }

        public HomeNameLong()
        { }
    }

    public class HomeNameShort
    {
        public string Name;

        public HomeNameShort(string name)
        {
            Name = name;
        }

        public HomeNameShort()
        { }
    }

    public class CollZone
    {
        public string Number { get; set; }
        public string Description { get; set; }

        public CollZone(string number, string description)
        {
            Number = number;
            Description = description;
        }

        public CollZone()
        { }
    }

    public class UserBitKUKA : IUserBit
    {
        public string Number { get; set; }
        public string NumberInPLC { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public UserBitKUKA(string number, string numberInPLC, string description, string path)
        {
            Number = number;
            NumberInPLC = numberInPLC;
            Description = description;
            Path = path;
        }

        public UserBitKUKA()
        { }

        public UserBitKUKA(UserBitKUKA bit)
        {
            Number = bit.Number;
            NumberInPLC = bit.NumberInPLC;
            Description = bit.Description;
            Path = bit.Path;
        }
    }

    public interface IUserBit
    {
        string Description { get; set; }
        string Path { get; set; }
    }

    public class UserBitABB : IUserBit
    {
        public string Description { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }

        public UserBitABB(string name, string path, string description)
        {
            Name = name;
            Path = path;
            Description = description;
        }
    }

    public class GripperConfigABB : IGripperConfig
    {
        public SortedDictionary<string, string> Inputs { get; set; }
        public SortedDictionary<string, string> Outputs { get; set; }

        public GripperConfigABB()
        {
            Inputs = new SortedDictionary<string, string>();
            Outputs = new SortedDictionary<string, string>();
        }

        public void AddItem(string signalType, string key, string value)
        {
            if (signalType == "Input")
            {
                if (!this.Inputs.Keys.Contains(key))
                    this.Inputs.Add(key, value);
                else
                {
                    MessageBox.Show("Signal " + key + " already exists in gripper config!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            if (signalType == "Output")
            {
                if (!this.Outputs.Keys.Contains(key))
                    this.Outputs.Add(key, value);
                else
                {
                    MessageBox.Show("Signal " + key + " already exists in gripper config!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }

    public interface IGripperConfig
    {

    }

    public interface IToolData
    {
        float Xpos { get; set; }
        float Ypos { get; set; }
        float Zpos { get; set; }
    }

    public class ToolDataABB : IToolData
    {
        public string Name { get; set; }
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Zpos { get; set; }
        public float Q1 { get; set; }
        public float Q2 { get; set; }
        public float Q3 { get; set; }
        public float Q4 { get; set; }
        public string Robhold { get; set; }

        public ToolDataABB(string name, float xpos, float ypos, float zpos, float q1, float q2, float q3, float q4, string robhold)
        {
            Name = name;
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
            Robhold = robhold;
        }
    }

    public class LoadDataABB : ILoadData
    {
        public string Name { get; set; }
        public float Mass { get; set; }
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Zpos { get; set; }
        public float Q1 { get; set; }
        public float Q2 { get; set; }
        public float Q3 { get; set; }
        public float Q4 { get; set; }
        public float JXpos { get; set; }
        public float JYpos { get; set; }
        public float JZpos { get; set; }

        public LoadDataABB(string name, float mass, float xpos, float ypos, float zpos, float q1, float q2, float q3, float q4, float jxpos, float jypos, float jzpos)
        {
            Name = name;
            Mass = mass;
            Xpos = xpos;
            Ypos = ypos;
            Zpos = zpos;
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
            JXpos = jxpos;
            JYpos = jypos;
            JZpos = jzpos;
        }
    }

    public class WobjDataABB : CommonLibrary.IRobotPoint
    {
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Q1_UF { get; set; }
        public double Q2_UF { get; set; }
        public double Q3_UF { get; set; }
        public double Q4_UF { get; set; }
        public double Xpos_OF { get; set; }
        public double Ypos_OF { get; set; }
        public double Zpos_OF { get; set; }
        public double Q1_OF { get; set; }
        public double Q2_OF { get; set; }
        public double Q3_OF { get; set; }
        public double Q4_OF { get; set; }
        public string Robhold { get; set; }

        public WobjDataABB(string name, double xpos, double ypos, double zpos, double q1_uf, double q2_uf, double q3_uf, double q4_uf, double xpos_of, double ypos_of, double zpos_of, double q1_of, double q2_of, double q3_of, double q4_of, string robhold)
        {
            Name = name;
            X = xpos;
            Y = ypos;
            Z = zpos;
            Q1_UF = q1_uf;
            Q2_UF = q2_uf;
            Q3_UF = q3_uf;
            Q4_UF = q4_uf;

            Xpos_OF = xpos_of;
            Ypos_OF = ypos_of;
            Zpos_OF = zpos_of;
            Q1_OF = q1_of;
            Q2_OF = q2_of;
            Q3_OF = q3_of;
            Q4_OF = q4_of;
            Robhold = robhold;
        }
    }

    //public class PointKUKA : CommonLibrary.IRobotPoint
    //{
    //    public double X { get; set; }
    //    public double Y { get; set; }
    //    public double Z { get; set; }
    //    public double A { get; set; }
    //    public double B { get; set; }
    //    public double C { get; set; }

    //    public PointKUKA(double xpos, double ypos, double zpos, double a, double b, double c)
    //    {
    //        X = xpos;
    //        Y = ypos;
    //        Z = zpos;
    //        A = a;
    //        B = b;
    //        C = c;
    //    }

    //    public PointKUKA()
    //    { }
    //}
}


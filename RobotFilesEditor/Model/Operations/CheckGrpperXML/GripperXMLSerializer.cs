using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RobotFilesEditor.Model.Operations
{
    public class GripperXMLSerializer
    {
        [XmlRoot(ElementName = "Num")]
        public class Num
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "Name")]
        public class Name
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_PrSwUsed")]
        public class I_PrSwUsed
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "UseCtrlValve")]
        public class UseCtrlValve
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "PP1")]
        public class PP1 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP2")]
        public class PP2 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP3")]
        public class PP3 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP4")]
        public class PP4 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP5")]
        public class PP5 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP6")]
        public class PP6 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP7")]
        public class PP7 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP8")]
        public class PP8 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP9")]
        public class PP9 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP10")]
        public class PP10 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP11")]
        public class PP11 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP12")]
        public class PP12 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP13")]
        public class PP13 : PPBase
        { 
        }

        [XmlRoot(ElementName = "PP14")] 
        public class PP14 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP15")]
        public class PP15 : PPBase
        {
        }

        [XmlRoot(ElementName = "PP16")]
        public class PP16 : PPBase
        {
        }

        [XmlRoot(ElementName = "Module_1")]
        public class Module_1
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_Address_1")]
        public class I_Address_1
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "O_Address_1")]
        public class O_Address_1
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "InUnit_1")]
        public class InUnit_1
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "OutUnit_1")]
        public class OutUnit_1
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "PNetModules")]
        public class PNetModules
        {
            [XmlElement(ElementName = "Module_1")]
            public Module_1 Module_1 { get; set; }
            [XmlElement(ElementName = "I_Address_1")]
            public I_Address_1 I_Address_1 { get; set; }
            [XmlElement(ElementName = "O_Address_1")]
            public O_Address_1 O_Address_1 { get; set; }
            [XmlElement(ElementName = "InUnit_1")]
            public InUnit_1 InUnit_1 { get; set; }
            [XmlElement(ElementName = "OutUnit_1")]
            public OutUnit_1 OutUnit_1 { get; set; }
        }

        [XmlRoot(ElementName = "IsUsed")]
        public class IsUsed
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "Type")]
        public class Type
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "Check")]
        public class Check
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "C1Used")]
        public class C1Used
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "C2Used")]
        public class C2Used
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "C3Used")]
        public class C3Used
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "C4Used")]
        public class C4Used
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "O_Advanced")]
        public class O_Advanced
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "O_Retracted")]
        public class O_Retracted
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C1Advanced")]
        public class I_C1Advanced
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C1Retracted")]
        public class I_C1Retracted
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C2Advanced")]
        public class I_C2Advanced
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C2Retracted")]
        public class I_C2Retracted
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C3Advanced")]
        public class I_C3Advanced
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C3Retracted")]
        public class I_C3Retracted
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C4Advanced")]
        public class I_C4Advanced
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "I_C4Retracted")]
        public class I_C4Retracted
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "T_ErrWait")]
        public class T_ErrWait
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "T_Advanced")]
        public class T_Advanced
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "T_Retracted")]
        public class T_Retracted
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }

        [XmlRoot(ElementName = "Actuator")]
        public class Actuator
        {
            [XmlElement(ElementName = "Num")]
            public Num Num { get; set; }
            [XmlElement(ElementName = "Name")]
            public Name Name { get; set; }
            [XmlElement(ElementName = "IsUsed")]
            public IsUsed IsUsed { get; set; }
            [XmlElement(ElementName = "Type")]
            public Type Type { get; set; }
            [XmlElement(ElementName = "Check")]
            public Check Check { get; set; }
            [XmlElement(ElementName = "C1Used")]
            public C1Used C1Used { get; set; }
            [XmlElement(ElementName = "C2Used")]
            public C2Used C2Used { get; set; }
            [XmlElement(ElementName = "C3Used")]
            public C3Used C3Used { get; set; }
            [XmlElement(ElementName = "C4Used")]
            public C4Used C4Used { get; set; }
            [XmlElement(ElementName = "O_Advanced")]
            public O_Advanced O_Advanced { get; set; }
            [XmlElement(ElementName = "O_Retracted")]
            public O_Retracted O_Retracted { get; set; }
            [XmlElement(ElementName = "I_C1Advanced")]
            public I_C1Advanced I_C1Advanced { get; set; }
            [XmlElement(ElementName = "I_C1Retracted")]
            public I_C1Retracted I_C1Retracted { get; set; }
            [XmlElement(ElementName = "I_C2Advanced")]
            public I_C2Advanced I_C2Advanced { get; set; }
            [XmlElement(ElementName = "I_C2Retracted")]
            public I_C2Retracted I_C2Retracted { get; set; }
            [XmlElement(ElementName = "I_C3Advanced")]
            public I_C3Advanced I_C3Advanced { get; set; }
            [XmlElement(ElementName = "I_C3Retracted")]
            public I_C3Retracted I_C3Retracted { get; set; }
            [XmlElement(ElementName = "I_C4Advanced")]
            public I_C4Advanced I_C4Advanced { get; set; }
            [XmlElement(ElementName = "I_C4Retracted")]
            public I_C4Retracted I_C4Retracted { get; set; }
            [XmlElement(ElementName = "T_ErrWait")]
            public T_ErrWait T_ErrWait { get; set; }
            [XmlElement(ElementName = "T_Advanced")]
            public T_Advanced T_Advanced { get; set; }
            [XmlElement(ElementName = "T_Retracted")]
            public T_Retracted T_Retracted { get; set; }
        }

        [XmlRoot(ElementName = "Gripper")]
        public class Gripper
        {
            [XmlElement(ElementName = "Num")]
            public Num Num { get; set; }
            [XmlElement(ElementName = "Name")]
            public Name Name { get; set; }
            [XmlElement(ElementName = "I_PrSwUsed")]
            public I_PrSwUsed I_PrSwUsed { get; set; }
            [XmlElement(ElementName = "UseCtrlValve")]
            public UseCtrlValve UseCtrlValve { get; set; }
            [XmlElement(ElementName = "PP1")]
            public PP1 PP1 { get; set; }
            [XmlElement(ElementName = "PP2")]
            public PP2 PP2 { get; set; }
            [XmlElement(ElementName = "PP3")]
            public PP3 PP3 { get; set; }
            [XmlElement(ElementName = "PP4")]
            public PP4 PP4 { get; set; }
            [XmlElement(ElementName = "PP5")]
            public PP5 PP5 { get; set; }
            [XmlElement(ElementName = "PP6")]
            public PP6 PP6 { get; set; }
            [XmlElement(ElementName = "PP7")]
            public PP7 PP7 { get; set; }
            [XmlElement(ElementName = "PP8")]
            public PP8 PP8 { get; set; }
            [XmlElement(ElementName = "PP9")]
            public PP9 PP9 { get; set; }
            [XmlElement(ElementName = "PP10")]
            public PP10 PP10 { get; set; }
            [XmlElement(ElementName = "PP11")]
            public PP11 PP11 { get; set; }
            [XmlElement(ElementName = "PP12")]
            public PP12 PP12 { get; set; }
            [XmlElement(ElementName = "PP13")]
            public PP13 PP13 { get; set; }
            [XmlElement(ElementName = "PP14")]
            public PP14 PP14 { get; set; }
            [XmlElement(ElementName = "PP15")]
            public PP15 PP15 { get; set; }
            [XmlElement(ElementName = "PP16")]
            public PP16 PP16 { get; set; }
            [XmlElement(ElementName = "PNetModules")]
            public PNetModules PNetModules { get; set; }
            [XmlElement(ElementName = "Actuator")]
            public List<Actuator> Actuator { get; set; }


        }
        public class PPBase
        {
            [XmlAttribute(AttributeName = "VarValue")]
            public string VarValue { get; set; }
            [XmlAttribute(AttributeName = "VarSelection")]
            public string VarSelection { get; set; }
            [XmlAttribute(AttributeName = "VarInputType")]
            public string VarInputType { get; set; }
        }
    }
}

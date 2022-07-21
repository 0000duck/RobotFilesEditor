using ParseModuleFile.KUKA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.KUKA
{

    [System.AttributeUsage(AttributeTargets.All)]
    public class CAttrField : System.Attribute
    {
        private EAttrType _type;
        private string label;
        private string endlabel;
        private int width = 100;
        private bool readOnly = false;
        public bool Visible = true;
        public double Min = 0;
        public double Max = 100;
        public bool BoundCheck = false;
        public CAttrField(string label, EAttrType type, bool RO = false)
        {
            this.label = label;
            _type = type;
            this.endlabel = "";
            this.readOnly = RO;
        }
        public CAttrField(string label, string endlabel, EAttrType type, bool RO = false)
        {
            this.label = label;
            _type = type;
            this.endlabel = endlabel;
            this.readOnly = RO;
        }
        public CAttrField(string label, int width, EAttrType type, bool RO = false)
        {
            this.label = label;
            _type = type;
            this.width = width;
            this.endlabel = "";
            this.readOnly = RO;
        }
        public string Label
        {
            get { return label; }
        }
        public string EndLabel
        {
            get { return endlabel; }
        }
        public EAttrType Type
        {
            get { return _type; }
        }
        public bool IsReadOnly
        {
            get { return readOnly; }
        }
        public int Width
        {
            get { return width; }
        }
    }
}

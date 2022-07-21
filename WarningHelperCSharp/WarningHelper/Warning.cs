using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarningHelper
{
    public class Warning : MyNotifyPropertyChanged
    {

	    #region fields
	    private int id;
	    private string file;
	    private string topic;
	    private int line;
	    private WarningType type;
	    private Level level;
	    private string text;
	    private string extraText;

        #endregion // fields

	    #region constructors
	    public Warning(int ID, string File, string Topic, int Line, WarningType Type, Level Level, string Text)
	    {
		    id = ID;
		    file = File;
		    topic = Topic;
		    line = Line;
		    type = Type;
		    level = Level;
		    text = Text;
		    extraText = "";
	    }

	    public Warning(int ID, string File, string Topic, int Line, WarningType Type, Level Level, string Text, string ExtraText)
	    {
		    id = ID;
		    file = File;
		    topic = Topic;
		    line = Line;
		    type = Type;
		    level = Level;
		    text = Text;
		    extraText = ExtraText;
	    }

	    #endregion // constructors

	    #region properties
        public int ID { get { return id; } set { Set(ref id, value); } }
        public string File { get { return file; } set { if (Set(ref file, value)) NotifyPropertyChanged("FileName"); } }
	    public string FileName { get { return Path.GetFileName(file); } }
        public string Topic { get { return topic; } set { Set(ref topic, value); } }
        public int Line { get { return line; } set { Set(ref line, value); } }
        public WarningType Type { get { return type; } set { Set(ref type, value); } }
        public Level Level { get { return level; } set { Set(ref level, value); } }
        public string Text { get { return text; } set { Set(ref text, value); } }
        public string ExtraText { get { return extraText; } set { Set(ref extraText, value); } }
	    #endregion // properties

        #region methods
        public override string ToString()
        {
            return text;
        }
        #endregion

    }
}

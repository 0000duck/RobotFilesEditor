using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WarningHelper
{
    public class Warnings : ObservableCollection<Warning>
    {

        public static int stop_at_warning = 0;
        private string act_file = "";
        private static int counter = 1;
 
        public static void ResetCounter()
        {
            counter = 1;
        }

        public string ActFile
        {
            set { act_file = value; }
        }

        private static string GetMethodName()
        {
            MethodBase x = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod();
            if (x.IsConstructor)
            {
                return x.DeclaringType.Name.ToString() + "()";
            }
            return x.Name;
        }

        private void AddCounter()
        {
#if DEBUG
            if (counter == stop_at_warning)
                System.Diagnostics.Debugger.Break();
#endif
            counter += 1;
        }

        public void Add(int Line, WarningType Type, Level Level, string Text, string ExtraText = "", [CallerMemberName()] string MethodName = null)
        {
            this.Add(new Warning(counter, act_file, MethodName, Line, Type, Level, Text, ExtraText));
            AddCounter();
        }
        /*
        public void Add(int Line, WarningType Type, Level Level, string Text)
        {
            this.Add(new Warning(counter, act_file, GetMethodName(), Line, Type, Level, Text, ExtraText));
            AddCounter();
        }*/

        public new void Clear()
        {
            this.Clear();
            counter = 1;
        }
    }

}

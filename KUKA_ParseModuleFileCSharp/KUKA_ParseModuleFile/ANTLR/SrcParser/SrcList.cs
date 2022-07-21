using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public class SrcList<T> : ObservableCollection<T>, ISrcList<T>, INotifyPropertyChanged 
    {
        #region fields
        private int line;
        private int endLine;
        #endregion fields

        #region properties
        public int Line { get { return line; } set { Set(ref line, value); } }
        public int EndLine { get { return endLine; } set { Set(ref endLine, value); } }
        #endregion properties

        #region constructors
        public SrcList(int Line, int EndLine)
        {
            line = Line;
            endLine = EndLine;
        }
        public SrcList(Antlr4.Runtime.ParserRuleContext context)
        {
            line = context.Start.Line;
            endLine = context.Stop.Line;
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// This method is called by the Set accessor of each property. 
        /// The CallerMemberName attribute that is applied to the optional propertyName 
        /// parameter causes the property name of the caller to be substituted as an argument. 
        /// </summary>
        public virtual void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper method for changing a field. 
        /// If the previous field differs from the new value, 
        /// an event will be raised.
        /// </summary>
        /// <typeparam name="T">Type of the field.</typeparam>
        /// <param name="field">Reference to the field.</param>
        /// <param name="value">New value of the field.</param>
        /// <param name="propertyName">Optional name of the property. If not used: it takes the Caller Member Name.</param>
        /// <returns>True if the field has changed.</returns>
        protected bool Set<U>(ref U field, U value, [CallerMemberName]string propertyName = null)
        {
            //#if DEBUG
            //            if ((DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())))
            //            {
            //                field = value; return true;
            //            }
            //#endif
            if (EqualityComparer<U>.Default.Equals(field, value)) { return false; }
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion methods
    }
}

using CommonLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommonLibrary.Views
{
    /// <summary>
    /// Interaction logic for LogControl.xaml
    /// </summary>
    public partial class LogControl : UserControl
    {

        #region contructor
        public LogControl()
        {
            InitializeComponent();
        }
        #endregion contructor


        #region dependency properties


        public LogCollection LogCollection
        {
            get { return (LogCollection)GetValue(LogCollectionProperty); }
            set { SetValue(LogCollectionProperty, value); }
        }

        public static readonly DependencyProperty LogCollectionProperty =
            DependencyProperty.Register("LogCollection", typeof(LogCollection), typeof(LogControl), new PropertyMetadata(new LogCollection()));

        //public string OkContent
        //{
        //    get { return (string)GetValue(OkContentProperty); }
        //    set { SetValue(OkContentProperty, value); }
        //}

        //public static readonly DependencyProperty OkContentProperty =
        //    DependencyProperty.Register(nameof(OkContent), typeof(object), typeof(LogControl), new PropertyMetadata());

        //public string WarningContent
        //{
        //    get { return (string)GetValue(WarningContentProperty); }
        //    set { SetValue(WarningContentProperty, value); }
        //}

        //public static readonly DependencyProperty WarningContentProperty =
        //    DependencyProperty.Register(nameof(WarningContent), typeof(object), typeof(LogControl), new PropertyMetadata());

        //public string ErrorContent
        //{
        //    get { return (string)GetValue(ErrorContentProperty); }
        //    set { SetValue(ErrorContentProperty, value); }
        //}

        //public static readonly DependencyProperty ErrorContentProperty =
        //    DependencyProperty.Register(nameof(ErrorContent), typeof(object), typeof(LogControl), new PropertyMetadata());

        //public string InfoContent
        //{
        //    get { return (string)GetValue(InfoContentProperty); }
        //    set { SetValue(InfoContentProperty, value); }
        //}

        //public static readonly DependencyProperty InfoContentProperty =
        //    DependencyProperty.Register(nameof(InfoContent), typeof(object), typeof(LogControl), new PropertyMetadata());

        //public ObservableCollection<LogResult> LogContent
        //{
        //    get { return (ObservableCollection<LogResult>)GetValue(LogContentProperty); }
        //    set { SetValue(LogContentProperty, value); }
        //}

        //public static readonly DependencyProperty LogContentProperty =
        //    DependencyProperty.Register(nameof(LogContent), typeof(object), typeof(LogControl), new PropertyMetadata(new ObservableCollection<LogResult>()));

        //public bool OKsFilterChecked
        //{
        //    get { return (bool)GetValue(OKsFilterCheckedProperty); }
        //    set { SetValue(OKsFilterCheckedProperty, value); }
        //}

        //public static readonly DependencyProperty OKsFilterCheckedProperty =
        //    DependencyProperty.Register(nameof(OKsFilterChecked), typeof(object), typeof(LogControl), new PropertyMetadata(true));

        //public bool WarningsFilterChecked
        //{
        //    get { return (bool)GetValue(WarningsFilterCheckedProperty); }
        //    set { SetValue(WarningsFilterCheckedProperty, value); }
        //}

        //public static readonly DependencyProperty WarningsFilterCheckedProperty =
        //    DependencyProperty.Register(nameof(WarningsFilterChecked), typeof(object), typeof(LogControl), new PropertyMetadata(true));

        //public bool ErrorsFilterChecked
        //{
        //    get { return (bool)GetValue(ErrorsFilterCheckedProperty); }
        //    set { SetValue(ErrorsFilterCheckedProperty, value); }
        //}

        //public static readonly DependencyProperty ErrorsFilterCheckedProperty =
        //    DependencyProperty.Register(nameof(ErrorsFilterChecked), typeof(object), typeof(LogControl), new PropertyMetadata(true));

        //public bool InfoFilterChecked
        //{
        //    get { return (bool)GetValue(InfoFilterCheckedProperty); }
        //    set { SetValue(InfoFilterCheckedProperty, value); }
        //}

        //public static readonly DependencyProperty InfoFilterCheckedProperty =
        //    DependencyProperty.Register(nameof(InfoFilterChecked), typeof(object), typeof(LogControl), new PropertyMetadata(true));
        #endregion dependency properties
    }
}

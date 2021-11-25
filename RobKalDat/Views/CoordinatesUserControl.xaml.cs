using RobKalDat.Model.ProjectData;
using System;
using System.Collections.Generic;
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

namespace RobKalDat.Views
{
    /// <summary>
    /// Logika interakcji dla klasy CoordinatesUserControl.xaml
    /// </summary>
    public partial class CoordinatesUserControl : UserControl
    {
        public CoordinatesUserControl()
        {
            InitializeComponent();
            //DataContext = this;
        }

        public Measurement CurrentMeas
        {
            get { return (Measurement)GetValue(CurrentMeasProperty); }
            set { SetValue(CurrentMeasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMeas.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMeasProperty =
            DependencyProperty.Register("CurrentMeas", typeof(Measurement), typeof(CoordinatesUserControl), new PropertyMetadata(new Measurement()));

    }
}

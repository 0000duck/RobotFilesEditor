/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:RobKalDat.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using RobKalDat.Model;

namespace RobKalDat.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MeasurementsViewModel>();
            SimpleIoc.Default.Register<MeasurementWindowViewModel>();
            SimpleIoc.Default.Register<EditMeasurementViewModel>();
            SimpleIoc.Default.Register<BasesViewModel>();
            SimpleIoc.Default.Register<SafetyViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public MeasurementsViewModel MeasurementUserControl
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MeasurementsViewModel>();
            }
        }

        public MeasurementWindowViewModel MeasurementWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MeasurementWindowViewModel>();
            }
        }

        public EditMeasurementViewModel EditMeasurement
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditMeasurementViewModel>();
            }
        }

        public BasesViewModel BasesUserControl
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BasesViewModel>();
            }
        }

        public SafetyViewModel SafetyUserControl
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SafetyViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            //Messenger.Default.Send(true, "KillVMs");
            SimpleIoc.Default.Unregister<SafetyViewModel>();
            SimpleIoc.Default.Unregister<BasesViewModel>();
            SimpleIoc.Default.Unregister<EditMeasurementViewModel>();
            SimpleIoc.Default.Unregister<MeasurementWindowViewModel>();
            SimpleIoc.Default.Unregister<MeasurementsViewModel>();
            SimpleIoc.Default.Unregister<MainViewModel>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MeasurementsViewModel>();
            SimpleIoc.Default.Register<MeasurementWindowViewModel>();
            SimpleIoc.Default.Register<EditMeasurementViewModel>();
            SimpleIoc.Default.Register<BasesViewModel>();
            SimpleIoc.Default.Register<SafetyViewModel>();
        }
    }
}
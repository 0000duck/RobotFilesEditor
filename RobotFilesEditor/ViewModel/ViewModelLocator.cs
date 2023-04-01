/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RobotFilesEditor"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RobotFilesEditor.Model.Operations;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RobotFilesEditor.ViewModel
{   
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {

                //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

                Serializer.FilesSerialization filesSerialization;
                List<Controler> controlers = new List<Controler>();

                try
                {
                    filesSerialization = new Serializer.FilesSerialization();
                    controlers = filesSerialization.GetControlersConfigurations();

                    if (controlers.Count == 0)
                    {
                        throw new NullReferenceException("Configuration not contain any controler");
                    }
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    MessageBoxResult ExeptionMessage = MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                try
                {
                    GlobalData.SetViewProgram();

                    SimpleIoc.Default.Register<List<Controler>>(() => { return controlers; });
                    SimpleIoc.Default.Register<MainViewModel>();
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    MessageBoxResult ExeptionMessage = MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                try
                {
                    SimpleIoc.Default.Register<RobKalDat.ViewModel.MainViewModel>();
                    SimpleIoc.Default.Register<RobKalDat.ViewModel.MeasurementsViewModel>();
                }
                catch (Exception ex)
                {
                    SrcValidator.GetExceptionLine(ex);
                    MessageBoxResult ExeptionMessage = MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public MainViewModel MainVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
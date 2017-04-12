using System;
using System.Windows;
using System.Windows.Data;

namespace MediaManager
{

    public partial class App : Application
    {

        void AppStartup(object sender, StartupEventArgs args)
        {
            WindowPhotoSelect mainWindow = new WindowPhotoSelect();
            mainWindow.Show();

            mainWindow.Photos = (MediaList)(this.Resources["Photos"] as ObjectDataProvider).Data;
            mainWindow.Photos.Path = "..\\..\\photos";

            
            
        }

    }
}
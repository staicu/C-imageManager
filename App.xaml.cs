using System;
using System.Windows;
using System.Windows.Data;

namespace MediaManager
{

    public partial class app : Application
    {

        void AppStartup(object sender, StartupEventArgs args)
        {
            Window1 mainWindow = new Window1();
            mainWindow.Show();

            mainWindow.Photos = (PhotoList)(this.Resources["Photos"] as ObjectDataProvider).Data;
            mainWindow.Photos.Path = "..\\..\\Photos";

            mainWindow.ShoppingCart = (MediaList)(this.Resources["ShoppingCart"] as ObjectDataProvider).Data;
            
        }

    }
}
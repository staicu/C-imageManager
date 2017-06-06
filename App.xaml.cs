using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Data;


namespace MediaManager
{

    public partial class app : Application
    {

        void AppStartup(object sender, StartupEventArgs args)
        {


            
            Window1 mainWindow = new Window1();
            //mainWindow.Show();
            Window2 window2 = new Window2();
            window2.Show();

            mainWindow.Photos = (PhotoList)(this.Resources["Photos"] as ObjectDataProvider).Data;
            mainWindow.Photos.Path = "..\\..\\PhotosSaved";

            mainWindow.ShoppingCart = (MediaList)(this.Resources["ShoppingCart"] as ObjectDataProvider).Data;



        }

    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ScannerManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void app_Startup(object sender, StartupEventArgs e)
        {
            string desinationPath = @"C:\Users\" + Environment.UserName + @"\Pictures\Scanner";
            MainWindow mw;

            if (e.Args.Length>0)
            {
                if(Directory.Exists(e.Args[0]))
                {
                    desinationPath = e.Args[0];
                }
            }else
            {
                if(Directory.Exists(desinationPath)==false)
                {
                    Directory.CreateDirectory(desinationPath);
                }
            }

            mw = new ScannerManager.MainWindow(desinationPath);
            mw.Show();            
        }
    }
}

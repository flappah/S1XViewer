using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK2b66a4aaf5a1438ebb189cf1a4dd11cbTm7IC09nBKxyjpHE55RUoSeH5eC-BofemcyydQFHJnBN-xs4n8T3Vw-ZlSmzxwyM";


        }
    }
}

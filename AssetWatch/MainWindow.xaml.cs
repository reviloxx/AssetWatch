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

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private IApiHandler apiHandler;

        private IApiLoader apiLoader;

        public MainWindow()
        {
            InitializeComponent();
            this.apiHandler = new MultiApiHandler();
            this.apiLoader = new DiskApiLoader();

            apiHandler.OnApiLoaded += ApiHandler_OnApiLoaded;
            apiHandler.OnApiReady += ApiHandler_OnApiReady;
            this.apiHandler.LoadApis(this.apiLoader);
        }

        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            this.apiHandler.EnableApi(api);
        }

        private void ApiHandler_OnApiReady(object sender, OnApiReadyEventArgs e)
        {
            
        }
    }
}

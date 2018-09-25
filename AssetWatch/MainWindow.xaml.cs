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
        private static IApiHandler apiHandler = new MultiApiHandler();        

        private static IFileHandler fileHandler = new StandardFileHandler();

        private ITileHandler tileHandler;

        private AppData appData;

        public MainWindow()
        {
            InitializeComponent();
            this.appData = fileHandler.LoadAppData();                          
            this.tileHandler = new MultiTileHandler(apiHandler,  this.appData);
            this.tileHandler.OnAppDataChanged += this.OnAppDataChanged;
        }
        

        private void MainSettingsWindow_OnGlobalTileColorChanged(object sender, EventArgs e)
        {
            this.tileHandler.RefreshTileStyles();
        }

        private void menuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsWindow mainSettingsWindow = new MainSettingsWindow(apiHandler, this.appData.TileHandlerData.GlobalTileStyle);
            mainSettingsWindow.Closed += this.OnAppDataChanged;
            mainSettingsWindow.OnGlobalTileStyleChanged += this.MainSettingsWindow_OnGlobalTileColorChanged;
            mainSettingsWindow.Show();
        }

        private void menuItem_AddAssetTile_Click(object sender, RoutedEventArgs e)
        {
            this.tileHandler.OpenNewAssetTile();
        }

        private void menuItem_Exit_Click(object sender, RoutedEventArgs e)
        {            
            Environment.Exit(0);
        }

        private void OnAppDataChanged(object sender, EventArgs e)
        {
            fileHandler.SaveAppData(this.appData);
        }
    }
}

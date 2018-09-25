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

        private static IApiLoader apiLoader = new DiskApiLoader();

        private ITileHandler tileHandler;

        private TileStyle globalTileStyle;        

        public MainWindow()
        {
            InitializeComponent();
                        
            this.globalTileStyle = new TileStyle();

            // TODO: load saved data from disk         
            
            apiHandler.LoadApis(apiLoader);
            this.tileHandler = new MultiTileHandler(apiHandler, this.globalTileStyle);
        }

        private void MainSettingsWindow_OnGlobalTileColorChanged(object sender, EventArgs e)
        {
            this.tileHandler.ActivateGlobalTileStyle();
        }

        private void menuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsWindow mainSettingsWindow = new MainSettingsWindow(apiHandler, this.globalTileStyle);
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
    }
}

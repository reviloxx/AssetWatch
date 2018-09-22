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

        private TileStyle globalTileStyle;

        private List<IApi> loadedApis;

        private Dictionary<IApi, List<Asset>> readyApis;

        private List<AssetTile> assetTiles;

        public MainWindow()
        {
            InitializeComponent();
            this.globalTileStyle = new TileStyle();
            this.loadedApis = new List<IApi>();
            this.readyApis = new Dictionary<IApi, List<Asset>>();
            this.assetTiles = new List<AssetTile>();              

            // TODO: load saved data from disk

            apiHandler.OnApiLoaded += ApiHandler_OnApiLoaded;
            apiHandler.OnApiReady += ApiHandler_OnApiReady;
            apiHandler.OnApiError += ApiHandler_OnApiError;
            apiHandler.OnApiDisabled += this.ApiHandler_OnApiDisabled;
            apiHandler.LoadApis(apiLoader);
        }

        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            // TODO: apply loaded save data to API
            this.loadedApis.Add(api);
        }
        private void ApiHandler_OnApiReady(object sender, OnApiReadyEventArgs e)
        {
            this.readyApis.Add(e.Api, e.Assets);         
        }

        private void ApiHandler_OnApiError(object sender, OnApiErrorEventArgs e)
        {
            IApi api = (IApi)sender;
            MessageBox.Show(e.ErrorMessage, api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ApiHandler_OnApiDisabled(object sender, IApi e)
        {
            this.readyApis.Remove(e);
        }           

        private void Asstile_OnAssetUnselected(object sender, EventArgs e)
        {
            apiHandler.UnsubscribeAssetTile((AssetTile)sender);
        }

        private void Asstile_OnAssetSelected(object sender, EventArgs e)
        {
            apiHandler.SubscribeAssetTile((AssetTile)sender);
        }

        private void menuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsWindow sWindow = new MainSettingsWindow(apiHandler, this.loadedApis, this.globalTileStyle);
            sWindow.Show();
        }

        private void menuItem_AddAssetTile_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                AssetTile asstile = new AssetTile(this.readyApis);
                asstile.OnAssetSelected += Asstile_OnAssetSelected;
                asstile.OnAssetUnselected += Asstile_OnAssetUnselected;
                this.assetTiles.Add(asstile);
                asstile.Show();
            });            
        }

        private void menuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

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

        private TileStyle globalTileStyle;

        private List<IApi> loadedApis;

        public MainWindow()
        {
            InitializeComponent();
            this.apiHandler = new MultiApiHandler();
            this.apiLoader = new DiskApiLoader();
            this.globalTileStyle = new TileStyle();
            this.loadedApis = new List<IApi>();

            MainSettingsWindow sWindow = new MainSettingsWindow(this.apiHandler, this.loadedApis, this.globalTileStyle);
            sWindow.Show();

            // TODO: load saved data from disk

            apiHandler.OnApiLoaded += ApiHandler_OnApiLoaded;
            apiHandler.OnApiReady += ApiHandler_OnApiReady;
            this.apiHandler.LoadApis(this.apiLoader);
        }

        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            // TODO: apply loaded save data to API
            this.loadedApis.Add(api);
        }

        private void ApiHandler_OnApiReady(object sender, OnApiReadyEventArgs e)
        {
            // simulate subscribing asset tile
            this.Dispatcher.Invoke(() =>
            {
                Asset ass = new Asset
                {
                    AssetId = e.Assets[0].AssetId,
                    ConvertCurrency = "EUR",
                    Name = "Bitcoin"
                };

                AssetTileData asstiledat = new AssetTileData
                {
                    ApiName = "Coinmarketcap Pro"
                };

                AssetTile asstile = new AssetTile
                {
                    Asset = ass,
                    AssetTileData = asstiledat
                };

                asstile.Show();
                this.apiHandler.SubscribeAssetTile(asstile);
            });
           
        }
    }
}

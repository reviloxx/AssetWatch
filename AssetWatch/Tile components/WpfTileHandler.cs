using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="WpfTileHandler" />
    /// </summary>
    public class WpfTileHandler : ITileHandler
    {
        /// <summary>
        /// Defines the apiHandler
        /// </summary>
        private readonly IApiHandler apiHandler;

        /// <summary>
        /// Defines the apiLoader
        /// </summary>
        private static readonly IApiLoader apiLoader = new DiskApiLoader();

        /// <summary>
        /// Defines the activeAssetTiles
        /// </summary>
        private readonly List<IAssetTile> activeAssetTiles;

        /// <summary>
        /// Defines the activePortfolioTiles
        /// </summary>
        private readonly List<IPortfolioTile> activePortfolioTiles;

        /// <summary>
        /// Contains asset tiles which are waiting for the API to be ready.
        /// </summary>
        private readonly List<IAssetTile> unattachedAssetTiles;

        /// <summary>
        /// Defines the appData
        /// </summary>
        private readonly AppData appData;

        /// <summary>
        /// The random number generator for asset tile ids.
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// Contains 'true' if there is an open portfolio tile settings window.
        /// </summary>
        private bool portfolioTileSettingsWindowActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfTileHandler"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        public WpfTileHandler(IApiHandler apiHandler, AppData appData)
        {
            this.apiHandler = apiHandler;
            this.activeAssetTiles = new List<IAssetTile>();
            this.activePortfolioTiles = new List<IPortfolioTile>();
            this.unattachedAssetTiles = new List<IAssetTile>();
            this.appData = appData;
            this.random = new Random();
            this.portfolioTileSettingsWindowActive = false;

            this.OpenLoadedAssetTiles();
            this.OpenLoadedPortfolioTiles();

            apiLoader.OnApiLoaderError += this.ApiLoader_OnApiLoaderError;
            apiHandler.OnApiLoaded += this.ApiHandler_OnApiLoaded;
            apiHandler.LoadApis(apiLoader);
        }

        private void ApiLoader_OnApiLoaderError(object sender, string e)
        {
            MessageBox.Show(e, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// The RefreshTileStyles is called after the global tile style has changed.
        /// Calls the RefreshTileStyle method of all tiles.
        /// </summary>
        public void RefreshTileStyles()
        {
            this.activeAssetTiles.ForEach(ass => ass.RefreshTileStyle());
            this.activePortfolioTiles.ForEach(port => port.RefreshTileStyle());
        }

        /// <summary>
        /// Locks the position of all tiles.
        /// </summary>
        /// <param name="locked">The locked<see cref="bool"/></param>
        public void LockTilePositions(bool locked)
        {
            this.activeAssetTiles.ForEach(ass => ass.LockPosition(locked));
            this.activePortfolioTiles.ForEach(port => port.LockPosition(locked));
            this.appData.TileHandlerData.PositionsLocked = locked;
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The OpenNewAssetTile opens a new asset tile, subscribes to it's events, 
        /// adds it to the active asset tiles and to the app data.
        /// Finally it calls the FireOnAppDataChanged method.
        /// </summary>
        public void OpenNewAssetTile()
        {
            if (this.portfolioTileSettingsWindowActive)
            {
                return;
            }

            IAssetTile assetTile = new WpfAssetTile(new AssetTileData(this.random), this.appData, this.apiHandler.ReadyApis);
            assetTile.OnTileClosed += this.AssetTile_OnTileClosed;
            assetTile.OnAssetSelected += this.AssetTile_OnAssetSelected;
            assetTile.OnAssetUnselected += this.AssetTile_OnAssetUnselected;
            assetTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
            assetTile.OnAssetTileUpdated += this.AssetTile_OnAssetTileUpdated;
            this.activeAssetTiles.Add(assetTile);
            this.appData.AssetTileDataSet.Add(assetTile.AssetTileData);
            this.FireOnAppDataChanged();
            assetTile.Show();
        }

        /// <summary>
        /// The OpenNewPortfolioTile opens a new portfolio tile, subscribes to it's events, 
        /// adds it to the active portfolio tiles and to the app data.
        /// Finally it calls the FireOnAppDataChanged method.
        /// </summary>
        public void OpenNewPortfolioTile()
        {
            if (this.portfolioTileSettingsWindowActive)
            {
                return;
            }

            IPortfolioTile portfolioTile = new WpfPortfolioTile(new PortfolioTileData(), this.appData);
            portfolioTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
            portfolioTile.OnTileClosed += this.PortfolioTile_OnTileClosed;
            portfolioTile.OnPortfolioSettingsWindowOpened += this.PortfolioTile_OnPortfolioSettingsWindowOpened;
            portfolioTile.OnPortfolioSettingsWindowClosed += this.PortfolioTile_OnPortfolioSettingsWindowClosed;
            this.activePortfolioTiles.Add(portfolioTile);
            this.appData.PortfolioTileDataSet.Add(portfolioTile.PortfolioTileData);
            this.FireOnAppDataChanged();
            portfolioTile.Show();
        }

        /// <summary>
        /// The OpenLoadedAssetTiles opens all asset tiles which were stored in the app data,
        /// subscribes to it's events and adds it to the active asset tiles.
        /// Finally it get's added to the list of unattached asset tiles if there is an API defined in the asset tile data.
        /// </summary>
        private void OpenLoadedAssetTiles()
        {
            this.appData.AssetTileDataSet.ForEach(assetTileData =>
            {
                IAssetTile assetTile = new WpfAssetTile(assetTileData, this.appData, this.apiHandler.ReadyApis);
                assetTile.OnTileClosed += this.AssetTile_OnTileClosed;
                assetTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
                assetTile.OnAssetSelected += this.AssetTile_OnAssetSelected;
                assetTile.OnAssetUnselected += this.AssetTile_OnAssetUnselected;
                assetTile.OnAssetTileUpdated += this.AssetTile_OnAssetTileUpdated;
                this.activeAssetTiles.Add(assetTile);

                if (assetTile.AssetTileData.ApiName != null && assetTile.AssetTileData.ApiName != string.Empty)
                {
                    this.unattachedAssetTiles.Add(assetTile);
                }
                if (!this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    assetTile.Show();
                }
            });
        }

        /// <summary>
        /// The OpenLoadedPortfolioTiles opens all portfolio tiles which were stored in the app data,
        /// subscribes to it's events and adds it to the active portfolio tiles.
        /// </summary>
        private void OpenLoadedPortfolioTiles()
        {
            this.appData.PortfolioTileDataSet.ForEach(portfoliotiledata =>
            {
                WpfPortfolioTile portfolioTile = new WpfPortfolioTile(portfoliotiledata, this.appData);
                portfolioTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
                portfolioTile.OnTileClosed += this.PortfolioTile_OnTileClosed;
                portfolioTile.OnPortfolioSettingsWindowOpened += this.PortfolioTile_OnPortfolioSettingsWindowOpened;
                portfolioTile.OnPortfolioSettingsWindowClosed += this.PortfolioTile_OnPortfolioSettingsWindowClosed;
                this.activePortfolioTiles.Add(portfolioTile);

                if (!this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    portfolioTile.Show();
                }
            });
        }

        /// <summary>
        /// The ApiHandler_OnApiLoaded applies a loaded API data to the loaded API, if a matching API data exists.
        /// Else it adds a new matching API data to the app data.
        /// Subscribes loaded asset tiles which were waiting for this API.
        /// Enables the API and starts the asset updater.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="api">The api<see cref="IApi"/> which was loaded.</param>
        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            List<IAssetTile> toRemove = new List<IAssetTile>();

            // subscribe all asset tiles which were waiting for this API to be loaded
            this.unattachedAssetTiles.ForEach(assToSub =>
            {
                if (assToSub.AssetTileData.ApiName == api.ApiInfo.ApiName)
                {
                    this.apiHandler.AttachAssetTile(assToSub, false);
                    toRemove.Add(assToSub);
                }
            });

            // remove the subscribed asset tiles from the waiting list
            toRemove.ForEach(rm => this.unattachedAssetTiles.Remove(rm));
        }

        /// <summary>
        /// The Asstile_OnAppDataChanged calls the FireOnAppDataChanged method after data to store has changed within the asset tile.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Tile_OnAppDataChanged(object sender, EventArgs e)
        {
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The AssetTile_OnAssetTileUpdated
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnAssetTileUpdated(object sender, EventArgs e)
        {
            WpfAssetTile updatedAssetTile = (WpfAssetTile)sender;
            this.activePortfolioTiles.ForEach(port => port.Update(updatedAssetTile));
        }

        /// <summary>
        /// The Asstile_OnAssetSelected calls the API handler to subscribe the asset tile after a new asset was selected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the asset tile to subscribe.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnAssetSelected(object sender, EventArgs e)
        {
            this.apiHandler.AttachAssetTile((WpfAssetTile)sender, true);
        }

        /// <summary>
        /// The Asstile_OnAssetUnselected calls the API handler to unsubscribe the asset tile after a the current asset was unselected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the asset tile to unsubscribe.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnAssetUnselected(object sender, EventArgs e)
        {
            this.apiHandler.DetachAssetTile((WpfAssetTile)sender);
        }

        /// <summary>
        /// The Asstile_Closed calls the API handler to unsubscribe the asset tile,
        /// removes is from the handled asset tiles and the app data and calls the FireOnAppDataChanged method.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the closed asset tile.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnTileClosed(object sender, EventArgs e)
        {
            IAssetTile closedAssetTile = (WpfAssetTile)sender;

            this.apiHandler.DetachAssetTile(closedAssetTile);
            this.activeAssetTiles.Remove(closedAssetTile);
            this.appData.AssetTileDataSet.Remove(closedAssetTile.AssetTileData);

            this.activePortfolioTiles.ForEach(portt =>
            {
                portt.RemoveAssetTile(closedAssetTile.AssetTileData.AssetTileId);
            });

            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The PortfolioTile_OnPortfolioTileClosed
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void PortfolioTile_OnTileClosed(object sender, EventArgs e)
        {
            IPortfolioTile closedPortfolioTile = (WpfPortfolioTile)sender;
            this.activePortfolioTiles.Remove(closedPortfolioTile);
            this.appData.PortfolioTileDataSet.Remove(closedPortfolioTile.PortfolioTileData);

            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// Blocks the functions to open new tiles while a portfolio settings window is open.
        /// </summary>
        private void PortfolioTile_OnPortfolioSettingsWindowOpened(object sender, EventArgs e)
        {
            this.portfolioTileSettingsWindowActive = true;
        }

        /// <summary>
        /// Allows to open new tiles while after a portfolio settings window was closed.
        /// </summary>
        private void PortfolioTile_OnPortfolioSettingsWindowClosed(object sender, EventArgs e)
        {
            this.portfolioTileSettingsWindowActive = false;
        }

        /// <summary>
        /// The FireOnAppDataChanged fires the OnAppDataChanged event.
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Defines the OnAppDataChanged event.
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

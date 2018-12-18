using System;
using System.Collections.Generic;
using System.Windows;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="MultiTileHandler" />
    /// </summary>
    public class MultiTileHandler : ITileHandler
    {
        /// <summary>
        /// Defines the apiHandler.
        /// </summary>
        private IApiHandler apiHandler;

        /// <summary>
        /// Defines the apiLoader.
        /// </summary>
        private static IApiLoader apiLoader = new DiskApiLoader();

        /// <summary>
        /// Defines the handledAssetTiles.
        /// </summary>
        private List<AssetTile> activeAssetTiles;

        private List<PortfolioTile> activePortfolioTiles;

        /// <summary>
        /// Contains asset tiles which are waiting for the API to be ready.
        /// </summary>
        private List<AssetTile> unattachedAssetTiles;

        /// <summary>
        /// Defines the appData.
        /// </summary>
        private AppData appData;

        /// <summary>
        /// The random number generator for asset tile ids.
        /// </summary>
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTileHandler"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        public MultiTileHandler(IApiHandler apiHandler, AppData appData)
        {
            this.apiHandler = apiHandler;
            this.activeAssetTiles = new List<AssetTile>();
            this.activePortfolioTiles = new List<PortfolioTile>();
            this.unattachedAssetTiles = new List<AssetTile>();
            this.appData = appData;
            this.random = new Random();

            this.OpenLoadedAssetTiles();
            this.OpenLoadedPortfolioTiles();

            apiHandler.OnApiLoaded += this.ApiHandler_OnApiLoaded;
            apiHandler.LoadApis(apiLoader);
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
        /// The OpenNewAssetTile opens a new asset tile, subscribes to it's events, 
        /// adds it to the handled asset tiles and to the app data and calls the FireOnAppDataChanged method.
        /// </summary>
        public void OpenNewAssetTile()
        {
            AssetTile assetTile = new AssetTile(new AssetTileData(this.random), this.appData, this.apiHandler.ReadyApis);
            assetTile.OnAssetTileClosed += this.AssetTile_OnAssetTileClosed;
            assetTile.OnAssetSelected += this.AssetTile_OnAssetSelected;
            assetTile.OnAssetUnselected += this.AssetTile_OnAssetUnselected;
            assetTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
            assetTile.OnAssetTileUpdated += this.AssetTile_OnAssetTileUpdated;
            this.activeAssetTiles.Add(assetTile);
            this.appData.AssetTileDataSet.Add(assetTile.AssetTileData);
            this.FireOnAppDataChanged();
            assetTile.Show();
        }        

        public void OpenNewPortfolioTile()
        {
            PortfolioTile portfolioTile = new PortfolioTile(new PortfolioTileData(), this.appData);
            portfolioTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
            portfolioTile.OnPortfolioTileClosed += this.PortfolioTile_OnPortfolioTileClosed;
            this.activePortfolioTiles.Add(portfolioTile);
            this.appData.PortfolioTileDataSet.Add(portfolioTile.PortfolioTileData);
            this.FireOnAppDataChanged();
            portfolioTile.Show();
        }        

        /// <summary>
        /// The OpenLoadedAssetTiles opens all asset tiles which were stored in the app data,
        /// subscribes to it's events, adds it to the handled asset tiles and to the asset tiles to subscribe
        /// if there is an API defined in the asset tile data.
        /// </summary>
        private void OpenLoadedAssetTiles()
        {
            this.appData.AssetTileDataSet.ForEach(assetTileData =>
            {
                AssetTile assetTile = new AssetTile(assetTileData, this.appData, this.apiHandler.ReadyApis);
                assetTile.OnAssetTileClosed += this.AssetTile_OnAssetTileClosed;
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

        private void OpenLoadedPortfolioTiles()
        {
            this.appData.PortfolioTileDataSet.ForEach(portfoliotiledata =>
            {
                PortfolioTile portfolioTile = new PortfolioTile(portfoliotiledata, this.appData);
                portfolioTile.OnAppDataChanged += this.Tile_OnAppDataChanged;
                portfolioTile.OnPortfolioTileClosed += this.PortfolioTile_OnPortfolioTileClosed;
                this.activePortfolioTiles.Add(portfolioTile);

                if (!this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    portfolioTile.Show();
                }
            });
        }

        public void LockTilePositions(bool locked)
        {
            this.activeAssetTiles.ForEach(ass => ass.LockPosition(locked));
            this.activePortfolioTiles.ForEach(port => port.LockPosition(locked));
            this.appData.TileHandlerData.PositionsLocked = locked;
            this.FireOnAppDataChanged();
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
            // Apply loaded save data to API
            if (this.appData.ApiDataSet.Exists(apiData => apiData.ApiName == api.ApiInfo.ApiName))
            {
                ApiData apiData = this.appData.ApiDataSet.Find(a => a.ApiName == api.ApiInfo.ApiName);
                apiData.IncreaseCounter(0);
                api.ApiData = apiData;
            }
            else
            {
                this.appData.ApiDataSet.Add(api.ApiData);
                this.FireOnAppDataChanged();
            }

            List<AssetTile> toRemove = new List<AssetTile>();

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

            if (api.ApiData.IsEnabled)
            {
                this.apiHandler.EnableApi(api);
            }
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

        private void AssetTile_OnAssetTileUpdated(object sender, EventArgs e)
        {
            AssetTile updatedAssetTile = (AssetTile)sender;
            this.activePortfolioTiles.ForEach(port => port.Update(updatedAssetTile));
        }

        /// <summary>
        /// The Asstile_OnAssetSelected calls the API handler to subscribe the asset tile after a new asset was selected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the asset tile to subscribe.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnAssetSelected(object sender, EventArgs e)
        {
            this.apiHandler.AttachAssetTile((AssetTile)sender, true);
        }

        /// <summary>
        /// The Asstile_OnAssetUnselected calls the API handler to unsubscribe the asset tile after a the current asset was unselected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the asset tile to unsubscribe.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnAssetUnselected(object sender, EventArgs e)
        {
            this.apiHandler.DetachAssetTile((AssetTile)sender);
        }

        /// <summary>
        /// The Asstile_Closed calls the API handler to unsubscribe the asset tile,
        /// removes is from the handled asset tiles and the app data and calls the FireOnAppDataChanged method.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the closed asset tile.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void AssetTile_OnAssetTileClosed(object sender, EventArgs e)
        {
            AssetTile closedAssetTile = (AssetTile)sender;

            this.apiHandler.DetachAssetTile(closedAssetTile);
            this.activeAssetTiles.Remove(closedAssetTile);
            this.appData.AssetTileDataSet.Remove(closedAssetTile.AssetTileData);

            // remove this asset tile from all portfolio tiles
            this.appData.PortfolioTileDataSet.ForEach(port =>
            {
                port.AssignedAssetTileIds.Remove(closedAssetTile.AssetTileData.AssetTileId);
            });

            this.activePortfolioTiles.ForEach(portt =>
            {
                portt.UpdateTextBlocks(null);                
                portt.RefreshTileStyle();
            });

            this.FireOnAppDataChanged();
        }

        private void PortfolioTile_OnPortfolioTileClosed(object sender, EventArgs e)
        {
            PortfolioTile closedPortfolioTile = (PortfolioTile)sender;
            this.activePortfolioTiles.Remove(closedPortfolioTile);
            this.appData.PortfolioTileDataSet.Remove(closedPortfolioTile.PortfolioTileData);

            this.FireOnAppDataChanged();
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

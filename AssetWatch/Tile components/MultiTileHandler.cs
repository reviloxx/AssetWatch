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
        private List<AssetTile> handledAssetTiles;

        private List<PortfolioTile> handledPortfolioTiles;

        /// <summary>
        /// Defines the assetTilesToSubscribe.
        /// </summary>
        private List<AssetTile> assetTilesToSubscribe;

        /// <summary>
        /// Defines the appData.
        /// </summary>
        private AppData appData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTileHandler"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        public MultiTileHandler(IApiHandler apiHandler, AppData appData)
        {
            this.apiHandler = apiHandler;
            this.handledAssetTiles = new List<AssetTile>();
            this.handledPortfolioTiles = new List<PortfolioTile>();
            this.assetTilesToSubscribe = new List<AssetTile>();
            this.appData = appData;

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
            this.handledAssetTiles.ForEach(ass => ass.RefreshTileStyle());
            this.handledPortfolioTiles.ForEach(port => port.RefreshTileStyle());
        }

        /// <summary>
        /// The OpenNewAssetTile opens a new asset tile, subscribes to it's events, 
        /// adds it to the handled asset tiles and to the app data and calls the FireOnAppDataChanged method.
        /// </summary>
        public void OpenNewAssetTile()
        {
            AssetTile asstile = new AssetTile(new AssetTileData(), this.appData, this.apiHandler.ReadyApis);
            asstile.OnAssetTileClosed += this.Asstile_Closed;
            asstile.OnAssetSelected += this.Asstile_OnAssetSelected;
            asstile.OnAssetUnselected += this.Asstile_OnAssetUnselected;
            asstile.OnAppDataChanged += this.Tile_OnAppDataChanged;
            this.handledAssetTiles.Add(asstile);
            this.appData.AssetTileDataSet.Add(asstile.AssetTileData);
            this.FireOnAppDataChanged();
            asstile.Show();
        }        

        public void OpenNewPortfolioTile()
        {
            // TODO: Baustelle
            PortfolioTile portfoliotile = new PortfolioTile(new PortfolioTileData(), this.appData);
            portfoliotile.OnAppDataChanged += this.Tile_OnAppDataChanged;
            this.handledPortfolioTiles.Add(portfoliotile);
            this.appData.PortfolioTileDataSet.Add(portfoliotile.PortfolioTileData);
            this.apiHandler.SubscribePortfolioTile(portfoliotile);
            this.FireOnAppDataChanged();
            portfoliotile.Show();
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
                AssetTile asstile = new AssetTile(assetTileData, this.appData, this.apiHandler.ReadyApis);
                asstile.OnAssetTileClosed += this.Asstile_Closed;
                asstile.OnAppDataChanged += this.Tile_OnAppDataChanged;
                asstile.OnAssetSelected += this.Asstile_OnAssetSelected;
                asstile.OnAssetUnselected += this.Asstile_OnAssetUnselected;
                this.handledAssetTiles.Add(asstile);

                if (asstile.AssetTileData.ApiName != null && asstile.AssetTileData.ApiName != string.Empty)
                {
                    this.assetTilesToSubscribe.Add(asstile);
                }
                
                if (!this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    asstile.Show();
                }                
            });
        }        

        private void OpenLoadedPortfolioTiles()
        {
            this.appData.PortfolioTileDataSet.ForEach(portfoliotiledata =>
            {
                PortfolioTile portfoliotile = new PortfolioTile(portfoliotiledata, this.appData);
                portfoliotile.OnAppDataChanged += this.Tile_OnAppDataChanged;
                this.handledPortfolioTiles.Add(portfoliotile);
                this.apiHandler.SubscribePortfolioTile(portfoliotile);

                if (!this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    portfoliotile.Show();
                }
            });
        }

        public void LockTilePositions(bool locked)
        {
            this.handledAssetTiles.ForEach(ass => ass.LockPosition(locked));
            this.handledPortfolioTiles.ForEach(port => port.LockPosition(locked));
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

                // reset call counter every month
                if ((DateTime.Now - apiData.CallCountStartTime).Days + 1 > 30)
                {
                    apiData.CallCountStartTime = DateTime.Now;
                    apiData.CallCount = 0;
                }

                api.ApiData = apiData;
            }
            else
            {
                this.appData.ApiDataSet.Add(api.ApiData);
                this.FireOnAppDataChanged();
            }

            List<AssetTile> toRemove = new List<AssetTile>();

            this.assetTilesToSubscribe.ForEach(assToSub =>
            {
                if (assToSub.AssetTileData.ApiName == api.ApiInfo.ApiName)
                {
                    this.apiHandler.SubscribeAssetTile(assToSub);
                    toRemove.Add(assToSub);
                }
            });

            toRemove.ForEach(rm => this.assetTilesToSubscribe.Remove(rm));

            if (api.ApiData.IsEnabled)
            {
                this.apiHandler.EnableApi(api);
                this.apiHandler.StartAssetUpdater(api);
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

        /// <summary>
        /// The Asstile_OnAssetSelected calls the API handler to subscribe the asset tile after a new asset was selected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the asset tile to subscribe.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Asstile_OnAssetSelected(object sender, EventArgs e)
        {
            this.apiHandler.SubscribeAssetTile((AssetTile)sender);
        }

        /// <summary>
        /// The Asstile_OnAssetUnselected calls the API handler to unsubscribe the asset tile after a the current asset was unselected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the asset tile to unsubscribe.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Asstile_OnAssetUnselected(object sender, EventArgs e)
        {
            this.apiHandler.UnsubscribeAssetTile((AssetTile)sender);
        }

        /// <summary>
        /// The Asstile_Closed calls the API handler to unsubscribe the asset tile,
        /// removes is from the handled asset tiles and the app data and calls the FireOnAppDataChanged method.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the closed asset tile.</param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Asstile_Closed(object sender, EventArgs e)
        {
            AssetTile closedAssetTile = (AssetTile)sender;

            this.apiHandler.UnsubscribeAssetTile(closedAssetTile);
            this.handledAssetTiles.Remove(closedAssetTile);
            this.appData.AssetTileDataSet.Remove(closedAssetTile.AssetTileData);

            // remove this asset tile from all portfolio tiles
            this.appData.PortfolioTileDataSet.ForEach(port =>
            {
                port.AssignedAssetTilesDataSet.Remove(closedAssetTile.AssetTileData);
            });

            this.handledPortfolioTiles.ForEach(portt =>
            {
                portt.UpdateTextBlocks(DateTime.Now);
                portt.RefreshTileStyle();
            });

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

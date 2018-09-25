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
        /// Defines the apiHandler
        /// </summary>
        private IApiHandler apiHandler;

        private static IApiLoader apiLoader = new DiskApiLoader();

        /// <summary>
        /// Defines the handledAssetTiles
        /// </summary>
        private List<AssetTile> handledAssetTiles;

        private List<AssetTile> assetTilesToSubscribe;

        private AppData appData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTileHandler"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="globalTileStyle">The globalTileStyle<see cref="TileStyle"/></param>
        /// <param name="tileHandlerData">The tileHandlerData<see cref="TileHandlerData"/></param>
        public MultiTileHandler(IApiHandler apiHandler, AppData appData)
        {
            this.apiHandler = apiHandler;
            this.handledAssetTiles = new List<AssetTile>();
            this.assetTilesToSubscribe = new List<AssetTile>();
            this.appData = appData;

            this.OpenLoadedAssetTiles();

            apiHandler.OnApiLoaded += this.ApiHandler_OnApiLoaded;
            apiHandler.OnApiError += this.ApiHandler_OnApiError;
            apiHandler.LoadApis(apiLoader);
        }

        /// <summary>
        /// The OpenNewAssetTile
        /// </summary>
        public void OpenNewAssetTile()
        {
            AssetTile asstile = new AssetTile(this.apiHandler.ReadyApis, new AssetTileData(), this.appData.TileHandlerData.GlobalTileStyle);
            asstile.Closed += this.Asstile_Closed;
            asstile.OnAssetSelected += this.Asstile_OnAssetSelected;
            asstile.OnAssetUnselected += this.Asstile_OnAssetUnselected;
            asstile.OnAppDataChanged += this.Asstile_OnAppDataChanged;
            this.handledAssetTiles.Add(asstile);
            this.appData.AssetTileDataSet.Add(asstile.AssetTileData);
            this.FireOnAppDataChanged();
            asstile.Show();
        }

        private void Asstile_Closed(object sender, EventArgs e)
        {
            AssetTile closedAssetTile = (AssetTile)sender;

            this.apiHandler.UnsubscribeAssetTile(closedAssetTile);
            this.handledAssetTiles.Remove(closedAssetTile);
            this.appData.AssetTileDataSet.Remove(closedAssetTile.AssetTileData);

            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The ActivateGlobalTileStyle
        /// </summary>
        public void RefreshTileStyles()
        {
            this.handledAssetTiles.ForEach(ass =>
            {
                ass.RefreshTileStyle();
            });
        }

        private void OpenLoadedAssetTiles()
        {
            this.appData.AssetTileDataSet.ForEach(assetTileData =>
            {
                AssetTile asstile = new AssetTile(this.apiHandler.ReadyApis, assetTileData, this.appData.TileHandlerData.GlobalTileStyle);
                asstile.Closed += this.Asstile_Closed;
                asstile.OnAppDataChanged += this.Asstile_OnAppDataChanged1;
                asstile.OnAssetSelected += this.Asstile_OnAssetSelected;
                asstile.OnAssetUnselected += this.Asstile_OnAssetUnselected;
                this.handledAssetTiles.Add(asstile);
                this.assetTilesToSubscribe.Add(asstile);
                asstile.Show();
            });
        }

        private void Asstile_OnAppDataChanged1(object sender, EventArgs e)
        {
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The ApiHandler_OnApiLoaded
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="api">The api<see cref="IApi"/></param>
        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            // Apply loaded save data to API
            if (this.appData.ApiDataSet.Exists(apiData => apiData.ApiName == api.ApiData.ApiName))
            {
                api.ApiData = this.appData.ApiDataSet.Find(apiData => apiData.ApiName == api.ApiData.ApiName);                
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
        /// The ApiHandler_OnApiError
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="OnApiErrorEventArgs"/></param>
        private void ApiHandler_OnApiError(object sender, OnApiErrorEventArgs e)
        {
            IApi api = (IApi)sender;
            MessageBox.Show(e.ErrorMessage, api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Asstile_OnAppDataChanged(object sender, EventArgs e)
        {
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The Asstile_OnAssetSelected
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Asstile_OnAssetSelected(object sender, EventArgs e)
        {
            this.apiHandler.SubscribeAssetTile((AssetTile)sender);
        }

        /// <summary>
        /// The Asstile_OnAssetUnselected
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Asstile_OnAssetUnselected(object sender, EventArgs e)
        {
            this.apiHandler.UnsubscribeAssetTile((AssetTile)sender);
        }

        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        public event EventHandler OnAppDataChanged;
    }
}

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

        /// <summary>
        /// Defines the globalTileStyle
        /// </summary>
        private TileStyle globalTileStyle;

        /// <summary>
        /// Defines the handledAssetTiles
        /// </summary>
        private List<AssetTile> handledAssetTiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTileHandler"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="globalTileStyle">The globalTileStyle<see cref="TileStyle"/></param>
        public MultiTileHandler(IApiHandler apiHandler, TileStyle globalTileStyle)
        {
            this.apiHandler = apiHandler;
            this.globalTileStyle = globalTileStyle;
            this.handledAssetTiles = new List<AssetTile>();
            this.TileHandlerData = new TileHandlerData();

            apiHandler.OnApiLoaded += this.ApiHandler_OnApiLoaded;
            apiHandler.OnApiError += this.ApiHandler_OnApiError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTileHandler"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="globalTileStyle">The globalTileStyle<see cref="TileStyle"/></param>
        /// <param name="tileHandlerData">The tileHandlerData<see cref="TileHandlerData"/></param>
        public MultiTileHandler(IApiHandler apiHandler, TileStyle globalTileStyle, TileHandlerData tileHandlerData)
        {
            this.apiHandler = apiHandler;
            this.globalTileStyle = globalTileStyle;
            this.handledAssetTiles = new List<AssetTile>();
            this.TileHandlerData = tileHandlerData;

            apiHandler.OnApiLoaded += this.ApiHandler_OnApiLoaded;
            apiHandler.OnApiError += this.ApiHandler_OnApiError;
        }

        /// <summary>
        /// The OpenNewAssetTile
        /// </summary>
        public void OpenNewAssetTile()
        {
            AssetTile asstile = new AssetTile(this.apiHandler.ReadyApis, this.globalTileStyle);
            asstile.OnAssetSelected += this.Asstile_OnAssetSelected;
            asstile.OnAssetUnselected += this.Asstile_OnAssetUnselected;
            this.handledAssetTiles.Add(asstile);
            asstile.Show();
        }

        /// <summary>
        /// The ActivateGlobalTileStyle
        /// </summary>
        public void ActivateGlobalTileStyle()
        {
            this.handledAssetTiles.ForEach(ass =>
            {
                ass.RefreshTileStyle();
            });
        }

        /// <summary>
        /// The ApiHandler_OnApiLoaded
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="api">The api<see cref="IApi"/></param>
        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            // TODO: apply loaded save data to API

            this.apiHandler.StartAssetUpdater(api);
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

        /// <summary>
        /// Gets or sets the TileHandlerData
        /// </summary>
        public TileHandlerData TileHandlerData { get; set; }
    }
}

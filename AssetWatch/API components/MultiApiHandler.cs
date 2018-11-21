using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="MultiApiHandler" />
    /// </summary>
    public class MultiApiHandler : IApiHandler
    {
        /// <summary>
        /// Contains the currently subscribed asset tiles.
        /// </summary>
        private List<AssetTile> subscribedAssetTiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiApiHandler"/> class.
        /// </summary>
        public MultiApiHandler()
        {
            this.subscribedAssetTiles = new List<AssetTile>();
            this.ReadyApis = new Dictionary<IApi, List<Asset>>();
        }

        /// <summary>
        /// Loads all available APIs by using an IApiLoader, sobscribes to it's events and requests it's available assets.
        /// </summary>
        /// <param name="apiLoader">The apiLoader<see cref="IApiLoader"/></param>
        public void LoadApis(IApiLoader apiLoader)
        {
            // look for valid API librarys, load them from the disk and remove duplicates
            this.LoadedApis = apiLoader.GetApis()
                .GroupBy(api => api.ApiInfo.ApiName)
                .Select(g => g.First())
                .ToList();

            this.LoadedApis.ForEach(api =>
            {
                api.OnAvailableAssetsReceived += this.Api_OnAvailableAssetsReceived;
                api.OnSingleAssetUpdated += this.Api_OnSingleAssetUpdated;
                api.OnApiError += this.Api_OnApiError;
                api.OnAppDataChanged += this.Api_OnAppDataChanged;
                this.FireOnApiLoaded(api);
            });
        }

        /// <summary>
        /// Subscribes an asset tile to the API handler and it's asset to the right API.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to subscribe in the dictionary
            IApi api = this.LoadedApis.FirstOrDefault(a => a.ApiInfo.ApiName == assetTile.AssetTileData.ApiName);
            api.SubscribeAssetToUpdater(assetTile.AssetTileData.Asset);
            api.RequestSingleAssetUpdateAsync(assetTile.AssetTileData.Asset);
            this.subscribedAssetTiles.Add(assetTile);
        }

        /// <summary>
        /// Unsubscribes an asset tile from the API handler.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to unsubscribe</param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            this.subscribedAssetTiles.Remove(assetTile);
            IApi api = this.LoadedApis.FirstOrDefault(a => a.ApiInfo.ApiName == assetTile.AssetTileData.ApiName);

            if (!this.subscribedAssetTiles.Exists(sub => sub.AssetTileData.ApiName == api.ApiInfo.ApiName &&
                                                         sub.AssetTileData.Asset.AssetId == assetTile.AssetTileData.Asset.AssetId &&
                                                         sub.AssetTileData.Asset.ConvertCurrency == assetTile.AssetTileData.Asset.ConvertCurrency))
            {
                // unsunscribe asset from API if it is not needed anymore
                api.UnsubscribeAssetFromUpdater(assetTile.AssetTileData.Asset);
            }
        }        

        /// <summary>
        /// Enables the API and requests it's available assets if neccessary.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/></param>
        public void EnableApi(IApi api)
        {
            api.Enable();

            // if this API has not received it's available assets yet, request it
            if (!this.ReadyApis.Any(k => k.Key.ApiInfo.ApiName == api.ApiInfo.ApiName))
            {
                api.RequestAvailableAssetsAsync();
            }
            else
            {
                // should never happen
                throw new Exception();
            }
        }

        /// <summary>
        /// Disables the API.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/></param>
        public void DisableApi(IApi api)
        {
            if (this.ReadyApis.Any(k => k.Key.ApiInfo.ApiName == api.ApiInfo.ApiName))
            {
                this.ReadyApis.Remove(api);
            }

            api.Disable();
        }

        /// <summary>
        /// Starts the asset updater of an API.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/> to start the asset updater.</param>
        public void StartAssetUpdater(IApi api)
        {
            api.StartAssetUpdater();
        }

        /// <summary>
        /// Is called after an API has received it's available assets.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API which has received it's available assets.</param>
        /// <param name="availableAssets">The availableAssets<see cref="List{AssetInfo}"/></param>
        private void Api_OnAvailableAssetsReceived(object sender, List<Asset> availableAssets)
        {
            // check which API sent it's available assets and put them in the dictionary
            IApi api = (IApi)sender;
            this.ReadyApis.Add(api, availableAssets);
        }

        /// <summary>
        /// Is called if something went wrong with the API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API where the error occured.</param>
        /// <param name="e">The e<see cref="EventArgs"/> contain an error type and an error message.</param>
        private void Api_OnApiError(object sender, OnApiErrorEventArgs e)
        {
            IApi api = (IApi)sender;

            if (e.ErrorType == ErrorType.Unauthorized || e.ErrorType == ErrorType.BadRequest)
            {
                this.DisableApi(api);
            }

            MessageBox.Show(e.ErrorMessage, api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Is called after an asset has received an update.
        /// Updates all asset tiles which are waiting for updates from this API, for this asset.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API which has received an asset update.</param>
        /// <param name="updatedAsset">The updatedAsset<see cref="Asset"/> contains the updated asset.</param>
        private void Api_OnSingleAssetUpdated(object sender, Asset updatedAsset)
        {
            IApi api = (IApi)sender;
            List<AssetTile> toNotify = this.subscribedAssetTiles.FindAll(at => at.AssetTileData.ApiName == api.ApiInfo.ApiName &&
                                                                                at.AssetTileData.Asset.AssetId == updatedAsset.AssetId &&
                                                                                at.AssetTileData.Asset.ConvertCurrency == updatedAsset.ConvertCurrency);

            toNotify.ForEach(a => a.Update(updatedAsset));            
        }

        /// <summary>
        /// Is called after an API has changed the call counter in the AppData.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Api_OnAppDataChanged(object sender, EventArgs e)
        {
            try
            {
                this.FireOnAppDataChanged();
            }
            catch
            {
                // save file might be in use by another process
            }
        }

        /// <summary>
        /// Fires the OnApiLoaded event.
        /// </summary>
        /// <param name="loadedApi">The api<see cref="IApi"/> which was loaded.</param>
        private void FireOnApiLoaded(IApi loadedApi)
        {
            OnApiLoaded?.Invoke(this, loadedApi);
        }

        /// <summary>
        /// Fires the OnAppDataChanged event.
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Gets the LoadedApis
        /// </summary>
        public List<IApi> LoadedApis { get; private set; }

        /// <summary>
        /// Gets the ReadyApis
        /// </summary>
        public Dictionary<IApi, List<Asset>> ReadyApis { get; }

        /// <summary>
        /// Is fired after an assembly which contains an IApi object was loaded.
        /// The event args contain the loaded API.
        /// </summary>
        public event EventHandler<IApi> OnApiLoaded;

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

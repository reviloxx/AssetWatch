using System;
using System.Collections.Generic;
using System.Linq;

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
                // TODO: load API data from disk and assign it to this API
                api.OnAvailableAssetsReceived += this.Api_OnAvailableAssetsReceived;
                api.OnApiError += this.Api_OnApiError;
                this.FireOnApiLoaded(api);
            });
        }

        /// <summary>
        /// Subscribes an asset tile to the api handler and it's asset to the right API.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to subscribe in the dictionary
            IApi api = this.ReadyApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.ApiName).Key;
            api.SubscribeAssetToUpdater(assetTile.AssetTileData.Asset);
            this.subscribedAssetTiles.Add(assetTile);
        }

        /// <summary>
        /// Unsubscribes an asset tile from the api handler.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to unsubscribe</param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            this.subscribedAssetTiles.Remove(assetTile);
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
                throw new Exception("should not happen");
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

            this.FireOnApiDisabled(api);

            api.Disable();
        }

        public void StartAssetUpdater(IApi api)
        {
            api.StartAssetUpdater();
        }

        /// <summary>
        /// The SetUpdateInterval sets a new update interval for updating all subscribed assets of an API.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/> to set the update interval.</param>
        /// <param name="seconds">The seconds<see cref="int"/> defines the new update interval.</param>
        public void SetUpdateInterval(IApi api, int seconds)
        {
            api.SetUpdateInterval(seconds);
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
            api.OnSingleAssetUpdated += this.Api_OnSingleAssetUpdated;
            this.ReadyApis.Add(api, availableAssets);

            this.FireOnApiReady(new OnApiReadyEventArgs { Api = api, Assets = availableAssets });
        }

        /// <summary>
        /// Is called if something went wrong with the API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API where the error occured.</param>
        /// <param name="e">The e<see cref="EventArgs"/> contain an error type and an error message.</param>
        private void Api_OnApiError(object sender, OnApiErrorEventArgs e)
        {
            this.FireOnApiError(sender, e);
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

            toNotify.ForEach(a => a.UpdateAsset(this, updatedAsset));
        }

        /// <summary>
        /// Fires the OnApiReady event.
        /// </summary>
        /// <param name="eventArgs">The eventArgs<see cref="OnApiReadyEventArgs"/> contain the API which is ready and it's available assets.</param>
        private void FireOnApiReady(OnApiReadyEventArgs eventArgs)
        {
            OnApiReady?.Invoke(this, eventArgs);
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
        /// Fires the OnApiError event.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API where the error occured.</param>
        /// <param name="e">The e<see cref="OnApiErrorEventArgs"/> contain an error type and an error message.</param>
        private void FireOnApiError(object sender, OnApiErrorEventArgs e)
        {
            OnApiError?.Invoke(sender, e);
        }

        /// <summary>
        /// Fires the OnApiDisabled event.
        /// </summary>
        /// <param name="disabledApi">The disabledApi<see cref="IApi"/></param>
        private void FireOnApiDisabled(IApi disabledApi)
        {
            this.OnApiDisabled?.Invoke(this, disabledApi);
        }

        public List<IApi> LoadedApis { get; private set; }

        public Dictionary<IApi, List<Asset>> ReadyApis { get; }

        /// <summary>
        /// Is fired when a handled API is ready to use.
        /// The event args contain the API which is ready and it's available assets.
        /// </summary>
        public event EventHandler<OnApiReadyEventArgs> OnApiReady;

        /// <summary>
        /// Is fired after an assembly which contains an IApi object was loaded.
        /// The event args contain the loaded API.
        /// </summary>
        public event EventHandler<IApi> OnApiLoaded;

        /// <summary>
        /// Is fired when any error occurs within the IApi.
        /// The event args contain the error type and a error message.
        /// </summary>
        public event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Is fired after an API was disabled.
        /// The event args contain the disabled API.
        /// </summary>
        public event EventHandler<IApi> OnApiDisabled;
    }
}

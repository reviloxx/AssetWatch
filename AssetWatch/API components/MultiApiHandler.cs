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

        private List<PortfolioTile> subscribedPortfolioTiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiApiHandler"/> class.
        /// </summary>
        public MultiApiHandler()
        {
            this.subscribedAssetTiles = new List<AssetTile>();
            this.subscribedPortfolioTiles = new List<PortfolioTile>();
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
        /// Subscribes an asset tile to the api handler and it's asset to the right API.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to subscribe in the dictionary
            IApi api = this.LoadedApis.FirstOrDefault(a => a.ApiInfo.ApiName == assetTile.AssetTileData.ApiName);
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

        public void SubscribePortfolioTile(PortfolioTile portfolioTile)
        {
            this.subscribedPortfolioTiles.Add(portfolioTile);
        }

        public void UnsubscribePortfolioTile(PortfolioTile portfolioTile)
        {
            this.subscribedPortfolioTiles.Remove(portfolioTile);
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
            this.ReadyApis.Add(api, availableAssets);
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

            toNotify.ForEach(a => a.Update(this, updatedAsset));

            this.subscribedPortfolioTiles.ForEach(port => port.Update(updatedAsset));
        }

        private void Api_OnAppDataChanged(object sender, EventArgs e)
        {
            this.FireOnAppDataChanged();
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
        
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        public List<IApi> LoadedApis { get; private set; }

        public Dictionary<IApi, List<Asset>> ReadyApis { get; }

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

        public event EventHandler OnAppDataChanged;
    }
}

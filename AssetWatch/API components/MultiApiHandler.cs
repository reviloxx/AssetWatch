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
        private List<AssetTile> subscribedAssetTiles;

        private Dictionary<IApi, List<Asset>> readyApis;

        public event EventHandler<OnApiReadyEventArgs> OnApiReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiApiHandler"/> class.
        /// </summary>
        public MultiApiHandler()
        {
            subscribedAssetTiles = new List<AssetTile>();
            this.readyApis = new Dictionary<IApi, List<Asset>>();

            List<IApi> loadedApis = new List<IApi>();

            // look for valid API librarys and load them from the hdd
            List<IApi> apis = ApiLoader.GetApisFromDisk();
            apis.ForEach(api =>
            {
                // skip this API if there is already another one with the same name in the dictionary
                if (!loadedApis.Any(a => a.ApiInfo.ApiName == api.ApiInfo.ApiName))
                {
                    // add the API to the dictionary
                    loadedApis.Add(api);

                    // subscribe to the API events and request the available assets
                    api.OnAvailableAssetsReceived += Api_OnAvailableAssetsReceived;
                    api.OnApiError += Api_OnApiError;
                    api.RequestAvailableAssetsAsync();
                }
            });
        }

        private void FireOnApiReady(OnApiReadyEventArgs eventArgs)
        {
            this.OnApiReady?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Is called after an API received it's available assets.
        /// Adds the available assets to the dictionary and subscribes waiting subscribers.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="availableAssets">The availableAssets<see cref="List{AssetInfo}"/></param>
        private void Api_OnAvailableAssetsReceived(object sender, List<Asset> availableAssets)
        {
            // check which API sent it's available assets and put them in the dictionary
            IApi api = (IApi)sender;
            api.OnSingleAssetUpdated += Api_OnSingleAssetUpdated;            
            this.readyApis.Add(api, availableAssets);

            this.FireOnApiReady(new OnApiReadyEventArgs { Api = api, Assets = availableAssets });
        }

        private void Api_OnApiError(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Api_OnSingleAssetUpdated(object sender, Asset updatedAsset)
        {
            IApi api = (IApi)sender;
            List<AssetTile> toNotify = this.subscribedAssetTiles.FindAll(at => at.AssetTileData.ApiName == api.ApiInfo.ApiName && at.AssetTileData.AssetId == updatedAsset.AssetId);

            toNotify.ForEach(a => a.UpdateAsset(this, updatedAsset));
        }

        /// <summary>
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/>The AssetTile to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to subscribe in the dictionary
            IApi api = this.readyApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.ApiName).Key;

            // if the same asset is not subscribet to this API yet, subscribe it
            if (!api.SubscribedAssets.Exists(a => a.AssetId == assetTile.AssetTileData.AssetId && a.ConvertCurrency == assetTile.AssetTileData.ConvertCurrency))
            {
                api.SubscribeAsset(assetTile.AssetTileData.AssetId, assetTile.AssetTileData.ConvertCurrency);
            }

            this.subscribedAssetTiles.Add(assetTile);
        }

        /// <summary>
        /// Unsub
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to unsubscribe in the dictionary
            IApi api = this.readyApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.ApiName).Key;

            // unsubscribe the asset from the API if there is no other assetTile subscribed to this asset
            if (!api.SubscribedAssets.Exists(a => a.AssetId == assetTile.AssetTileData.AssetId && a.ConvertCurrency == assetTile.AssetTileData.ConvertCurrency))
            {
                api.UnsubscribeAsset(assetTile.AssetTileData.AssetId, assetTile.AssetTileData.ConvertCurrency);
            }

            this.subscribedAssetTiles.Remove(assetTile);
        }
    }
}

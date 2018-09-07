using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ApiHandler" />
    /// </summary>
    public class ApiHandler
    {
        /// <summary>
        /// Contains assetTiles which are waiting for the matching API to be ready for accepting subscribers.
        /// </summary>
        private List<AssetTile> assetTilesToSubscribe;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHandler"/> class.
        /// </summary>
        public ApiHandler()
        {
            assetTilesToSubscribe = new List<AssetTile>();

            // look for valid API librarys and load them from the hdd
            List<IApi> apis = ApiLoader.GetApisFromDisk();
            apis.ForEach(api =>
            {
                // skip this API if there is already another one with the same name in the dictionary
                if (!AvailableApis.Any(a => a.Key.ApiInfo.ApiName == api.ApiInfo.ApiName))
                {
                    // add the API to the dictionary
                    AvailableApis.Add(api, null);

                    // subscribe to the event and request the available assets
                    api.OnAvailableAssetsReceived += Api_OnAvailableAssetsReceived;
                    api.RequestAvailableAssetsAsync();
                }
            });
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
            api.ApiInfo.IsReady = true;
            AvailableApis[api] = availableAssets;

            // subscribe assetTiles which were waiting for this API
            List<AssetTile> assetTilesForApi = (List<AssetTile>)assetTilesToSubscribe.Where(a => a.AssetTileData.Api.ApiName == api.ApiInfo.ApiName);
            assetTilesForApi.ForEach(assetTile =>
            {
                // checks if the assetTile is allowed to subscribe to this API
                CheckAssetTile(api, assetTile);

                // if the same asset is not subscribet to this API yet, subscribe it
                if (!api.SubscribedAssets.Exists(a => a.Id == assetTile.AssetTileData.Asset.Id && a.ConvertCurrency == assetTile.AssetTileData.Asset.ConvertCurrency))
                {
                    api.SubscribeAsset(assetTile.AssetTileData.Asset);
                }

                // if everything is fine subscribe asset to the API
                api.OnSingleAssetUpdated += assetTile.Refresh;

                // remove this assetTile from the waiting list
                assetTilesToSubscribe.Remove(assetTile);
            });
        }

        /// <summary>
        /// Checks if an AssetTile can be subscribed to a specific API.
        /// </summary>
        /// <param name="api">The API<see cref="IApi"/></param>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/>The AssetTile to subscribe.</param>
        private void CheckAssetTile(IApi api, AssetTile assetTile)
        {
            // TODO: should never happen, but handle it instead of throwing an exception
            if (!api.ApiInfo.SupportedFiatCurrencies.Contains(assetTile.AssetTileData.Asset.ConvertCurrency))
            {
                throw new Exception("Invalid fiat currency requested!");
            }

            // TODO: handle assets which are not available anymore
            if (!AvailableApis[api].Contains(assetTile.AssetTileData.Asset))
            {
                throw new Exception("Invalid asset requested!");
            }
        }

        /// <summary>
        /// Subscribes an AssetTile to an API if the API is ready.
        /// If not, the AssetTile gets added to the waiting list.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/>The AssetTile to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to subscribe in the dictionary
            IApi api = AvailableApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.Api.ApiName).Key;

            // put the assetTile on the waiting list if the requested API is not available
            if (api == null || !api.ApiInfo.IsReady)
            {
                assetTilesToSubscribe.Add(assetTile);
                return;
            }

            // checks if the assetTile is allowed to subscribe to this API
            CheckAssetTile(api, assetTile);

            // if the same asset is not subscribet to this API yet, subscribe it
            if (!api.SubscribedAssets.Exists(a => a.Id == assetTile.AssetTileData.Asset.Id && a.ConvertCurrency == assetTile.AssetTileData.Asset.ConvertCurrency))
            {
                api.SubscribeAsset(assetTile.AssetTileData.Asset);
            }

            // if everything is fine subscribe asset to the update event of the API
            api.OnSingleAssetUpdated += assetTile.Refresh;            
        }

        /// <summary>
        /// Unsub
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to unsubscribe in the dictionary
            IApi api = AvailableApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.Api.ApiName).Key;            

            // unsubscribe the asset from the API if there is no other assetTile subscribed to this asset
            if (!api.SubscribedAssets.Exists(a => a.Id == assetTile.AssetTileData.Asset.Id && a.ConvertCurrency == assetTile.AssetTileData.Asset.ConvertCurrency))
            {
                api.UnsubscribeAsset(assetTile.AssetTileData.Asset);
            }

            // unsubscribe the assetTile from the update event
            api.OnSingleAssetUpdated -= assetTile.Refresh;
        }

        /// <summary>
        /// Gets the available APIs and Assets.
        /// </summary>
        public Dictionary<IApi, List<Asset>> AvailableApis { get; private set; }
    }
}

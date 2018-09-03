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
        /// Contains assetTiles which are waiting for the matching api to be ready for accepting subscribers.
        /// </summary>
        private List<AssetTile> assetTilesToSubscribe;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHandler"/> class.
        /// </summary>
        public ApiHandler()
        {
            List<IApi> apis = ApiLoader.GetApisFromDisk();
            apis.ForEach(api =>
            {
                if (!AvailableApis.ContainsKey(api))
                {
                    AvailableApis.Add(api, null);
                    api.OnAvailableAssetsReceived += Api_OnAvailableAssetsReceived;
                    api.RequestAvailableAssets();
                }
            });
        }

        /// <summary>
        /// Is called after an Api received it's available assets.
        /// Adds the available assets to the dictionary and subscribes waiting subscribers.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="availableAssets">The availableAssets<see cref="List{AssetInfo}"/></param>
        private void Api_OnAvailableAssetsReceived(object sender, List<AssetInfo> availableAssets)
        {
            IApi api = (IApi)sender;
            AvailableApis[api] = availableAssets;

            List<AssetTile> assetTilesForApi = (List<AssetTile>)assetTilesToSubscribe.Where(a => a.Api.ApiName == api.GetApiInfo().ApiName);
            assetTilesForApi.ForEach(assetTile =>
            {
                CheckAssetTile(api, assetTile);

                api.OnSingleAssetUpdated += assetTile.Refresh;
                api.SubscribeAsset(assetTile.Asset);
                assetTilesToSubscribe.Remove(assetTile);
            });
        }

        /// <summary>
        /// Checks if an AssetTile can be subscribed to a specific Api.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/>The Api.</param>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/>The AssetTile to subscribe.</param>
        private void CheckAssetTile(IApi api, AssetTile assetTile)
        {
            if (!api.GetApiInfo().FiatCurrencies.Contains(assetTile.FiatCurrency))
            {
                throw new Exception("Invalid fiat currency requested!");
            }

            if (!AvailableApis[api].Contains(assetTile.Asset))
            {
                throw new Exception("Invalid asset requested!");
            }
        }

        /// <summary>
        /// Subscribes an AssetTile to an Api if the Api is ready.
        /// If not, the AssetTile gets added to the waiting list.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/>The AssetTile to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            IApi api = AvailableApis.FirstOrDefault(a => a.Key.GetApiInfo().ApiName == assetTile.Api.ApiName).Key;

            // Puts the assetTile on the waiting list if the requested Api is not available
            if (api == null)
            {
                assetTilesToSubscribe.Add(assetTile);
                return;
            }

            CheckAssetTile(api, assetTile);

            api.OnSingleAssetUpdated += assetTile.Refresh;
            api.SubscribeAsset(assetTile.Asset);
        }

        /// <summary>
        /// The UnsubscribeAssetTile
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the available Apis.
        /// </summary>
        public Dictionary<IApi, List<AssetInfo>> AvailableApis { get; private set; }
    }
}

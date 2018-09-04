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
        /// Contains assetTiles which are waiting for the matching Api to be ready for accepting subscribers.
        /// </summary>
        private List<AssetTile> assetTilesToSubscribe;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHandler"/> class.
        /// </summary>
        public ApiHandler()
        {
            // looks for valid Api librarys and loads them from the hdd
            List<IApi> apis = ApiLoader.GetApisFromDisk();
            apis.ForEach(api =>
            {
                // skip this Api if there is already another one with the same name in the dictionary
                if (!AvailableApis.Any(a => a.Key.GetApiInfo().ApiName == api.GetApiInfo().ApiName))
                {
                    // add the Api to the dictionary
                    AvailableApis.Add(api, null);

                    // subscribe to the event and request the available assets
                    api.OnAvailableAssetsReceived += Api_OnAvailableAssetsReceived;

                    // TODO: call must be async
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
        private void Api_OnAvailableAssetsReceived(object sender, List<Asset> availableAssets)
        {
            // check which Api sent it's available assets and put it in the dictionary
            IApi api = (IApi)sender;
            AvailableApis[api] = availableAssets;

            // subscribe assetTiles which were waiting for this Api
            List<AssetTile> assetTilesForApi = (List<AssetTile>)assetTilesToSubscribe.Where(a => a.Api.ApiName == api.GetApiInfo().ApiName);
            assetTilesForApi.ForEach(assetTile =>
            {
                // checks if the assetTile is allowed to subscribe to this Api
                CheckAssetTile(api, assetTile);

                // if everything is fine subscribe asset to the Api
                api.OnSingleAssetUpdated += assetTile.Refresh;
                api.SubscribeAsset(assetTile.Asset);

                // remove this assetTile from the waiting list
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
            // TODO: should never happen, but handle it instead of throwing an exception
            if (!api.GetApiInfo().FiatCurrencies.Contains(assetTile.FiatCurrency))
            {
                throw new Exception("Invalid fiat currency requested!");
            }

            // TODO: handle assets which are not available anymore
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
            // search the Api to subscribe in the dictionary
            IApi api = AvailableApis.FirstOrDefault(a => a.Key.GetApiInfo().ApiName == assetTile.Api.ApiName).Key;

            // put the assetTile on the waiting list if the requested Api is not available
            if (api == null)
            {
                assetTilesToSubscribe.Add(assetTile);
                return;
            }

            // checks if the assetTile is allowed to subscribe to this Api
            CheckAssetTile(api, assetTile);

            // if everything is fine subscribe asset to the Api
            api.OnSingleAssetUpdated += assetTile.Refresh;
            api.SubscribeAsset(assetTile.Asset);
        }

        /// <summary>
        /// The UnsubscribeAssetTile
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            // TODO: check if other assetTiles are subscribed to the same Api/asset and if not, unsubscribe the asset
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the available Apis and Assets.
        /// </summary>
        public Dictionary<IApi, List<Asset>> AvailableApis { get; private set; }
    }
}

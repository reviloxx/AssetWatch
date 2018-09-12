﻿using System;
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
        /// Contains all APIs which are ready to use.
        /// </summary>
        private Dictionary<IApi, List<Asset>> readyApis;

        /// <summary>
        /// Is fired when a handled API is ready to use.
        /// The event args contain the API which is ready and it's available assets.
        /// </summary>
        public event EventHandler<OnApiReadyEventArgs> OnApiReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiApiHandler"/> class.
        /// </summary>
        public MultiApiHandler()
        {
            subscribedAssetTiles = new List<AssetTile>();
            readyApis = new Dictionary<IApi, List<Asset>>();

            LoadApis();
        }

        /// <summary>
        /// Loads all available APIs by using an IApiLoader, sobscribes to it's events and requests it's available assets.
        /// </summary>
        private void LoadApis()
        {
            IApiLoader apiLoader = new DiskApiLoader();

            // look for valid API librarys, load them from the disk and remove duplicates
            List<IApi> apis = apiLoader.GetApis()
                .GroupBy(api => api.ApiInfo.ApiName)
                .Select(g => g.First())
                .ToList();

            apis.ForEach(api =>
            {
                // subscribe to the API events and request the available assets
                api.OnAvailableAssetsReceived += Api_OnAvailableAssetsReceived;
                api.OnApiError += Api_OnApiError;
                api.RequestAvailableAssetsAsync();
            });
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
        /// Is called after an API has received it's available assets.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="availableAssets">The availableAssets<see cref="List{AssetInfo}"/></param>
        private void Api_OnAvailableAssetsReceived(object sender, List<Asset> availableAssets)
        {
            // check which API sent it's available assets and put them in the dictionary
            IApi api = (IApi)sender;
            api.OnSingleAssetUpdated += Api_OnSingleAssetUpdated;
            readyApis.Add(api, availableAssets);

            FireOnApiReady(new OnApiReadyEventArgs { Api = api, Assets = availableAssets });
        }

        /// <summary>
        /// Is called if something went wrong with the API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Api_OnApiError(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Is called after an asset has received an update.
        /// Updates all asset tiles which are waiting for updates from this API, for this asset.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="updatedAsset">The updatedAsset<see cref="Asset"/></param>
        private void Api_OnSingleAssetUpdated(object sender, Asset updatedAsset)
        {
            IApi api = (IApi)sender;
            List<AssetTile> toNotify = subscribedAssetTiles.FindAll(at => at.AssetTileData.ApiName == api.ApiInfo.ApiName && at.AssetTileData.AssetId == updatedAsset.AssetId);

            toNotify.ForEach(a => a.UpdateAsset(this, updatedAsset));
        }

        /// <summary>
        /// Subscribes an asset tile to the api handler.
        /// Subscribes the asset to the API if it is not subscribed yet.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/>The AssetTile to subscribe.</param>
        public void SubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to subscribe in the dictionary
            IApi api = readyApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.ApiName).Key;

            // if the same asset is not subscribet to this API yet, subscribe it
            if (!api.SubscribedAssets.Exists(a => a.AssetId == assetTile.AssetTileData.AssetId && a.ConvertCurrency == assetTile.AssetTileData.ConvertCurrency))
            {
                api.SubscribeAsset(assetTile.AssetTileData.AssetId, assetTile.AssetTileData.ConvertCurrency);
            }

            subscribedAssetTiles.Add(assetTile);
        }

        /// <summary>
        /// Unsubscribes an asset tile from the api handler.
        /// Unsubscribes the asset from the API if there is no other asset tile waiting for updates of the same asset.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            // search the API to unsubscribe in the dictionary
            IApi api = readyApis.FirstOrDefault(a => a.Key.ApiInfo.ApiName == assetTile.AssetTileData.ApiName).Key;

            // unsubscribe the asset from the API if there is no other assetTile subscribed to this asset
            if (!api.SubscribedAssets.Exists(a => a.AssetId == assetTile.AssetTileData.AssetId && a.ConvertCurrency == assetTile.AssetTileData.ConvertCurrency))
            {
                api.UnsubscribeAsset(assetTile.AssetTileData.AssetId, assetTile.AssetTileData.ConvertCurrency);
            }

            subscribedAssetTiles.Remove(assetTile);
        }
    }
}

using System;
using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IApi" />
    /// </summary>
    public interface IApi
    {
        /// <summary>
        /// Is fired after the API has received it's available assets.
        /// </summary>
        event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Is fired after the API has received new data for a single asset.
        /// </summary>
        event EventHandler<Asset> OnSingleAssetUpdated;

        /// <summary>
        /// Is fired after something went wrong.
        /// </summary>
        event EventHandler OnApiError;

        List<Asset> SubscribedAssets { get; }
        
        ApiInfo ApiInfo { get; }

        /// <summary>
        /// Subscribes an asset to the API which then will be updated in the currently defined update interval.
        /// </summary>
        /// <param name="assetInfo">The assetInfo<see cref="Asset"/></param>
        void SubscribeAsset(string assetName, string convertCurrency);

        /// <summary>
        /// Unsubscribes an asset from the API which then will be no longer updated.
        /// </summary>
        /// <param name="assetInfo">The assetInfo<see cref="Asset"/></param>
        void UnsubscribeAsset(string assetName, string convertCurrency);

        /// <summary>
        /// Requests the available assets of the API.
        /// Should be called asynchronous.
        /// Fires the OnAvailableAssetsReceived event if successful.
        /// </summary>
        void RequestAvailableAssetsAsync();

        /// <summary>
        /// Sets the update interval of the API.
        /// </summary>
        /// <param name="updateInterval">The update interval in seconds.</param>
        void SetUpdateInterval(int updateInterval);

        /// <summary>
        /// Starts the asset update thread.
        /// </summary>
        void StartAssetUpdater();
    }
}

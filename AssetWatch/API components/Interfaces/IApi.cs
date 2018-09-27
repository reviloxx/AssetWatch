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
        event EventHandler<OnApiErrorEventArgs> OnApiError;

        event EventHandler OnAppDataChanged;

        /// <summary>
        /// Gets the serializable ApiData which contains the current update interval of the API.
        /// </summary>
        ApiData ApiData { get; set; }

        /// <summary>
        /// Gets the ApiInfo which contains all neccessary information about the API.
        /// </summary>
        ApiInfo ApiInfo { get; }

        /// <summary>
        /// Starts the asset update thread.
        /// </summary>
        void StartAssetUpdater();

        /// <summary>
        /// Subscribes an asset to the API which then will be updated in the currently defined update interval.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        void SubscribeAssetToUpdater(Asset asset);

        /// <summary>
        /// Unsubscribes an asset from the API which then will be no longer updated.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        void UnsubscribeAssetFromUpdater(Asset asset);

        /// <summary>
        /// Requests the available assets of the API.
        /// Fires the OnAvailableAssetsReceived event if successful.
        /// </summary>
        void RequestAvailableAssetsAsync();

        /// <summary>
        /// Sets the update interval of the API.
        /// </summary>
        /// <param name="updateInterval">The update interval in seconds.</param>
        void SetUpdateInterval(int updateInterval);

        /// <summary>
        /// Enables the API and starts the asset update thread if there are any subscribed assets.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the API and suspends the asset update thread.
        /// </summary>
        void Disable();
    }
}

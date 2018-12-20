﻿using System;
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

        /// <summary>
        /// Is fired after the AppData was changed.
        /// </summary>
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
        /// Attaches an asset to the API which then will be updated in the currently defined update interval.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        void AttachAsset(Asset asset);

        /// <summary>
        /// Detaches an asset from the API which then will be no longer updated.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        void DetachAsset(Asset asset);

        /// <summary>
        /// Requests the available assets of the API.
        /// Fires the OnAvailableAssetsReceived event if successful.
        /// </summary>
        void RequestAvailableAssetsAsync();

        /// <summary>
        /// Requests a single asset update.
        /// Useful to get the data immediately when subscribing a new asset.
        /// </summary>
        void RequestSingleAssetUpdateAsync(Asset asset);

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

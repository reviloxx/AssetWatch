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
        /// Must be fired after the API has received it's available assets.
        /// </summary>
        event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Must be fired after the API has received new data for a single asset.
        /// </summary>
        event EventHandler<Asset> OnSingleAssetUpdated;

        /// <summary>
        /// Should be fired after something went wrong.
        /// </summary>
        event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Should be fired after the counter in the ApiData has been increased.
        /// </summary>
        event EventHandler OnAppDataChanged;

        /// <summary>
        /// Gets the serializable ApiData which contains modifiable data for the API.
        /// </summary>
        ApiData ApiData { get; set; }

        /// <summary>
        /// Gets the ApiInfo which contains general information about the API.
        /// </summary>
        ApiInfo ApiInfo { get; }

        /// <summary>
        /// Attaches an asset to the API which then should be updated in the currently defined update interval if the API is enabled.
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
        /// </summary>
        void RequestAvailableAssetsAsync();

        /// <summary>
        /// Requests a single asset update.
        /// </summary>
        void RequestSingleAssetUpdateAsync(Asset asset);

        /// <summary>
        /// The client expects regulary updates for the attached assets after this function was called.
        /// </summary>
        void Enable();

        /// <summary>
        /// The client no longer expects updates for the attached assets after this function was called.
        /// </summary>
        void Disable();
    }
}

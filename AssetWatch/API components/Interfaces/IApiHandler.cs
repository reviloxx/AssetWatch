using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IApiHandler" />
    /// </summary>
    public interface IApiHandler
    {
        /// <summary>
        /// Is fired when a handled API is ready to use.
        /// The event args contain the API which is ready and it's available assets.
        /// </summary>
        event EventHandler<OnApiReadyEventArgs> OnApiReady;
        event EventHandler<IApi> OnApiLoaded;
        event EventHandler<OnApiErrorEventArgs> OnApiError;

        void LoadApis(IApiLoader apiLoader);

        void EnableApi(IApi api);

        void DisableApi(IApi api);

        void SetUpdateInterval(IApi api, int seconds);

        /// <summary>
        /// Subscribes an asset tile to the API handler.
        /// A subscribed asset tile gets informed everytime it's asset is updated by calling the asset tile's "UpdateAsset" function.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        void SubscribeAssetTile(AssetTile assetTile);

        /// <summary>
        /// Unsubscribes an asset tile from the API handler.
        /// An unsubscribed asset tile will no longer receive updates of it's asset.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/></param>
        void UnsubscribeAssetTile(AssetTile assetTile);        
    }
}

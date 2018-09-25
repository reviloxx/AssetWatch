using System;
using System.Collections.Generic;

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
        event EventHandler<OnApiReadyEventArgs> OnApiReady;// TODO: maybe unneccessary

        /// <summary>
        /// Is fired after an assembly which contains an IApi object was loaded.
        /// The event args contain the loaded API.
        /// </summary>
        event EventHandler<IApi> OnApiLoaded;

        /// <summary>
        /// Is fired when any error occurs within the API.
        /// The event args contain the error type and a error message.
        /// </summary>
        event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Is fired after an API was disabled.
        /// The event args contain the disabled API.
        /// </summary>
        event EventHandler<IApi> OnApiDisabled;// TODO: maybe unneccessary

        /// <summary>
        /// Loads IApi objects by using an IApiLoader instance.
        /// </summary>
        /// <param name="apiLoader">The apiLoader<see cref="IApiLoader"/> to load the IApi objects.</param>
        void LoadApis(IApiLoader apiLoader);

        /// <summary>
        /// Enables the API.
        /// </summary>
        /// <param name="api">The API<see cref="IApi"/> to enable.</param>
        void EnableApi(IApi api);

        /// <summary>
        /// Disables the API.
        /// </summary>
        /// <param name="api">The API<see cref="IApi"/> to disable.</param>
        void DisableApi(IApi api);

        /// <summary>
        /// Starts the asset update thread of a specific API.
        /// </summary>
        /// <param name="api">The API to start the asset update thread.<see cref="IApi"/></param>
        void StartAssetUpdater(IApi api);

        /// <summary>
        /// The SetUpdateInterval sets a new update interval for updating all subscribed assets of an API.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/> to set the update interval.</param>
        /// <param name="seconds">The seconds<see cref="int"/> defines the new update interval.</param>
        void SetUpdateInterval(IApi api, int seconds);

        /// <summary>
        /// Subscribes an asset tile to the API handler.
        /// A subscribed asset tile gets informed everytime it's asset is updated by calling the asset tile's "UpdateAsset" function.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to subscribe.</param>
        void SubscribeAssetTile(AssetTile assetTile);

        /// <summary>
        /// Unsubscribes an asset tile from the API handler.
        /// An unsubscribed asset tile will no longer receive updates of it's asset.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="AssetTile"/> to unsubscribe.</param>
        void UnsubscribeAssetTile(AssetTile assetTile);

        /// <summary>
        /// Gets a list of APIs which were loaded from assemblies.
        /// </summary>
        List<IApi> LoadedApis { get; }

        /// <summary>
        /// Gets a dictionary of ready APIs and their available assets.
        /// </summary>
        Dictionary<IApi, List<Asset>> ReadyApis { get; }
    }
}

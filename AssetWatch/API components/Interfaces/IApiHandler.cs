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
        /// Is fired after an assembly which contains an implementation of the IApi interface was loaded.
        /// </summary>
        event EventHandler<IApi> OnApiLoaded;

        /// <summary>
        /// Is fired after the AppData has been changed.
        /// </summary>
        event EventHandler OnAppDataChanged;

        /// <summary>
        /// Loads IApi objects by using an IApiLoader instance.
        /// </summary>
        /// <param name="apiLoader">The apiLoader<see cref="IApiLoader"/></param>
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
        /// Attaches an asset tile to the API handler.
        /// An attached asset tile gets informed about price changes by calling it's update function.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="IAssetTile"/> to subscribe.</param>
        /// <param name="requestUpdate">If true, the API will request an update for this asset instantly.</param>
        void AttachAssetTile(IAssetTile assetTile, bool requestUpdate);

        /// <summary>
        /// Detaches an asset tile from the API handler.
        /// A detached asset tile will no longer receive price updates.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="WpfAssetTile"/> to unsubscribe.</param>
        void DetachAssetTile(IAssetTile assetTile);

        /// <summary>
        /// Gets a list of APIs which were loaded by an IApiLoader.
        /// </summary>
        List<IApi> LoadedApis { get; }

        /// <summary>
        /// Gets a dictionary of ready APIs and their available assets.
        /// </summary>
        Dictionary<IApi, List<Asset>> ReadyApis { get; }
    }
}

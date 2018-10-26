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
        /// Is fired after an assembly which contains an IApi object was loaded.
        /// The event args contain the loaded API.
        /// </summary>
        event EventHandler<IApi> OnApiLoaded;

        /// <summary>
        /// Is fired after the AppData was changed.
        /// </summary>
        event EventHandler OnAppDataChanged;

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
        /// Subscribes a portfolio tile to the API handler.
        /// </summary>
        /// <param name="portfolioTile">The portfolioTile<see cref="PortfolioTile"/></param>
        void SubscribePortfolioTile(PortfolioTile portfolioTile);

        /// <summary>
        /// Unsubscribes a portfolio tile from the API handler.
        /// </summary>
        /// <param name="portfolioTile">The portfolioTile<see cref="PortfolioTile"/></param>
        void UnsubscribePortfolioTile(PortfolioTile portfolioTile);

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

using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IPortfolioTile" />
    /// </summary>
    public interface IPortfolioTile : ITile
    {
        /// <summary>
        /// Updates the portfolio tile after any of it's asset tiles has received an update.
        /// </summary>
        /// <param name="updatedAssetTile">The updatedAsset<see cref="IAssetTile"/></param>
        void Update(IAssetTile updatedAssetTile);

        /// <summary>
        /// Removes an asset tile from the portfolio tile.
        /// </summary>
        /// <param name="assetTileId">The assetTileId<see cref="int"/> to remove.</param>
        void RemoveAssetTile(int assetTileId);

        /// <summary>
        /// Gets or sets the PortfolioTileData.
        /// </summary>
        PortfolioTileData PortfolioTileData { get; set; }

        /// <summary>
        /// Is fired after a portfolio settings window was opened.
        /// The client then does not allow the user to open a new tile because this would cause a crash.
        /// </summary>
        event EventHandler OnPortfolioSettingsWindowOpened;

        /// <summary>
        /// Is fired after a portfolio settings window was closed.
        /// </summary>
        event EventHandler OnPortfolioSettingsWindowClosed;
    }
}

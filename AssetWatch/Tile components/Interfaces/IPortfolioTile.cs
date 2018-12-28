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
        /// <param name="updatedAsset">The updatedAsset<see cref="IAssetTile"/></param>
        void Update(IAssetTile updatedAsset);

        /// <summary>
        /// Removes an asset tile from the portfolio tile.
        /// </summary>
        /// <param name="assetTileId">The assetTileId<see cref="int"/> to remove.</param>
        void RemoveAssetTile(int assetTileId);

        /// <summary>
        /// Gets or sets the PortfolioTileData
        /// Gets the PortfolioTileData
        /// </summary>
        PortfolioTileData PortfolioTileData { get; set; }
    }
}

using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IAssetTile" />
    /// </summary>
    public interface IAssetTile : ITile
    {
        /// <summary>
        /// Is called by the API handler after it has received an asset update for this asset tile.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> which has received the update.</param>
        void Update(Asset asset);

        /// <summary>
        /// Gets or sets the AssetTileData.
        /// </summary>
        AssetTileData AssetTileData { get; set; }

        /// <summary>
        /// Defines the OnAssetSelected.
        /// </summary>
        event EventHandler OnAssetSelected;

        /// <summary>
        /// Defines the OnAssetUnselected.
        /// </summary>
        event EventHandler OnAssetUnselected;

        /// <summary>
        /// Defines the OnAssetTileUpdated.
        /// </summary>
        event EventHandler OnAssetTileUpdated;        
    }
}

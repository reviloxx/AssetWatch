using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ITileHandler" />
    /// </summary>
    public interface ITileHandler
    {
        /// <summary>
        /// Opens a new asset tile which gets handled by the tile handler.
        /// </summary>
        void OpenNewAssetTile();

        void OpenNewPortfolioTile();

        /// <summary>
        /// The SetGlobalTileStyle
        /// </summary>
        void RefreshTileStyles();

        event EventHandler OnAppDataChanged;
    }
}

using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ITileHandler" />
    /// </summary>
    public interface ITileHandler
    {
        /// <summary>
        /// Opens a new asset tile which then is handled by the tile handler.
        /// </summary>
        void OpenNewAssetTile();

        /// <summary>
        /// Opens a new portfolio tile which then is handled by the tile handler.
        /// </summary>
        void OpenNewPortfolioTile();

        /// <summary>
        /// Refreshes the style of all handled tiles.
        /// </summary>
        void RefreshTileStyles();

        /// <summary>
        /// Locks or unlocks the positions of all tiles.
        /// </summary>
        /// <param name="locked">The locked<see cref="bool"/></param>
        void LockTilePositions(bool locked);

        /// <summary>
        /// Defines the OnAppDataChanged event.
        /// </summary>
        event EventHandler OnAppDataChanged;
    }
}

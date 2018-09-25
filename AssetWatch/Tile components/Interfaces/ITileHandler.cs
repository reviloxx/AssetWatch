namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ITileHandler" />
    /// </summary>
    public interface ITileHandler
    {
        /// <summary>
        /// Gets or sets the TileHandlerData.
        /// </summary>
        TileHandlerData TileHandlerData { get; set; }

        /// <summary>
        /// Opens a new asset tile which gets handled by the tile handler.
        /// </summary>
        void OpenNewAssetTile();

        /// <summary>
        /// The SetGlobalTileStyle
        /// </summary>
        void ActivateGlobalTileStyle();
    }
}

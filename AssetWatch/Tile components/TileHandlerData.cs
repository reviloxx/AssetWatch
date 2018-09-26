using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="TileHandlerData" />
    /// </summary>
    [Serializable]
    public class TileHandlerData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileHandlerData"/> class.
        /// </summary>
        public TileHandlerData()
        {
            this.GlobalTileStyle = new TileStyle();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile positions are locked or not.
        /// </summary>
        public bool PositionsLocked { get; set; }

        /// <summary>
        /// Gets or sets the GlobalTileStyle.
        /// </summary>
        public TileStyle GlobalTileStyle { get; set; }
    }
}

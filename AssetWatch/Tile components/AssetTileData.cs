using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="AssetTileData" />
    /// </summary>
    [Serializable]
    public class AssetTileData
    {
        /// <summary>
        /// Gets or sets the name of the used API.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the used A
        /// </summary>
        public Asset Asset { get; set; }        

        /// <summary>
        /// Gets or sets the HoldingsCount
        /// </summary>
        public double HoldingsCount { get; set; }

        /// <summary>
        /// Gets or sets the InvestedSum
        /// </summary>
        public int InvestedSum { get; set; }

        /// <summary>
        /// Gets or sets the TileStyle
        /// </summary>
        public TileStyle TileStyle { get; set; }

        /// <summary>
        /// Gets or sets the AssetTileId
        /// </summary>
        public int AssetTileId { get; set; }

        public AssetTileData()
        {
            this.Asset = new Asset();
        }
    }
}

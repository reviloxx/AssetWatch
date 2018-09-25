using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="AssetTileData" />
    /// </summary>
    [Serializable]
    public class AssetTileData
    {
        public string AssetTileName { get; set; }
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
        public double InvestedSum { get; set; }

        public Position TilePosition { get; set; }

        /// <summary>
        /// Gets or sets the CustomTileStyle
        /// </summary>
        public TileStyle CustomTileStyle { get; set; }

        public bool HasCustomTileStyle { get; set; }

        public AssetTileData()
        {
            this.Asset = new Asset();
            this.HoldingsCount = 0;
            this.InvestedSum = 0;
            this.TilePosition = new Position();
            this.CustomTileStyle = new TileStyle();
            this.HasCustomTileStyle = false;
        }
    }

    public class Position
    {
        public double FromLeft { get; set; }

        public double FromTop { get; set; }
    }
}

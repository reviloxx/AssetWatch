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
        /// Gets or sets the AssetTileId
        /// </summary>
        public int AssetTileId { get; set; }

        /// <summary>
        /// Gets or sets the AssetTileName
        /// </summary>
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

        /// <summary>
        /// Gets or sets the TilePosition
        /// </summary>
        public Position TilePosition { get; set; }

        /// <summary>
        /// Gets or sets the CustomTileStyle
        /// </summary>
        public TileStyle CustomTileStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HasCustomTileStyle
        /// </summary>
        public bool HasCustomTileStyle { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTileData"/> class.
        /// </summary>
        public AssetTileData()
        {
            this.Asset = new Asset();
            this.HoldingsCount = 0;
            this.InvestedSum = 0;
            this.TilePosition = new Position();
            this.CustomTileStyle = new TileStyle();
            this.HasCustomTileStyle = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTileData"/> class.
        /// </summary>
        /// <param name="rand">The rand<see cref="Random"/></param>
        public AssetTileData(Random rand)
        {
            this.AssetTileId = rand.Next();
            this.Asset = new Asset();
            this.HoldingsCount = 0;
            this.InvestedSum = 0;
            this.TilePosition = new Position();
            this.CustomTileStyle = new TileStyle();
            this.HasCustomTileStyle = false;
        }
    }

    /// <summary>
    /// Defines the <see cref="Position" />
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Gets or sets the FromLeft
        /// </summary>
        public double FromLeft { get; set; }

        /// <summary>
        /// Gets or sets the FromTop
        /// </summary>
        public double FromTop { get; set; }
    }
}

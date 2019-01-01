using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="PortfolioTileData" />
    /// </summary>
    public class PortfolioTileData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioTileData"/> class.
        /// </summary>
        public PortfolioTileData()
        {
            this.TilePosition = new Position();
            this.CustomTileStyle = new TileStyle();
            this.AssignedAssetTileIds = new List<int>();
        }

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
        /// Gets or sets the AssignedAssetTileIds
        /// </summary>
        public List<int> AssignedAssetTileIds { get; set; }

        /// <summary>
        /// Gets or sets the PortfolioTileName
        /// </summary>
        public string PortfolioTileName { get; set; }
    }
}

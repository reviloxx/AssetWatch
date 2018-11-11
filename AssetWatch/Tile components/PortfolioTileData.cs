using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class PortfolioTileData
    {
        public PortfolioTileData()
        {
            this.TilePosition = new Position();
            this.CustomTileStyle = new TileStyle();
            this.AssignedAssetTileIds = new List<int>();
        }

        public Position TilePosition { get; set; }

        /// <summary>
        /// Gets or sets the CustomTileStyle
        /// </summary>
        public TileStyle CustomTileStyle { get; set; }

        public bool HasCustomTileStyle { get; set; }

        public List<int> AssignedAssetTileIds { get; set; }

        public string PortfolioTileName { get; set; }
    }
}
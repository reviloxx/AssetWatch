using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class AssetTileData
    {
        public string ApiName { get; set; }

        public string AssetId { get; set; }

        public string ConvertCurrency { get; set; }

        public double HoldingsCount { get; set; }
        public int InvestedSum { get; set; }

        public TileStyle TileStyle
        {
            get; set;
        }

        public int AssetTileId { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="AppData" />
    /// </summary>
    [Serializable]
    public class AppData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppData"/> class.
        /// </summary>
        public AppData()
        {
            this.TileHandlerData = new TileHandlerData();
            this.AssetTileDataSet = new List<AssetTileData>();
            this.PortfolioTileDataSet = new List<PortfolioTileData>();
            this.ApiDataSet = new List<ApiData>();
        }

        /// <summary>
        /// Gets or sets the TileHandlerData.
        /// </summary>
        public TileHandlerData TileHandlerData { get; set; }

        /// <summary>
        /// Gets or sets the AssetTileDataSet.
        /// </summary>
        public List<AssetTileData> AssetTileDataSet { get; set; }

        /// <summary>
        /// Gets or sets the PortfolioTileDataSet.
        /// </summary>
        public List<PortfolioTileData> PortfolioTileDataSet { get; set; }

        /// <summary>
        /// Gets or sets the ApiDataSet.
        /// </summary>
        public List<ApiData> ApiDataSet { get; set; }
    }
}

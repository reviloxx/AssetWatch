using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    [Serializable]
    public class AppData
    {
        public AppData()
        {
            this.TileHandlerData = new TileHandlerData();
            this.AssetTileDataSet = new List<AssetTileData>();
            this.PortfolioTileDataSet = new List<PortfolioTileData>();
            this.ApiDataSet = new List<ApiData>();
        }

        public TileHandlerData TileHandlerData { get; set; }

        public List<AssetTileData> AssetTileDataSet { get; set; }

        public List<PortfolioTileData> PortfolioTileDataSet { get; set; }

        public List<ApiData> ApiDataSet { get; set; }
    }
}
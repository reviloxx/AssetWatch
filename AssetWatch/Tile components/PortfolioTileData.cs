﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class PortfolioTileData
    {
        public TileStyle TileStyle
        {
            get; set;
        }

        public List<AssetTileData> AssetTiles
        {
            get; set;
        }
    }
}
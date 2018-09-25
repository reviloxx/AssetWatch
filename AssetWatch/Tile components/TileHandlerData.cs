using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    [Serializable]
    public class TileHandlerData
    {
        public TileHandlerData()
        {
            this.GlobalTileStyle = new TileStyle();
        }
        public bool PositionsLocked { get; set; }

        public TileStyle GlobalTileStyle { get; set; }
    }
}
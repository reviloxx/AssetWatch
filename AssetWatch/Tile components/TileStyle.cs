﻿using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="TileStyle" />
    /// </summary>
    [Serializable]
    public class TileStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileStyle"/> class.
        /// </summary>
        public TileStyle()
        {
            this.BackgroundColorProfit = "#FF000000";
            this.BackgroundColorLoss = "#FF000000";

            this.FontColorProfit = "#FFFFFFFF";
            this.FontColorLoss = "#FFFFFFFF";
        }

        /// <summary>
        /// Gets or sets the BackgroundColorProfit
        /// Contains the background color when the asset is making profits.
        /// </summary>
        public string BackgroundColorProfit { get; set; }

        /// <summary>
        /// Gets or sets the BackgroundColorLoss
        /// Contains the background color when the asset is making losses.
        /// </summary>
        public string BackgroundColorLoss { get; set; }

        /// <summary>
        /// Gets or sets the Size of the tile.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the FontColorProfit
        /// Contains the font color when the asset is making profits.
        /// </summary>
        public string FontColorProfit { get; set; }

        /// <summary>
        /// Gets or sets the FontColorLoss
        /// Contains the font color when the asset is making losses.
        /// </summary>
        public string FontColorLoss { get; set; }        
    }
}

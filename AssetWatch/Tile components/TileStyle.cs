using System;

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
            this.BackgroundColorProfit = "#3200FF00";
            this.BackgroundColorLoss = "#32FF0000";

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

        // TODO: ADD FEATURE: adjustable tile size
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

        public bool Hidden { get; set; }
    }
}

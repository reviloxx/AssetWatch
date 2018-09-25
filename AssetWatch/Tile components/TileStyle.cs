
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
        /// Contains the background color when the asset is making profits
        /// </summary>
        public string BackgroundColorProfit
        {
            get; set;
        }

        /// <summary>
        /// Contains the background color when the asset is making losses
        /// </summary>
        public string BackgroundColorLoss
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Size
        /// </summary>
        public int Size
        {
            get; set;
        }

        /// <summary>
        /// Contains the font color when the asset is making profits
        /// </summary>
        public string FontColorProfit
        {
            get; set;
        }

        /// <summary>
        /// Contains the font color when the asset is making losses
        /// </summary>
        public string FontColorLoss
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Position
        /// </summary>
        public int Position
        {
            get; set;
        }

        /// <summary>
        /// Contains True if the position of the Tile is locked
        /// </summary>
        public int PositionLocked
        {
            get; set;
        }

        public TileStyle()
        {
            this.BackgroundColorProfit = "#FF000000";
            this.BackgroundColorLoss = "#FF000000";

            this.FontColorProfit = "#FFFFFFFF";
            this.FontColorLoss = "#FFFFFFFF";
        }
    }
}

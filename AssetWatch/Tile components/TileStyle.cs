
namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="TileStyle" />
    /// </summary>
    public class TileStyle
    {
        /// <summary>
        /// Contains the background color when the asset is making profits
        /// </summary>
        public string BackgroundColorPos
        {
            get; set;
        }

        /// <summary>
        /// Contains the background color when the asset is making losses
        /// </summary>
        public string BackgroundColorNeg
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Transparency
        /// </summary>
        public int Transparency
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
        public string FontColorPos
        {
            get; set;
        }

        /// <summary>
        /// Contains the font color when the asset is making losses
        /// </summary>
        public string FontColorNeg
        {
            get; set;
        }

        /// <summary>
        /// Contains the icon color when the asset is making profits
        /// </summary>
        public string IconColorPos
        {
            get; set;
        }

        /// <summary>
        /// Contains the icon color when the asset is making losses
        /// </summary>
        public int IconColorNeg
        {
            get; set;
        }

        /// <summary>
        /// Contains True if the Tile uses an unique style instead of the global style
        /// </summary>
        public bool IsUniqueTileStyle
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
    }
}

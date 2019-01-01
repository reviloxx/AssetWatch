using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetWatch
{
    public interface ITile
    {
        /// <summary>
        /// Shows the tile.
        /// </summary>
        void Show();

        /// <summary>
        /// Refreshes the tile style.
        /// </summary>
        void RefreshTileStyle();

        /// <summary>
        /// Locks or unlocks the position of the tile.
        /// </summary>
        /// <param name="locked">The locked<see cref="bool"/></param>
        void LockPosition(bool locked);

        /// <summary>
        /// Defines the OnAppDataChanged.
        /// </summary>
        event EventHandler OnAppDataChanged;

        /// <summary>
        /// Defines the OnTileClosed.
        /// </summary>
        event EventHandler OnTileClosed;
    }
}

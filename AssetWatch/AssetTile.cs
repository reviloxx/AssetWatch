using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class AssetTile : Tile
    {

        public AssetInfo Asset
        {
            get => null;
            set
            {
            }
        }

        public string ApiName
        {
            get => string.Empty;
            set
            {
            }
        }

        public void Refresh()
        {
            throw new System.NotImplementedException();
        }
    }
}
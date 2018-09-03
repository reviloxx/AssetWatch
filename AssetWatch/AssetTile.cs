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

        public ApiInfo Api
        {
            get => null;
            set
            {
            }
        }

        public void Refresh(object sender, AssetInfo assetInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}
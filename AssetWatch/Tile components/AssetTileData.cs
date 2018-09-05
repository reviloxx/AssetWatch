using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class AssetTileData
    {
        public ApiInfo Api
        {
            get => null;
            set
            {
            }
        }

        public Asset Asset
        {
            get => null;
            set
            {
            }
        }

        public int HoldingsCount
        {
            get => default(int);
            set
            {
            }
        }

        public int InvestedSum
        {
            get => default(int);
            set
            {
            }
        }

        public TileStyle TileStyle
        {
            get; set;
        }

        public int Id
        {
            get => default(int);
            set
            {
            }
        }
    }
}
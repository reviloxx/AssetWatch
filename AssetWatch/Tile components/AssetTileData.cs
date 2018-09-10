using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class AssetTileData
    {
        public string ApiName
        {
            get => null;
            set
            {
            }
        }

        public string AssetId
        {
            get => null;
            set
            {
            }
        }

        public string ConvertCurrency { get; set; }

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
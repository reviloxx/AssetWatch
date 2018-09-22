using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    [Serializable]
    public class Asset
    {
        public string SupplyAvailable { get; set; }

        public string ConvertCurrency { get; set; }

        public string AssetId { get; set; }

        public DateTime LastUpdated { get; set; }

        public string MarketCapConvert { get; set; }

        public string MarketCapUsd { get; set; }

        public string Name { get; set; }

        public string PercentChange1h { get; set; }

        public string PercentChange24h { get; set; }

        public string PercentChange7d { get; set; }

        public string PriceConvert { get; set; }

        public string PriceUsd { get; set; }

        public string Rank { get; set; }

        public string Symbol { get; set; }

        public string SupplyTotal { get; set; }

        public string Volume24hConvert { get; set; }

        public string Volume24hUsd { get; set; }
    }
}
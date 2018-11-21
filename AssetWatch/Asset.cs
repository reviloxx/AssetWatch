using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="Asset" />
    /// </summary>
    [Serializable]
    public class Asset
    {      
        /// <summary>
        /// Gets or sets the ConvertCurrency.
        /// </summary>
        public string ConvertCurrency { get; set; }

        /// <summary>
        /// Gets or sets the AssetId.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// Gets or sets the LastUpdated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the MarketCapConvert.
        /// </summary>
        public double MarketCap { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the PercentChange1h.
        /// </summary>
        public double PercentChange1h { get; set; }

        /// <summary>
        /// Gets or sets the PercentChange24h.
        /// </summary>
        public double PercentChange24h { get; set; }

        /// <summary>
        /// Gets or sets the PercentChange7d.
        /// </summary>
        public double PercentChange7d { get; set; }

        /// <summary>
        /// Gets or sets the PriceConvert.
        /// </summary>
        public double Price { get; set; }        

        /// <summary>
        /// Gets or sets the Rank.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the Symbol.
        /// </summary>
        public string Symbol { get; set; }

        public string SymbolName { get { return this.Symbol + " (" + this.Name + ")"; } }

        /// <summary>
        /// Gets or sets the SupplyAvailable.
        /// </summary>
        public double SupplyAvailable { get; set; }

        /// <summary>
        /// Gets or sets the SupplyTotal.
        /// </summary>
        public double SupplyTotal { get; set; }

        /// <summary>
        /// Gets or sets the Volume24hConvert.
        /// </summary>
        public string Volume24hConvert { get; set; }

        /// <summary>
        /// Gets or sets the Volume24hUsd.
        /// </summary>
        public string Volume24hUsd { get; set; }
    }
}

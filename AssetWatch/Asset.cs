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
        /// Gets or sets the SupplyAvailable.
        /// </summary>
        public string SupplyAvailable { get; set; }

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
        public string MarketCapConvert { get; set; }

        /// <summary>
        /// Gets or sets the MarketCapUsd.
        /// </summary>
        public string MarketCapUsd { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the PercentChange1h.
        /// </summary>
        public string PercentChange1h { get; set; }

        /// <summary>
        /// Gets or sets the PercentChange24h.
        /// </summary>
        public string PercentChange24h { get; set; }

        /// <summary>
        /// Gets or sets the PercentChange7d.
        /// </summary>
        public string PercentChange7d { get; set; }

        /// <summary>
        /// Gets or sets the PriceConvert.
        /// </summary>
        public string PriceConvert { get; set; }

        /// <summary>
        /// Gets or sets the PriceUsd.
        /// </summary>
        public string PriceUsd { get; set; }

        /// <summary>
        /// Gets or sets the Rank.
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// Gets or sets the Symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the SupplyTotal.
        /// </summary>
        public string SupplyTotal { get; set; }

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

using AssetWatch;
using System;
using System.Collections.Generic;

namespace ApiCoinmarketcap
{
    /// <summary>
    /// Defines the <see cref="ApiCoinmarketcapProBTC" />
    /// </summary>
    public class ApiCoinmarketcapProBTC : Client, IApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCoinmarketcapProBTC"/> class.
        /// </summary>
        public ApiCoinmarketcapProBTC() : base(CoinMarketCapPro.Types.ApiSchema.Pro)
        {
        }

        /// <summary>
        /// Gets the ApiInfo
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Liefert aktuelle Daten zu Kryptowährungen.\n\n" +
                    "+ unterstützt kurze Update\n" +
                    "  Intervalle (ab 1 Minute)\n\n" +
                    "+ unterstützt prozentuale 24h\n" +
                    "  und 7-Tage Preisänderung\n\n" +
                    "- Registrierung und API Key nötig\n\n" +
                    "Basiswährung: BTC",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro - BTC",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen...",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    StdUpdateInterval = 180,
                    MaxUpdateInterval = 1800,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 30,
                    SupportedConvertCurrencies = new List<string>() { "BTC" },
                    UpdateIntervalInfoText = "Diese API erlaubt Update Intervalle ab 1 Minute." 
                };
            }
        }
    }
}

using AssetWatch;
using System;
using System.Collections.Generic;

namespace ApiCoinmarketcap
{
    /// <summary>
    /// Defines the <see cref="ApiCoinmarketcapProUSD" />
    /// </summary>
    public class ApiCoinmarketcapProUSD : Client, IApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCoinmarketcapProUSD"/> class.
        /// </summary>
        public ApiCoinmarketcapProUSD() : base(CoinMarketCapPro.Types.ApiSchema.Pro)
        {
            this.ApiData = new ApiData
            {
                ApiName = this.ApiInfo.ApiName,
                UpdateInterval = 180
            };
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
                    ApiInfoText = "Liefert aktuelle Daten zu den meisten Kryptowährungen.\n\n" +
                    "+ unterstützt kurzes Update\n" +
                    "  Intervall (1 Minute)\n\n" +
                    "+ unterstützt prozentuale 24h\n" +
                    "  und 7-Tage Preisentwicklung\n\n" +
                    "- Registrierung und API Key nötig\n\n" +
                    "Basiswährung: USD",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro - USD",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen...",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    MaxUpdateInterval = 1800,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 30,
                    SupportedConvertCurrencies = new List<string>() { "USD" },
                    UpdateIntervalInfoText = "Diese API stellt einmal pro Minute aktualisierte Daten bereit."
                };
            }
        }
    }
}

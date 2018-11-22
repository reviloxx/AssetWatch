using AssetWatch;
using System;
using System.Collections.Generic;

namespace ApiCoinmarketcap
{
    /// <summary>
    /// Defines the <see cref="ApiCoinmarketcapProEUR" />
    /// </summary>
    public class ApiCoinmarketcapProEUR : Client, IApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCoinmarketcapProEUR"/> class.
        /// </summary>
        public ApiCoinmarketcapProEUR() : base(CoinMarketCapPro.Types.ApiSchema.Pro)
        {
            this.ApiData = new ApiData
            {
                ApiKey = string.Empty,
                ApiName = this.ApiInfo.ApiName,
                CallCountStartTime = DateTime.Now,
                CallCount = 0,
                IsEnabled = false,
                UpdateInterval = 300
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
                    ApiInfoText = "Diese API bietet minütlich aktuelle Daten über die wichtigsten Kryptowährungen. Unterstützt nur EUR als Basiswährung.",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro - EUR",
                    ApiClientVersion = "1.0",
                    Market = Market.Cryptocurrencies,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    MaxUpdateInterval = 3600,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 60,
                    SupportedConvertCurrencies = new List<string>() { "EUR" },
                    UpdateIntervalInfoText = "Diese API stellt einmal pro Minute aktualisierte Daten bereit."
                };
            }
        }
    }
}

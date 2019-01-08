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
        public ApiCoinmarketcapProEUR() : base(CoinMarketCapPro.Types.ApiSchema.Pro) { }

        /// <summary>
        /// Gets the ApiInfo
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "\n" +
                    " - Min. Update Intervall:\n" +
                    "     1 Minute\n\n" +
                    " - Prozentuale Preisänderung:\n" +
                    "     24h, 7d\n\n" +
                    " - API Key nötig:\n" +
                    "     ja\n\n" +
                    " - Basiswährungen:\n" +
                    "     EUR",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro - EUR",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen...",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    StdUpdateInterval = 180,
                    MaxUpdateInterval = 1800,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 30,
                    SupportedConvertCurrencies = new List<string>() { "EUR" },
                    UpdateIntervalInfoText = "Diese API erlaubt Update Intervalle ab 1 Minute."
                };
            }
        }
    }
}

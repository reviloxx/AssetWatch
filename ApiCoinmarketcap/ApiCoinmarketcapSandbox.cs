using AssetWatch;
using System;
using System.Collections.Generic;

namespace ApiCoinmarketcap
{
    /// <summary>
    /// Defines the <see cref="ApiCoinmarketcapSandbox" />
    /// </summary>
    public class ApiCoinmarketcapSandbox : Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCoinmarketcapSandbox"/> class.
        /// </summary>
        public ApiCoinmarketcapSandbox() : base(CoinMarketCapPro.Types.ApiSchema.Sandbox)
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
                    ApiInfoText = "Diese API stellt veraltete Testdaten ohne Abruf-Limit zur Verfügung.",
                    ApiKeyRequired = false,
                    ApiName = "Coinmarketcap Sandbox",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    StdUpdateInterval = 300,
                    MaxUpdateInterval = 1800,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 30,
                    SupportedConvertCurrencies = new List<string>() { "AUD", "BRL", "CAD", "CHF", "CLP", "CNY",
                        "CZK", "DKK", "EUR", "GBP", "HKD", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP",
                        "PKR", "PLN", "RUB", "SEK", "SGD", "THB", "TRY", "TWD", "USD", "ZAR", "BTC", "ETH", "XRP", "LTC", "BCH" },
                    UpdateIntervalInfoText = "Diese API stellt keine aktuellen Daten bereit, eine Änderung des Update Intervalls hat daher keine Auswirkung."
                };
            }
        }
    }
}

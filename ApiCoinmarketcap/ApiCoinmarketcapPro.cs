using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCoinmarketcap
{
    public class ApiCoinmarketcapPro : Client, IApi
    {
        public ApiCoinmarketcapPro() : base(CoinMarketCapPro.Types.ApiSchema.Pro)
            
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
                    ApiInfoText = "Diese API bietet alle 5 Minuten aktuelle Daten über die wichtigsten Kryptowährungen.",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro",
                    ApiClientVersion = "1.0",
                    Market = Market.Cryptocurrencies,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    MaxUpdateInterval = 3600,
                    MinUpdateInterval = 300,
                    UpdateIntervalStepSize = 300,
                    SupportedConvertCurrencies = new List<string>() { "AUD", "BRL", "CAD", "CHF", "CLP", "CNY",
                    "CZK", "DKK", "EUR", "GBP", "HKD", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP",
                    "PKR", "PLN", "RUB", "SEK", "SGD", "THB", "TRY", "TWD", "USD", "ZAR", "BTC", "ETH", "XRP", "LTC", "BCH" },
                    UpdateIntervalInfoText = "Diese API stellt alle 5 Minuten aktualisierte Daten bereit."
                };
            }
        }
    }
}

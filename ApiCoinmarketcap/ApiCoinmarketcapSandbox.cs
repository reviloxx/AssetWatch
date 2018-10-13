using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCoinmarketcap
{
    public class ApiCoinmarketcapSandbox : Client, IApi
    {
        public ApiCoinmarketcapSandbox() : base(CoinMarketCapPro.Types.ApiSchema.Sandbox)
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
                    Market = Market.Cryptocurrencies,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    MaxUpdateInterval = 3600,
                    MinUpdateInterval = 300,
                    UpdateIntervalStepSize = 300,
                    SupportedConvertCurrencies = new List<string>() { "AUD", "BRL", "CAD", "CHF", "CLP", "CNY",
                        "CZK", "DKK", "EUR", "GBP", "HKD", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP",
                        "PKR", "PLN", "RUB", "SEK", "SGD", "THB", "TRY", "TWD", "USD", "ZAR", "BTC", "ETH", "XRP", "LTC", "BCH" },
                    UpdateIntervalInfoText = "Diese API stellt keine aktuellen Daten bereit, eine Änderung des Update Intervalls hat daher keine Auswirkung."
                };
            }
        }
    }    
}

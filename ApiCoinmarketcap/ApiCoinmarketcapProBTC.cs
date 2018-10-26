using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCoinmarketcap
{
    public class ApiCoinmarketcapProBTC : Client, IApi
    {
        public ApiCoinmarketcapProBTC() : base(CoinMarketCapPro.Types.ApiSchema.Pro)
            
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
                    ApiInfoText = "Diese API bietet alle 5 Minuten aktuelle Daten über die wichtigsten Kryptowährungen. Unterstützt nur BTC als Basiswährung.",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro - BTC",
                    ApiClientVersion = "1.0",
                    Market = Market.Cryptocurrencies,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    MaxUpdateInterval = 3600,
                    MinUpdateInterval = 300,
                    UpdateIntervalStepSize = 300,
                    SupportedConvertCurrencies = new List<string>() { "BTC" },
                    UpdateIntervalInfoText = "Diese API stellt alle 5 Minuten aktualisierte Daten bereit."
                };
            }
        }
    }
}

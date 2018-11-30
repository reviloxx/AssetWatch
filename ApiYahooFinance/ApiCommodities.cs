using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiYahooFinance
{
    public class ApiCommodities : Client, IApi
    {
        public ApiCommodities()
        {
            this.ApiData = new ApiData
            {
                ApiName = this.ApiInfo.ApiName,
                UpdateInterval = 3600
            };

            this.availableAssets = new List<Asset>()
            {
                new Asset()
                {
                    AssetId = "GC=F",
                    Symbol = "GC=F",
                    Name = "Gold"
                },
                new Asset()
                {
                    AssetId = "SI=F",
                    Symbol = "SI=F",
                    Name = "Silver"
                },
                new Asset()
                {
                    AssetId = "CL=F",
                    Symbol = "CL=F",
                    Name = "Crude Oil"
                },
                new Asset()
                {
                    AssetId = "SPY",
                    Symbol = "SPY",
                    Name = "SPDR S&P 500"
                },
            }.OrderBy(a => a.Symbol).ToList();
        }

        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Liefert aktuelle Daten zu den wichtigsten Rohstoffen. Unterstützt keine prozentuale 24 Stunden / 7-Tage Preisentwicklung.",
                    ApiKeyRequired = false,
                    ApiName = "Yahoo! Finance - Commodities",
                    ApiClientVersion = "1.0",
                    Market = Market.Rohstoffe,
                    AssetUrl = "https://finance.yahoo.com/quote/#SYMBOL#",
                    AssetUrlName = "Auf finance.yahoo.com anzeigen...",
                    GetApiKeyUrl = "",
                    MaxUpdateInterval = 7200,
                    MinUpdateInterval = 1800,
                    UpdateIntervalStepSize = 1800,
                    SupportedConvertCurrencies = new List<string>() { "USD" },
                    UpdateIntervalInfoText = ""
                };
            }
        }
    }
}

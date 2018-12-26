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
            // TODO: ADD more commodities
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
                    ApiInfoText = "Liefert aktuelle Daten zu Rohstoffen.\n\n" +
                    "+ Update Intervall ab 15 Minuten\n\n" +
                    "- unterstützt keine prozentuale\n" +
                    "  24h Preisentwicklung\n\n" +
                    "- unterstützt keine prozentuale\n" +
                    "  7-Tage Preisentwicklung\n\n" +
                    "Basiswährungen: USD, EUR",
                    ApiKeyRequired = false,
                    ApiName = "Yahoo! Finance - Commodities",
                    ApiClientVersion = "1.0",
                    Market = Market.Rohstoffe,
                    AssetUrl = "https://finance.yahoo.com/quote/#SYMBOL#",
                    AssetUrlName = "Auf finance.yahoo.com anzeigen...",
                    GetApiKeyUrl = "",
                    StdUpdateInterval = 1800,
                    MaxUpdateInterval = 7200,
                    MinUpdateInterval = 900,
                    UpdateIntervalStepSize = 900,
                    SupportedConvertCurrencies = new List<string>() { "USD", "EUR" },
                    UpdateIntervalInfoText = "Diese API erlaubt Update Intervalle ab 15 Minuten."
                };
            }
        }
    }
}

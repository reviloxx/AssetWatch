using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiYahooFinance
{
    public class ApiETFs : Client, IApi
    {
        public ApiETFs()
        {
            // TODO: ADD FEATURE: more ETFs
            this.ApiData = new ApiData
            {
                ApiName = this.ApiInfo.ApiName,
                UpdateInterval = 3600
            };

            this.availableAssets = new List<Asset>()
            {
                new Asset()
                {
                    AssetId = "H4Z3.SG",
                    Symbol = "H4Z3.SG",
                    Name = "HSBC MSCI WORLD UCITS"
                },
                new Asset()
                {
                    AssetId = "IDX",
                    Symbol = "IDX",
                    Name = "VanEck Vectors Indonesia"
                },
                new Asset()
                {
                    AssetId = "FTHI",
                    Symbol = "FTHI",
                    Name = "First Trust BuyWrite Income"
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
                    ApiInfoText = "Liefert aktuelle Daten zu den wichtigsten ETFs.\n\n" +
                    "+ Update Intervall ab 30 Minuten\n\n" +
                    "- unterstützt keine prozentuale\n" +
                    "  24h Preisentwicklung\n\n" +
                    "- unterstützt keine prozentuale\n" +
                    "  7-Tage Preisentwicklung\n\n" +
                    "Basiswährungen: USD, EUR",
                    ApiKeyRequired = false,
                    ApiName = "Yahoo! Finance - Top ETFs",
                    ApiClientVersion = "1.0",
                    Market = Market.ETFs,
                    AssetUrl = "https://finance.yahoo.com/quote/#SYMBOL#",
                    AssetUrlName = "Auf finance.yahoo.com anzeigen...",
                    GetApiKeyUrl = "",
                    MaxUpdateInterval = 7200,
                    MinUpdateInterval = 1800,
                    UpdateIntervalStepSize = 1800,
                    SupportedConvertCurrencies = new List<string>() { "USD", "EUR" },
                    UpdateIntervalInfoText = ""
                };
            }
        }        
    }
}

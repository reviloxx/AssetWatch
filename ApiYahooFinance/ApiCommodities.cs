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
                new Asset
                {

                },
                new Asset()
                {
                    AssetId = "EWG2.SG",
                    Symbol = "EWG2.SG",
                    Name = "EUWAX Gold II"
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
                    ApiInfoText = "\n" +
                    " - Min. Update Intervall:\n" +
                    "     15 Minuten\n\n" +
                    " - Prozentuale Preisänderung:\n" +
                    "     nicht unterstützt\n\n" +
                    " - API Key nötig:\n" +
                    "     nein\n\n" +
                    " - Basiswährungen:\n" +
                    "     USD, EUR",
                    ApiKeyRequired = false,
                    ApiName = "Yahoo! Finance - Commodities",
                    ApiClientVersion = "1.0",
                    Market = Market.Rohstoffe,
                    AssetUrl = "https://finance.yahoo.com/quote/#SYMBOL#",
                    AssetUrlName = "auf finance.yahoo.com anzeigen...",
                    GetApiKeyUrl = "",
                    StdUpdateInterval = 1800,
                    MaxUpdateInterval = 7200,
                    MinUpdateInterval = 900,
                    UpdateIntervalStepSize = 900,
                    SupportedConvertCurrencies = new List<string>() { "USD", "EUR" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall ab 15 Minuten."
                };
            }
        }
    }
}

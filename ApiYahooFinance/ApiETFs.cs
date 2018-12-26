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
            // TODO: ADD more ETFs
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
                    AssetId = "IQQ6.SG",
                    Symbol = "IQQ6.SG",
                    Name = "iShares Dev Mkts Prpty Yld"
                },
                new Asset()
                {
                    AssetId = "IQQ4.SG",
                    Symbol = "IQQ4.SG",
                    Name = "iShares Asia Property Yield"
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
                new Asset()
                {
                    AssetId = "ISPA.DE",
                    Symbol = "ISPA.DE",
                    Name = "iShares STOXX Global Sel Div 100"
                },
                new Asset()
                {
                    AssetId = "IUS7.SG",
                    Symbol = "IUS7.SG",
                    Name = "iShares J.P. Morgan Em Mkts Bond"
                },
            }.OrderBy(a => a.Symbol).ToList();            
        }

        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Liefert aktuelle Daten zu ETFs.\n\n" +
                    "+ Update Intervall ab 15 Minuten\n\n" +
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

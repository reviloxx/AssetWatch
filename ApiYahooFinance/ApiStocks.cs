using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiYahooFinance
{
    public class ApiStocks : Client, IApi
    {
        public ApiStocks()
        {
            // TODO: ADD more stocks
            this.availableAssets = new List<Asset>()
            {
                new Asset()
                {
                    AssetId = "GE",
                    Symbol = "GE",
                    Name = "General Electric Company"
                },
                new Asset()
                {
                    AssetId = "TWTR",
                    Symbol = "TWTR",
                    Name = "Twitter, Inc."
                },
                new Asset()
                {
                    AssetId = "BAC",
                    Symbol = "BAC",
                    Name = "Bank of America Corporation"
                },
                new Asset()
                {
                    AssetId = "ABEV",
                    Symbol = "ABEV",
                    Name = "Ambev S.A."
                },
                new Asset()
                {
                    AssetId = "SIRI",
                    Symbol = "SIRI",
                    Name = "Sirius XM Holdings Inc."
                },
                new Asset()
                {
                    AssetId = "T",
                    Symbol = "T",
                    Name = "AT&T Inc."
                },
                new Asset()
                {
                    AssetId = "F",
                    Symbol = "F",
                    Name = "Ford Motor Company"
                },
                new Asset()
                {
                    AssetId = "VALE",
                    Symbol = "VALE",
                    Name = "Vale S.A."
                },
                new Asset()
                {
                    AssetId = "QCOM",
                    Symbol = "QCOM",
                    Name = "QUALCOMM Incorporated"
                },
                new Asset()
                {
                    AssetId = "VALE",
                    Symbol = "VALE",
                    Name = "Vale S.A."
                },
                new Asset()
                {
                    AssetId = "PFE",
                    Symbol = "PFE",
                    Name = "Pfizer Inc."
                },
                new Asset()
                {
                    AssetId = "CHK",
                    Symbol = "CHK",
                    Name = "Chesapeake Energy Corporation"
                },
                new Asset()
                {
                    AssetId = "INTC",
                    Symbol = "INTC",
                    Name = "Intel Corporation"
                },
                new Asset()
                {
                    AssetId = "CSCO",
                    Symbol = "CSCO",
                    Name = "Cisco Systems, Inc."
                },
                new Asset()
                {
                    AssetId = "PBR",
                    Symbol = "INTC",
                    Name = "Petróleo Brasileiro S.A. - Petrobras"
                },
                new Asset()
                {
                    AssetId = "BABA",
                    Symbol = "BABA",
                    Name = "Alibaba Group Holding Limited"
                },
                new Asset()
                {
                    AssetId = "ECA",
                    Symbol = "ECA",
                    Name = "Encana Corporation"
                },
                new Asset()
                {
                    AssetId = "FCX",
                    Symbol = "FCX",
                    Name = "Freeport-McMoRan Inc."
                },
                new Asset()
                {
                    AssetId = "SWN",
                    Symbol = "SWN",
                    Name = "Southwestern Energy Company"
                },
                new Asset()
                {
                    AssetId = "NOK",
                    Symbol = "NOK",
                    Name = "Nokia Corporation"
                },
                new Asset()
                {
                    AssetId = "FOXA",
                    Symbol = "FOXA",
                    Name = "Twenty-First Century Fox, Inc."
                },
                new Asset()
                {
                    AssetId = "HPQ",
                    Symbol = "HPQ",
                    Name = "HP Inc."
                },
                new Asset()
                {
                    AssetId = "ORCL",
                    Symbol = "ORCL",
                    Name = "Oracle Corporation"
                },
                new Asset()
                {
                    AssetId = "BBD",
                    Symbol = "BBD",
                    Name = "Banco Bradesco S.A."
                },
                new Asset()
                {
                    AssetId = "NYCB",
                    Symbol = "NYCB",
                    Name = "New York Community Bancorp, Inc."
                },
                new Asset()
                {
                    AssetId = "KMI",
                    Symbol = "KMI",
                    Name = "Kinder Morgan, Inc."
                },

                new Asset()
                {
                    AssetId = "MO",
                    Symbol = "MO",
                    Name = "Altria Group, Inc."
                },
                new Asset()
                {
                    AssetId = "GGB",
                    Symbol = "GGB",
                    Name = "Gerdau S.A."
                },
                new Asset()
                {
                    AssetId = "WFC",
                    Symbol = "WFC",
                    Name = "Wells Fargo & Company"
                },
                new Asset()
                {
                    AssetId = "VZ",
                    Symbol = "VZ",
                    Name = "Verizon Communications Inc."
                },
                new Asset()
                {
                    AssetId = "RIG",
                    Symbol = "RIG",
                    Name = "Transocean Ltd."
                },
                new Asset()
                {
                    AssetId = "ITUB",
                    Symbol = "ITUB",
                    Name = "Itaú Unibanco Holding S.A."
                },
                new Asset()
                {
                    AssetId = "PBR-A",
                    Symbol = "PBR-A",
                    Name = "Petróleo Brasileiro S.A. - Petrobras"
                },
                new Asset()
                {
                    AssetId = "RF",
                    Symbol = "RF",
                    Name = "Regions Financial Corporation"
                },
                new Asset()
                {
                    AssetId = "P",
                    Symbol = "P",
                    Name = "Pandora Media, Inc."
                },
                new Asset()
                {
                    AssetId = "SQ",
                    Symbol = "SQ",
                    Name = "Square, Inc."
                },
                new Asset()
                {
                    AssetId = "SLB",
                    Symbol = "SLB",
                    Name = "Schlumberger Limited"
                },
                new Asset()
                {
                    AssetId = "NIO",
                    Symbol = "NIO",
                    Name = "NIO Inc."
                },
                new Asset()
                {
                    AssetId = "C",
                    Symbol = "C",
                    Name = "Citigroup Inc."
                },
                new Asset()
                {
                    AssetId = "SRC",
                    Symbol = "SRC",
                    Name = "Spirit Realty Capital, Inc."
                },
                new Asset()
                {
                    AssetId = "ET",
                    Symbol = "ET",
                    Name = "Energy Transfer LP"
                },
                new Asset()
                {
                    AssetId = "CX",
                    Symbol = "CX",
                    Name = "CEMEX, S.A.B. de C.V."
                },
                new Asset()
                {
                    AssetId = "NLY",
                    Symbol = "NLY",
                    Name = "Annaly Capital Management, Inc."
                },
                new Asset()
                {
                    AssetId = "INFY",
                    Symbol = "INFY",
                    Name = "Infosys Limited"
                },
                new Asset()
                {
                    AssetId = "KGC",
                    Symbol = "KGC",
                    Name = "Kinross Gold Corporation"
                },
                new Asset()
                {
                    AssetId = "ABX",
                    Symbol = "ABX",
                    Name = "Barrick Gold Corporation"
                },
                new Asset()
                {
                    AssetId = "DLTR",
                    Symbol = "DLTR",
                    Name = "Dollar Tree, Inc."
                },
                new Asset()
                {
                    AssetId = "AET",
                    Symbol = "AET",
                    Name = "Aetna Inc."
                },
                new Asset()
                {
                    AssetId = "KO",
                    Symbol = "KO",
                    Name = "The Coca-Cola Company"
                },
                new Asset()
                {
                    AssetId = "DB",
                    Symbol = "DB",
                    Name = "Deutsche Bank Aktiengesellschaft"
                },
                new Asset()
                {
                    AssetId = "FDC",
                    Symbol = "FDC",
                    Name = "First Data Corporation"
                }
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
                    ApiName = "Yahoo! Finance - Top Stocks",
                    ApiClientVersion = "1.0",
                    Market = Market.Aktien,
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

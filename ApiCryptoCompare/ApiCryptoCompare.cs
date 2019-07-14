using AssetWatch;
using CryptoCompare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiCryptoCompare
{
    // TODO: ApiCryptoCompare is currently not working

    /// <summary>
    /// Defines the <see cref="ApiCryptoCompare" />
    /// </summary>
    public class ApiCryptoCompare : Api
    {
        /// <summary>
        /// Defines the CryptoCompare client.
        /// </summary>
        private CryptoCompareClient client;

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            this.client = new CryptoCompareClient();

            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

        /// <summary>
        /// Requests the available assets of this API.
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            Task.Run(() => this.GetAvailableAssetsAsync());
        }
        
        /// <summary>
        /// Gets the available assets of the API and fires the OnAvailableAssetsReceived event if successful.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        private async Task GetAvailableAssetsAsync()
        {
            bool callFailed = false;
            List<Asset> availableAssets = new List<Asset>();

            do
            {
                callFailed = false;

                try
                {
                    CoinListResponse response = await this.client.Coins.ListAsync();
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    foreach (KeyValuePair<string, CoinInfo> coin in response.Coins)
                    {
                        this.availableAssets.Add(
                            new Asset
                            {
                                AssetId = coin.Value.Id,
                                Name = coin.Value.Name,
                                Symbol = coin.Value.Symbol
                            });
                    }

                    this.availableAssets = this.availableAssets.OrderBy(ass => ass.SymbolName).ToList();
                    this.FireOnAvailableAssetsReceived();
                }
                catch (HttpRequestException)
                {
                    callFailed = true;
                    Thread.Sleep(retryDelay);
                }
            }
            while (callFailed);
        }
        
        /// <summary>
        /// Gets updates for a list of assets.
        /// Fires the OnSingleAssetUpdated event for each updated asset from the list.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        /// <param name="assets">The assets<see cref="List{Asset}"/> to update.</param>
        protected override async Task GetAssetUpdatesAsync(List<Asset> assets)
        {
            if (assets.Count < 1)
            {
                return;
            }

            List<string> fromSymbols = new List<string>();
            assets.ForEach(ass => fromSymbols.Add(ass.Symbol));

            bool callFailed = false;
            int timeout = this.ApiData.UpdateInterval * 1000;

            do
            {
                callFailed = false;

                try
                {
                    PriceMultiFullResponse response = await this.client.Prices.MultipleSymbolFullDataAsync(fromSymbols, this.attachedConvertCurrencies);
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    var res = response.Raw;

                    if (res == null)
                    {
                        return;
                    }

                    assets.ForEach(ass =>
                    {
                        this.attachedConvertCurrencies.ForEach(con =>
                        {
                            try
                            {
                                CoinFullAggregatedData data = res[ass.Symbol][con];

                                if (ass.ConvertCurrency == con)
                                {
                                    ass.Price = (double)data.Price;
                                    ass.LastUpdated = DateTime.Now;
                                    ass.PercentChange24h = data.ChangePCT24Hour != null? (double)data.ChangePCT24Hour : -101;
                                    ass.PercentChange24h = -101;
                                    ass.PercentChange1h = -101;
                                    ass.MarketCap = data.MarketCap != null? (double)data.MarketCap : -1;
                                    ass.SupplyAvailable = -1;
                                    ass.SupplyTotal = -1;
                                    ass.Volume24hConvert = data.TotalVolume24HTo != null? data.TotalVolume24HTo.ToString() : "-";

                                    this.FireOnAssetUpdateReceived(ass);
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                // ignore
                            }                            
                        });
                    });
                }
                catch (HttpRequestException)
                {
                    // no internet connection?                    
                    callFailed = true;
                    Thread.Sleep(retryDelay);
                    timeout -= retryDelay;
                }
                catch (Exception e)
                {
                    OnApiErrorEventArgs args = new OnApiErrorEventArgs
                    {
                        ErrorType = ErrorType.General,
                        ErrorMessage = e.Message
                    };

                    this.FireOnApiError(args);
                }
            }
            while (callFailed && timeout >= 0);
        }        

        /// <summary>
        /// Gets the ApiInfo.
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "\n" +
                    " - Min. Update Intervall:\n" +
                    "     15 Sekunden\n\n" +
                    " - Prozentuale Preisänderung:\n" +
                    "     24h\n\n" +
                    " - API Key nötig:\n" +
                    "     nein\n\n" +
                    " - Basiswährungen:\n" +
                    "     USD, EUR, BTC",
                    ApiKeyRequired = false,
                    ApiName = "CryptoCompare",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://www.cryptocompare.com/coins/#SYMBOL#/overview",
                    AssetUrlName = "auf cryptocompare.com anzeigen...",
                    GetApiKeyUrl = "",
                    StdUpdateInterval = 15,
                    MaxUpdateInterval = 300,
                    MinUpdateInterval = 15,
                    UpdateIntervalStepSize = 15,
                    SupportedConvertCurrencies = new List<string>() { "EUR", "USD", "BTC" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall ab 15 Sekunden."
                };
            }
        }
    }
}

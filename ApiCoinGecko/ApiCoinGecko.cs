using AssetWatch;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Entities.Response.Simple;
using CoinGecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiCoinGecko
{
    public class ApiCoinGecko : AssetWatch.Api, IApi
    {
        /// <summary>
        /// Defines the CoinGecko client.
        /// </summary>
        private ICoinGeckoClient client;

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            this.client = new CoinGeckoClient();

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
                    IReadOnlyList<CoinList> response = await this.client.CoinsClient.GetCoinList();
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    foreach (CoinList coin in response)
                    {
                        availableAssets.Add(
                            new Asset
                            {
                                AssetId = coin.Id,
                                Name = coin.Name,
                                Symbol = coin.Symbol
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

            List<string> fromIds = new List<string>();
            assets.ForEach(ass => fromIds.Add(ass.AssetId));

            bool callFailed = false;
            int timeout = this.ApiData.UpdateInterval * 1000;

            do
            {
                callFailed = false;

                try
                {
                    Price response = await this.client.SimpleClient.GetSimplePrice(fromIds.ToArray(), this.attachedConvertCurrencies.ToArray());
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();
                    


                    assets.ForEach(ass =>
                    {
                        try
                        {
                            var assres = response[ass.AssetId];

                            if (assres.ContainsKey(ass.ConvertCurrency.ToLower()))
                            {
                                ass.LastUpdated = DateTime.Now;
                                ass.Price = (double)assres[ass.ConvertCurrency.ToLower()];
                                //ass.PercentChange24h = (double)assres[ass.ConvertCurrency.ToLower() + "_24h_change"];
                                this.FireOnAssetUpdateReceived(ass);
                            }                            
                        }
                        catch (KeyNotFoundException)
                        {
                            // ignore
                        }
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
                    "     1 Minute\n\n" +
                    " - Prozentuale Preisänderung:\n" +
                    "     nicht unterstützt\n\n" +
                    " - API Key nötig:\n" +
                    "     nein\n\n" +
                    " - Basiswährungen:\n" +
                    "     USD, EUR, BTC, ETH, EOS",
                    ApiKeyRequired = false,
                    ApiName = "CoinGecko",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://www.coingecko.com/en/coins/#ID#",
                    AssetUrlName = "auf coingecko.com anzeigen...",
                    GetApiKeyUrl = "",
                    StdUpdateInterval = 180,
                    MaxUpdateInterval = 1800,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 30,
                    SupportedConvertCurrencies = new List<string>() { "EUR", "USD", "BTC", "ETH", "EOS" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall ab 1 Minute."
                };
            }
        }
    }
}

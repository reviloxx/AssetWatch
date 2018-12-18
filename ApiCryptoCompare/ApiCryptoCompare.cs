using AssetWatch;
using CryptoCompare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ApiCryptoCompare
{
    /// <summary>
    /// Defines the <see cref="ApiCryptoCompare" />
    /// </summary>
    public class ApiCryptoCompare : IApi
    {
        /// <summary>
        /// Defines the retryDelay in case there is no connection.
        /// </summary>
        private static int retryDelay = 1000;

        /// <summary>
        /// Defines the client.
        /// </summary>
        private CryptoCompareClient client;

        /// <summary>
        /// Defines the assetUpdateWorker thread.
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// The AssetRequestDelegate.
        /// </summary>
        private delegate void AssetRequestDelegate();

        /// <summary>
        /// Defines the assetRequestDelegate.
        /// </summary>
        private AssetRequestDelegate assetRequestDelegate;

        /// <summary>
        /// Defines the list of available assets.
        /// </summary>
        private List<Asset> availableAssets;

        /// <summary>
        /// Defines the list of subscribed assets.
        /// </summary>
        private List<Asset> subscribedAssets;

        /// <summary>
        /// Defines the list of subscribed convert currencies.
        /// </summary>
        private List<string> subscribedConvertCurrencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCryptoCompare"/> class.
        /// </summary>
        public ApiCryptoCompare()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetRequestDelegate = new AssetRequestDelegate(this.GetAvailableAssets);
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
            this.subscribedConvertCurrencies = new List<string>();
            this.ApiData = new ApiData
            {
                ApiName = this.ApiInfo.ApiName,
                UpdateInterval = 15
            };
        }

        /// <summary>
        /// Disables the API.
        /// </summary>
        public void Disable()
        {
            this.StopAssetUpdater();
            this.ApiData.IsEnabled = false;
        }

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            this.client = new CryptoCompareClient();
            this.ApiData.IsEnabled = true;
            this.StartAssetUpdater();
        }

        /// <summary>
        /// Requests the available assets of this API.
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            this.assetRequestDelegate.BeginInvoke(null, null);
        }

        /// <summary>
        /// Requests a single asset update.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to update.</param>
        public void RequestSingleAssetUpdateAsync(Asset asset)
        {
            if (!this.assetUpdateWorker.IsAlive)
            {
                return;
            }

            List<Asset> assets = new List<Asset>();
            assets.Add(asset);
            this.GetAssetUpdates(assets);
        }

        /// <summary>
        /// Starts the asset updater.
        /// </summary>
        private void StartAssetUpdater()
        {
            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

        /// <summary>
        /// Subscribes an asset to the updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to subscribe.</param>
        public void SubscribeAssetToUpdater(Asset asset)
        {
            if (!this.subscribedAssets.Exists(sub => sub.Symbol == asset.Symbol && sub.ConvertCurrency == asset.ConvertCurrency))
            {
                this.subscribedAssets.Add(asset);
            }

            if (!this.subscribedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                this.subscribedConvertCurrencies.Add(asset.ConvertCurrency);
            }
        }

        /// <summary>
        /// Unsubscribes an asset from the updater.
        /// Not implemented because at this API the number of calls is not dependent on the number of subscribed assets.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to unsubscribe.</param>
        public void UnsubscribeAssetFromUpdater(Asset asset)
        {
        }

        /// <summary>
        /// Requests updates of the subscribed assets while this API is enabled.
        /// </summary>
        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                this.GetAssetUpdates(this.subscribedAssets);
                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
        }

        /// <summary>
        /// Gets updates for a list of assets.
        /// Fires the OnSingleAssetUpdated event for each updated asset from the list.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        /// <param name="assets">The assets<see cref="List{Asset}"/> to update.</param>
        private async void GetAssetUpdates(List<Asset> assets)
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
                    PriceMultiFullResponse response = await this.client.Prices.MultipleSymbolFullDataAsync(fromSymbols, this.subscribedConvertCurrencies);
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    var res = response.Raw;

                    if (res == null)
                    {
                        return;
                    }

                    assets.ForEach(ass =>
                    {
                        this.subscribedConvertCurrencies.ForEach(con =>
                        {
                            try
                            {
                                CoinFullAggregatedData data = res[ass.Symbol][con];

                                if (ass.ConvertCurrency == con)
                                {
                                    ass.LastUpdated = DateTime.Now;
                                    ass.MarketCap = (double)data.MarketCap;
                                    ass.PercentChange24h = (double)data.ChangePCT24Hour;
                                    ass.Price = (double)data.Price;
                                    ass.Volume24hConvert = data.TotalVolume24HTo.ToString();
                                    this.FireOnSingleAssetUpdated(ass);
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
        /// Stops the asset updater.
        /// </summary>
        private void StopAssetUpdater()
        {
            try
            {
                this.assetUpdateWorker.Abort();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the available assets of the API and fires the OnAvailableAssetsReceived event if successful.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        private async void GetAvailableAssets()
        {
            bool callFailed = false;

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
        /// Fires the OnAvailableAssetsReceived event.
        /// </summary>
        private void FireOnAvailableAssetsReceived()
        {
            this.OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        /// <summary>
        /// Fires the OnSingleAssetUpdated event.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        private void FireOnSingleAssetUpdated(Asset asset)
        {
            this.OnSingleAssetUpdated?.Invoke(this, asset);
        }

        /// <summary>
        /// Fires the OnApiError event.
        /// </summary>
        /// <param name="eventArgs">The eventArgs<see cref="OnApiErrorEventArgs"/></param>
        private void FireOnApiError(OnApiErrorEventArgs onApiErrorEventArgs)
        {
            this.OnApiError?.Invoke(this, onApiErrorEventArgs);
        }

        /// <summary>
        /// Fires the OnAppDataChanged event.
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Gets or sets the ApiData.
        /// </summary>
        public ApiData ApiData { get; set; }

        /// <summary>
        /// Gets the ApiInfo.
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Liefert aktuelle Daten zu den meisten Kryptowährungen.\n\n" +
                    "+ unterstützt kurzes Update\n" +
                    "  Intervall (15 Sekunden)\n\n" +
                    "+ unterstützt prozentuale\n" +
                    "  24h Preisentwicklung\n\n" +
                    "- unterstützt keine prozentuale\n" +
                    "  7-Tage Preisentwicklung\n\n" +
                    "Basiswährungen: USD, EUR, BTC",
                    ApiKeyRequired = false,
                    ApiName = "CryptoCompare",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://www.cryptocompare.com/coins/#SYMBOL#/overview",
                    AssetUrlName = "Auf cryptocompare.com anzeigen...",
                    GetApiKeyUrl = "",
                    MaxUpdateInterval = 300,
                    MinUpdateInterval = 15,
                    UpdateIntervalStepSize = 15,
                    SupportedConvertCurrencies = new List<string>() { "EUR", "USD", "BTC" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall von 15 Sekunden - 5 Minuten und erlaubt 100.000 Aufrufe pro Monat."
                };
            }
        }

        /// <summary>
        /// Defines the OnAvailableAssetsReceived event.
        /// </summary>
        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Defines the OnSingleAssetUpdated event.
        /// </summary>
        public event EventHandler<Asset> OnSingleAssetUpdated;

        /// <summary>
        /// Defines the OnApiError event.
        /// </summary>
        public event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Defines the OnAppDataChanged event.
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

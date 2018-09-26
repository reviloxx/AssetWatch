using AssetWatch;
using CoinMarketCapPro;
using CoinMarketCapPro.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace ApiCoinmarketcapSandbox
{
    /// <summary>
    /// Defines the <see cref="Client" />
    /// </summary>
    public class Client : IApi
    {
        /// <summary>
        /// Defines the apiSchema
        /// </summary>
        private static ApiSchema apiSchema = ApiSchema.Sandbox;

        private static string apiKey = "29bc6cc3-7219-42f6-af87-f0147e9ee089";

        /// <summary>
        /// Defines the availableAssets
        /// </summary>
        private List<Asset> availableAssets;

        /// <summary>
        /// Gets the SubscribedAssets
        /// </summary>
        private List<Asset> subscribedAssets;

        /// <summary>
        /// Defines the subscribedConvertCurrencies
        /// </summary>
        private List<string> subscribedConvertCurrencies;

        /// <summary>
        /// The AssetRequestDelegate
        /// </summary>
        private delegate void AssetRequestDelegate();

        /// <summary>
        /// Defines the assetRequestDelegate
        /// </summary>
        private AssetRequestDelegate assetRequestDelegate;

        /// <summary>
        /// Defines the client
        /// </summary>
        private CoinMarketCapClient client;

        /// <summary>
        /// Defines the assetUpdateWorker
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Gets the ApiData
        /// </summary>
        public ApiData ApiData { get; set; }

        /// <summary>
        /// Gets the ApiInfo
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Diese API stellt veraltete Testdaten ohne Abruf-Limit zur Verfügung.",
                    ApiKeyRequired = false,
                    ApiName = "Coinmarketcap Sandbox",
                    ApiClientVersion = "1.0",
                    Market = Market.Cryptocurrencies,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    MaxUpdateInterval = 3600,
                    MinUpdateInterval = 300,
                    SupportedConvertCurrencies = new List<string>() { "AUD", "BRL", "CAD", "CHF", "CLP", "CNY",
                        "CZK", "DKK", "EUR", "GBP", "HKD", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP",
                        "PKR", "PLN", "RUB", "SEK", "SGD", "THB", "TRY", "TWD", "USD", "ZAR", "BTC", "ETH", "XRP", "LTC", "BCH" },
                    UpdateIntervalInfoText = "Diese API stellt keine aktuellen Daten bereit, eine Änderung des Update Intervalls hat daher keine Auswirkung."
                };
            }
        }

        /// <summary>
        /// Defines the OnAvailableAssetsReceived
        /// </summary>
        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Defines the OnSingleAssetUpdated
        /// </summary>
        public event EventHandler<Asset> OnSingleAssetUpdated;

        /// <summary>
        /// Defines the OnApiError
        /// </summary>
        public event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
        {
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
            this.subscribedConvertCurrencies = new List<string>();
            this.assetRequestDelegate = new AssetRequestDelegate(this.GetAvailableAssets);
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.ApiData = new ApiData
            {
                ApiKey = string.Empty,
                ApiName = this.ApiInfo.ApiName,
                CallCount1moStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                CallsLeft1mo = 6000,
                IsEnabled = false,
                UpdateInterval = 300
            };
        }

        /// <summary>
        /// The EnableApi
        /// </summary>
        public void Enable()
        {
            this.client = new CoinMarketCapClient(apiSchema, apiKey);
            this.ApiData.IsEnabled = true;
        }

        /// <summary>
        /// The DisableApi
        /// </summary>
        public void Disable()
        {
            this.ApiData.IsEnabled = false;
            this.StopAssetUpdater();
        }

        /// <summary>
        /// The RequestAvailableAssetsAsync
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            if (!this.ApiData.IsEnabled)
            {
                throw new Exception("API is not enabled!");
            }

            this.assetRequestDelegate.BeginInvoke(null, null);
        }

        /// <summary>
        /// The GetSingleAssetUpdate
        /// </summary>
        /// <param name="asset">The ass<see cref="Asset"/></param>
        private void RequestSingleAssetUpdateAsync(Asset asset)
        {
            if (!this.ApiData.IsEnabled)
            {
                throw new Exception("API is not enabled!");
            }

            List<Asset> assets = new List<Asset>();
            assets.Add(asset);
            this.RequestAssetUpdates(assets);
        }

        /// <summary>
        /// The SubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void SubscribeAssetToUpdater(Asset asset)
        {
            if (!this.subscribedAssets.Exists(sub => sub.AssetId == asset.AssetId))
            {
                this.subscribedAssets.Add(asset);
            }

            if (!this.subscribedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                this.subscribedConvertCurrencies.Add(asset.ConvertCurrency);
            }

            // request an update for this asset if the worker thread is already running, so there is no delay for receiving data for this asset
            if (this.assetUpdateWorker.IsAlive)
            {
                this.RequestSingleAssetUpdateAsync(asset);
            }
        }

        /// <summary>
        /// The UnsubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void UnsubscribeAssetFromUpdater(Asset asset)
        {
        }

        /// <summary>
        /// The SetUpdateInterval
        /// </summary>
        /// <param name="updateInterval">The updateInterval<see cref="int"/></param>
        public void SetUpdateInterval(int updateInterval)
        {
            this.ApiData.UpdateInterval = updateInterval;
        }

        /// <summary>
        /// The WaitForConnection
        /// </summary>
        private void WaitForConnection()
        {
            bool connected = false;

            do
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        using (var stream = client.OpenRead("http://www.coinmarketcap.com"))
                        {
                            connected = true;
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
            while (!connected);
        }

        /// <summary>
        /// The GetAvailableAssets
        /// </summary>
        private async void GetAvailableAssets()
        {
            this.WaitForConnection();
            try
            {
                var map = await this.client.GetCurrencyMapAsync();

                map.Data.ForEach(c =>
                {
                    this.availableAssets.Add(new Asset
                    {
                        AssetId = c.Id.ToString(),
                        LastUpdated = DateTime.Now,
                        Name = c.Name,
                        Symbol = c.Symbol
                    });
                });

                this.availableAssets.OrderBy(ass => ass.Rank);
                this.FireOnAvailableAssetsReceived();
            }
            catch (Exception e)
            {
                this.ApiData.IsEnabled = false;

                if (e.Message.Contains("401"))
                {
                    this.FireOnApiError(new OnApiErrorEventArgs
                    {
                        ErrorMessage = "API Key ungültig!",
                        ErrorType = ErrorType.Unauthorized
                    });
                }
            }
        }

        /// <summary>
        /// The FireOnAvailableAssetsReceived
        /// </summary>
        public void FireOnAvailableAssetsReceived()
        {
            OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        /// <summary>
        /// The StartAssetUpdater
        /// </summary>
        public void StartAssetUpdater()
        {
            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

        /// <summary>
        /// The StartAssetUpdater
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
        /// The AssetUpdateWorker
        /// </summary>
        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                this.RequestAssetUpdates(this.subscribedAssets);
                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
        }

        private async void RequestAssetUpdates(List<Asset> assets)
        {
            if (assets.Count < 1)
            {
                return;
            }

            this.WaitForConnection();

            List<int> ids = new List<int>();

            assets.ForEach(sub =>
            {
                if (!ids.Exists(ex => ex == int.Parse(sub.AssetId)))
                {
                    ids.Add(int.Parse(sub.AssetId));
                }
            });

            try
            {
                var a = await this.client.GetCurrencyMarketQuotesAsync(ids, this.subscribedConvertCurrencies);
                if (a.Status.ErrorCode != 0)
                {
                    // TODO: handle API error codes
                }

                assets.ForEach(ass =>
                {
                    var assetUpdate = a.Data.FirstOrDefault(d => d.Key == ass.AssetId).Value;
                    ass.PriceConvert = assetUpdate.Quote[ass.ConvertCurrency].Price.ToString();
                    ass.LastUpdated = DateTime.Now;
                    ass.MarketCapConvert = assetUpdate.Quote[ass.ConvertCurrency].MarketCap.ToString();
                    ass.PercentChange1h = assetUpdate.Quote[ass.ConvertCurrency].PercentChange1h.ToString();
                    ass.PercentChange24h = assetUpdate.Quote[ass.ConvertCurrency].PercentChange24h.ToString();
                    ass.PercentChange7d = assetUpdate.Quote[ass.ConvertCurrency].PercentChange7d.ToString();
                    ass.Rank = assetUpdate.CmcRank.ToString();
                    this.FireOnSingleAssetUpdated(ass);
                });
            }
            catch (Exception e)
            {
                OnApiErrorEventArgs eventArgs = new OnApiErrorEventArgs
                {
                    ErrorMessage = e.Message,
                    ErrorType = ErrorType.General
                };

                this.FireOnApiError(eventArgs);
                this.ApiData.IsEnabled = false;
                return;
            }

        }

        /// <summary>
        /// The FireOnApiError
        /// </summary>
        /// <param name="eventArgs">The eventArgs<see cref="OnApiErrorEventArgs"/></param>
        private void FireOnApiError(OnApiErrorEventArgs eventArgs)
        {
            this.OnApiError?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// The FireOnSingleAssetUpdated
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        private void FireOnSingleAssetUpdated(Asset asset)
        {
            OnSingleAssetUpdated?.Invoke(this, asset);
        }
    }
}

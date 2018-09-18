using AssetWatch;
using CoinMarketCapPro;
using CoinMarketCapPro.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace ApiCoinmarketcapPro
{
    /// <summary>
    /// Defines the <see cref="Client" />
    /// </summary>
    public class Client : IApi
    {
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
        /// Defines the apiKey
        /// </summary>
        private static string apiKey = "29bc6cc3-7219-42f6-af87-f0147e9ee089";

        /// <summary>
        /// Defines the apiSchema
        /// </summary>
        private static ApiSchema apiSchema = ApiSchema.Sandbox;

        /// <summary>
        /// Defines the assetUpdateWorker
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Defines the assetUpdaterRunning
        /// </summary>
        private bool assetUpdaterRunning;

        /// <summary>
        /// Gets the ApiData
        /// </summary>
        public ApiData ApiData { get; private set; }

        /// <summary>
        /// Gets the ApiInfo
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Diese API bietet alle 5 Minuten aktuelle Daten über die wichtigsten Kryptowährungen.",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro",
                    ApiClientVersion = "1.0",
                    AssetType = AssetType.Cryptocurrencies,
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                    GetApiKeyUrl = "https://pro.coinmarketcap.com/signup",
                    MaxUpdateInterval = 3600,
                    MinUpdateInterval = 300,
                    SupportedConvertCurrencies = new List<string>() { "AUD", "BRL", "CAD", "CHF", "CLP", "CNY",
                        "CZK", "DKK", "EUR", "GBP", "HKD", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP",
                        "PKR", "PLN", "RUB", "SEK", "SGD", "THB", "TRY", "TWD", "ZAR", "BTC", "ETH", "XRP", "LTC", "BCH" },
                    UpdateIntervalInfoText = "Diese API stellt alle 5 Minuten aktualisierte Daten bereit."
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
            availableAssets = new List<Asset>();
            subscribedAssets = new List<Asset>();
            subscribedConvertCurrencies = new List<string>();            
            assetRequestDelegate = new AssetRequestDelegate(GetAvailableAssets);            
            ApiData = new ApiData {
                ApiKey = string.Empty,
                ApiName = this.ApiInfo.ApiName,
                CallCount1moStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                CallsLeft1mo = 6000,
                IsEnabled = false,
                UpdateInterval = 5
            };
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
            WaitForConnection();
            var map = await client.GetCurrencyMapAsync();

            map.Data.ForEach(c =>
            {
                availableAssets.Add(new Asset
                {
                    AssetId = c.Id.ToString(),
                    LastUpdated = DateTime.Now,
                    Name = c.Name,
                    Symbol = c.Symbol
                });
            });

            FireOnAvailableAssetsReceived();
        }

        /// <summary>
        /// The FireOnAvailableAssetsReceived
        /// </summary>
        private void FireOnAvailableAssetsReceived()
        {
            OnAvailableAssetsReceived?.Invoke(this, availableAssets);
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

            assetRequestDelegate.BeginInvoke(null, null);
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
        /// The StartAssetUpdater
        /// </summary>
        private void StartAssetUpdater()
        {
            this.assetUpdateWorker = new Thread(AssetUpdateWorker);
            assetUpdateWorker.Start();
            assetUpdaterRunning = true;            
        }

        /// <summary>
        /// The StartAssetUpdater
        /// </summary>
        public void StopAssetUpdater()
        {
            try
            {
                assetUpdateWorker.Abort();
            }
            catch (Exception e) { }
            assetUpdaterRunning = false;
        }

        /// <summary>
        /// The AssetUpdateWorker
        /// </summary>
        private async void AssetUpdateWorker()
        {
            while (assetUpdaterRunning)
            {
                int startId = subscribedAssets.Min(sub => int.Parse(sub.AssetId));
                int endId = subscribedAssets.Max(sub => int.Parse(sub.AssetId));

                try
                {
                    var a = await client.GetCurrencyListingsAsync(startId, endId, subscribedConvertCurrencies);
                    if (a.Status.ErrorCode != 0)
                    {
                        // TODO: handle API error codes
                    }
                    
                    subscribedAssets.ForEach(ass =>
                    {
                        var assetUpdate = a.Data.First(upd => upd.Id.ToString() == ass.AssetId);
                        ass.PriceConvert = assetUpdate.Quote[ass.ConvertCurrency].Price.ToString();
                        ass.LastUpdated = assetUpdate.LastUpdated;
                        ass.MarketCapConvert = assetUpdate.Quote[ass.ConvertCurrency].MarketCap.ToString();
                        ass.PercentChange1h = assetUpdate.Quote[ass.ConvertCurrency].PercentChange1h.ToString();
                        ass.PercentChange24h = assetUpdate.Quote[ass.ConvertCurrency].PercentChange24h.ToString();
                        ass.PercentChange7d = assetUpdate.Quote[ass.ConvertCurrency].PercentChange7d.ToString();
                        ass.Rank = assetUpdate.CmcRank.ToString();
                        FireOnSingleAssetUpdated(ass);
                    });
                }
                catch (Exception e)
                {
                    FireOnApiError(e.Message);
                    assetUpdaterRunning = false;
                    return;
                }

                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
        }

        /// <summary>
        /// The FireOnApiError
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        private void FireOnApiError(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The FireOnSingleAssetUpdated
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        private void FireOnSingleAssetUpdated(Asset asset)
        {
            OnSingleAssetUpdated?.Invoke(this, asset);
        }

        /// <summary>
        /// The SubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void SubscribeAsset(Asset asset)
        {
            if (!subscribedAssets.Exists(sub => sub.AssetId == asset.AssetId))
            {
                subscribedAssets.Add(asset);
            }

            if (!subscribedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                subscribedConvertCurrencies.Add(asset.ConvertCurrency);
            }

            // Start the asset updater if there are subscribed assets and the API is enabled and it is not running yet
            if (subscribedAssets.Count > 0 && ApiData.IsEnabled && !assetUpdaterRunning)
            {
                StartAssetUpdater();
            }
        }

        /// <summary>
        /// The UnsubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void UnsubscribeAsset(Asset asset)
        {
        }

        /// <summary>
        /// The EnableApi
        /// </summary>
        public void EnableApi()
        {
            if (this.ApiData.ApiKey == string.Empty)
            {
                throw new Exception("API Key missing!");
            }

            client = new CoinMarketCapClient(apiSchema, this.ApiData.ApiKey);
            ApiData.IsEnabled = true;

            // Start the asset updater if there are subscribed assets and it is not running yet
            if (subscribedAssets.Count > 0 && !assetUpdaterRunning)
            {
                StartAssetUpdater();
            }
        }

        /// <summary>
        /// The DisableApi
        /// </summary>
        public void DisableApi()
        {
            ApiData.IsEnabled = false;
            assetUpdaterRunning = false;
        }
    }
}

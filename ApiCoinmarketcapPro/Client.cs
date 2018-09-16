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

        private int updateInterval;

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
                    ApiInfoText = "",
                    ApiKeyRequired = true,
                    ApiName = "Coinmarketcap Pro",
                    ApiVersion = "1.0",
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
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
            this.subscribedConvertCurrencies = new List<string>();
            this.client = new CoinMarketCapClient(apiSchema, apiKey);
            this.assetRequestDelegate = new AssetRequestDelegate(GetAvailableAssets);
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.ApiData = new ApiData();
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
            assetRequestDelegate.BeginInvoke(null, null);
        }

        /// <summary>
        /// The SetUpdateInterval
        /// </summary>
        /// <param name="updateInterval">The updateInterval<see cref="int"/></param>
        public void SetUpdateInterval(int updateInterval)
        {
            this.updateInterval = updateInterval;
        }

        /// <summary>
        /// The StartAssetUpdater
        /// </summary>
        private void StartAssetUpdater()
        {
            this.assetUpdaterRunning = true;
            this.assetUpdateWorker.Start();
        }

        /// <summary>
        /// The StartAssetUpdater
        /// </summary>
        public void StopAssetUpdater()
        {
            this.assetUpdaterRunning = false;
        }

        /// <summary>
        /// The AssetUpdateWorker
        /// </summary>
        private async void AssetUpdateWorker()
        {
            while (this.assetUpdaterRunning)
            {
                int startId = this.subscribedAssets.Min(sub => int.Parse(sub.AssetId));
                int endId = this.subscribedAssets.Max(sub => int.Parse(sub.AssetId));

                try
                {
                    var a = await this.client.GetCurrencyListingsAsync(startId, endId, this.subscribedConvertCurrencies);
                    if (a.Status.ErrorCode != 0)
                    {
                        // TODO: handle API error codes
                    }

                    this.subscribedAssets.ForEach(ass =>
                    {
                        var assetUpdate = a.Data.First(upd => upd.Id.ToString() == ass.AssetId);
                        ass.PriceConvert = assetUpdate.Quote[ass.ConvertCurrency].Price.ToString();
                        this.FireOnSingleAssetUpdated(ass);
                    });
                }
                catch(Exception e)
                {
                    this.FireOnApiError(e.Message);
                    this.assetUpdaterRunning = false;
                    return;
                }
                
                Thread.Sleep(this.updateInterval * 1000);
            }
        }

        private void FireOnApiError(string message)
        {
            throw new NotImplementedException();
        }

        private void FireOnSingleAssetUpdated(Asset asset)
        {
            this.OnSingleAssetUpdated?.Invoke(this, asset);
        }

        /// <summary>
        /// The SubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void SubscribeAsset(Asset asset)
        {
            if (!this.subscribedAssets.Exists(sub => sub.AssetId == asset.AssetId))
            {
                this.subscribedAssets.Add(asset);
            }

            if (!this.subscribedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                this.subscribedConvertCurrencies.Add(asset.ConvertCurrency);
            }

            // Start the asset updater if there are subscribed assets and the API is enabled and it is not running yet
            if (this.subscribedAssets.Count > 0 && this.ApiData.IsEnabled && !this.assetUpdaterRunning)
            {
                this.StartAssetUpdater();
            }
        }

        /// <summary>
        /// The UnsubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void UnsubscribeAsset(Asset asset)
        {
            
        }

        public void EnableApi()
        {
            this.ApiData.IsEnabled = true;

            // Start the asset updater if there are subscribed assets and it is not running yet
            if (this.subscribedAssets.Count > 0 && !this.assetUpdaterRunning)
            {
                this.StartAssetUpdater();
            }
        }

        public void DisableApi()
        {
            this.ApiData.IsEnabled = false;
            this.assetUpdaterRunning = false;
        }
    }
}

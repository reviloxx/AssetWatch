using AssetWatch;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace ApiCoinmarketcap
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
        /// Defines the subscribedAssets
        /// </summary>
        private List<Asset> subscribedAssets;

        /// <summary>
        /// The AssetRequestDelegate
        /// </summary>
        private delegate void AssetRequestDelegate();

        /// <summary>
        /// Defines the assetRequestDelegate
        /// </summary>
        private AssetRequestDelegate assetRequestDelegate;

        /// <summary>
        /// Defines the assetUpdateWorker
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Defines the updateInterval
        /// </summary>
        private int updateInterval;

        /// <summary>
        /// Gets the SubscribedAssets
        /// </summary>
        public List<Asset> SubscribedAssets
        {
            get
            {
                return subscribedAssets;
            }
            private set { }
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
        public event EventHandler OnApiError;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
        {
            assetRequestDelegate = new AssetRequestDelegate(GetAvailableAssets);
            assetUpdateWorker = new Thread(AssetUpdateWorker);
            availableAssets = new List<Asset>();
            subscribedAssets = new List<Asset>();
        }

        /// <summary>
        /// Gets the ApiInfo
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Warnung: Die öffentliche Coinmaketcap API wird am 4.12.2018 deaktiviert.",
                    ApiKeyRequired = false,
                    ApiName = "Coinmarketcap.com (public)",
                    ApiVersion = "1.0",
                    AssetUrl = "https://coinmarketcap.com/currencies/#NAME#/",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
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
        /// Gets the CallsLeft24h
        /// </summary>
        public int CallsLeft24h
        {
            get { return -1; }
        }

        /// <summary>
        /// Gets the ApiData
        /// </summary>
        public ApiData ApiData => throw new NotImplementedException();

        /// <summary>
        /// The RequestAvailableAssetsAsync
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            assetRequestDelegate.BeginInvoke(null, null);
        }

        /// <summary>
        /// The GetAvailableAssets
        /// </summary>
        private void GetAvailableAssets()
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

            ICoinmarketcapClient cmcClient = new CoinmarketcapClient();
            List<Currency> currencies = (List<Currency>)cmcClient.GetCurrencies();

            currencies.ForEach(currency =>
            {
                availableAssets.Add(new Asset()
                {
                    SupplyAvailable = currency.AvailableSupply,
                    AssetId = currency.Id,
                    LastUpdated = DateTime.Now,
                    MarketCapUsd = currency.MarketCapUsd,
                    MarketCapConvert = currency.MarketCapConvert,
                    Name = currency.Name,
                    PercentChange1h = currency.PercentChange1h,
                    PercentChange24h = currency.PercentChange24h,
                    PercentChange7d = currency.PercentChange7d,
                    PriceUsd = currency.PriceUsd,
                    Rank = currency.Rank,
                    Symbol = currency.Symbol,
                    SupplyTotal = currency.TotalSupply,
                    Volume24hConvert = currency.Volume24Convert,
                    Volume24hUsd = currency.Volume24hUsd
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
        /// The AssetUpdateWorker
        /// </summary>
        private void AssetUpdateWorker()
        {
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
        public void StartAssetUpdater()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The SubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void SubscribeAsset(Asset asset)
        {
            SubscribedAssets.Add(asset);
        }

        /// <summary>
        /// The UnsubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void UnsubscribeAsset(Asset asset)
        {
            SubscribedAssets.RemoveAll(sub => sub.AssetId == asset.AssetId && sub.ConvertCurrency == asset.ConvertCurrency);
        }
    }
}

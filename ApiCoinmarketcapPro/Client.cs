using AssetWatch;
using CoinMarketCapPro;
using CoinMarketCapPro.Types;
using System;
using System.Collections.Generic;
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
        /// Gets the SubscribedAssets
        /// </summary>
        public List<Asset> SubscribedAssets { get; private set; }

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
                    // TODO
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
        public event EventHandler OnApiError;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
        {
            availableAssets = new List<Asset>();
            client = new CoinMarketCapClient(apiSchema, apiKey);
            assetRequestDelegate = new AssetRequestDelegate(GetAvailableAssets);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// The UnsubscribeAsset
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void UnsubscribeAsset(Asset asset)
        {
            throw new NotImplementedException();
        }
    }
}

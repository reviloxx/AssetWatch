using AssetWatch;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiCoinmarketcap
{
    public class Public : IApi
    {
        private List<Asset> availableAssets;
        private List<Asset> subscribedAssets;
        private delegate void AssetRequestDelegate();
        private AssetRequestDelegate assetRequestDelegate;
        private Thread assetUpdateWorker;
        private int updateInterval;

        public List<Asset> SubscribedAssets
        {
            get
            {
                return this.subscribedAssets;
            }
            private set { }
        }

        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;
        public event EventHandler<Asset> OnSingleAssetUpdated;
        public event EventHandler OnApiError;

        public Public()
        {
            this.assetRequestDelegate = new AssetRequestDelegate(this.GetAvailableAssets);
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
        }

        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiName = "Coinmarketcap.com (public)",
                    ApiVersion = "1.0",
                    AssetUrl = "",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                };
            }           
        }
        
        public void RequestAvailableAssetsAsync()
        {
            this.assetRequestDelegate.BeginInvoke(null, null);
        }

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
                this.availableAssets.Add(new Asset()
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
            
            this.FireOnAvailableAssetsReceived();
        }

        private void FireOnAvailableAssetsReceived()
        {
            this.OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        private void AssetUpdateWorker()
        {
            
        }

        public void SetUpdateInterval(int updateInterval)
        {
            this.updateInterval = updateInterval;
        }

        public void StartAssetUpdater()
        {
            throw new NotImplementedException();
        }

        public void SubscribeAsset(string assetName, string convertCurrency)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAsset(string assetName, string convertCurrency)
        {
            throw new NotImplementedException();
        }
    }
}

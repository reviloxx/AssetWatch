using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YahooFinanceApi;

namespace ApiYahooFinance
{
    public abstract class Client
    {
        private Thread assetUpdateWorker;
        protected List<Asset> availableAssets;
        private List<Asset> subscribedAssets;
        private List<string> subscribedConvertCurrencies;

        public Client()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
            this.subscribedConvertCurrencies = new List<string>();
            
        }

        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                this.GetAssetUpdates(this.subscribedAssets);
                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
        }

        public void Disable()
        {
            this.StopAssetUpdater();
            this.ApiData.IsEnabled = false;
        }

        public void Enable()
        {
            this.ApiData.IsEnabled = true;
        }

        public void RequestAvailableAssetsAsync()
        {
            this.FireOnAvailableAssetsReceived();
        }

        public void RequestSingleAssetUpdateAsync(Asset asset)
        {
            if (!this.assetUpdateWorker.IsAlive)
            {
                return;
            }

            if (!this.ApiData.IsEnabled)
            {
                throw new Exception("API is not enabled!");
            }

            List<Asset> assets = new List<Asset>();
            assets.Add(asset);
            this.GetAssetUpdates(assets);
        }

        public void StartAssetUpdater()
        {
            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

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

        public void UnsubscribeAssetFromUpdater(Asset asset)
        {
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

        private async void GetAssetUpdates(List<Asset> assets)
        {
            if (assets.Count < 1)
            {
                return;
            }

            // TODO: FINISH: API error handling
            string[] symbols = new string[assets.Count];

            for (int i = 0; i < assets.Count; i++)
            {
                symbols[i] = assets[i].Symbol;
            }
            
            var response = await Yahoo.Symbols(symbols).Fields(Field.Symbol, Field.RegularMarketPrice, Field.MarketCap).QueryAsync();
            this.ApiData.IncreaseCounter(1);

            foreach(Asset asset in assets)
            {
                var assetResponse = response[asset.Symbol];

                Asset updatedAsset = new Asset()
                {
                    AssetId = assetResponse.Symbol,
                    Symbol = assetResponse.Symbol,
                    Name = asset.Name,
                    ConvertCurrency = asset.ConvertCurrency,
                    LastUpdated = DateTime.Now,
                    Price = assetResponse.RegularMarketPrice
                };

                this.FireOnSingleAssetUpdated(updatedAsset);
            }
        }

        public ApiData ApiData { get; set; }        

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

        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;
        public event EventHandler<Asset> OnSingleAssetUpdated;
        public event EventHandler<OnApiErrorEventArgs> OnApiError;
        public event EventHandler OnAppDataChanged;
    }
}

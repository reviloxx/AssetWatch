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
        /// <summary>
        /// Defines the retryDelay in case there is no connection.
        /// </summary>
        private static readonly int retryDelay = 1000;
        private Thread assetUpdateWorker;
        protected List<Asset> availableAssets;
        private readonly List<Asset> subscribedAssets;
        private double eurConvert;

        public Client()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
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
            // get EUR convert price
            this.GetAssetUpdates(new List<Asset> { new Asset { Symbol = "EUR=X" } });
            this.StartAssetUpdater();
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

            List<Asset> assets = new List<Asset>();
            assets.Add(asset);
            this.GetAssetUpdates(assets);
        }

        private void StartAssetUpdater()
        {
            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

        public void AttachAsset(Asset asset)
        {
            if (!this.subscribedAssets.Exists(sub => sub.Symbol == asset.Symbol && sub.ConvertCurrency == asset.ConvertCurrency))
            {
                this.subscribedAssets.Add(asset);
            }
        }

        public void DetachAsset(Asset asset)
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
            
            string[] symbols = new string[assets.Count];
            bool callFailed = false;
            int timeout = this.ApiData.UpdateInterval * 1000;

            for (int i = 0; i < assets.Count; i++)
            {
                symbols[i] = assets[i].Symbol;
            }

            do
            {
                callFailed = false;                

                try
                {
                    var response = await Yahoo.Symbols(symbols).Fields(Field.Symbol, Field.RegularMarketPrice, Field.Currency).QueryAsync();
                    this.ApiData.IncreaseCounter(1);                    

                    foreach (Asset asset in assets)
                    {
                        if (asset.ConvertCurrency == "EUR" && this.eurConvert == 0)
                        {
                            throw new NullReferenceException();
                        }

                        try
                        {
                            var assetResponse = response[asset.Symbol];                            

                            if (asset.Symbol == "EUR=X")
                            {
                                this.eurConvert = assetResponse.RegularMarketPrice;
                                continue;
                            }

                            Asset updatedAsset = null;

                            if (assetResponse.Currency == "USD")
                            {
                                updatedAsset = new Asset()
                                {
                                    AssetId = assetResponse.Symbol,
                                    Symbol = assetResponse.Symbol,
                                    Name = asset.Name,
                                    ConvertCurrency = asset.ConvertCurrency,
                                    LastUpdated = DateTime.Now,
                                    Price = asset.ConvertCurrency == "USD" ? assetResponse.RegularMarketPrice : assetResponse.RegularMarketPrice * this.eurConvert
                                };
                            }
                            else if (assetResponse.Currency == "EUR")
                            {
                                // response in EUR
                                updatedAsset = new Asset()
                                {
                                    AssetId = assetResponse.Symbol,
                                    Symbol = assetResponse.Symbol,
                                    Name = asset.Name,
                                    ConvertCurrency = asset.ConvertCurrency,
                                    LastUpdated = DateTime.Now,
                                    Price = asset.ConvertCurrency == "EUR" ? assetResponse.RegularMarketPrice : assetResponse.RegularMarketPrice / this.eurConvert
                                };
                            }     
                            else
                            {
                                throw new Exception("Die API antwortete mit einem ungültigen Tradingpaar: " + assetResponse.Currency + "/" + assetResponse.Symbol);
                            }

                            this.FireOnSingleAssetUpdated(updatedAsset);
                            
                        }
                        catch (KeyNotFoundException)
                        {
                            // ignore
                        }                        
                    }
                }
                catch (NullReferenceException)
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

using AssetWatch;
using System;
using System.Collections.Generic;
using System.Threading;
using YahooFinanceApi;

namespace ApiYahooFinance
{
    /// <summary>
    /// Defines the <see cref="Client" />
    /// </summary>
    public abstract class Client
    {
        /// <summary>
        /// Defines the delay to try again in case there is no connection.
        /// </summary>
        private static readonly int retryDelay = 1000;        

        /// <summary>
        /// Defines the availableAssets
        /// </summary>
        protected List<Asset> availableAssets;

        /// <summary>
        /// Defines the attachedAssets
        /// </summary>
        private readonly List<Asset> attachedAssets;

        /// <summary>
        /// Defines the eur/usd exchange rate
        /// </summary>
        private double eurExchangeRate;

        /// <summary>
        /// Defines the assetUpdateWorker
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        public Client()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.availableAssets = new List<Asset>();
            this.attachedAssets = new List<Asset>();
        }

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            // get EUR convert price
            this.GetAssetUpdates(new List<Asset> { new Asset { Symbol = "EUR=X" } });

            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

        /// <summary>
        /// Disables the API.
        /// </summary>
        public void Disable()
        {
            try
            {
                this.assetUpdateWorker.Abort();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Requests the available assets of the API.
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            this.FireOnAvailableAssetsReceived();
        }

        /// <summary>
        /// Requests a single asset update.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
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
        /// Attaches an asset to the asset updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to attach.</param>
        public void AttachAsset(Asset asset)
        {
            if (!this.attachedAssets.Exists(sub => sub.Symbol == asset.Symbol && sub.ConvertCurrency == asset.ConvertCurrency))
            {
                this.attachedAssets.Add(asset);
            }
        }

        /// <summary>
        /// Detaches an asset from the asset updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to detach.</param>
        public void DetachAsset(Asset asset)
        {
        }

        /// <summary>
        /// Requests updates of the subscribed assets while this API is enabled.
        /// </summary>
        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                this.GetAssetUpdates(this.attachedAssets);
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
                        if (asset.ConvertCurrency == "EUR" && this.eurExchangeRate == 0)
                        {
                            throw new NullReferenceException();
                        }

                        try
                        {
                            var assetResponse = response[asset.Symbol];

                            if (asset.Symbol == "EUR=X")
                            {
                                this.eurExchangeRate = assetResponse.RegularMarketPrice;
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
                                    Price = asset.ConvertCurrency == "USD" ? assetResponse.RegularMarketPrice : assetResponse.RegularMarketPrice * this.eurExchangeRate
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
                                    Price = asset.ConvertCurrency == "EUR" ? assetResponse.RegularMarketPrice : assetResponse.RegularMarketPrice / this.eurExchangeRate
                                };
                            }
                            else
                            {
                                throw new Exception("Die API antwortete mit einem ungültigen Tradingpaar: " + assetResponse.Currency + "/" + assetResponse.Symbol);
                            }

                            this.FireOnAssetUpdateReceived(updatedAsset);

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

        /// <summary>
        /// Fires the OnAvailableAssetsReceived event.
        /// </summary>
        private void FireOnAvailableAssetsReceived()
        {
            this.OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        /// <summary>
        /// Fires the OnAssetUpdateReceived event.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        private void FireOnAssetUpdateReceived(Asset asset)
        {
            this.OnAssetUpdateReceived?.Invoke(this, asset);
        }

        /// <summary>
        /// Fires the OnApiError event.
        /// </summary>
        /// <param name="onApiErrorEventArgs">The onApiErrorEventArgs<see cref="OnApiErrorEventArgs"/></param>
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
        /// Gets or sets the ApiData
        /// </summary>
        public ApiData ApiData { get; set; }

        /// <summary>
        /// Defines the OnAvailableAssetsReceived
        /// </summary>
        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Defines the OnSingleAssetUpdated
        /// </summary>
        public event EventHandler<Asset> OnAssetUpdateReceived;

        /// <summary>
        /// Defines the OnApiError
        /// </summary>
        public event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

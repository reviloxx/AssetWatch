using AssetWatch;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace ApiYahooFinance
{
    /// <summary>
    /// Defines the <see cref="Client" />
    /// </summary>
    public abstract class Client : AssetWatch.Api
    {
        /// <summary>
        /// Defines the eur/usd exchange rate
        /// </summary>
        private double eurExchangeRate;

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            // get EUR convert price
            Task.Run(() => this.GetAssetUpdatesAsync(new List<Asset> { new Asset { Symbol = "EUR=X" } }));

            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
        }

        /// <summary>
        /// Requests the available assets of the API.
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            this.FireOnAvailableAssetsReceived();
        }

        /// <summary>
        /// Attaches an asset to the asset updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to attach.</param>
        public override void AttachAsset(Asset asset)
        {
            if (!this.attachedAssets.Exists(sub => sub.Symbol == asset.Symbol && sub.ConvertCurrency == asset.ConvertCurrency))
            {
                this.attachedAssets.Add(asset);
            }
        }

        /// <summary>
        /// Gets updates for a list of assets.
        /// Fires the OnSingleAssetUpdated event for each updated asset from the list.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        /// <param name="assets">The assets<see cref="List{Asset}"/> to update.</param>
        protected override async Task GetAssetUpdatesAsync(List<Asset> assets)
        {
            if (assets.Count < 1)
            {
                return;
            }

            string[] symbols = new string[assets.Count];
            int timeout = this.ApiData.UpdateInterval * 1000;

            for (int i = 0; i < assets.Count; i++)
            {
                symbols[i] = assets[i].Symbol;
            }

            bool callFailed;
            do
            {
                callFailed = false;

                try
                {
                    var response = await Yahoo.Symbols(symbols).Fields(Field.Symbol, Field.RegularMarketPrice, Field.Currency).QueryAsync();
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

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
    }
}

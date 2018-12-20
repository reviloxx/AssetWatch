using AssetWatch;
using CoinMarketCapPro;
using CoinMarketCapPro.Types;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ApiCoinmarketcap
{
    /// <summary>
    /// Defines the <see cref="Client" />
    /// </summary>
    public abstract class Client
    {
        private static int retryDelay = 1000;

        /// <summary>
        /// Defines the apiSchema.
        /// </summary>
        private ApiSchema apiSchema;

        /// <summary>
        /// Defines the availableAssets.
        /// </summary>
        private List<Asset> availableAssets;

        /// <summary>
        /// Gets the SubscribedAssets.
        /// </summary>
        private List<Asset> subscribedAssets;

        /// <summary>
        /// Defines the subscribedConvertCurrencies.
        /// </summary>
        private List<string> subscribedConvertCurrencies;

        /// <summary>
        /// The AssetRequestDelegate.
        /// </summary>
        private delegate void AssetRequestDelegate();

        /// <summary>
        /// Defines the assetRequestDelegate.
        /// </summary>
        private AssetRequestDelegate assetRequestDelegate;

        /// <summary>
        /// Defines the client.
        /// </summary>
        private CoinMarketCapClient client;

        /// <summary>
        /// Defines the assetUpdateWorker
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="apiSchema">The apiSchema<see cref="ApiSchema"/></param>
        public Client(ApiSchema apiSchema)
        {
            this.apiSchema = apiSchema;
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
            this.subscribedConvertCurrencies = new List<string>();
            this.assetRequestDelegate = new AssetRequestDelegate(this.GetAvailableAssets);
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
        }

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            if (this.apiSchema == ApiSchema.Pro && this.ApiData.ApiKey == string.Empty)
            {
                throw new Exception("API Key missing!");
            }

            if (this.apiSchema == ApiSchema.Pro)
            {
                this.client = new CoinMarketCapClient(this.apiSchema, this.ApiData.ApiKey);
            }
            else
            {
                string apiKey = "29bc6cc3-7219-42f6-af87-f0147e9ee089";
                this.client = new CoinMarketCapClient(this.apiSchema, apiKey);
            }
            
            this.StartAssetUpdater();
        }

        /// <summary>
        /// Disables the API.
        /// </summary>
        public void Disable()
        {
            this.StopAssetUpdater();
        }

        /// <summary>
        /// Requests the available assets of the API.
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            this.assetRequestDelegate.BeginInvoke(null, null);
        }

        /// <summary>
        /// Requests a single asset update.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to update.</param>
        public void RequestSingleAssetUpdateAsync(Asset asset)
        {
            List<Asset> assets = new List<Asset>();
            assets.Add(asset);
            this.GetAssetUpdates(assets);
        }

        /// <summary>
        /// Subscribes an asset to the asset updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to subscribe.</param>
        public void AttachAsset(Asset asset)
        {
            // Add the asset to the list of subscribed assets if there doesn't exist one with the same id yet.
            if (!this.subscribedAssets.Exists(sub => sub.AssetId == asset.AssetId))
            {
                this.subscribedAssets.Add(asset);
            }

            // Add the convert currency to the list of subscribed convert currencies if it is not in the list yet.
            if (!this.subscribedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                this.subscribedConvertCurrencies.Add(asset.ConvertCurrency);
            }
        }

        /// <summary>
        /// Unsubscribes an asset from the asset updater.
        /// Not implemented because at this API the number of calls is not dependent on the number of subscribed assets.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to unsubscribe.</param>
        public void DetachAsset(Asset asset)
        {
        }

        /// <summary>
        /// Gets the available assets of the API and fires the OnAvailableAssetsReceived event if successful.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        private async void GetAvailableAssets()
        {
            bool callFailed;

            do
            {
                callFailed = false;

                try
                {
                    var map = await this.client.GetCurrencyMapAsync();
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

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

                    this.availableAssets = this.availableAssets.OrderBy(ass => ass.SymbolName).ToList();

                    this.FireOnAvailableAssetsReceived();
                }
                catch (NullReferenceException)
                {
                    callFailed = true;
                    Thread.Sleep(retryDelay);
                }
                catch (FlurlHttpException)
                {
                    callFailed = true;
                    Thread.Sleep(retryDelay);
                }
                catch (Exception e)
                {
                    this.FireOnApiError(this.BuildOnApiErrorEventArgs(e.Message));
                }
            }
            while (callFailed);
            
        }

        /// <summary>
        /// Starts the asset updater.
        /// </summary>
        private void StartAssetUpdater()
        {
            if (this.assetUpdateWorker.IsAlive)
            {
                throw new Exception("Already running!");
            }

            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetUpdateWorker.Start();
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

        /// <summary>
        /// Requests updates of the subscribed assets while this API is enabled.
        /// </summary>
        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                this.GetAssetUpdates(this.subscribedAssets);
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
            
            List<int> ids = new List<int>();
            bool callFailed = false;
            int timeout = this.ApiData.UpdateInterval * 1000;

            assets.ForEach(sub =>
            {
                if (!ids.Exists(ex => ex == int.Parse(sub.AssetId)))
                {
                    ids.Add(int.Parse(sub.AssetId));
                }
            });

            do
            {
                callFailed = false;

                try
                {
                    var response = await this.client.GetCurrencyMarketQuotesAsync(ids, this.subscribedConvertCurrencies);
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    if (response.Status.ErrorCode != 0)
                    {
                        this.FireOnApiError(this.BuildOnApiErrorEventArgs(response.Status.ErrorMessage));
                    }
                    else
                    {
                        assets.ForEach(ass =>
                        {
                            var assetUpdate = response.Data.FirstOrDefault(d => d.Key == ass.AssetId).Value;
                            ass.Price = (double)assetUpdate.Quote[ass.ConvertCurrency].Price;
                            ass.LastUpdated = DateTime.Now;
                            ass.MarketCap = (double)assetUpdate.Quote[ass.ConvertCurrency].MarketCap;
                            ass.PercentChange1h = (double)assetUpdate.Quote[ass.ConvertCurrency].PercentChange1h;
                            ass.PercentChange24h = (double)assetUpdate.Quote[ass.ConvertCurrency].PercentChange24h;
                            ass.PercentChange7d = (double)assetUpdate.Quote[ass.ConvertCurrency].PercentChange7d;
                            ass.Rank = assetUpdate.CmcRank;
                            ass.SupplyAvailable = (double)assetUpdate.CirculatingSupply;
                            ass.SupplyTotal = (double)assetUpdate.TotalSupply;
                            this.FireOnSingleAssetUpdated(ass);
                        });
                    }

                }
                catch (NullReferenceException)
                {
                    // happened once without further consequences, can be ignored
                }
                catch (FlurlHttpException)
                {
                    // no internet connection?
                    callFailed = true;
                    Thread.Sleep(retryDelay);
                    timeout -= retryDelay;
                }
            }
            while (callFailed && timeout >= 0);
        }

        /// <summary>
        /// Builds the OnApiError event args depending on the error message.
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        /// <returns>The <see cref="OnApiErrorEventArgs"/></returns>
        private OnApiErrorEventArgs BuildOnApiErrorEventArgs(string message)
        {
            if (message.Contains("400"))
            {
                return new OnApiErrorEventArgs
                {
                    ErrorMessage = message,
                    ErrorType = ErrorType.BadRequest
                };
            }

            if (message.Contains("401"))
            {
                return new OnApiErrorEventArgs
                {
                    ErrorMessage = message,
                    ErrorType = ErrorType.Unauthorized
                };
            }

            if (message.Contains("429"))
            {
                return new OnApiErrorEventArgs
                {
                    ErrorMessage = message,
                    ErrorType = ErrorType.TooManyRequests
                };
            }

            return new OnApiErrorEventArgs
            {
                ErrorMessage = message,
                ErrorType = ErrorType.General
            };
        }

        /// <summary>
        /// Fires the OnApiError event.
        /// </summary>
        /// <param name="eventArgs">The eventArgs<see cref="OnApiErrorEventArgs"/></param>
        private void FireOnApiError(OnApiErrorEventArgs eventArgs)
        {
            this.OnApiError?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Fires the OnSingleAssetUpdated event.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        private void FireOnSingleAssetUpdated(Asset asset)
        {
            OnSingleAssetUpdated?.Invoke(this, asset);
        }

        /// <summary>
        /// Fires the OnAvailableAssetsReceived event.
        /// </summary>
        private void FireOnAvailableAssetsReceived()
        {
            OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        /// <summary>
        /// Fires the OnAppDataChanged event.
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Gets or sets the ApiData.
        /// </summary>
        public ApiData ApiData { get; set; }

        /// <summary>
        /// Defines the OnAvailableAssetsReceived event.
        /// </summary>
        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Defines the OnSingleAssetUpdated event.
        /// </summary>
        public event EventHandler<Asset> OnSingleAssetUpdated;

        /// <summary>
        /// Defines the OnApiError event.
        /// </summary>
        public event EventHandler<OnApiErrorEventArgs> OnApiError;

        /// <summary>
        /// Defines the OnAppDataChanged event.
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

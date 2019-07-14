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
using System.Threading.Tasks;

namespace ApiCoinmarketcap
{
    /// <summary>
    /// Defines the <see cref="Client" />
    /// </summary>
    public abstract class Client : AssetWatch.Api
    {
        /// <summary>
        /// Defines the apiSchema.
        /// </summary>
        private readonly ApiSchema apiSchema;

        /// <summary>
        /// Defines the client.
        /// </summary>
        private CoinMarketCapClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="apiSchema">The apiSchema<see cref="ApiSchema"/></param>
        public Client(ApiSchema apiSchema)
        {
            this.apiSchema = apiSchema;
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
            Task.Run(() => this.GetAvailableAssetsAsync());
        }

        /// <summary>
        /// Attaches an asset to the asset updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to attach.</param>
        public override void AttachAsset(Asset asset)
        {
            // Add the asset to the list of subscribed assets if there doesn't exist one with the same id yet.
            if (!this.attachedAssets.Exists(sub => sub.AssetId == asset.AssetId))
            {
                this.attachedAssets.Add(asset);
            }

            // Add the convert currency to the list of subscribed convert currencies if it is not in the list yet.
            if (!this.attachedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                this.attachedConvertCurrencies.Add(asset.ConvertCurrency);
            }
        }

        /// <summary>
        /// Detaches an asset from the updater.
        /// </summary>
        /// <param name="asset">The <see cref="DetachAssetArgs"/></param>
        public override void DetachAsset(DetachAssetArgs args)
        {
            if (args.DetachAsset)
            {
                this.attachedAssets.RemoveAll(a => a.AssetId == args.Asset.AssetId);
            }
        }

        /// <summary>
        /// Gets the available assets of the API and fires the OnAvailableAssetsReceived event if successful.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        private async Task GetAvailableAssetsAsync()
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
                    var response = await this.client.GetCurrencyMarketQuotesAsync(ids, this.attachedConvertCurrencies);
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
                            ass.PercentChange24h = assetUpdate.Quote[ass.ConvertCurrency].PercentChange24h == null ? -101 : (double)assetUpdate.Quote[ass.ConvertCurrency].PercentChange24h;
                            ass.PercentChange7d = assetUpdate.Quote[ass.ConvertCurrency].PercentChange7d == null ? -101 : (double)assetUpdate.Quote[ass.ConvertCurrency].PercentChange7d;
                            ass.PercentChange1h = assetUpdate.Quote[ass.ConvertCurrency].PercentChange1h == null ? -101 : (double)assetUpdate.Quote[ass.ConvertCurrency].PercentChange1h;
                            ass.Rank = assetUpdate.CmcRank;
                            ass.MarketCap = assetUpdate.Quote[ass.ConvertCurrency].MarketCap == null? -1 : (double)assetUpdate.Quote[ass.ConvertCurrency].MarketCap;
                            ass.SupplyAvailable = assetUpdate.CirculatingSupply == null ? -1 : (double)assetUpdate.CirculatingSupply;
                            ass.SupplyTotal = assetUpdate.TotalSupply == null ? -1 : (double)assetUpdate.TotalSupply;
                            
                            this.FireOnAssetUpdateReceived(ass);
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
                catch (Exception e)
                {
                    this.FireOnApiError(this.BuildOnApiErrorEventArgs(e.Message));
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
    }
}

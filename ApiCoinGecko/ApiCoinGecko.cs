using AssetWatch;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Entities.Response.Simple;
using CoinGecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ApiCoinGecko
{
    public class ApiCoinGecko : IApi
    {
        /// <summary>
        /// Defines the delay to try again in case there is no connection.
        /// </summary>
        private static readonly int retryDelay = 1000;

        /// <summary>
        /// Defines the CoinGecko client.
        /// </summary>
        private ICoinGeckoClient client;

        /// <summary>
        /// Defines the list of attached assets.
        /// </summary>
        private readonly List<Asset> attachedAssets;

        /// <summary>
        /// Defines the list of attached convert currencies.
        /// </summary>
        private readonly List<string> attachedConvertCurrencies;

        /// <summary>
        /// Defines the assetUpdateWorker thread.
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCoinGecko"/> class.
        /// </summary>
        public ApiCoinGecko()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.attachedAssets = new List<Asset>();
            this.attachedConvertCurrencies = new List<string>();
        }

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            this.client = new CoinGeckoClient();

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
        /// Requests the available assets of this API.
        /// </summary>
        public void RequestAvailableAssetsAsync()
        {
            Task.Run(() => this.GetAvailableAssetsAsync());
        }

        /// <summary>
        /// Requests a single asset update.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to update.</param>
        public void RequestSingleAssetUpdateAsync(Asset asset)
        {
            List<Asset> assets = new List<Asset>
            {
                asset
            };
            Task.Run(() => this.GetAssetUpdatesAsync(assets));
        }

        /// <summary>
        /// Attaches an asset to the updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to attach.</param>
        public void AttachAsset(Asset asset)
        {
            if (!this.attachedAssets.Exists(sub => sub.Symbol == asset.Symbol && sub.ConvertCurrency == asset.ConvertCurrency))
            {
                this.attachedAssets.Add(asset);
            }

            if (!this.attachedConvertCurrencies.Exists(sub => sub == asset.ConvertCurrency))
            {
                this.attachedConvertCurrencies.Add(asset.ConvertCurrency);
            }
        }

        /// <summary>
        /// Detaches an asset/convert currency from the updater.
        /// </summary>
        /// <param name="asset">The <see cref="DetachAssetArgs"/></param>
        public void DetachAsset(DetachAssetArgs args)
        {
            if (args.DetachAsset)
            {
                this.attachedAssets.RemoveAll(a => a.AssetId == args.Asset.AssetId);
            }

            if (args.DetachConvertCurrency)
            {
                this.attachedConvertCurrencies.Remove(args.Asset.ConvertCurrency);
            }
        }

        /// <summary>
        /// Gets the available assets of the API and fires the OnAvailableAssetsReceived event if successful.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        private async Task GetAvailableAssetsAsync()
        {
            bool callFailed = false;
            List<Asset> availableAssets = new List<Asset>();

            do
            {
                callFailed = false;

                try
                {
                    IReadOnlyList<CoinList> response = await this.client.CoinsClient.GetCoinList();
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    foreach (CoinList coin in response)
                    {
                        availableAssets.Add(
                            new Asset
                            {
                                AssetId = coin.Id,
                                Name = coin.Name,
                                Symbol = coin.Symbol
                            });
                    }

                    availableAssets = availableAssets.OrderBy(ass => ass.SymbolName).ToList();
                    this.FireOnAvailableAssetsReceived(availableAssets);
                }
                catch (HttpRequestException)
                {
                    callFailed = true;
                    Thread.Sleep(retryDelay);
                }
            }
            while (callFailed);
        }

        /// <summary>
        /// Requests updates of the subscribed assets while this API is enabled.
        /// </summary>
        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                Task.Run(() => this.GetAssetUpdatesAsync(this.attachedAssets));
                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
        }

        /// <summary>
        /// Gets updates for a list of assets.
        /// Fires the OnSingleAssetUpdated event for each updated asset from the list.
        /// Fires the OnApiError event if something has gone wrong.
        /// </summary>
        /// <param name="assets">The assets<see cref="List{Asset}"/> to update.</param>
        private async Task GetAssetUpdatesAsync(List<Asset> assets)
        {
            if (assets.Count < 1)
            {
                return;
            }

            List<string> fromIds = new List<string>();
            assets.ForEach(ass => fromIds.Add(ass.AssetId));

            bool callFailed = false;
            int timeout = this.ApiData.UpdateInterval * 1000;

            do
            {
                callFailed = false;

                try
                {
                    Price response = await this.client.SimpleClient.GetSimplePrice(fromIds.ToArray(), this.attachedConvertCurrencies.ToArray());
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();
                    


                    assets.ForEach(ass =>
                    {
                        try
                        {
                            var assres = response[ass.AssetId];

                            if (assres.ContainsKey(ass.ConvertCurrency.ToLower()))
                            {
                                ass.LastUpdated = DateTime.Now;
                                ass.Price = (double)assres[ass.ConvertCurrency.ToLower()];
                                //ass.PercentChange24h = (double)assres[ass.ConvertCurrency.ToLower() + "_24h_change"];
                                this.FireOnAssetUpdateReceived(ass);
                            }                            
                        }
                        catch (KeyNotFoundException)
                        {
                            // ignore
                        }
                    });
                }
                catch (HttpRequestException)
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
        private void FireOnAvailableAssetsReceived(List<Asset> availableAssets)
        {
            this.OnAvailableAssetsReceived?.Invoke(this, availableAssets);
        }

        /// <summary>
        /// Fires the OnAssetUpdatedReceived event.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        private void FireOnAssetUpdateReceived(Asset asset)
        {
            this.OnAssetUpdateReceived?.Invoke(this, asset);
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

        /// <summary>
        /// Gets or sets the ApiData.
        /// </summary>
        public ApiData ApiData { get; set; }

        /// <summary>
        /// Gets the ApiInfo.
        /// </summary>
        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "\n" +
                    " - Min. Update Intervall:\n" +
                    "     1 Minute\n\n" +
                    " - Prozentuale Preisänderung:\n" +
                    "     nicht unterstützt\n\n" +
                    " - API Key nötig:\n" +
                    "     nein\n\n" +
                    " - Basiswährungen:\n" +
                    "     USD, EUR, BTC, ETH, EOS",
                    ApiKeyRequired = false,
                    ApiName = "CoinGecko",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://www.coingecko.com/en/coins/#ID#",
                    AssetUrlName = "auf coingecko.com anzeigen...",
                    GetApiKeyUrl = "",
                    StdUpdateInterval = 180,
                    MaxUpdateInterval = 1800,
                    MinUpdateInterval = 60,
                    UpdateIntervalStepSize = 30,
                    SupportedConvertCurrencies = new List<string>() { "EUR", "USD", "BTC", "ETH", "EOS" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall ab 1 Minute."
                };
            }
        }

        /// <summary>
        /// Defines the OnAvailableAssetsReceived event.
        /// </summary>
        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;

        /// <summary>
        /// Defines the OnAssetUpdateReceived event.
        /// </summary>
        public event EventHandler<Asset> OnAssetUpdateReceived;

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

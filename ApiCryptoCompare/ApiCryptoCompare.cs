using AssetWatch;
using CryptoCompare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ApiCryptoCompare
{
    /// <summary>
    /// Defines the <see cref="ApiCryptoCompare" />
    /// </summary>
    public class ApiCryptoCompare : IApi
    {
        /// <summary>
        /// Defines the delay to try again in case there is no connection.
        /// </summary>
        private static readonly int retryDelay = 1000;

        /// <summary>
        /// Defines the CryptoCompare client.
        /// </summary>
        private CryptoCompareClient client;

        /// <summary>
        /// Defines the list of attached assets.
        /// </summary>
        private readonly List<Asset> attachedAssets;

        /// <summary>
        /// Defines the list of attached convert currencies.
        /// </summary>
        private readonly List<string> attachedConvertCurrencies;

        /// <summary>
        /// The AssetRequestDelegate.
        /// </summary>
        private delegate void AssetRequestDelegate();

        /// <summary>
        /// Defines the assetRequestDelegate.
        /// </summary>
        private readonly AssetRequestDelegate assetRequestDelegate;        

        /// <summary>
        /// Defines the assetUpdateWorker thread.
        /// </summary>
        private Thread assetUpdateWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCryptoCompare"/> class.
        /// </summary>
        public ApiCryptoCompare()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.assetRequestDelegate = new AssetRequestDelegate(this.GetAvailableAssets);
            this.attachedAssets = new List<Asset>();
            this.attachedConvertCurrencies = new List<string>();
        }

        /// <summary>
        /// Enables the API.
        /// </summary>
        public void Enable()
        {
            this.client = new CryptoCompareClient();

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
            this.assetRequestDelegate.BeginInvoke(null, null);
        }

        /// <summary>
        /// Requests a single asset update.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to update.</param>
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
                this.attachedAssets.RemoveAll(a => a.Symbol == args.Asset.Symbol);
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
        private async void GetAvailableAssets()
        {
            bool callFailed = false;
            List<Asset> availableAssets = new List<Asset>();

            do
            {
                callFailed = false;

                try
                {
                    CoinListResponse response = await this.client.Coins.ListAsync();
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    foreach (KeyValuePair<string, CoinInfo> coin in response.Coins)
                    {
                        availableAssets.Add(
                            new Asset
                            {
                                AssetId = coin.Value.Id,
                                Name = coin.Value.Name,
                                Symbol = coin.Value.Symbol
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

            List<string> fromSymbols = new List<string>();
            assets.ForEach(ass => fromSymbols.Add(ass.Symbol));

            bool callFailed = false;
            int timeout = this.ApiData.UpdateInterval * 1000;

            do
            {
                callFailed = false;

                try
                {
                    PriceMultiFullResponse response = await this.client.Prices.MultipleSymbolFullDataAsync(fromSymbols, this.attachedConvertCurrencies);
                    this.ApiData.IncreaseCounter(1);
                    this.FireOnAppDataChanged();

                    var res = response.Raw;

                    if (res == null)
                    {
                        return;
                    }

                    assets.ForEach(ass =>
                    {
                        this.attachedConvertCurrencies.ForEach(con =>
                        {
                            try
                            {
                                CoinFullAggregatedData data = res[ass.Symbol][con];

                                if (ass.ConvertCurrency == con)
                                {
                                    ass.LastUpdated = DateTime.Now;
                                    ass.Price = (double)data.Price;
                                    ass.PercentChange24h = (double)data.ChangePCT24Hour;

                                    try
                                    {                                        
                                        ass.MarketCap = (double)data.MarketCap;
                                        ass.Volume24hConvert = data.TotalVolume24HTo.ToString();
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        // properties of CoinFullAggregatedData might be null
                                    }

                                    this.FireOnAssetUpdateReceived(ass);
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                // ignore
                            }                            
                        });
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
                    "     15 Sekunden\n\n" +
                    " - Prozentuale Preisänderung:\n" +
                    "     24h\n\n" +
                    " - API Key nötig:\n" +
                    "     nein\n\n" +
                    " - Basiswährungen:\n" +
                    "     USD, EUR, BTC",
                    ApiKeyRequired = false,
                    ApiName = "CryptoCompare",
                    ApiClientVersion = "1.0",
                    Market = Market.Kryptowährungen,
                    AssetUrl = "https://www.cryptocompare.com/coins/#SYMBOL#/overview",
                    AssetUrlName = "auf cryptocompare.com anzeigen...",
                    GetApiKeyUrl = "",
                    StdUpdateInterval = 15,
                    MaxUpdateInterval = 300,
                    MinUpdateInterval = 15,
                    UpdateIntervalStepSize = 15,
                    SupportedConvertCurrencies = new List<string>() { "EUR", "USD", "BTC" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall ab 15 Sekunden."
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

using AssetWatch;
using CryptoCompare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ApiCryptoCompare
{
    public class Api : IApi
    {
        private CryptoCompareClient client;

        private Thread assetUpdateWorker;

        private List<Asset> availableAssets;

        /// <summary>
        /// Gets the SubscribedAssets
        /// </summary>
        private List<Asset> subscribedAssets;

        /// <summary>
        /// Defines the subscribedConvertCurrencies
        /// </summary>
        private List<string> subscribedConvertCurrencies;

        public Api()
        {
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
            this.availableAssets = new List<Asset>();
            this.subscribedAssets = new List<Asset>();
            this.subscribedConvertCurrencies = new List<string>();
            this.ApiData = new ApiData
            {
                ApiKey = string.Empty,
                ApiName = this.ApiInfo.ApiName,
                CallCountStartTime = DateTime.Now,
                CallCount = 0,
                IsEnabled = false,
                UpdateInterval = 15
            };
        }

        public void Disable()
        {
            this.StopAssetUpdater();
            this.ApiData.IsEnabled = false;
        }

        public void Enable()
        {
            this.client = new CryptoCompareClient();
            this.ApiData.IsEnabled = true;
        }

        public void RequestAvailableAssetsAsync()
        {
            this.GetAvailableAssetsAsync();
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
            this.RequestAssetUpdates(assets);
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
            // not implemented because at this API the number of calls is not dependent on the number of subscribed assets
        }

        private void WaitForConnection()
        {
            bool connected = false;

            do
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        using (var stream = client.OpenRead("https://www.cryptocompare.com/"))
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

        private void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                this.RequestAssetUpdates(this.subscribedAssets);
                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
        }

        private async void RequestAssetUpdates(List<Asset> assets)
        {
            if (assets.Count < 1)
            {
                return;
            }

            this.WaitForConnection();

            List<string> fromSymbols = new List<string>();
            assets.ForEach(ass => fromSymbols.Add(ass.Symbol));

            PriceMultiFullResponse response = await this.client.Prices.MultipleSymbolFullDataAsync(fromSymbols, this.subscribedConvertCurrencies);
            this.ApiData.CallCount++;
            this.FireOnAppDataChanged();
            
            var res = response.Raw;

            assets.ForEach(ass =>
            {
                this.subscribedConvertCurrencies.ForEach(con =>
                {
                    try
                    {
                        CoinFullAggregatedData data = res[ass.Symbol][con];

                        if (ass.ConvertCurrency == con)
                        {
                            ass.LastUpdated = DateTime.Now;
                            ass.MarketCap = (double)data.MarketCap;
                            ass.PercentChange24h = (double)data.ChangePCT24Hour;
                            ass.PercentChange1h = -101;
                            ass.PercentChange7d = -101;
                            ass.Price = (double)data.Price;
                            ass.Volume24hConvert = data.TotalVolume24HTo.ToString();
                            this.FireOnSingleAssetUpdated(ass);
                        }                        
                    }
                    catch (KeyNotFoundException)
                    {
                        // ignore
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
                });
            });
        }

        private void StopAssetUpdater()
        {
            try
            {
                this.assetUpdateWorker.Abort();
            }
            catch (Exception) { }
        }

        private async void GetAvailableAssetsAsync()
        {
            this.WaitForConnection();
            CoinListResponse response = await this.client.Coins.ListAsync();
            this.ApiData.CallCount++;
            this.FireOnAppDataChanged();

            foreach (KeyValuePair<string, CoinInfo> coin in response.Coins)
            {
                this.availableAssets.Add(
                    new Asset
                    {
                        AssetId = coin.Value.Id,
                        Name = coin.Value.Name,
                        Symbol = coin.Value.Symbol
                    });
            }

            this.availableAssets = this.availableAssets.OrderBy(ass => ass.SymbolName).ToList();
            this.FireOnAvailableAssetsReceived();
        }        

        private void FireOnAvailableAssetsReceived()
        {
            this.OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        private void FireOnSingleAssetUpdated(Asset updatedAsset)
        {
            this.OnSingleAssetUpdated?.Invoke(this, updatedAsset);
        }

        private void FireOnApiError(OnApiErrorEventArgs onApiErrorEventArgs)
        {
            this.OnApiError?.Invoke(this, onApiErrorEventArgs);
        }

        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        public ApiData ApiData { get; set; }

        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiInfoText = "Liefert alle 15 Sekunden Daten zu den meisten Kryptowährungen. Unterstützt keine 7-Tage Preisentwicklung in Prozent.",
                    ApiKeyRequired = false,
                    ApiName = "CryptoCompare",
                    ApiClientVersion = "1.0",
                    Market = Market.Cryptocurrencies,
                    AssetUrl = "https://www.cryptocompare.com/coins/#SYMBOL#/overview",
                    AssetUrlName = "Auf cryptocompare.com anzeigen...",
                    GetApiKeyUrl = "",
                    MaxUpdateInterval = 300,
                    MinUpdateInterval = 15,
                    UpdateIntervalStepSize = 15,
                    SupportedConvertCurrencies = new List<string>() { "EUR", "USD", "BTC" },
                    UpdateIntervalInfoText = "Diese API unterstützt ein Update Intervall von 15 Sekunden - 5 Minuten und erlaubt 100.000 Aufrufe pro Monat."
                };
            }
        }

        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;
        public event EventHandler<Asset> OnSingleAssetUpdated;
        public event EventHandler<OnApiErrorEventArgs> OnApiError;
        public event EventHandler OnAppDataChanged;
    }
}

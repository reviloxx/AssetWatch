using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssetWatch
{
    public abstract class Api
    {
        /// <summary>
        /// Defines the delay to try again in case there is no connection.
        /// </summary>
        protected static readonly int retryDelay = 1000;

        /// <summary>
        /// Defines the availableAssets.
        /// </summary>
        protected List<Asset> availableAssets;

        /// <summary>
        /// Gets the SubscribedAssets.
        /// </summary>
        protected readonly List<Asset> attachedAssets;

        /// <summary>
        /// Defines the subscribedConvertCurrencies.
        /// </summary>
        protected readonly List<string> attachedConvertCurrencies;

        /// <summary>
        /// Defines the assetUpdateWorker
        /// </summary>
        protected Thread assetUpdateWorker;

        public Api()
        {
            this.availableAssets = new List<Asset>();
            this.attachedAssets = new List<Asset>();
            this.attachedConvertCurrencies = new List<string>();
            this.assetUpdateWorker = new Thread(this.AssetUpdateWorker);
        }

        /// <summary>
        /// Requests updates of the subscribed assets while this API is enabled.
        /// </summary>
        protected void AssetUpdateWorker()
        {
            while (this.ApiData.IsEnabled)
            {
                Task.Run(() => this.GetAssetUpdatesAsync(this.attachedAssets));
                Thread.Sleep(this.ApiData.UpdateInterval * 1000);
            }
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
        /// Attaches an asset to the asset updater.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to attach.</param>
        public virtual void AttachAsset(Asset asset)
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
        public virtual void DetachAsset(DetachAssetArgs args)
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
        /// Requests a single asset update.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> to update.</param>
        public void RequestSingleAssetUpdateAsync(Asset asset)
        {
            //if (!this.assetUpdateWorker.IsAlive)
            //{
            //    return;
            //}

            var assets = new List<Asset>
            {
                asset
            };
            Task.Run(() => this.GetAssetUpdatesAsync(assets));
        }

        protected virtual Task GetAssetUpdatesAsync(List<Asset> assets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fires the OnAvailableAssetsReceived event.
        /// </summary>
        protected void FireOnAvailableAssetsReceived()
        {
            OnAvailableAssetsReceived?.Invoke(this, this.availableAssets);
        }

        /// <summary>
        /// Fires the OnAssetUpdateReceived event.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        protected void FireOnAssetUpdateReceived(Asset asset)
        {
            OnAssetUpdateReceived?.Invoke(this, asset);
        }

        /// <summary>
        /// Fires the OnApiError event.
        /// </summary>
        /// <param name="eventArgs">The eventArgs<see cref="OnApiErrorEventArgs"/></param>
        protected void FireOnApiError(OnApiErrorEventArgs eventArgs)
        {
            this.OnApiError?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Fires the OnAppDataChanged event.
        /// </summary>
        protected void FireOnAppDataChanged()
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

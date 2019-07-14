using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="MultiApiHandler" />
    /// </summary>
    public class MultiApiHandler : IApiHandler
    {
        private readonly AppData appData;

        /// <summary>
        /// Contains the currently asset tiles which are attached to the api handler.
        /// </summary>
        private readonly List<IAssetTile> attachedAssetTiles;

        /// <summary>
        /// Defines the apiHandler
        /// </summary>
        private readonly IApiLoader apiLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiApiHandler"/> class.
        /// </summary>
        public MultiApiHandler(AppData appData, IApiLoader apiLoader)
        {
            this.appData = appData;
            this.apiLoader = apiLoader;
            this.attachedAssetTiles = new List<IAssetTile>();
            this.ReadyApis = new Dictionary<IApi, List<Asset>>();
        }

        /// <summary>
        /// Loads all available APIs by using an IApiLoader, sobscribes to it's events and requests it's available assets.
        /// </summary>
        /// <param name="apiLoader">The apiLoader<see cref="IApiLoader"/></param>
        public void LoadApis()
        {
            // look for valid API librarys, load them from the disk and remove duplicates
            this.LoadedApis = apiLoader.LoadApis()
                .GroupBy(api => api.ApiInfo.ApiName)
                .Select(g => g.First())
                .ToList();

            this.LoadedApis.ForEach(api =>
            {
                // Apply loaded save data to API
                if (this.appData.ApiDataSet.Exists(apiData => apiData.ApiName == api.ApiInfo.ApiName))
                {
                    ApiData apiData = this.appData.ApiDataSet.Find(a => a.ApiName == api.ApiInfo.ApiName);
                    apiData.IncreaseCounter(0);
                    api.ApiData = apiData;
                }
                else
                {
                    ApiData newApiData = new ApiData()
                    {
                        ApiName = api.ApiInfo.ApiName,
                        UpdateInterval = api.ApiInfo.StdUpdateInterval
                    };

                    api.ApiData = newApiData;
                    this.appData.ApiDataSet.Add(newApiData);

                    this.FireOnAppDataChanged();
                }

                // subscribe to events
                api.OnAvailableAssetsReceived += this.Api_OnAvailableAssetsReceived;
                api.OnAssetUpdateReceived += this.Api_OnAssetUpdateReceived;
                api.OnApiError += this.Api_OnApiError;
                api.OnAppDataChanged += this.Api_OnAppDataChanged;

                this.FireOnApiLoaded(api);

                if (api.ApiData.IsEnabled)
                {
                    this.EnableApi(api);
                }
            });
        }

        /// <summary>
        /// Attaches an asset tile to the API handler and it's asset to the right API.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="WpfAssetTile"/> to subscribe.</param>
        /// <param name="requestUpdate">If true, the API will request an update for this asset instantly.</param>
        public void AttachAssetTile(IAssetTile assetTile, bool requestUpdate)
        {
            // search the API to subscribe in the dictionary
            IApi api = this.LoadedApis.FirstOrDefault(a => a.ApiInfo.ApiName == assetTile.AssetTileData.ApiName);
            api.AttachAsset(assetTile.AssetTileData.Asset);

            if (api.ApiData.IsEnabled && requestUpdate)
            {
                api.RequestSingleAssetUpdateAsync(assetTile.AssetTileData.Asset);
            }
            
            this.attachedAssetTiles.Add(assetTile);
        }

        /// <summary>
        /// Detaches an asset tile from the API handler.
        /// </summary>
        /// <param name="assetTile">The assetTile<see cref="WpfAssetTile"/> to unsubscribe</param>
        public void DetachAssetTile(IAssetTile assetTile)
        {
            this.attachedAssetTiles.Remove(assetTile);
            IApi api = this.LoadedApis.FirstOrDefault(a => a.ApiInfo.ApiName == assetTile.AssetTileData.ApiName);

            if (api == null)
            {
                return;
            }
            
            // detach asset if there is no other asset tile with the same asset subscribed to this api
            bool detachAsset = !this.attachedAssetTiles.Exists(att => att.AssetTileData.ApiName == api.ApiInfo.ApiName &&
                                                                      att.AssetTileData.Asset.AssetId == assetTile.AssetTileData.Asset.AssetId);

            // detach convert currency if there is no other asset tile with the same convert currency subscribed to this api
            bool detachConvertCurrency = !this.attachedAssetTiles.Exists(att => att.AssetTileData.ApiName == api.ApiInfo.ApiName &&
                                                                                att.AssetTileData.Asset.ConvertCurrency == assetTile.AssetTileData.Asset.ConvertCurrency);
            
            api.DetachAsset(new DetachAssetArgs()
            {
                Asset = assetTile.AssetTileData.Asset,
                DetachAsset = detachAsset,
                DetachConvertCurrency = detachConvertCurrency
            });
            
        }        

        /// <summary>
        /// Enables the API and requests it's available assets if neccessary.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/></param>
        public void EnableApi(IApi api)
        {
            api.Enable();
            api.ApiData.IsEnabled = true;

            // if this API has not received it's available assets yet, request it
            if (!this.ReadyApis.Any(k => k.Key.ApiInfo.ApiName == api.ApiInfo.ApiName))
            {
                api.RequestAvailableAssetsAsync();
            }
        }

        /// <summary>
        /// Disables the API.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/></param>
        public void DisableApi(IApi api)
        {
            if (this.ReadyApis.Any(k => k.Key.ApiInfo.ApiName == api.ApiInfo.ApiName))
            {
                this.ReadyApis.Remove(api);
            }

            api.Disable();
            api.ApiData.IsEnabled = false;
        }

        /// <summary>
        /// Is called after an API has received it's available assets.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API which has received it's available assets.</param>
        /// <param name="availableAssets">The availableAssets<see cref="List{AssetInfo}"/></param>
        private void Api_OnAvailableAssetsReceived(object sender, List<Asset> availableAssets)
        {
            // check which API sent it's available assets and put them in the dictionary
            IApi api = (IApi)sender;
            this.ReadyApis.Add(api, availableAssets);
        }

        /// <summary>
        /// Is called if something went wrong with the API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API where the error occured.</param>
        /// <param name="e">The e<see cref="EventArgs"/> contain an error type and an error message.</param>
        private void Api_OnApiError(object sender, OnApiErrorEventArgs e)
        {
            IApi api = (IApi)sender;

            if (e.ErrorType == ErrorType.TooManyRequests)
            {
                this.DisableApi(api);
                MessageBox.Show("Die erlaubte Anzahl an Aufrufen dieser API wurde überschritten! \nAPI deaktiviert.", api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (e.ErrorType == ErrorType.Unauthorized)
            {
                this.DisableApi(api);
                MessageBox.Show("API Key ungültig! \nAPI deaktiviert.", api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (e.ErrorType == ErrorType.BadRequest || e.ErrorType == ErrorType.General)
            {
                this.DisableApi(api);
                MessageBox.Show("Fehler: " + e.ErrorMessage + "\nAPI deaktiviert.", api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Is called after an asset has received an update.
        /// Updates all asset tiles which are waiting for updates from this API, for this asset.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/> contains the API which has received an asset update.</param>
        /// <param name="updatedAsset">The updatedAsset<see cref="Asset"/> contains the updated asset.</param>
        private void Api_OnAssetUpdateReceived(object sender, Asset updatedAsset)
        {
            IApi api = (IApi)sender;
            List<IAssetTile> toNotify = this.attachedAssetTiles.FindAll(at => at.AssetTileData.ApiName == api.ApiInfo.ApiName &&
                                                                                at.AssetTileData.Asset.AssetId == updatedAsset.AssetId &&
                                                                                at.AssetTileData.Asset.ConvertCurrency == updatedAsset.ConvertCurrency);

            toNotify.ForEach(a => a.Update(updatedAsset));            
        }

        /// <summary>
        /// Is called after an API has changed the call counter in the AppData.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Api_OnAppDataChanged(object sender, EventArgs e)
        {
            try
            {
                this.FireOnAppDataChanged();
            }
            catch
            {
                // save file might be in use by another process
            }
        }

        /// <summary>
        /// Fires the OnApiLoaded event.
        /// </summary>
        /// <param name="loadedApi">The api<see cref="IApi"/> which was loaded.</param>
        private void FireOnApiLoaded(IApi loadedApi)
        {
            OnApiLoaded?.Invoke(this, loadedApi);
        }

        /// <summary>
        /// Fires the OnAppDataChanged event.
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Gets the LoadedApis
        /// </summary>
        public List<IApi> LoadedApis { get; private set; }

        /// <summary>
        /// Gets the ReadyApis
        /// </summary>
        public Dictionary<IApi, List<Asset>> ReadyApis { get; }

        /// <summary>
        /// Is fired after an assembly which contains an IApi object was loaded.
        /// The event args contain the loaded API.
        /// </summary>
        public event EventHandler<IApi> OnApiLoaded;

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

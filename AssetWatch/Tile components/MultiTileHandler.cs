using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssetWatch
{
    public class MultiTileHandler : ITileHandler
    {
        private IApiHandler apiHandler;

        private List<AssetTile> assetTiles;

        public MultiTileHandler(IApiHandler apiHandler)
        {
            this.apiHandler = apiHandler;
            this.assetTiles = new List<AssetTile>();

            apiHandler.OnApiLoaded += ApiHandler_OnApiLoaded;
            apiHandler.OnApiError += ApiHandler_OnApiError;
        }

        public void AddAssetTile()
        {
            AssetTile asstile = new AssetTile(this.apiHandler.ReadyApis);
            asstile.OnAssetSelected += Asstile_OnAssetSelected;
            asstile.OnAssetUnselected += Asstile_OnAssetUnselected;
            this.assetTiles.Add(asstile);
            asstile.Show();
        }

        private void ApiHandler_OnApiLoaded(object sender, IApi api)
        {
            // TODO: apply loaded save data to API

            this.apiHandler.StartAssetUpdater(api);
        }

        private void ApiHandler_OnApiError(object sender, OnApiErrorEventArgs e)
        {
            IApi api = (IApi)sender;
            MessageBox.Show(e.ErrorMessage, api.ApiInfo.ApiName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Asstile_OnAssetSelected(object sender, EventArgs e)
        {
            apiHandler.SubscribeAssetTile((AssetTile)sender);
        }

        private void Asstile_OnAssetUnselected(object sender, EventArgs e)
        {
            apiHandler.UnsubscribeAssetTile((AssetTile)sender);
        }
    }
}

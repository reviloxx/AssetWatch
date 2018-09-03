using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class ApiHandler
    {
        private Dictionary<IApi, List<AssetInfo>> availableApis;

        private List<AssetTile> assetTilesToSubscribe;

        public ApiHandler()
        {
            List<IApi> apis = ApiLoader.GetApisFromDisk();
            apis.ForEach(api =>
            {
                this.availableApis.Add(api, null);
                api.OnAvailableAssetsReceived += Api_OnAvailableAssetsReceived;
                api.RequestAvailableAssets();
            });
        }

        private void Api_OnAvailableAssetsReceived(object sender, List<AssetInfo> availableAssets)
        {
            IApi api = (IApi)sender;
            this.availableApis[api] = availableAssets;

            List<AssetTile> assetTilesForApi = (List<AssetTile>)this.assetTilesToSubscribe.Where(a => a.ApiName == api.GetApiInfo().ApiName);
            assetTilesForApi.ForEach(assetTile =>
            {
                api.SubscribeAsset(assetTile.Asset);
                this.assetTilesToSubscribe.Remove(assetTile);
            });
        }

        public Dictionary<IApi, List<AssetInfo>> GetAvailableApis()
        {
            return this.availableApis;
        }

        public void SubscribeAssetTile(AssetTile assetTile)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeAssetTile(AssetTile assetTile)
        {
            throw new System.NotImplementedException();
        }
    }
}
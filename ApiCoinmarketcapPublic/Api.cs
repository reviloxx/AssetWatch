using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCoinmarketcapPublic
{
    public class Api : IApi
    {
        private List<Asset> activeAssets;

        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;
        public event EventHandler<Asset> OnSingleAssetUpdated;
        public event EventHandler OnApiError;

        public Api()
        {
            this.activeAssets = new List<Asset>();
        }

        public ApiInfo GetApiInfo()
        {
            return new ApiInfo
            {
                ApiName = "Coinmarketcap.com (public)",
                ApiVersion = "1.0",
                AssetUrl = "",
                AssetUrlName = "Auf Coinmarketcap.com anzeigen",                 
            };                
        }

        public void RequestAvailableAssets()
        {
            throw new NotImplementedException();
        }

        public void SetRefreshInterval()
        {
            throw new NotImplementedException();
        }

        public void SubscribeAsset(Asset assetInfo)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAsset(Asset assetInfo)
        {
            throw new NotImplementedException();
        }
    }
}

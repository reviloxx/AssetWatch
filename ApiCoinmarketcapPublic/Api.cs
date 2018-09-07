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
        private List<Asset> subscribedAssets;
        private int updateInterval;

        public List<Asset> SubscribedAssets
        {
            get
            {
                return this.subscribedAssets;
            }
            private set { }
        }

        public event EventHandler<List<Asset>> OnAvailableAssetsReceived;
        public event EventHandler<Asset> OnSingleAssetUpdated;
        public event EventHandler OnApiError;

        public Api()
        {
            this.subscribedAssets = new List<Asset>();
        }

        public ApiInfo ApiInfo
        {
            get
            {
                return new ApiInfo
                {
                    ApiName = "Coinmarketcap.com (public)",
                    ApiVersion = "1.0",
                    AssetUrl = "",
                    AssetUrlName = "Auf Coinmarketcap.com anzeigen",
                };
            }           
        }

        public void RequestAvailableAssetsAsync()
        {
            throw new NotImplementedException();
        }

        public void SetUpdateInterval(int updateInterval)
        {
            this.updateInterval = updateInterval;
        }

        public void SubscribeAsset(Asset asset)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAsset(Asset asset)
        {
            this.subscribedAssets.Remove(asset);
        }

        public void StartAssetUpdater()
        {
            throw new NotImplementedException();
        }
    }
}

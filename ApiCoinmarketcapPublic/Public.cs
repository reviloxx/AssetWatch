using AssetWatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCoinmarketcapPublic
{
    public class Public : IApi
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

        public Public()
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

        public void StartAssetUpdater()
        {
            throw new NotImplementedException();
        }

        public void SubscribeAsset(string assetName, string convertCurrency)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAsset(string assetName, string convertCurrency)
        {
            throw new NotImplementedException();
        }
    }
}

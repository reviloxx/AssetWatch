using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetWatch;

namespace AssetWatch
{
    public interface IApi
    {
        event EventHandler<List<Asset>> OnAvailableAssetsReceived;
        event EventHandler<Asset> OnSingleAssetUpdated;
        event EventHandler OnApiError;

        ApiInfo GetApiInfo();
        void SubscribeAsset(Asset assetInfo);
        void UnsubscribeAsset(Asset assetInfo);
        void RequestAvailableAssets();
        void SetRefreshInterval();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetWatch;

namespace AssetWatch
{
    public interface IApi
    {
        event EventHandler<List<AssetInfo>> OnAvailableAssetsReceived;
        event EventHandler<AssetInfo> OnSingleAssetUpdated;
        event System.EventHandler OnApiError;

        ApiInfo GetApiInfo();
        void SubscribeAsset(AssetInfo assetInfo);
        void UnsubscribeAsset();
        void RequestAvailableAssets();
        void SetRefreshInterval();
    }
}
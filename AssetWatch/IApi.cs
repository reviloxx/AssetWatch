using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetWatch;

namespace AssetWatch
{
    public interface IApi
    {
        event EventHandler<List<AssetInfo>> OnApiReady;
        event EventHandler<AssetInfo> OnAssetUpdated;
        event System.EventHandler OnApiError;

        ApiInfo GetApiInfo();
        void SubscribeAsset();
        void UnsubscribeAsset();
        void GetAvailableAssets();
        void SetRefreshInterval();
    }
}
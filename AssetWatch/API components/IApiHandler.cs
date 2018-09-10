using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public interface IApiHandler
    {
        event EventHandler<OnApiReadyEventArgs> OnApiReady;

        void SubscribeAssetTile(AssetTile assetTile);
        void UnsubscribeAssetTile(AssetTile assetTile);
    }
}
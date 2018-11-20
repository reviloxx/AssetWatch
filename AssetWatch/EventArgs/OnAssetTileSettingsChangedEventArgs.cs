using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetWatch
{
    public class OnAssetTileSettingsChangedEventArgs
    {
        public Asset NewAsset { get; set; }

        public string NewApiName { get; set; }
    }
}

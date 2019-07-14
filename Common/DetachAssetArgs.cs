using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetWatch
{
    public class DetachAssetArgs
    {
        public Asset Asset { get; set; }

        public bool DetachAsset { get; set; }

        public bool DetachConvertCurrency { get; set; }
    }
}

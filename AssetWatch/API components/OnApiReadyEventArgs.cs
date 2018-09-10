using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class OnApiReadyEventArgs
    {
        public IApi Api { get; set; }

        public List<Asset> Assets { get; set; }
    }
}
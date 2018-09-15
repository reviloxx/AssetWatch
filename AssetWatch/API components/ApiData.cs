using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class ApiData
    {
        public string ApiName { get; set; }

        public int UpdateInterval { get; set; }

        public string ApiKey { get; set; }

        /// <summary>
        /// Gets the number of API calls which are left today.
        /// Negative value for ininite calls.
        /// </summary>
        public int CallsLeft24h { get; set; }        

        public int CallsLeft1mo { get; set; }

        public DateTime CallCount24hStartTime { get; set; }

        public DateTime CallCount1moStartTime { get; set; }

        public bool IsEnabled { get; set; }
    }
}
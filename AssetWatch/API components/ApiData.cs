using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public class ApiData
    {
        public int ApiName
        {
            get => default(int);
            set
            {
            }
        }

        public int UpdateInterval
        {
            get => default(int);
            set
            {
            }
        }

        public int ApiKey
        {
            get => default(int);
            set
            {
            }
        }

        /// <summary>
        /// Gets the number of API calls which are left today.
        /// Negative value for ininite calls.
        /// </summary>
        public int CallsLeft24h { get; set; }

        public DateTime SaveTime { get; set; }
    }
}
using ApiCoinGecko;
using ApiYahooFinance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetWatch
{
    public class ApiLoader : IApiLoader
    {
        public List<IApi> LoadApis()
        {
            var apis = new List<IApi>();

            try
            {
                apis = new List<IApi>
                {
                    new ApiCoinmarketcap.ApiCoinmarketcapProEUR(),
                    new ApiCoinmarketcap.ApiCoinmarketcapProUSD(),
                    new ApiCoinmarketcap.ApiCoinmarketcapProBTC(),
                    new ApiCoinGecko.ApiCoinGecko(),
                    new ApiETFs(),
                    new ApiCommodities(),
                    new ApiStocks()
                };
            }
            catch (Exception e)
            {
                this.FireOnApiLoaderError(e.Message);
            }

            return apis;
        }

        /// <summary>
        /// Fires the OnApiLoaderError event.
        /// </summary>
        /// <param name="message">The message<see cref="string"/> contains a message with information about the occured error.</param>
        private void FireOnApiLoaderError(string message)
        {
            OnApiLoaderError?.Invoke(this, message);
        }

        /// <summary>
        /// Defines the OnApiLoaderError event.
        /// </summary>
        public event EventHandler<string> OnApiLoaderError;
    }
}

using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ApiInfo" />
    /// </summary>
    public class ApiInfo
    {
        /// <summary>
        /// Contains the minimum supported update interval of the API.
        /// </summary>
        public int MinUpdateInterval
        {
            get; set;
        }

        /// <summary>
        /// Contains the maximum supported update interval of the API.
        /// </summary>
        public int MaxUpdateInterval
        {
            get; set;
        }

        /// <summary>
        /// Contains an info text about the supported update intervals of the API.
        /// </summary>
        public string UpdateIntervalInfoText
        {
            get; set;
        }

        /// <summary>
        /// Contains an info text about the API.
        /// </summary>
        public string ApiInfoText
        {
            get; set;
        }

        /// <summary>
        /// Contains an Url to show the asset on a webpage.
        /// </summary>
        public string AssetUrl
        {
            get; set;
        }

        /// <summary>
        /// Contains the name of the Url to display.
        /// </summary>
        public string AssetUrlName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the supported convert currencies.
        /// </summary>
        public List<string> SupportedConvertCurrencies
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the API Name.
        /// </summary>
        public string ApiName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the API Version.
        /// </summary>
        public string ApiVersion
        {
            get; set;
        }
    }
}

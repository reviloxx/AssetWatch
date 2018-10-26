using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ApiInfo" />
    /// </summary>
    public class ApiInfo
    {
        /// <summary>
        /// Gets or sets the MinUpdateInterval.
        /// Contains the minimum supported update interval of the API.
        /// </summary>
        public int MinUpdateInterval { get; set; }

        /// <summary>
        /// Gets or sets the MaxUpdateInterval.
        /// Contains the maximum supported update interval of the API.
        /// </summary>
        public int MaxUpdateInterval { get; set; }

        /// <summary>
        /// Gets or sets the UpdateIntervalStepSize.
        /// Is used for the slider steps.
        /// </summary>
        public int UpdateIntervalStepSize { get; set; }

        /// <summary>
        /// Gets or sets the UpdateIntervalInfoText.
        /// Contains an info text about the supported update intervals of the API.
        /// </summary>
        public string UpdateIntervalInfoText { get; set; }

        /// <summary>
        /// Gets or sets the ApiInfoText.
        /// Contains an info text about the API.
        /// </summary>
        public string ApiInfoText { get; set; }

        /// <summary>
        /// Gets or sets the AssetUrl.
        /// Contains an Url to show the asset on a webpage.
        /// </summary>
        public string AssetUrl { get; set; }

        /// <summary>
        /// Gets or sets the AssetUrlName.
        /// Contains the name of the Url to display.
        /// </summary>
        public string AssetUrlName { get; set; }

        /// <summary>
        /// Gets or sets the supported convert currencies.
        /// </summary>
        public List<string> SupportedConvertCurrencies { get; set; }

        /// <summary>
        /// Gets or sets the API Name.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the API Version.
        /// </summary>
        public string ApiClientVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an API key is required to use this API.
        /// </summary>
        public bool ApiKeyRequired { get; set; }

        /// <summary>
        /// Gets or sets the Url to the website where the user can get an API key.
        /// </summary>
        public string GetApiKeyUrl { get; set; }

        /// <summary>
        /// Gets or sets the type of market for which this API offers data.
        /// </summary>
        public Market Market { get; set; }
    }

    /// <summary>
    /// Defines the Market.
    /// </summary>
    public enum Market
    {
        /// <summary>
        /// Defines the market Cryptocurrencies.
        /// </summary>
        Cryptocurrencies,
        /// <summary>
        /// Defines market the Stocks.
        /// </summary>
        Stocks
    }
}

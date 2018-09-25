using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="ApiData" />
    /// </summary>
    [Serializable]
    public class ApiData
    {
        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the update interval of the API.
        /// </summary>
        public int UpdateInterval { get; set; }

        /// <summary>
        /// Gets or sets the API key to authorize the user.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the number of API calls which are left today.
        /// Negative value for infinite calls.
        /// </summary>
        public int CallsLeft24h { get; set; }

        /// <summary>
        /// Gets or sets the number of API calls which are left this month.
        /// Negative value for infinite calls.
        /// </summary>
        public int CallsLeft1mo { get; set; }

        /// <summary>
        /// Gets or sets the time when the calls left 24h counter was started.
        /// </summary>
        public DateTime CallCount24hStartTime { get; set; }

        /// <summary>
        /// Gets or sets the time when the calls left 1mo counter was started.
        /// </summary>
        public DateTime CallCount1moStartTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the API is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}

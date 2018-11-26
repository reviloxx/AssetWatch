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
        /// Gets or sets the time when the current average call count period started.
        /// Gets reset every 30 days.
        /// </summary>
        public DateTime CallCountStartTime { get; set; }

        /// <summary>
        /// Gets or sets the count of the calls in the current period.
        /// Gets reset every 30 days.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// Gets the average daily call count for the current month.
        /// </summary>
        public int CallCountDailyAverage1m
        {
            get
            {
                int daysCount = Math.Max((DateTime.Now - this.CallCountStartTime).Days + 1, 1);
                

                return this.CallCount / daysCount;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the API is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        public ApiData()
        {
            this.ApiKey = string.Empty;
            this.CallCountStartTime = DateTime.Now;
            this.CallCount = 0;
            this.IsEnabled = false;
        }
    }
}

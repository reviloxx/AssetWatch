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
        /// Gets or sets the date when the current call count per day started.
        /// </summary>
        public DateTime CallCountDayStartTime { get; set; }

        /// <summary>
        /// Gets or sets the date when the current call count per month started.
        /// </summary>
        public DateTime CallCountMonthStartTime { get; set; }

        /// <summary>
        /// Gets or sets the count of the calls at the current day.
        /// </summary>
        public int CallCountDay { get; set; }

        /// <summary>
        /// Gets or sets the count of the calls in the current month.
        /// </summary>
        public int CallCountMonth { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the API is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiData"/> class.
        /// </summary>
        public ApiData()
        {
            this.ApiKey = string.Empty;
            this.CallCountDayStartTime = DateTime.Today;
            this.CallCountMonthStartTime = DateTime.Today;
            this.IsEnabled = false;
        }

        /// <summary>
        /// Increases the call counters or resets them if a new counting period has started.
        /// </summary>
        /// <param name="increaseSum">The increaseSum<see cref="int"/> contains the value to add to the counters.</param>
        public void IncreaseCounter(int increaseSum)
        {
            if (DateTime.Today.Date != this.CallCountDayStartTime.Date)
            {
                this.CallCountDayStartTime = DateTime.Today;
                this.CallCountDay = increaseSum;
            }
            else
            {
                this.CallCountDay += increaseSum;
            }

            if (DateTime.Today.Year != this.CallCountMonthStartTime.Year ||
                DateTime.Today.Month != this.CallCountMonthStartTime.Month)
            {
                this.CallCountMonthStartTime = DateTime.Today;
                this.CallCountMonth = increaseSum;
            }
            else
            {
                this.CallCountMonth += increaseSum;
            }
        }
    }
}

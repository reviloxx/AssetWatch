namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="OnApiErrorEventArgs" />
    /// </summary>
    public class OnApiErrorEventArgs
    {
        /// <summary>
        /// Gets or sets the ErrorType.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Defines the ErrorType
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Defines the General ErrorType.
        /// </summary>
        General,

        /// <summary>
        /// Defines the Unauthorized ErrorType which is caused by an invalid API key.
        /// </summary>
        Unauthorized,

        BadRequest,

        /// <summary>
        /// Defines the TooManyRequests ErrorType.
        /// </summary>
        TooManyRequests
    }
}

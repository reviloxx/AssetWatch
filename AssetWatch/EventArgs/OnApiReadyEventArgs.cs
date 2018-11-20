using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="OnApiReadyEventArgs" />
    /// </summary>
    public class OnApiReadyEventArgs
    {
        /// <summary>
        /// Gets or sets the Api which is ready.
        /// </summary>
        public IApi Api { get; set; }

        /// <summary>
        /// Gets or sets the available assets of the API.
        /// </summary>
        public List<Asset> Assets { get; set; }
    }
}

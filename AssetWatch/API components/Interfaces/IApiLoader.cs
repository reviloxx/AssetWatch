using System;
using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IApiLoader" />
    /// </summary>
    public interface IApiLoader
    {
        /// <summary>
        /// Loads the available APIs.
        /// </summary>
        /// <returns>The <see cref="List{IApi}"/> contains all loaded APIs.</returns>
        List<IApi> LoadApis();

        /// <summary>
        /// Is fired when any error occurs within the API loader.
        /// </summary>
        event EventHandler<string> OnApiLoaderError;
    }
}

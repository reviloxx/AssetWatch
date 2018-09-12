using System.Collections.Generic;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IApiLoader" />
    /// </summary>
    public interface IApiLoader
    {
        /// <summary>
        /// Gets all available APIs.
        /// </summary>
        /// <returns>The <see cref="List{IApi}"/> contains all available APIs.</returns>
        List<IApi> GetApis();
    }
}

using System;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="IFileHandler" />
    /// </summary>
    public interface IFileHandler
    {
        /// <summary>
        /// Saves the app data.
        /// </summary>
        /// <param name="appData">The appData<see cref="AppData"/> to save.</param>
        void SaveAppData(AppData appData);

        /// <summary>
        /// Loads the app data.
        /// </summary>
        /// <returns>The <see cref="AppData"/></returns>
        AppData LoadAppData();

        /// <summary>
        /// Is fired when any error occurs within the file handler.
        /// </summary>
        event EventHandler<string> OnFileHandlerError;
    }
}

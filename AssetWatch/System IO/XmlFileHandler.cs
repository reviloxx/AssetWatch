﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="XmlFileHandler" />
    /// </summary>
    public class XmlFileHandler : IFileHandler
    {
        /// <summary>
        /// Defines the path to load and save the file.
        /// </summary>
        private static readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssetWatchData.xml");

        /// <summary>
        /// Defines the path to load and save the file.
        /// </summary>
        private static readonly string tempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssetWatchDataTemp.xml");

        /// <summary>
        /// Loads the app data.
        /// </summary>
        /// <returns>The <see cref="AppData"/></returns>
        public AppData LoadAppData()
        {
            AppData loadedAppData = new AppData();
            var dataLoaded = false;

            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(AppData));
                        loadedAppData = (AppData)xs.Deserialize(fs);
                    }

                    dataLoaded = true;
                }
            }
            catch (Exception e)
            {
                
            }

            // try to load from temp file
            if (!dataLoaded)
            {
                try
                {
                    if (File.Exists(tempPath))
                    {
                        using (FileStream fs = new FileStream(tempPath, FileMode.Open))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(AppData));
                            loadedAppData = (AppData)xs.Deserialize(fs);
                        }

                        dataLoaded = true;
                    }
                }
                catch (Exception ex)
                {
                    this.FireOnFileHandlerError(ex.Message);
                }
            }

            loadedAppData.AssetTileDataSet = loadedAppData.AssetTileDataSet?
                    .OrderBy(atds => atds.AssetTileName)
                    .ToList();

            return loadedAppData;
        }

        /// <summary>
        /// Saves the app data.
        /// </summary>
        /// <param name="appData">The appData<see cref="AppData"/> to save.</param>
        public void SaveAppData(AppData appData)
        {
            try
            {
                using (FileStream fs = new FileStream(tempPath, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(AppData));

                    if (fs.CanWrite)
                    {
                        xs.Serialize(fs, appData);
                    }
                }

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(AppData));

                    if (fs.CanWrite)
                    {
                        xs.Serialize(fs, appData);
                    }
                }
            }
            catch
            {
                // save file might be used by another process
            }
        }

        /// <summary>
        /// Fires the OnFileHandlerError event.
        /// </summary>
        /// <param name="message">The message<see cref="string"/></param>
        private void FireOnFileHandlerError(string message)
        {
            this.OnFileHandlerError?.Invoke(this, message);
        }

        /// <summary>
        /// Defines the OnFileHandlerError event.
        /// </summary>
        public event EventHandler<string> OnFileHandlerError;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AssetWatch
{
    public class StandardFileHandler : IFileHandler
    {
        private static string saveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AssetWatchData.xml");

        public AppData LoadAppData()
        {
            AppData loadedAppData = new AppData();

            if (File.Exists(saveLocation))
            {
                using (FileStream fs = new FileStream(saveLocation, FileMode.Open))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(AppData));
                    loadedAppData = (AppData)xs.Deserialize(fs);
                }
            }

            loadedAppData.AssetTileDataSet = loadedAppData.AssetTileDataSet.OrderBy(atds => atds.AssetTileName).ToList();

            return loadedAppData;
        }

        public void SaveAppData(AppData appData)
        {
            try
            {
                using (FileStream fs = new FileStream(saveLocation, FileMode.Create))
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
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="DiskApiLoader" />
    /// </summary>
    public class DiskApiLoader : IApiLoader
    {
        // TODO: Cryptowatch import

        /// <summary>
        /// Defines the path which contains the assemblies to load.
        /// </summary>
        private static string path = AppDomain.CurrentDomain.BaseDirectory + @"\Apis\";

        /// <summary>
        /// Loads all available types which implement the IApi interface.
        /// </summary>
        /// <returns>The <see cref="List{IApi}"/> contains instances of the loaded types.</returns>
        public List<IApi> GetApis()
        {
            Assembly ass;
            List<IApi> loadedApis = new List<IApi>();
            string[] folders = Directory.GetDirectories(path);
            List<string> files = new List<string>();

            foreach (string folder in folders)
            {
                files.AddRange((Directory.GetFiles(folder, "*.dll")).ToList());
            }

            for (int i = 0; i < files.Count; i++)
            {
                try
                {
                    ass = Assembly.LoadFile(files[i]);
                    ass.GetExportedTypes().ToList().ForEach(type =>
                    {
                        if (type.GetInterfaces().Contains(typeof(IApi)))
                        {
                            IApi api = (IApi)Activator.CreateInstance(type);
                            loadedApis.Add(api);
                        }
                    });
                }
                catch (Exception e)
                {
                    FireOnApiLoaderError(e.Message);
                }
            }

            return loadedApis;
        }

        /// <summary>
        /// Fires the OnApiLoaderError event.
        /// </summary>
        /// <param name="message">The message<see cref="string"/> contains a message with information about the occured error.</param>
        private void FireOnApiLoaderError(string message)
        {
            OnApiLoaderError?.Invoke(this, message);
        }

        /// <summary>
        /// Defines the OnApiLoaderError event.
        /// </summary>
        public event EventHandler<string> OnApiLoaderError;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AssetWatch
{
    /// <summary>
    /// Defines the <see cref="DiskApiLoader" />
    /// </summary>
    public class DiskApiLoader : IApiLoader
    {
        /// <summary>
        /// Defines the path which contains the assemblies to load.
        /// </summary>
        private static readonly string path = AppDomain.CurrentDomain.BaseDirectory + @"\Apis\";

        /// <summary>
        /// Loads all available types which implement the IApi interface.
        /// </summary>
        /// <returns>The <see cref="List{IApi}"/> contains instances of the loaded types.</returns>
        public List<IApi> LoadApis()
        {
            Assembly ass;
            List<IApi> loadedApis = new List<IApi>();
            string[] folders = Directory.GetDirectories(path);
            List<string> files = new List<string>();

            foreach (string folder in folders)
            {
                files.AddRange((Directory.GetFiles(folder, "*.dll")).ToList());
            }

            Parallel.ForEach(files, file =>
            {
                try
                {
                    ass = Assembly.LoadFile(file);
                    ass.GetExportedTypes()
                    .ToList()
                    .ForEach(type =>
                    {
                        if (type.GetInterfaces().Contains(typeof(IApi)) && !type.IsAbstract)
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
            });

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

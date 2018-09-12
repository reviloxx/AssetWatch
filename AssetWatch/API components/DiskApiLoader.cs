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
        /// <summary>
        /// Defines the directory which contains the assemblies to load.
        /// </summary>
        private static string dir = Directory.GetCurrentDirectory() + @"\Apis\";

        /// <summary>
        /// Loads all available types which implement the IApi interface.
        /// </summary>
        /// <returns>The <see cref="List{IApi}"/> contains instances of the loaded types.</returns>
        public List<IApi> GetApis()
        {
            Assembly ass;
            List<IApi> loadedApis = new List<IApi>();
            string[] files = Directory.GetFiles(dir, "*.dll");

            for (int i = 0; i < files.Length; i++)
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

            return loadedApis;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public static class ApiLoader
    {
        public static List<IApi> GetApisFromDisk()
        {
            string workingDirectory = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(workingDirectory + @"\Apis\", "*.dll");

            return null;
        }
    }
}
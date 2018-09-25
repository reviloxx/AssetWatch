using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetWatch
{
    public interface IFileHandler
    {
        void SaveAppData(AppData appData);

        AppData LoadAppData();
    }
}
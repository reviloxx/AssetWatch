using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for AssetTile.xaml
    /// </summary>
    public partial class AssetTile : Window
    {
        private int currentWorth;

        public event EventHandler<AssetTile> OnRemovingApiSubscription;
        public event EventHandler<AssetTile> OnAddingApiSubscription;

        public AssetTile()
        {
            InitializeComponent();
        }

        public AssetTileData AssetTileData
        {
            get; set;
        }

        public void SetTileStyle()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAsset(object sender, Asset assetInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}

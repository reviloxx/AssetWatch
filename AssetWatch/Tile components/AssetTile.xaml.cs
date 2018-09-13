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
        private double currentWorth;
        private Asset asset;

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

        public void UpdateAsset(object sender, Asset asset)
        {
            this.asset = asset;
            this.currentWorth = double.Parse(this.asset.PriceConvert) * this.AssetTileData.HoldingsCount;
        }
    }
}

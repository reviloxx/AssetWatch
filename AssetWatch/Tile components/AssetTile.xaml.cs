using Blue.Windows;
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
        private StickyWindow stickyWindow;

        private double currentWorth;

        private Dictionary<IApi, List<Asset>> readyApis;

        public event EventHandler OnAssetSelected;
        public event EventHandler OnAssetUnselected;

        public AssetTile(Dictionary<IApi, List<Asset>> readyApis)
        {
            this.readyApis = readyApis;
            this.AssetTileData = new AssetTileData();
            this.Loaded += this.walletWindow_Loaded;
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
            this.AssetTileData.Asset = asset;
            this.currentWorth = double.Parse(this.AssetTileData.Asset.PriceConvert) * this.AssetTileData.HoldingsCount;
            this.Dispatcher.Invoke(() =>
            {
                this.button_Calc.Visibility = Visibility.Visible;
                this.label_AssetPrice.Text = asset.ConvertCurrency + "/" + asset.Symbol;
                this.textBlock_AssetPrice.Text = asset.PriceConvert;
                this.label_Worth.Text = asset.ConvertCurrency;
                this.textBlock_Worth.Text = currentWorth.ToString();
                this.textBlock_AssetSymbol.Text = asset.Symbol;
                textBlock_AssetAmount.Text = this.AssetTileData.HoldingsCount.ToString();
                textBlock_last_Refresh.Text = "@" + asset.LastUpdated.ToString("hh:mm");
            });
        }

        private void walletWindow_Loaded(object sender, RoutedEventArgs e)
        {
            stickyWindow = new StickyWindow(this);
            stickyWindow.StickToScreen = true;
            stickyWindow.StickToOther = true;
            stickyWindow.StickOnResize = true;
            stickyWindow.StickOnMove = true;
            stickyWindow.IsEnabled = true;
        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void button_Info_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Settings_Click(object sender, RoutedEventArgs e)
        {
            AssetTileSettingsWindow assetTileSettingsWindow = new AssetTileSettingsWindow(this.readyApis, this.AssetTileData);
            assetTileSettingsWindow.OnAssetChanged += assetTileSettingsWindow_OnAssetChanged;
            assetTileSettingsWindow.ShowDialog();
        }

        private void assetTileSettingsWindow_OnAssetChanged(object sender, Asset e)
        {
            if (this.AssetTileData.Asset != null)
            {
                this.FireOnAssetUnselected();
            }

            this.AssetTileData.Asset = e;
            this.FireOnAssetSelected();
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Calc_Click(object sender, RoutedEventArgs e)
        {
            CalculatorWindow calc = new CalculatorWindow(this.AssetTileData.Asset);
            calc.ShowDialog();
        }

        private void window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void FireOnAssetUnselected()
        {
            this.OnAssetUnselected?.Invoke(this, null);
        }

        private void FireOnAssetSelected()
        {
            this.OnAssetSelected?.Invoke(this, null);
        }
    }
}

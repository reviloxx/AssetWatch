using Blue.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for AssetTile.xaml
    /// </summary>
    public partial class AssetTile : Window
    {
        /// <summary>
        /// Defines the stickyWindow
        /// </summary>
        private StickyWindow stickyWindow;

        /// <summary>
        /// Defines the currentWorth
        /// </summary>
        private double currentWorth;

        /// <summary>
        /// Defines the profitLoss
        /// </summary>
        private double profitLoss;

        /// <summary>
        /// Defines the globalTileStyle
        /// </summary>
        private TileStyle globalTileStyle;

        /// <summary>
        /// Defines the readyApis
        /// </summary>
        private Dictionary<IApi, List<Asset>> readyApis;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTile"/> class.
        /// </summary>
        /// <param name="readyApis">The readyApis<see cref="Dictionary{IApi, List{Asset}}"/></param>
        /// <param name="globalTileStyle">The globalTileStyle<see cref="TileStyle"/></param>
        public AssetTile(Dictionary<IApi, List<Asset>> readyApis, TileStyle globalTileStyle)
        {
            this.readyApis = readyApis;
            this.globalTileStyle = globalTileStyle;
            this.AssetTileData = new AssetTileData();
            this.Loaded += this.walletWindow_Loaded;
            this.InitializeComponent();
            this.RefreshTileStyle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTile"/> class.
        /// </summary>
        /// <param name="readyApis">The readyApis<see cref="Dictionary{IApi, List{Asset}}"/></param>
        /// <param name="globalTileStyle">The globalTileStyle<see cref="TileStyle"/></param>
        /// <param name="assetTileData">The assetTileData<see cref="AssetTileData"/></param>
        public AssetTile(Dictionary<IApi, List<Asset>> readyApis, TileStyle globalTileStyle, AssetTileData assetTileData)
        {
            this.readyApis = readyApis;
            this.globalTileStyle = globalTileStyle;
            this.AssetTileData = assetTileData;
            this.Loaded += this.walletWindow_Loaded;
            this.RefreshTileStyle();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the AssetTileData
        /// </summary>
        public AssetTileData AssetTileData { get; set; }

        /// <summary>
        /// The UpdateAsset
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void UpdateAsset(object sender, Asset asset)
        {
            this.AssetTileData.Asset = asset;

            this.Dispatcher.Invoke(() =>
            {
                this.RefreshTileStyle();
                this.button_Calc.Visibility = Visibility.Visible;
                this.label_AssetPrice.Text = asset.ConvertCurrency + "/" + asset.Symbol;
                this.textBlock_AssetPrice.Text = asset.PriceConvert;
                this.label_Worth.Text = asset.ConvertCurrency;
                this.textBlock_Worth.Text = this.currentWorth.ToString();
                this.textBlock_AssetSymbol.Text = asset.Symbol;
                this.textBlock_AssetAmount.Text = this.AssetTileData.HoldingsCount.ToString();
                this.textBlock_last_Refresh.Text = "@" + asset.LastUpdated.ToString("hh:mm");
                this.textBlock_Win.Text = string.Format("{0:F4}", this.profitLoss);
            });
        }

        /// <summary>
        /// The RefreshTileStyle
        /// </summary>
        public void RefreshTileStyle()
        {
            this.CalculateProfit();

            if (this.AssetTileData.HasCustomTileStyle)
            {
                if (this.profitLoss > -1)
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.BackgroundColorProfit);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.FontColorProfit));
                }
                else
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.BackgroundColorLoss);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.FontColorLoss));
                }
            }
            else
            {
                if (this.profitLoss > -1)
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.BackgroundColorProfit);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.FontColorProfit));
                }
                else
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.BackgroundColorLoss);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.FontColorLoss));
                }
            }
        }

        /// <summary>
        /// The ChangeFontColor
        /// </summary>
        /// <param name="brush">The brush<see cref="Brush"/></param>
        private void ChangeFontColor(Brush brush)
        {
            this.label_AssetPrice.Foreground = brush;
            this.label_WalletName.Foreground = brush;
            this.label_Worth.Foreground = brush;
            this.textBlock_AssetAmount.Foreground = brush;
            this.textBlock_AssetPrice.Foreground = brush;
            this.textBlock_AssetSymbol.Foreground = brush;
            this.textBlock_last_Refresh.Foreground = brush;
            this.textBlock_Win.Foreground = brush;
            this.textBlock_Worth.Foreground = brush;

            string color = brush.ToString() == "#FFFFFFFF" ? "white" : "black";

            if (!this.button_Calc.IsMouseOver)
            {
                Uri uri = new Uri(@"../Icons/calculator_" + color + ".png", UriKind.Relative);
                this.calculator_Image.Source = new BitmapImage(uri);
            }

            if (!this.button_Settings.IsMouseOver)
            {
                Uri uri = new Uri(@"../Icons/settings_" + color + ".png", UriKind.Relative);
                this.settings_Image.Source = new BitmapImage(uri);
            }

            if (!this.button_Close.IsMouseOver)
            {
                Uri uri = new Uri(@"../Icons/remove-icon-" + color + ".png", UriKind.Relative);
                this.close_Image.Source = new BitmapImage(uri);
            }

            if (!this.button_Info.IsMouseOver)
            {
                Uri uri = new Uri(@"../Icons/info_" + color + ".png", UriKind.Relative);
                this.info_Image.Source = new BitmapImage(uri);
            }
        }

        /// <summary>
        /// The CalculateProfit
        /// </summary>
        private void CalculateProfit()
        {
            if (this.AssetTileData.Asset.PriceConvert == null)
            {
                return;
            }

            this.currentWorth = double.Parse(this.AssetTileData.Asset.PriceConvert) * this.AssetTileData.HoldingsCount;
            this.profitLoss = this.currentWorth - this.AssetTileData.InvestedSum;
        }

        /// <summary>
        /// The walletWindow_Loaded
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void walletWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.stickyWindow = new StickyWindow(this);
            this.stickyWindow.StickToScreen = true;
            this.stickyWindow.StickToOther = true;
            this.stickyWindow.StickOnResize = true;
            this.stickyWindow.StickOnMove = true;
            this.stickyWindow.IsEnabled = true;
        }

        /// <summary>
        /// The button_MouseEnter
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// The button_MouseLeave
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseEventArgs"/></param>
        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// The button_Info_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Info_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The button_Settings_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Settings_Click(object sender, RoutedEventArgs e)
        {
            AssetTileSettingsWindow assetTileSettingsWindow = new AssetTileSettingsWindow(this.readyApis, this.AssetTileData);
            assetTileSettingsWindow.OnAssetChanged += this.assetTileSettingsWindow_OnAssetChanged;
            assetTileSettingsWindow.ShowDialog();
        }

        /// <summary>
        /// The assetTileSettingsWindow_OnAssetChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="Asset"/></param>
        private void assetTileSettingsWindow_OnAssetChanged(object sender, Asset e)
        {
            if (this.AssetTileData.Asset != null)
            {
                this.FireOnAssetUnselected();
            }

            this.AssetTileData.Asset = e;
            this.FireOnAssetSelected();
        }

        /// <summary>
        /// The button_Close_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The button_Calc_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Calc_Click(object sender, RoutedEventArgs e)
        {
            CalculatorWindow calc = new CalculatorWindow(this.AssetTileData.Asset);
            calc.ShowDialog();
        }

        /// <summary>
        /// The window_MouseUp
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/></param>
        private void window_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// The FireOnAssetUnselected
        /// </summary>
        private void FireOnAssetUnselected()
        {
            this.OnAssetUnselected?.Invoke(this, null);
        }

        /// <summary>
        /// The FireOnAssetSelected
        /// </summary>
        private void FireOnAssetSelected()
        {
            this.OnAssetSelected?.Invoke(this, null);
        }

        /// <summary>
        /// Defines the OnAssetSelected
        /// </summary>
        public event EventHandler OnAssetSelected;

        /// <summary>
        /// Defines the OnAssetUnselected
        /// </summary>
        public event EventHandler OnAssetUnselected;
    }
}

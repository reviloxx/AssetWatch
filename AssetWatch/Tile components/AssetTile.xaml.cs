﻿using Blue.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="assetTileData">The assetTileData<see cref="AssetTileData"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        /// <param name="readyApis">The readyApis<see cref="Dictionary{IApi, List{Asset}}"/></param>
        public AssetTile(AssetTileData assetTileData, AppData appData, Dictionary<IApi, List<Asset>> readyApis)
        {
            this.readyApis = readyApis;
            this.globalTileStyle = appData.TileHandlerData.GlobalTileStyle;
            this.AssetTileData = assetTileData;
            this.Loaded += this.walletWindow_Loaded;
            this.InitializeComponent();
            this.Left = this.AssetTileData.TilePosition.FromLeft;
            this.Top = this.AssetTileData.TilePosition.FromTop;
            this.RefreshTileStyle();
        }

        /// <summary>
        /// The UpdateAsset
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public void Update(object sender, Asset asset)
        {
            this.AssetTileData.Asset.PriceConvert = asset.PriceConvert;
            this.AssetTileData.Asset.AssetId = asset.AssetId;
            this.AssetTileData.Asset.Symbol = asset.Symbol;
            this.AssetTileData.Asset.Name = asset.Name;
            this.AssetTileData.Asset.Rank = asset.Rank;
            this.AssetTileData.Asset.SupplyAvailable = asset.SupplyAvailable;
            this.AssetTileData.Asset.SupplyTotal = asset.SupplyTotal;
            this.AssetTileData.Asset.PercentChange24h = asset.PercentChange24h;
            this.CalculateProfit();

            this.Dispatcher.Invoke(() =>
            {
                this.RefreshTileStyle();
                this.RefreshAssetTextblocks();
                this.RefreshTileDataTextblocks();
                this.button_Calc.Visibility = Visibility.Visible;
                this.button_Info.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// The RefreshAssetTextblocks
        /// </summary>
        private void RefreshAssetTextblocks()
        {
            this.label_AssetPrice.Text = this.AssetTileData.Asset.ConvertCurrency + "/" + this.AssetTileData.Asset.Symbol;
            this.textBlock_AssetPrice.Text = TileHelpers.ConvertToValueString(double.Parse(this.AssetTileData.Asset.PriceConvert));
            this.label_Worth.Text = this.AssetTileData.Asset.ConvertCurrency;
            this.textBlock_AssetSymbol.Text = this.AssetTileData.Asset.Symbol;
            this.textBlock_last_Refresh.Text = "@" + this.AssetTileData.Asset.LastUpdated.ToString("HH:mm");
        }

        /// <summary>
        /// The RefreshTileDataTextblocks
        /// </summary>
        private void RefreshTileDataTextblocks()
        {
            this.label_WalletName.Text = this.AssetTileData.AssetTileName;
            this.textBlock_Worth.Text = TileHelpers.ConvertToValueString(this.currentWorth);
            this.textBlock_AssetAmount.Text = TileHelpers.ConvertToValueString(this.AssetTileData.HoldingsCount);
            char sign = this.profitLoss > 0 ? '+' : '-';
            string textBoxWinText = sign + TileHelpers.ConvertToValueString(this.profitLoss) + " " + this.AssetTileData.Asset.ConvertCurrency;
            this.textBlock_Win.Text = textBoxWinText;
        }

        /// <summary>
        /// The RefreshTileStyle
        /// </summary>
        public void RefreshTileStyle()
        {
            if (this.globalTileStyle.Hidden)
            {
                this.Visibility = Visibility.Hidden;
                return;
            }

            this.Visibility = Visibility.Visible;

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
            IApi api = this.readyApis.First(r => r.Key.ApiInfo.ApiName == this.AssetTileData.ApiName).Key;
            InfoWindow infWin = new InfoWindow(api.ApiInfo, this.AssetTileData.Asset);
            infWin.Show();
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
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The assetTileSettingsWindow_OnAssetChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="Asset"/></param>
        private void assetTileSettingsWindow_OnAssetChanged(object sender, Asset e)
        {
            // fire the events if the asset has changed
            if (e.AssetId != this.AssetTileData.Asset.AssetId || e.ConvertCurrency != this.AssetTileData.Asset.ConvertCurrency)
            {
                if (this.AssetTileData.Asset != null)
                {
                    this.FireOnAssetUnselected();
                }

                this.AssetTileData.Asset = e;
                this.FireOnAssetSelected();
            }

            this.CalculateProfit();

            this.Dispatcher.Invoke(() =>
            {
                this.RefreshTileStyle();
                this.RefreshTileDataTextblocks();
            });
        }

        /// <summary>
        /// The button_Close_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Asset löschen?", this.AssetTileData.AssetTileName == null ? string.Empty : this.AssetTileData.AssetTileName,
                MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                this.FireOnAssetTileCLosed();
                this.Close();
            }
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
            this.AssetTileData.TilePosition.FromLeft = this.Left;
            this.AssetTileData.TilePosition.FromTop = this.Top;
            this.FireOnAppDataChanged();
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
        /// The FireOnAppDataChanged
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// The FireOnAssetTileCLosed
        /// </summary>
        private void FireOnAssetTileCLosed()
        {
            this.OnAssetTileClosed?.Invoke(this, null);
        }

        /// <summary>
        /// Gets the AssetTileData
        /// Gets or sets the AssetTileData
        /// </summary>
        public AssetTileData AssetTileData { get; private set; }

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;

        /// <summary>
        /// Defines the OnAssetSelected
        /// </summary>
        public event EventHandler OnAssetSelected;

        /// <summary>
        /// Defines the OnAssetUnselected
        /// </summary>
        public event EventHandler OnAssetUnselected;

        /// <summary>
        /// Defines the OnAssetTileClosed
        /// </summary>
        public event EventHandler OnAssetTileClosed;
    }
}

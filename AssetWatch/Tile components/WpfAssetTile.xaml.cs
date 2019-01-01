using Blue.Windows;
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
    public partial class WpfAssetTile : Window, IAssetTile
    {
        /// <summary>
        /// Defines the stickyWindow.
        /// </summary>
        private StickyWindow stickyWindow;

        /// <summary>
        /// Defines the positionLocked.
        /// </summary>
        private bool positionLocked;

        /// <summary>
        /// Defines the currentWorth.
        /// </summary>
        private double currentWorth;

        /// <summary>
        /// Defines the profitLoss.
        /// </summary>
        private double profitLoss;

        /// <summary>
        /// Defines the globalTileStyle.
        /// </summary>
        private TileStyle globalTileStyle;

        /// <summary>
        /// Defines the readyApis.
        /// </summary>
        private Dictionary<IApi, List<Asset>> readyApis;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfAssetTile"/> class.
        /// </summary>
        /// <param name="assetTileData">The assetTileData<see cref="AssetTileData"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        /// <param name="readyApis">The readyApis<see cref="Dictionary{IApi, List{Asset}}"/></param>
        public WpfAssetTile(AssetTileData assetTileData, AppData appData, Dictionary<IApi, List<Asset>> readyApis)
        {
            this.positionLocked = false;
            this.readyApis = readyApis;
            this.globalTileStyle = appData.TileHandlerData.GlobalTileStyle;
            this.AssetTileData = assetTileData;
            this.Loaded += this.AssetTile_Loaded;
            this.InitializeComponent();
            this.Left = this.AssetTileData.TilePosition.FromLeft;
            this.Top = this.AssetTileData.TilePosition.FromTop;
            this.RefreshTileStyle();
        }

        /// <summary>
        /// Is called by the API handler after it has received an asset update for this asset tile.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/> which has received the update.</param>
        public void Update(Asset asset)
        {
            this.AssetTileData.Asset = asset;
            this.CalculateProfit();

            this.Dispatcher.Invoke(() =>
            {
                this.RefreshTileStyle();
                this.RefreshAssetTextblocks();
                this.RefreshTileDataTextblocks();
                this.button_Calc.Visibility = Visibility.Visible;
                this.button_Info.Visibility = Visibility.Visible;
            });

            // Fires the event so that possibly involved portfolio tiles can refresh
            this.FireOnAssetTileUpdated();
        }

        /// <summary>
        /// Shows the AssetTile.
        /// </summary>
        public new void Show()
        {
            base.Show();
        }

        /// <summary>
        /// The RefreshAssetTextblocks
        /// </summary>
        private void RefreshAssetTextblocks()
        {
            this.label_AssetPrice.Text = this.AssetTileData.Asset.ConvertCurrency + "/" + this.AssetTileData.Asset.Symbol;
            this.textBlock_AssetPrice.Text = TileHelpers.GetValueString(this.AssetTileData.Asset.Price, false);
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
            this.textBlock_Worth.Text = TileHelpers.GetValueString(this.currentWorth, false);
            this.textBlock_AssetAmount.Text = TileHelpers.GetValueString(this.AssetTileData.HoldingsCount, false);
            string textBoxWinText = TileHelpers.GetValueString(this.profitLoss, true) + " " + this.AssetTileData.Asset.ConvertCurrency;
            this.textBlock_Win.Text = textBoxWinText;
        }

        /// <summary>
        /// Refreshes the tile style.
        /// </summary>
        public void RefreshTileStyle()
        {
            if (this.globalTileStyle.Hidden)
            {
                this.Visibility = Visibility.Hidden;
                return;
            }

            this.Visibility = Visibility.Visible;

            Brush backgroundColor = Brushes.Black;
            Brush fontColor = Brushes.Black;

            if (this.AssetTileData.HasCustomTileStyle)
            {
                backgroundColor = this.profitLoss >= 0 ? (Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.BackgroundColorProfit) :
                                                            (Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.BackgroundColorLoss);

                fontColor = this.profitLoss >= 0 ? (Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.FontColorProfit) :
                                                      (Brush)new BrushConverter().ConvertFromString(this.AssetTileData.CustomTileStyle.FontColorLoss);
            }
            else
            {
                backgroundColor = this.profitLoss >= 0 ? (Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.BackgroundColorProfit) :
                                                            (Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.BackgroundColorLoss);

                fontColor = this.profitLoss >= 0 ? (Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.FontColorProfit) :
                                                      (Brush)new BrushConverter().ConvertFromString(this.globalTileStyle.FontColorLoss);
            }

            this.Background = backgroundColor;
            this.ChangeFontColor(fontColor);
        }

        /// <summary>
        /// Locks or unlocks the position of the tile.
        /// </summary>
        /// <param name="locked">The locked<see cref="bool"/></param>
        public void LockPosition(bool locked)
        {
            if (this.stickyWindow != null)
            {
                this.stickyWindow.IsEnabled = !locked;
            }
            this.positionLocked = locked;
        }

        /// <summary>
        /// Changes the font color of the asset tile.
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
        /// Calculates the profit of the asset tile.
        /// </summary>
        private void CalculateProfit()
        {
            this.currentWorth = this.AssetTileData.Asset.Price * this.AssetTileData.HoldingsCount;
            this.profitLoss = this.currentWorth - this.AssetTileData.InvestedSum;
        }

        /// <summary>
        /// Initializes the stickyWindow after the asset tile was loaded.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void AssetTile_Loaded(object sender, RoutedEventArgs e)
        {
            this.stickyWindow = new StickyWindow(this);
            this.stickyWindow.StickToScreen = true;
            this.stickyWindow.StickToOther = true;
            this.stickyWindow.StickOnResize = true;
            this.stickyWindow.StickOnMove = true;
            this.stickyWindow.IsEnabled = !this.positionLocked;
        }

        /// <summary>
        /// The button_Info_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Info_Click(object sender, RoutedEventArgs e)
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
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            AssetTileSettingsWindow assetTileSettingsWindow = new AssetTileSettingsWindow(this.readyApis, this.AssetTileData);
            assetTileSettingsWindow.OnAssetTileSettingsChanged += this.AssetTileSettingsWindow_OnAssetTileSettingsChanged;
            assetTileSettingsWindow.ShowDialog();            
        }

        /// <summary>
        /// The assetTileSettingsWindow_OnAssetChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="Asset"/></param>
        private void AssetTileSettingsWindow_OnAssetTileSettingsChanged(object sender, OnAssetTileSettingsChangedEventArgs e)
        {
            if (e.NewApiName != this.AssetTileData.ApiName ||
                e.NewAsset.AssetId != this.AssetTileData.Asset.AssetId ||
                e.NewAsset.ConvertCurrency != this.AssetTileData.Asset.ConvertCurrency)
            {
                if (this.AssetTileData.Asset != null)
                {
                    this.FireOnAssetUnselected();
                }

                this.AssetTileData.Asset = e.NewAsset;
                this.AssetTileData.ApiName = e.NewApiName;
                this.FireOnAssetSelected();
            }
            else
            {
                // Fire the event if the asset did not change, so that possibly involved portfolio tiles can refresh instantly
                this.FireOnAssetTileUpdated();
            }

            this.CalculateProfit();

            this.Dispatcher.Invoke(() =>
            {
                this.RefreshTileStyle();
                this.RefreshAssetTextblocks();
                this.button_Calc.Visibility = Visibility.Visible;
                this.button_Info.Visibility = Visibility.Visible;

                this.RefreshTileDataTextblocks();
            });

            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The button_Close_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Asset löschen?", this.AssetTileData.AssetTileName == null ? string.Empty : this.AssetTileData.AssetTileName,
                MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                this.FireOnTileCLosed();
                this.Close();
            }
        }

        /// <summary>
        /// The button_Calc_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Calc_Click(object sender, RoutedEventArgs e)
        {
            CalculatorWindow calc = new CalculatorWindow(this.AssetTileData.Asset);
            calc.ShowDialog();
        }

        /// <summary>
        /// The window_MouseUp
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/></param>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
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
        /// The FireOnAssetTileUpdated
        /// </summary>
        private void FireOnAssetTileUpdated()
        {
            this.OnAssetTileUpdated?.Invoke(this, null);
        }

        /// <summary>
        /// The FireOnAssetTileCLosed
        /// </summary>
        private void FireOnTileCLosed()
        {
            this.OnTileClosed?.Invoke(this, null);
        }

        /// <summary>
        /// Gets the AssetTileData
        /// Gets or sets the AssetTileData
        /// </summary>
        public AssetTileData AssetTileData { get; set; }

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
        /// Defines the OnAssetTileUpdated
        /// </summary>
        public event EventHandler OnAssetTileUpdated;

        /// <summary>
        /// Defines the OnAssetTileClosed
        /// </summary>
        public event EventHandler OnTileClosed;
    }
}

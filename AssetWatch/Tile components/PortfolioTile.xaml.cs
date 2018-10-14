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
    /// Interaction logic for PortfolioTile.xaml
    /// </summary>
    public partial class PortfolioTile : Window
    {
        /// <summary>
        /// Defines the stickyWindow
        /// </summary>
        private StickyWindow stickyWindow;

        private bool positionLocked;

        /// <summary>
        /// Defines the availableAssets
        /// </summary>
        private List<Asset> availableAssets;

        /// <summary>
        /// Defines the appData
        /// </summary>
        private AppData appData;
        private double winTotal;

        /// <summary>
        /// Defines the percentage24h
        /// </summary>
        private double percentage24h;

        /// <summary>
        /// Defines the percentage1W
        /// </summary>
        private double percentage1W;        

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioTile"/> class.
        /// </summary>
        /// <param name="portfolioTileData">The portfolioTileData<see cref="PortfolioTileData"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        public PortfolioTile(PortfolioTileData portfolioTileData, AppData appData)
        {
            // TODO: refresh invest if invest of an asset tile changes
            this.InitializeComponent();
            this.availableAssets = new List<Asset>();
            this.PortfolioTileData = portfolioTileData;
            this.appData = appData;
            this.UpdateTextBlocks(null);
            this.Left = this.PortfolioTileData.TilePosition.FromLeft;
            this.Top = this.PortfolioTileData.TilePosition.FromTop;
            this.RefreshTileStyle();
        }

        /// <summary>
        /// The Update
        /// </summary>
        /// <param name="updatedAsset">The updatedAsset<see cref="Asset"/></param>
        public void Update(Asset updatedAsset)
        {
            if (this.PortfolioTileData.AssignedAssetTilesDataSet.Any(asst => asst.Asset.Symbol == updatedAsset.Symbol &&
                                                                             asst.Asset.ConvertCurrency == updatedAsset.ConvertCurrency))
            {
                List<AssetTileData> toUpdate = this.PortfolioTileData.AssignedAssetTilesDataSet.FindAll(asst => asst.Asset.Symbol == updatedAsset.Symbol &&
                                                                             asst.Asset.ConvertCurrency == updatedAsset.ConvertCurrency);

                toUpdate.ForEach(upd => upd.Asset = updatedAsset);
            }
            else
            {
                return;
            }

            this.UpdateTextBlocks(updatedAsset.LastUpdated);
            this.RefreshTileStyle();
        }

        /// <summary>
        /// The RefreshTileStyle
        /// </summary>
        public void RefreshTileStyle()
        {                     
            this.Dispatcher.Invoke(() =>
            {
                Brush posBackgroundColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.BackgroundColorProfit);
                Brush negBackgroundColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.BackgroundColorLoss);                

                if (this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    this.Visibility = Visibility.Hidden;
                    return;
                }

                this.Visibility = Visibility.Visible;

                if (this.PortfolioTileData.HasCustomTileStyle)
                {
                    // TODO: custom tile styles
                }
                else
                {
                    rectangle_Head.Fill = this.winTotal >= 0 ? posBackgroundColor : negBackgroundColor;
                    rectangle_between.Fill = this.winTotal >= 0 ? posBackgroundColor : negBackgroundColor;
                    rectangle_foot.Fill = this.winTotal >= 0 ? posBackgroundColor : negBackgroundColor;

                    rectangle_24h.Fill = this.percentage24h >= 0 ? posBackgroundColor : negBackgroundColor;
                    rectangle_1W.Fill = this.percentage1W >= 0 ? posBackgroundColor : negBackgroundColor;

                    this.ChangeFontColor();
                }
            });            
        }

        /// <summary>
        /// The ChangeFontColor
        /// </summary>
        private void ChangeFontColor()
        {
            Brush posFontColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.FontColorProfit);
            Brush negFontColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.FontColorLoss);

            Brush brushMain = this.winTotal >= 0 ? posFontColor : negFontColor;
            Brush brush24h = this.percentage24h >= 0 ? posFontColor : negFontColor;
            Brush brush1W = this.percentage1W >= 0 ? posFontColor : negFontColor;

            textBlock_PortfolioName.Foreground = brushMain;
            textBlock_last_Refresh.Foreground = brushMain;
            textBlock_Invest.Foreground = brushMain;
            label_Invest.Foreground = brushMain;
            label_Worth.Foreground = brushMain;
            textBlock_Worth.Foreground = brushMain;
            textBlock_ATWin.Foreground = brushMain;

            label_24h.Foreground = brush24h;
            textBlock_24hPercentage.Foreground = brush24h;
            textBlock_24hWin.Foreground = brush24h;
            rectangle_24h.Stroke = brushMain;

            label_1W.Foreground = brush1W;
            textBlock_1WPercentage.Foreground = brush1W;
            textBlock_1WWin.Foreground = brush1W;
            rectangle_1W.Stroke = brush1W;

            string color = brushMain.ToString() == "#FFFFFFFF" ? "white" : "black";

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
        }

        public void LockPosition(bool locked)
        {
            if (this.stickyWindow != null)
            {
                this.stickyWindow.IsEnabled = !locked;
            }
            this.positionLocked = locked;
        }

        /// <summary>
        /// The UpdateTextBlocks
        /// </summary>
        public void UpdateTextBlocks(DateTime? updatedTime)
        {
            List<AssetTileData> assetTilesDataSet = this.PortfolioTileData.AssignedAssetTilesDataSet;

            double investTotal = PortfolioTileHelpers.CalculateInvest(assetTilesDataSet);
            double worthTotal = PortfolioTileHelpers.CalculateWorth(assetTilesDataSet);
            this.winTotal = worthTotal - investTotal;
            this.percentage24h = PortfolioTileHelpers.Calculate24hPercentage(assetTilesDataSet, worthTotal);
            double win24h = PortfolioTileHelpers.CalculateWinLoss(this.percentage24h, worthTotal);
            this.percentage1W = PortfolioTileHelpers.Calculate1WPercentage(assetTilesDataSet, worthTotal);
            double win1W = PortfolioTileHelpers.CalculateWinLoss(this.percentage1W, worthTotal);

            string convertCurrency = string.Empty;

            if (this.PortfolioTileData.AssignedAssetTilesDataSet.Count > 0)
            {
                convertCurrency = " " + this.PortfolioTileData.AssignedAssetTilesDataSet[0].Asset.ConvertCurrency;
            }

            this.Dispatcher.Invoke(() =>
            {
                this.textBlock_PortfolioName.Text = this.PortfolioTileData.PortfolioTileName;

                if (updatedTime != null)
                {
                    this.textBlock_last_Refresh.Text = "@" + ((DateTime)updatedTime).ToString("HH:mm");
                }
                
                this.textBlock_Invest.Text = TileHelpers.FormatValueString(investTotal, false) + convertCurrency;
                this.textBlock_Worth.Text = TileHelpers.FormatValueString(worthTotal, false) + convertCurrency;
                
                this.textBlock_24hPercentage.Text = TileHelpers.FormatValueString(this.percentage24h, true) + " %";
                this.textBlock_24hWin.Text = TileHelpers.FormatValueString(win24h, true) + convertCurrency;
                
                this.textBlock_1WPercentage.Text = TileHelpers.FormatValueString(this.percentage1W, true) + " %";
                this.textBlock_1WWin.Text = TileHelpers.FormatValueString(win1W, true) + convertCurrency;
                
                this.textBlock_ATWin.Text = TileHelpers.FormatValueString(winTotal, true) + convertCurrency;
            });
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
        /// The button_Settings_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Settings_Click(object sender, RoutedEventArgs e)
        {
            PortfolioTileSettingsWindow portfolioTileSettingsWindow = new PortfolioTileSettingsWindow(this.appData, this.PortfolioTileData);
            portfolioTileSettingsWindow.OnPortfolioTileDataChanged += this.PortfolioTileSettingsWindow_OnPortfolioTileDataChanged;
            portfolioTileSettingsWindow.Show();
        }

        /// <summary>
        /// The PortfolioTileSettingsWindow_OnPortfolioTileDataChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void PortfolioTileSettingsWindow_OnPortfolioTileDataChanged(object sender, EventArgs e)
        {
            this.UpdateTextBlocks(null);
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The button_Close_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Portfolio löschen?", this.PortfolioTileData.PortfolioTileName == null ? string.Empty : this.PortfolioTileData.PortfolioTileName,
                MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                // TODO: remove closed portfolio tile
                // this.FireOnPortfoloiTileCLosed();
                this.Close();
            }
        }

        /// <summary>
        /// The Window_MouseUp
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/></param>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.PortfolioTileData.TilePosition.FromLeft = this.Left;
            this.PortfolioTileData.TilePosition.FromTop = this.Top;
            this.FireOnAppDataChanged();
        }

        /// <summary>
        /// The Window_Loaded
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.stickyWindow = new StickyWindow(this);
            this.stickyWindow.StickToScreen = true;
            this.stickyWindow.StickToOther = true;
            this.stickyWindow.StickOnResize = true;
            this.stickyWindow.StickOnMove = true;
            this.stickyWindow.IsEnabled = true;
        }

        /// <summary>
        /// The FireOnAppDataChanged
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Gets the PortfolioTileData
        /// </summary>
        public PortfolioTileData PortfolioTileData { get; private set; }

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;
    }
}

using Blue.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        /// <summary>
        /// Defines the availableAssets
        /// </summary>
        private List<Asset> availableAssets;

        /// <summary>
        /// Defines the appData
        /// </summary>
        private AppData appData;

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

            this.UpdateTextBlocks(updatedAsset.LastUpdated);
        }

        /// <summary>
        /// The UpdateTextBlocks
        /// </summary>
        private void UpdateTextBlocks(DateTime? updatedTime)
        {
            List<AssetTileData> assetTilesDataSet = this.PortfolioTileData.AssignedAssetTilesDataSet;

            double investTotal = PortfolioTileHelpers.CalculateInvest(assetTilesDataSet);
            double worthTotal = PortfolioTileHelpers.CalculateWorth(assetTilesDataSet);
            double winTotal = worthTotal - investTotal;
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
                
                this.textBlock_Invest.Text = TileHelpers.ConvertToValueString(investTotal) + convertCurrency;
                this.textBlock_Worth.Text = TileHelpers.ConvertToValueString(worthTotal) + convertCurrency;
                this.textBlock_24hPercentage.Text = TileHelpers.ConvertToValueString(this.percentage24h).TrimEnd('0') + " %";
                this.textBlock_24hWin.Text = TileHelpers.ConvertToValueString(win24h) + convertCurrency;
                this.textBlock_1WPercentage.Text = TileHelpers.ConvertToValueString(this.percentage1W).TrimEnd('0') + " %";
                this.textBlock_1WWin.Text = TileHelpers.ConvertToValueString(win1W) + convertCurrency;

                char sign = winTotal > 0 ? '+' : '-';
                this.textBlock_ATWin.Text = sign + TileHelpers.ConvertToValueString(winTotal) + convertCurrency;
            });
        }

        /// <summary>
        /// The RefreshTileStyle
        /// </summary>
        public void RefreshTileStyle()
        {
            if (this.appData.TileHandlerData.GlobalTileStyle.Hidden)
            {
                this.Visibility = Visibility.Hidden;
                return;
            }

            this.Visibility = Visibility.Visible;

            if (this.PortfolioTileData.HasCustomTileStyle)
            {
                //if (this.profitLoss > -1)
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.PortfolioTileData.CustomTileStyle.BackgroundColorProfit);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.PortfolioTileData.CustomTileStyle.FontColorProfit));
                }
                //else
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.PortfolioTileData.CustomTileStyle.BackgroundColorLoss);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.PortfolioTileData.CustomTileStyle.FontColorLoss));
                }
            }
            else
            {
                if (this.percentage24h >= 0)
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.BackgroundColorProfit);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.FontColorProfit));
                }
                else
                {
                    this.Background = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.BackgroundColorLoss);
                    this.ChangeFontColor((Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.FontColorLoss));
                }
            }
        }

        /// <summary>
        /// The ChangeFontColor
        /// </summary>
        /// <param name="brush">The brush<see cref="Brush"/></param>
        private void ChangeFontColor(Brush brush)
        {
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

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
        public void Update(AssetTile updatedAsset)
        {
            if (this.PortfolioTileData.AssignedAssetTileIds.Any(id => id == updatedAsset.AssetTileData.AssetTileId))
            {
                // remove asset if the convert currency has changed
                if (this.PortfolioTileData.AssignedAssetTileIds.Count > 1)
                {
                    int otherAssignedId = this.PortfolioTileData.AssignedAssetTileIds.First(a => a != updatedAsset.AssetTileData.AssetTileId);
                    
                    if (this.appData.AssetTileDataSet.First(a => a.AssetTileId == otherAssignedId).Asset.ConvertCurrency != updatedAsset.AssetTileData.Asset.ConvertCurrency)
                    {
                        this.PortfolioTileData.AssignedAssetTileIds.Remove(updatedAsset.AssetTileData.AssetTileId);
                        this.FireOnAppDataChanged();
                    }
                }

                this.UpdateTextBlocks(updatedAsset.AssetTileData.Asset.LastUpdated);
                this.RefreshTileStyle();
            }
            else
            {
                return;
            }            
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
                    // TODO: ADD FEATURE: custom tile styles
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
            List<AssetTileData> assignedAssetTileDatas = this.appData.AssetTileDataSet
                .Where(ass => this.PortfolioTileData.AssignedAssetTileIds.Contains(ass.AssetTileId))
                .ToList();

            double investTotal = TileHelpers.CalculateInvest(assignedAssetTileDatas);
            double worthTotal = TileHelpers.CalculateWorth(assignedAssetTileDatas);
            this.winTotal = worthTotal - investTotal;
            this.percentage24h = TileHelpers.Calculate24hPercentage(assignedAssetTileDatas, worthTotal);
            double win24h = TileHelpers.CalculateWinLoss(this.percentage24h, worthTotal);
            this.percentage1W = TileHelpers.Calculate7dPercentage(assignedAssetTileDatas, worthTotal);
            double win1W = TileHelpers.CalculateWinLoss(this.percentage1W, worthTotal);

            string convertCurrency = string.Empty;

            if (assignedAssetTileDatas.Count > 0)
            {
                convertCurrency = " " + assignedAssetTileDatas[0].Asset.ConvertCurrency;
            }

            this.Dispatcher.Invoke(() =>
            {
                this.textBlock_PortfolioName.Text = this.PortfolioTileData.PortfolioTileName;

                if (updatedTime != null)
                {
                    this.textBlock_last_Refresh.Text = "@" + ((DateTime)updatedTime).ToString("HH:mm");
                }
                
                this.textBlock_Invest.Text = TileHelpers.GetValueString(investTotal, false) + convertCurrency;
                this.textBlock_Worth.Text = TileHelpers.GetValueString(worthTotal, false) + convertCurrency;
                
                this.textBlock_24hPercentage.Text = TileHelpers.GetValueString(this.percentage24h, true) + " %";
                this.textBlock_24hWin.Text = TileHelpers.GetValueString(win24h, true) + convertCurrency;
                
                this.textBlock_1WPercentage.Text = TileHelpers.GetValueString(this.percentage1W, true) + " %";
                this.textBlock_1WWin.Text = TileHelpers.GetValueString(win1W, true) + convertCurrency;
                
                this.textBlock_ATWin.Text = TileHelpers.GetValueString(winTotal, true) + convertCurrency;
            });
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
            portfolioTileSettingsWindow.ShowDialog();
            // TODO: FIX BUG: app crashes when adding an asset tile while a portfolio tile settings window is open
        }

        /// <summary>
        /// The PortfolioTileSettingsWindow_OnPortfolioTileDataChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void PortfolioTileSettingsWindow_OnPortfolioTileDataChanged(object sender, EventArgs e)
        {
            this.UpdateTextBlocks(null);
            this.RefreshTileStyle();
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
                this.FireOnPortfolioTileClosed();
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

        private void FireOnPortfolioTileClosed()
        {
            this.OnPortfolioTileClosed?.Invoke(this, null);
        }

        /// <summary>
        /// Gets the PortfolioTileData
        /// </summary>
        public PortfolioTileData PortfolioTileData { get; private set; }

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;

        public event EventHandler OnPortfolioTileClosed;
    }
}

using Blue.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for PortfolioTile.xaml
    /// </summary>
    public partial class WpfPortfolioTile : Window, IPortfolioTile
    {
        /// <summary>
        /// Defines the stickyWindow
        /// </summary>
        private StickyWindow stickyWindow;

        /// <summary>
        /// Defines the appData
        /// </summary>
        private readonly AppData appData;

        /// <summary>
        /// Defines the winTotal
        /// </summary>
        private double winTotal;

        /// <summary>
        /// Defines the percentage24h
        /// </summary>
        private double percentage24h;

        /// <summary>
        /// Defines the percentage7d
        /// </summary>
        private double percentage7d;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfPortfolioTile"/> class.
        /// </summary>
        /// <param name="portfolioTileData">The portfolioTileData<see cref="PortfolioTileData"/></param>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        public WpfPortfolioTile(PortfolioTileData portfolioTileData, AppData appData)
        {
            this.InitializeComponent();
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
        public void Update(IAssetTile updatedAsset)
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
        }

        /// <summary>
        /// The Show
        /// </summary>
        public new void Show()
        {
            base.Show();
        }

        /// <summary>
        /// The LockPosition
        /// </summary>
        /// <param name="locked">The locked<see cref="bool"/></param>
        public void LockPosition(bool locked)
        {
            if (this.stickyWindow != null)
            {
                this.stickyWindow.IsEnabled = !locked;
            }
        }

        /// <summary>
        /// The RemoveAssetTile
        /// </summary>
        /// <param name="assetTileId">The assetTileId<see cref="int"/></param>
        public void RemoveAssetTile(int assetTileId)
        {
            if (this.PortfolioTileData.AssignedAssetTileIds.Any(id => id == assetTileId))
            {
                this.PortfolioTileData.AssignedAssetTileIds.Remove(assetTileId);
                this.UpdateTextBlocks(null);
                this.RefreshTileStyle();
            }
        }

        /// <summary>
        /// The RefreshTileStyle
        /// </summary>
        public void RefreshTileStyle()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.appData.TileHandlerData.GlobalTileStyle.Hidden)
                {
                    this.Visibility = Visibility.Hidden;
                    return;
                }

                this.Visibility = Visibility.Visible;

                this.SetBackgroundColor();
                this.SetFontColor();
            });
        }

        /// <summary>
        /// Sets the background color.
        /// </summary>
        private void SetBackgroundColor()
        {
            Brush posBackgroundColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.BackgroundColorProfit);
            Brush negBackgroundColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.BackgroundColorLoss);
            
            this.rectangle_Head.Fill = this.winTotal >= 0 ? posBackgroundColor : negBackgroundColor;
            this.rectangle_between.Fill = this.winTotal >= 0 ? posBackgroundColor : negBackgroundColor;
            this.rectangle_foot.Fill = this.winTotal >= 0 ? posBackgroundColor : negBackgroundColor;

            this.rectangle_24h.Fill = this.percentage24h >= 0 ? posBackgroundColor : negBackgroundColor;
            this.rectangle_7d.Fill = this.percentage7d >= 0 ? posBackgroundColor : negBackgroundColor;
        }

        /// <summary>
        /// Sets the font color.
        /// </summary>
        private void SetFontColor()
        {
            Brush posFontColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.FontColorProfit);
            Brush negFontColor = (Brush)new BrushConverter().ConvertFromString(this.appData.TileHandlerData.GlobalTileStyle.FontColorLoss);

            Brush brushMain = this.winTotal >= 0 ? posFontColor : negFontColor;
            Brush brush24h = this.percentage24h >= 0 ? posFontColor : negFontColor;
            Brush brush1W = this.percentage7d >= 0 ? posFontColor : negFontColor;

            this.textBlock_PortfolioName.Foreground = brushMain;
            this.textBlock_last_Refresh.Foreground = brushMain;
            this.textBlock_Invest.Foreground = brushMain;
            this.label_Invest.Foreground = brushMain;
            this.label_Worth.Foreground = brushMain;
            this.textBlock_Worth.Foreground = brushMain;
            this.textBlock_ATWin.Foreground = brushMain;

            this.label_24h.Foreground = brush24h;
            this.textBlock_24hPercentage.Foreground = brush24h;
            this.textBlock_24hWin.Foreground = brush24h;
            this.rectangle_24h.Stroke = brushMain;

            this.label_7d.Foreground = brush1W;
            this.textBlock_7dPercentage.Foreground = brush1W;
            this.textBlock_7dWin.Foreground = brush1W;
            this.rectangle_7d.Stroke = brush1W;

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

        /// <summary>
        /// The UpdateTextBlocks
        /// </summary>
        /// <param name="updateTime">The updateTime<see cref="DateTime?"/></param>
        private void UpdateTextBlocks(DateTime? updateTime)
        {
            List<AssetTileData> assignedAssetTileDatas = this.appData.AssetTileDataSet
                .Where(ass => this.PortfolioTileData.AssignedAssetTileIds.Contains(ass.AssetTileId))
                .ToList();

            double investTotal = TileHelpers.CalculateInvest(assignedAssetTileDatas);
            double worthTotal = TileHelpers.CalculateWorth(assignedAssetTileDatas);
            this.winTotal = worthTotal - investTotal;

            bool percentage24hValid = TileHelpers.Calculate24hPercentage(assignedAssetTileDatas, worthTotal, out this.percentage24h);
            double win24h = TileHelpers.CalculateWinLoss(this.percentage24h, worthTotal);

            bool percentage7dValid = TileHelpers.Calculate7dPercentage(assignedAssetTileDatas, worthTotal, out this.percentage7d);
            double win1W = TileHelpers.CalculateWinLoss(this.percentage7d, worthTotal);

            string convertCurrency = string.Empty;

            if (assignedAssetTileDatas.Count > 0)
            {
                convertCurrency = " " + assignedAssetTileDatas[0].Asset.ConvertCurrency;
            }

            this.Dispatcher.Invoke(() =>
            {
                this.textBlock_PortfolioName.Text = this.PortfolioTileData.PortfolioTileName;

                if (updateTime != null)
                {
                    this.textBlock_last_Refresh.Text = "@" + ((DateTime)updateTime).ToString("HH:mm");
                }

                this.textBlock_Invest.Text = TileHelpers.GetValueString(investTotal, false) + convertCurrency;
                this.textBlock_Worth.Text = TileHelpers.GetValueString(worthTotal, false) + convertCurrency;

                this.AdjustPercentageAreas(percentage24hValid, percentage7dValid);

                this.textBlock_24hPercentage.Text = percentage24hValid ? TileHelpers.GetValueString(Math.Round(this.percentage24h, 2), true) + " %" : "-";
                this.textBlock_24hWin.Text = percentage24hValid ? TileHelpers.GetValueString(win24h, true) + convertCurrency : "-";

                this.textBlock_7dPercentage.Text = percentage7dValid ? TileHelpers.GetValueString(Math.Round(this.percentage7d, 2), true) + " %" : "-";
                this.textBlock_7dWin.Text = percentage7dValid ? TileHelpers.GetValueString(win1W, true) + convertCurrency : "-";

                this.textBlock_ATWin.Text = TileHelpers.GetValueString(this.winTotal, true) + convertCurrency;
            });
        }

        private void AdjustPercentageAreas(bool percentage24hValid, bool percentage7dValid)
        {
            double sizeChange;

            if (!percentage24hValid && Row24h1.Height.Value != 0)
            {
                sizeChange = Row24h1.Height.Value + Row24h2.Height.Value + Row24h3.Height.Value;
                Row24h1.Height = new GridLength(0);
                Row24h2.Height = new GridLength(0);
                Row24h3.Height = new GridLength(0);
                this.Height -= sizeChange;
            }

            if (percentage24hValid && Row24h1.Height.Value == 0)
            {
                Row24h1.Height = new GridLength(33.6);
                Row24h2.Height = new GridLength(33.6);
                Row24h3.Height = new GridLength(32.8);
                sizeChange = Row24h1.Height.Value + Row24h2.Height.Value + Row24h3.Height.Value;
                this.Height += sizeChange;
            }

            if (!percentage7dValid && Row7d1.Height.Value != 0)
            {
                sizeChange = Row7d1.Height.Value + Row7d2.Height.Value + Row7d3.Height.Value;
                Row7d1.Height = new GridLength(0);
                Row7d2.Height = new GridLength(0);
                Row7d3.Height = new GridLength(0);
                this.Height -= sizeChange;
            }

            if (percentage7dValid && Row7d1.Height.Value == 0)
            {
                Row7d1.Height = new GridLength(33.6);
                Row7d2.Height = new GridLength(33.6);
                Row7d3.Height = new GridLength(32.8);
                sizeChange = Row7d1.Height.Value + Row7d2.Height.Value + Row7d3.Height.Value;
                this.Height += sizeChange;
            }
        }

        /// <summary>
        /// The Button_Settings_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            PortfolioTileSettingsWindow portfolioTileSettingsWindow = new PortfolioTileSettingsWindow(this.appData, this.PortfolioTileData);
            portfolioTileSettingsWindow.OnPortfolioTileDataChanged += this.PortfolioTileSettingsWindow_OnPortfolioTileDataChanged;
            this.FireOnPortfolioSettingsWindowOpened();
            portfolioTileSettingsWindow.ShowDialog();
            this.FireOnPortfolioSettingsWindowClosed();
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
        /// The Button_Close_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Portfolio löschen?", this.PortfolioTileData.PortfolioTileName ?? string.Empty,
                MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                this.FireOnTileClosed();
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

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.25)
            };

            settings_Image.BeginAnimation(UIElement.OpacityProperty, animation);
            close_Image.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                To = 0.25,
                Duration = TimeSpan.FromSeconds(0.25)
            };

            settings_Image.BeginAnimation(UIElement.OpacityProperty, animation);
            close_Image.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        /// <summary>
        /// The Window_Loaded
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.stickyWindow = new StickyWindow(this)
            {
                StickToScreen = true,
                StickToOther = true,
                StickOnResize = true,
                StickOnMove = true,
                IsEnabled = true
            };
        }

        /// <summary>
        /// The FireOnAppDataChanged
        /// </summary>
        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// The FireOnTileClosed
        /// </summary>
        private void FireOnTileClosed()
        {
            this.OnTileClosed?.Invoke(this, null);
        }

        /// <summary>
        /// Is fired after a portfolio settings window was opened.
        /// The client then does not allow the user to open a new tile because this would cause a crash.
        /// </summary>
        private void FireOnPortfolioSettingsWindowOpened()
        {
            this.OnPortfolioSettingsWindowOpened?.Invoke(this, null);
        }

        /// <summary>
        /// Is fired after a portfolio settings window was closed.
        /// </summary>
        private void FireOnPortfolioSettingsWindowClosed()
        {
            this.OnPortfolioSettingsWindowClosed?.Invoke(this, null);
        }

        /// <summary>
        /// Gets or sets the PortfolioTileData
        /// Gets the PortfolioTileData
        /// </summary>
        public PortfolioTileData PortfolioTileData { get; set; }

        /// <summary>
        /// Defines the OnAppDataChanged
        /// </summary>
        public event EventHandler OnAppDataChanged;

        /// <summary>
        /// Defines the OnTileClosed
        /// </summary>
        public event EventHandler OnTileClosed;

        /// <summary>
        /// Defines the OnPortfolioSettingsWindowOpened
        /// </summary>
        public event EventHandler OnPortfolioSettingsWindowOpened;

        /// <summary>
        /// Defines the OnPortfolioSettingsWindowClosed
        /// </summary>
        public event EventHandler OnPortfolioSettingsWindowClosed;

    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Defines the apiHandler.
        /// </summary>
        private static IApiHandler apiHandler = new MultiApiHandler();

        /// <summary>
        /// Defines the fileHandler.
        /// </summary>
        private static IFileHandler fileHandler = new StandardFileHandler();

        /// <summary>
        /// Defines the tileHandler.
        /// </summary>
        private ITileHandler tileHandler;

        /// <summary>
        /// Defines the appData to store on the hard disk.
        /// </summary>
        private AppData appData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.appData = fileHandler.LoadAppData();
            menuItem_HideAssetTiles.IsChecked = this.appData.TileHandlerData.GlobalTileStyle.Hidden;
            this.tileHandler = new MultiTileHandler(apiHandler, this.appData);
            this.tileHandler.OnAppDataChanged += this.OnAppDataChanged;
            apiHandler.OnAppDataChanged += this.OnAppDataChanged;
        }

        /// <summary>
        /// The OnAppDataChanged uses the fileHandler to save the changed app data.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void OnAppDataChanged(object sender, EventArgs e)
        {
            fileHandler.SaveAppData(this.appData);
        }

        /// <summary>
        /// The MainSettingsWindow_OnGlobalTileColorChanged calls the tile handler to refresh the style
        /// of it's handled tiles when the user changes it in the settings window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void MainSettingsWindow_OnGlobalTileColorChanged(object sender, EventArgs e)
        {
            this.tileHandler.RefreshTileStyles();
        }

        /// <summary>
        /// The menuItem_Settings_Click opens a new settings window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void menuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsWindow mainSettingsWindow = new MainSettingsWindow(apiHandler, this.appData.TileHandlerData.GlobalTileStyle);
            mainSettingsWindow.Closed += this.OnAppDataChanged;
            mainSettingsWindow.OnGlobalTileStyleChanged += this.MainSettingsWindow_OnGlobalTileColorChanged;
            mainSettingsWindow.Show();
        }

        /// <summary>
        /// The menuItem_AddAssetTile_Click calls the tile handler to open a new asset tile.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void menuItem_AddAssetTile_Click(object sender, RoutedEventArgs e)
        {
            if (this.appData.TileHandlerData.GlobalTileStyle.Hidden)
            {
                this.HideTiles(false);
            }

            this.tileHandler.OpenNewAssetTile();
        }

        /// <summary>
        /// The menuItem_Exit_Click quits the application.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void menuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void menuItem_HideAssetTiles_Checked(object sender, RoutedEventArgs e)
        {
            if (!((MenuItem)sender).IsFocused)
            {
                return;
            }

            this.HideTiles(true);
        }

        private void menuItem_HideAssetTiles_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!((MenuItem)sender).IsFocused)
            {
                return;
            }

            this.HideTiles(false);
        }

        private void HideTiles(bool hide)
        {
            this.appData.TileHandlerData.GlobalTileStyle.Hidden = hide;
            menuItem_HideAssetTiles.IsChecked = hide;
            fileHandler.SaveAppData(this.appData);
            this.tileHandler.RefreshTileStyles();
        }

        private void menuItem_AddPortfolioTile_Click(object sender, RoutedEventArgs e)
        {
            this.tileHandler.OpenNewPortfolioTile();
        }
    }
}

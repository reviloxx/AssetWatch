﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Defines the apiHandler
        /// </summary>
        private readonly IApiHandler apiHandler;

        /// <summary>
        /// Defines the apiLoader
        /// </summary>
        private static readonly IApiLoader apiLoader = new ApiLoader();

        /// <summary>
        /// Defines the fileHandler
        /// </summary>
        private static readonly IFileHandler fileHandler = new XmlFileHandler();

        /// <summary>
        /// Defines the tileHandler
        /// </summary>
        private readonly ITileHandler tileHandler;

        /// <summary>
        /// Defines the mainSettingsWindow
        /// </summary>
        private MainSettingsWindow mainSettingsWindow;

        /// <summary>
        /// Defines the appData to store on the hard disk.
        /// </summary>
        private readonly AppData appData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.CheckRunningProcesses();

            fileHandler.OnFileHandlerError += this.FileHandler_OnFileHandlerError;
            this.appData = fileHandler.LoadAppData();
            this.apiHandler = new MultiApiHandler(this.appData, apiLoader);
            this.tileHandler = new WpfTileHandler(this.apiHandler, this.appData);
            this.mainSettingsWindow = new MainSettingsWindow(this.apiHandler, this.appData.TileHandlerData.GlobalTileStyle);
            this.tileHandler.OnAppDataChanged += this.OnAppDataChanged;
            this.menuItem_HideAssetTiles.IsChecked = this.appData.TileHandlerData.GlobalTileStyle.Hidden;
            this.menuItem_LockTilePositions.IsChecked = this.appData.TileHandlerData.PositionsLocked;
            this.apiHandler.OnAppDataChanged += this.OnAppDataChanged;
        }

        /// <summary>
        /// Shows an error message if something went wrong inside the file handler.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="string"/></param>
        private void FileHandler_OnFileHandlerError(object sender, string e)
        {
            MessageBox.Show(e, "Fehler beim Laden der Daten!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// The HideTiles
        /// </summary>
        /// <param name="hide">The hide<see cref="bool"/></param>
        private void HideTiles(bool hide)
        {
            this.appData.TileHandlerData.GlobalTileStyle.Hidden = hide;
            fileHandler.SaveAppData(this.appData);
            this.tileHandler.RefreshTileStyles();
        }

        /// <summary>
        /// The CheckRunningProcesses
        /// </summary>
        private void CheckRunningProcesses()
        {
            if (Process.GetProcessesByName("Cryptowatch").Count() > 0)
            {
                MessageBox.Show("Bitte Cryptowatch schließen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            if (Process.GetProcessesByName("AssetWatch").Count() > 1)
            {
                MessageBox.Show("AssetWatch läuft bereits!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// The menuItem_AddAssetTile_Click calls the tile handler to open a new asset tile.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_AddAssetTile_Click(object sender, RoutedEventArgs e)
        {
            this.menuItem_LockTilePositions.IsChecked = false;
            this.tileHandler.OpenNewAssetTile();
        }

        /// <summary>
        /// The menuItem_AddPortfolioTile_Click calls the tile handler to open a new asset tile.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_AddPortfolioTile_Click(object sender, RoutedEventArgs e)
        {
            this.menuItem_LockTilePositions.IsChecked = false;
            this.tileHandler.OpenNewPortfolioTile();
        }

        /// <summary>
        /// The menuItem_HideAssetTiles_Checked calls the HideTiles function to hide the tiles.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_HideAssetTiles_Checked(object sender, RoutedEventArgs e)
        {
            this.HideTiles(true);
        }

        /// <summary>
        /// The menuItem_HideAssetTiles_Unchecked calls the HideTiles function to unhide the tiles.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_HideAssetTiles_Unchecked(object sender, RoutedEventArgs e)
        {
            this.HideTiles(false);
        }

        /// <summary>
        /// The menuItem_LockTilePositions_Checked calls the tile handler to lock the tile positions.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_LockTilePositions_Checked(object sender, RoutedEventArgs e)
        {
            this.tileHandler.LockTilePositions(true);
        }

        /// <summary>
        /// The menuItem_LockTilePositions_Unchecked calls the tile handler to unlock the tile positions.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_LockTilePositions_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tileHandler.LockTilePositions(false);
        }

        /// <summary>
        /// The menuItem_Settings_Click opens a new settings window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            if (this.mainSettingsWindow.IsVisible)
            {
                return;
            }

            this.mainSettingsWindow = new MainSettingsWindow(this.apiHandler, this.appData.TileHandlerData.GlobalTileStyle);
            this.mainSettingsWindow.OnAppDataChanged += this.OnAppDataChanged;
            this.mainSettingsWindow.OnGlobalTileStyleChanged += this.MainSettingsWindow_OnGlobalTileStyleChanged;

            this.mainSettingsWindow.ShowDialog();
        }

        /// <summary>
        /// The menuItem_Exit_Click quits the application.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// The MainSettingsWindow_OnGlobalTileColorChanged calls the tile handler to refresh the style
        /// of it's handled tiles when the user changes it in the settings window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void MainSettingsWindow_OnGlobalTileStyleChanged(object sender, EventArgs e)
        {
            this.tileHandler.RefreshTileStyles();
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
    }
}

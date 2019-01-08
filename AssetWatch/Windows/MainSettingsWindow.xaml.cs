using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for MainSettingsWindow.xaml
    /// </summary>
    public partial class MainSettingsWindow : Window
    {
        /// <summary>
        /// Defines the apiHandler
        /// </summary>
        private IApiHandler apiHandler;

        /// <summary>
        /// Defines the globalTileStyle
        /// </summary>
        private TileStyle globalTileStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainSettingsWindow"/> class.
        /// </summary>
        /// <param name="apiHandler">The apiHandler<see cref="IApiHandler"/></param>
        /// <param name="globalTileStyle">The globalTileStyle<see cref="TileStyle"/></param>
        public MainSettingsWindow(IApiHandler apiHandler, TileStyle globalTileStyle)
        {
            this.InitializeComponent();
            this.apiHandler = apiHandler;
            this.globalTileStyle = globalTileStyle;
            this.checkBox_Autostart.IsChecked = this.IsStartupItem();
            this.InitializeColorPickers();

            this.DataContext = new MainSettingsWindowViewModel(this.apiHandler.LoadedApis);
        }

        /// <summary>
        /// The InitializeColorPickers sets the values of the color pickers depending on the values in the global tile style.
        /// </summary>
        private void InitializeColorPickers()
        {
            this.clrPcker_BackgroundProfit.SelectedColor = (Color)ColorConverter.ConvertFromString(this.globalTileStyle.BackgroundColorProfit);
            this.clrPcker_BackgroundLoss.SelectedColor = (Color)ColorConverter.ConvertFromString(this.globalTileStyle.BackgroundColorLoss);

            this.clrPcker_FontProfit.SelectedColor = (Color)ColorConverter.ConvertFromString(this.globalTileStyle.FontColorProfit);
            this.clrPcker_FontLoss.SelectedColor = (Color)ColorConverter.ConvertFromString(this.globalTileStyle.FontColorLoss);

            ObservableCollection<ColorItem> availableFontColors = new ObservableCollection<ColorItem>();
            availableFontColors.Add(new ColorItem(Colors.Black, "black"));
            availableFontColors.Add(new ColorItem(Colors.White, "white"));

            this.clrPcker_FontProfit.AvailableColors = availableFontColors;
            this.clrPcker_FontLoss.AvailableColors = availableFontColors;
        }

        /// <summary>
        /// The IsStartupItem checks if this application is set as a startup item in the windows registry.
        /// </summary>
        /// <returns>The <see cref="bool"/> returns true if this app is set as a startup item.</returns>
        private bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("AssetWatch") == null)

                // The value doesn't exist, the application is not set to run at startup
                return false;
            else

                // The value exists, the application is set to run at startup
                return true;
        }

        /// <summary>
        /// The button_EnableApi_Click calls the API handler to enable the selected API.
        /// Shows an error if an API key is required, but missing.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_EnableApi_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.listView_loadedApis.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            IApi selectedApi = (IApi)this.listView_loadedApis.SelectedItem;

            if (selectedApi.ApiInfo.ApiKeyRequired &&
                (selectedApi.ApiData.ApiKey == null || selectedApi.ApiData.ApiKey == string.Empty))
            {
                System.Windows.MessageBox.Show("Kein API Key gefunden, bitte Key in den API Einstellungen hinzufügen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            this.apiHandler.EnableApi(selectedApi);
            this.listView_loadedApis.SelectedIndex = -1;
            this.listView_loadedApis.SelectedIndex = selectedIndex;
        }

        /// <summary>
        /// The button_disableApi_Click calls the API handler to disable the selected API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_disableApi_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.listView_loadedApis.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            IApi selectedApi = (IApi)this.listView_loadedApis.SelectedItem;

            this.apiHandler.DisableApi(selectedApi);
            this.listView_loadedApis.SelectedIndex = -1;
            this.listView_loadedApis.SelectedIndex = selectedIndex;
        }

        /// <summary>
        /// The button_ApiSettings_Click shows a new API settings window for the selected API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_ApiSettings_Click(object sender, RoutedEventArgs e)
        {
            APISettingsWindow asw = new APISettingsWindow((IApi)this.listView_loadedApis.SelectedItem);
            int selectedIndex = this.listView_loadedApis.SelectedIndex;
            asw.ShowDialog();
            this.listView_loadedApis.SelectedIndex = -1;
            this.listView_loadedApis.SelectedIndex = selectedIndex;
        }

        /// <summary>
        /// The button_OK_Click closes the settings window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.FireOnAppDataChanged();
            this.Close();
        }

        /// <summary>
        /// The listView_loadedApis_SelectionChanged sets the values of the user interface depending on the selected API.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/></param>
        private void ListView_loadedApis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                this.button_ApiSettings.IsEnabled = false;
                return;
            }

            IApi selectedApi = (IApi)e.AddedItems[0];

            this.textBlock_API_Info.Text = selectedApi.ApiInfo.ApiInfoText;
            this.button_ApiSettings.IsEnabled = true;
            this.image_button_ApiSettings.Source = new BitmapImage(new Uri(@"../Icons/settings_black.png", UriKind.Relative));
            this.button_disableApi.IsEnabled = selectedApi.ApiData.IsEnabled;
            this.button_EnableApi.IsEnabled = !selectedApi.ApiData.IsEnabled;
        }

        /// <summary>
        /// The checkBox_Autostart_Checked adds this application to the windows registry as an startup item.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void CheckBox_Autostart_Checked(object sender, RoutedEventArgs e)
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!this.IsStartupItem())
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("AssetWatch", System.Reflection.Assembly.GetEntryAssembly().Location);
            }
        }

        /// <summary>
        /// The checkBox_Autostart_Unchecked removes this application from the windows registry as an startup item.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void CheckBox_Autostart_Unchecked(object sender, RoutedEventArgs e)
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (this.IsStartupItem())
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("AssetWatch", false);
            }
        }

        /// <summary>
        /// The clrPcker_BackgroundProfit_SelectedColorChanged stores the new color string in the globalTileStyle
        /// and calls the FireOnGlobalTileStyleChanged method to fire the event.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedPropertyChangedEventArgs{Color?}"/></param>
        private void ClrPcker_BackgroundProfit_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.BackgroundColorProfit = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        /// <summary>
        /// The clrPcker_BackgroundLoss_SelectedColorChanged stores the new color string in the globalTileStyle
        /// and calls the FireOnGlobalTileStyleChanged method to fire the event.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedPropertyChangedEventArgs{Color?}"/></param>
        private void ClrPcker_BackgroundLoss_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.BackgroundColorLoss = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        /// <summary>
        /// The clrPcker_FontLoss_SelectedColorChanged stores the new color string in the globalTileStyle
        /// and calls the FireOnGlobalTileStyleChanged method to fire the event.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedPropertyChangedEventArgs{Color?}"/></param>
        private void ClrPcker_FontLoss_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.FontColorLoss = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        /// <summary>
        /// The clrPcker_FontProfit_SelectedColorChanged stores the new color string in the globalTileStyle
        /// and calls the FireOnGlobalTileStyleChanged method to fire the event.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedPropertyChangedEventArgs{Color?}"/></param>
        private void ClrPcker_FontProfit_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.FontColorProfit = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        /// <summary>
        /// The FireOnGlobalTileStyleChanged fires the OnGlobalTileStyleChanged event.
        /// </summary>
        private void FireOnGlobalTileStyleChanged()
        {
            this.OnGlobalTileStyleChanged?.Invoke(this, null);
        }

        private void FireOnAppDataChanged()
        {
            this.OnAppDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Defines the OnGlobalTileStyleChanged event.
        /// </summary>
        public event EventHandler OnGlobalTileStyleChanged;

        public event EventHandler OnAppDataChanged;        
    }

    /// <summary>
    /// Defines the <see cref="MainSettingsWindowViewModel" />
    /// </summary>
    public class MainSettingsWindowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainSettingsWindowViewModel"/> class.
        /// </summary>
        /// <param name="loadedApis">The loadedApis<see cref="List{IApi}"/></param>
        public MainSettingsWindowViewModel(List<IApi> loadedApis)
        {
            this.LoadedApis = loadedApis;
        }

        /// <summary>
        /// Gets or sets the LoadedApis
        /// </summary>
        public List<IApi> LoadedApis { get; set; }

        /// <summary>
        /// Gets or sets the GlobalTileStyle
        /// </summary>
        public TileStyle GlobalTileStyle { get; set; }       
    }
}

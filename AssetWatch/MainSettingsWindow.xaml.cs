using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for MainSettingsWindow.xaml
    /// </summary>
    public partial class MainSettingsWindow : Window
    {
        private IApiHandler apiHandler;

        private TileStyle globalTileStyle;        

        public MainSettingsWindow(IApiHandler apiHandler, TileStyle globalTileStyle)
        {
            InitializeComponent();
            this.apiHandler = apiHandler;
            this.globalTileStyle = globalTileStyle;
            checkBox_Autostart.IsChecked = this.IsStartupItem();
            this.InitializeColorPickers();
            

            DataContext = new MainSettingsWindowViewModel
            {
                LoadedApis = this.apiHandler.LoadedApis,
                GlobalTileStyle = this.globalTileStyle
            };
        }

        private void InitializeColorPickers()
        {
            clrPcker_BackgroundProfit.SelectedColor = (Color)ColorConverter.ConvertFromString(globalTileStyle.BackgroundColorProfit);
            clrPcker_BackgroundLoss.SelectedColor = (Color)ColorConverter.ConvertFromString(globalTileStyle.BackgroundColorLoss);

            clrPcker_FontProfit.SelectedColor = (Color)ColorConverter.ConvertFromString(globalTileStyle.FontColorProfit);
            clrPcker_FontLoss.SelectedColor = (Color)ColorConverter.ConvertFromString(globalTileStyle.FontColorLoss);

            ObservableCollection<ColorItem> availableFontColors = new ObservableCollection<ColorItem>();
            availableFontColors.Add(new ColorItem(Colors.Black, "black"));
            availableFontColors.Add(new ColorItem(Colors.White, "white"));

            clrPcker_FontProfit.AvailableColors = availableFontColors;
            clrPcker_FontLoss.AvailableColors = availableFontColors;
        }

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

        private void button_EnableApi_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView_loadedApis.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            IApi selectedApi = (IApi)listView_loadedApis.SelectedItem;

            if (selectedApi.ApiInfo.ApiKeyRequired && selectedApi.ApiData.ApiKey == string.Empty)
            {
                System.Windows.MessageBox.Show("Kein API Key gefunden, bitte Key in den API Einstellungen hinzufügen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.apiHandler.EnableApi(selectedApi);
            this.apiHandler.StartAssetUpdater(selectedApi);
            listView_loadedApis.SelectedIndex = -1;
            listView_loadedApis.SelectedIndex = selectedIndex;
        }

        private void button_disableApi_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView_loadedApis.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            IApi selectedApi = (IApi)listView_loadedApis.SelectedItem;

            this.apiHandler.DisableApi(selectedApi);
            listView_loadedApis.SelectedIndex = -1;
            listView_loadedApis.SelectedIndex = selectedIndex;
        }

        private void button_ApiSettings_Click(object sender, RoutedEventArgs e)
        {
            APISettingsWindow asw = new APISettingsWindow((IApi)listView_loadedApis.SelectedItem);
            int selectedIndex = listView_loadedApis.SelectedIndex;
            asw.ShowDialog();
            listView_loadedApis.SelectedIndex = -1;
            listView_loadedApis.SelectedIndex = selectedIndex;
        }

        private void listView_loadedApis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                button_ApiSettings.IsEnabled = false;
                return;
            }

            IApi selectedApi = (IApi)e.AddedItems[0];

            textBlock_API_Info.Text = selectedApi.ApiInfo.ApiInfoText;
            button_ApiSettings.IsEnabled = true;
            button_disableApi.IsEnabled = selectedApi.ApiData.IsEnabled;
            button_EnableApi.IsEnabled = !selectedApi.ApiData.IsEnabled;
        }

        private void checkBox_Autostart_Checked(object sender, RoutedEventArgs e)
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!IsStartupItem())
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue("AssetWatch", System.Reflection.Assembly.GetEntryAssembly().Location);
            }
        }

        private void checkBox_Autostart_Unchecked(object sender, RoutedEventArgs e)
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (IsStartupItem())
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue("AssetWatch", false);
            }
        }

        private void clrPcker_BackgroundProfit_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.BackgroundColorProfit = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        private void clrPcker_BackgroundLoss_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.BackgroundColorLoss = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        private void clrPcker_FontLoss_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.FontColorLoss = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        private void clrPcker_FontProfit_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.globalTileStyle.FontColorProfit = e.NewValue.ToString();
            this.FireOnGlobalTileStyleChanged();
        }

        private void FireOnGlobalTileStyleChanged()
        {
            this.OnGlobalTileStyleChanged?.Invoke(this, null);
        }

        public event EventHandler OnGlobalTileStyleChanged;
    }

    public class MainSettingsWindowViewModel
    {
        public List<IApi> LoadedApis { get; set; }

        public TileStyle GlobalTileStyle { get; set; }
    }
}

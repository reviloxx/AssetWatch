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

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for MainSettingsWindow.xaml
    /// </summary>
    public partial class MainSettingsWindow : Window
    {
        private IApiHandler apiHandler;

        private List<IApi> loadedApis;

        private TileStyle globalTileStyle;

        public MainSettingsWindow(IApiHandler apiHandler, List<IApi> loadedApis, TileStyle globalTileStyle)
        {
            InitializeComponent();
            this.apiHandler = apiHandler;
            this.loadedApis = loadedApis;
            this.globalTileStyle = globalTileStyle;

            DataContext = new MainSettingsWindowViewModel
            {
                LoadedApis = this.loadedApis,
                GlobalTileStyle = this.globalTileStyle
            };
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
                MessageBox.Show("Kein API Key gefunden, bitte Key in den API Einstellungen hinzufügen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.apiHandler.EnableApi(selectedApi);
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
            asw.ShowDialog();                      
        }

        private void listView_loadedApis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }

            IApi selectedApi = (IApi)e.AddedItems[0];

            textBlock_API_Info.Text = selectedApi.ApiInfo.ApiInfoText;
            button_ApiSettings.IsEnabled = true;
            button_disableApi.IsEnabled = selectedApi.ApiData.IsEnabled;
            button_EnableApi.IsEnabled = !selectedApi.ApiData.IsEnabled;
        }        
    }

    public class MainSettingsWindowViewModel
    {
        public List<IApi> LoadedApis { get; set; }

        public TileStyle GlobalTileStyle { get; set; }
    }
}

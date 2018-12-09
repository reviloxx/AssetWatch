using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for AssetTileSettingsWindow.xaml
    /// </summary>
    public partial class AssetTileSettingsWindow : Window
    {
        /// <summary>
        /// Contains the available APIs which are ready to use.
        /// </summary>
        private Dictionary<IApi, List<Asset>> readyApis;

        /// <summary>
        /// Defines the assetTileData.
        /// </summary>
        private AssetTileData assetTileData;

        /// <summary>
        /// Contains the currently selected API.
        /// </summary>
        private IApi selectedApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTileSettingsWindow"/> class.
        /// </summary>
        /// <param name="readyApis">The readyApis<see cref="Dictionary{IApi, List{Asset}}"/></param>
        /// <param name="assetTileData">The assetTileData<see cref="AssetTileData"/></param>
        public AssetTileSettingsWindow(Dictionary<IApi, List<Asset>> readyApis, AssetTileData assetTileData)
        {
            this.InitializeComponent();
            this.assetTileData = assetTileData;
            this.readyApis = readyApis.OrderBy(r => r.Key.ApiInfo.ApiName).ToDictionary(pair => pair.Key, pair => pair.Value);            
            this.DataContext = new AssetTileSettingsWindowViewModel { ReadyApis = this.readyApis };
            this.InitializeTextBoxes();
            this.InitializeComboBoxes();
        }

        /// <summary>
        /// The InitializeTextBoxes
        /// </summary>
        private void InitializeTextBoxes()
        {
            this.textBox_TileName.Text = this.assetTileData.AssetTileName;
            this.textBox_HoldingsCount.Text = this.assetTileData.HoldingsCount.ToString();
            this.textBox_InvestedSum.Text = this.assetTileData.InvestedSum.ToString();
        }

        /// <summary>
        /// The InitializeComboBoxes
        /// </summary>
        private void InitializeComboBoxes()
        {
            IApi api;

            if (this.assetTileData.ApiName == null)
            {
                return;
            }

            if (this.readyApis.Any(r => r.Key.ApiInfo.ApiName == this.assetTileData.ApiName))
            {
                api = this.readyApis.First(r => r.Key.ApiInfo.ApiName == this.assetTileData.ApiName).Key;
                this.comboBox_Apis.SelectedItem = this.readyApis.First(r => r.Key.ApiInfo.ApiName == this.assetTileData.ApiName);
            }
            else
            {
                MessageBox.Show("API " + this.assetTileData.ApiName + " nicht verfügbar!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (api.ApiInfo.SupportedConvertCurrencies.Contains(this.assetTileData.Asset.ConvertCurrency))
            {
                this.comboBox_ConvertCurrencies.ItemsSource = api.ApiInfo.SupportedConvertCurrencies;
                this.comboBox_ConvertCurrencies.SelectedItem = api.ApiInfo.SupportedConvertCurrencies.Find(cur => cur == this.assetTileData.Asset.ConvertCurrency);
            }
            else
            {
                MessageBox.Show("Währung " + this.assetTileData.Asset.ConvertCurrency + " nicht verfügbar!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.readyApis[api].Any(ass => ass.Symbol == this.assetTileData.Asset.Symbol))
            {
                this.comboBox_Assets.SelectedItem = this.readyApis[api].First(a => a.Symbol == this.assetTileData.Asset.Symbol);
            }
            else
            {
                MessageBox.Show("Asset " + this.assetTileData.Asset.Symbol + " nicht verfügbar!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// The comboBox_Apis_SelectionChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/></param>
        private void comboBox_Apis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBox_Apis.SelectedIndex < 0)
            {
                this.selectedApi = null;
                this.comboBox_ConvertCurrencies.ItemsSource = null;
                this.comboBox_Assets.ItemsSource = null;
                return;
            }

            this.selectedApi = this.readyApis.ElementAt(this.comboBox_Apis.SelectedIndex).Key;
            if (this.selectedApi == null)
            {
                return;
            }

            this.comboBox_ConvertCurrencies.ItemsSource = this.selectedApi.ApiInfo.SupportedConvertCurrencies;

            if (this.comboBox_ConvertCurrencies.Items.Count == 1)
            {
                this.comboBox_ConvertCurrencies.SelectedIndex = 0;
            }

            this.comboBox_Assets.ItemsSource = this.readyApis.ElementAt(this.comboBox_Apis.SelectedIndex).Value;
        }

        /// <summary>
        /// The button_Ok_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {
            double investedSum;
            double holdingsCount;
            Asset selectedAsset = (Asset)this.comboBox_Assets.SelectedValue;

            if (double.TryParse(this.textBox_InvestedSum.Text.Replace('.', ','), out investedSum) &&
                double.TryParse(this.textBox_HoldingsCount.Text.Replace('.', ','), out holdingsCount) &&
                selectedAsset != null && this.comboBox_ConvertCurrencies.SelectedValue != null)
            {

                

                this.assetTileData.AssetTileName = this.textBox_TileName.Text;
                this.assetTileData.InvestedSum = investedSum;
                this.assetTileData.HoldingsCount = holdingsCount;

                Asset newAsset = new Asset
                {
                    AssetId = selectedAsset.AssetId,
                    ConvertCurrency = this.comboBox_ConvertCurrencies.SelectedValue.ToString(),
                    Name = selectedAsset.Name,
                    Symbol = selectedAsset.Symbol
                };

                this.FireOnAssetChanged(new OnAssetTileSettingsChangedEventArgs
                {
                    NewApiName = this.selectedApi.ApiInfo.ApiName,
                    NewAsset = newAsset
                });

                this.assetTileData.ApiName = this.selectedApi.ApiInfo.ApiName;
                this.assetTileData.Asset.AssetId = selectedAsset.AssetId;

                this.Close();
            }
            else
            {
                MessageBox.Show("Ungültige Eingabe!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.button_Ok_Click(null, null);
            }
        }

        /// <summary>
        /// The FireOnAssetChanged
        /// </summary>
        /// <param name="newAsset">The newAsset<see cref="Asset"/></param>
        private void FireOnAssetChanged(OnAssetTileSettingsChangedEventArgs onAssetTileSettingsChangedEventArgs)
        {
            this.OnAssetTileSettingsChanged?.Invoke(this, onAssetTileSettingsChangedEventArgs);
        }

        /// <summary>
        /// Defines the OnAssetChanged
        /// </summary>
        public event EventHandler<OnAssetTileSettingsChangedEventArgs> OnAssetTileSettingsChanged;

        /// <summary>
        /// Defines the <see cref="AssetTileSettingsWindowViewModel" />
        /// </summary>
        public class AssetTileSettingsWindowViewModel
        {
            /// <summary>
            /// Gets or sets the ReadyApis
            /// </summary>
            public Dictionary<IApi, List<Asset>> ReadyApis { get; set; }
        }        
    }
}

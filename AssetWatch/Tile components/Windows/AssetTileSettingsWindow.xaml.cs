using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AssetTileSettingsWindow.xaml
    /// </summary>
    public partial class AssetTileSettingsWindow : Window
    {
        private Dictionary<IApi, List<Asset>> readyApis;

        private AssetTileData assetTileData;

        private IApi selectedApi;
        
        public event EventHandler<Asset> OnAssetChanged;

        public AssetTileSettingsWindow(Dictionary<IApi, List<Asset>> readyApis, AssetTileData assetTileData)
        {
            InitializeComponent();
            this.readyApis = readyApis;
            this.assetTileData = assetTileData;

            this.DataContext = new AssetTileSettingsWindowViewModel { ReadyApis = this.readyApis };
            this.InitializeTextBoxes();
            this.InitializeComboBoxes();
        }

        private void InitializeTextBoxes()
        {
            textBox_TileName.Text = this.assetTileData.AssetTileName;
            textBox_HoldingsCount.Text = this.assetTileData.HoldingsCount.ToString();
            textBox_InvestedSum.Text = this.assetTileData.InvestedSum.ToString();
        }

        private void InitializeComboBoxes()
        {
            IApi api;

            if(this.assetTileData.ApiName == null)
            {
                return;
            }

            if (this.readyApis.Any(r => r.Key.ApiInfo.ApiName == this.assetTileData.ApiName))
            {
                api = this.readyApis.First(r => r.Key.ApiInfo.ApiName == this.assetTileData.ApiName).Key;
                comboBox_Apis.SelectedItem = this.readyApis.First(r => r.Key.ApiInfo.ApiName == this.assetTileData.ApiName);                
            }
            else
            {
                MessageBox.Show("API " + this.assetTileData.ApiName + " nicht verfügbar!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (api.ApiInfo.SupportedConvertCurrencies.Contains(this.assetTileData.Asset.ConvertCurrency))
            {
                comboBox_ConvertCurrencies.SelectedItem = this.assetTileData.Asset.ConvertCurrency;
            }
            else
            {
                MessageBox.Show("Währung " + this.assetTileData.Asset.ConvertCurrency + " nicht verfügbar!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.readyApis[api].Any(ass => ass.Symbol == this.assetTileData.Asset.Symbol))
            {
                comboBox_Assets.SelectedItem = this.readyApis[api].First(a => a.Symbol == this.assetTileData.Asset.Symbol);
            }
            else
            {
                MessageBox.Show("Asset " + this.assetTileData.Asset.Symbol + " nicht verfügbar!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void comboBox_Apis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Apis.SelectedIndex < 0)
            {
                this.selectedApi = null;
                comboBox_ConvertCurrencies.ItemsSource = null;
                comboBox_Assets.ItemsSource = null;
                return;
            }

            this.selectedApi = this.readyApis.ElementAt(comboBox_Apis.SelectedIndex).Key;

            comboBox_ConvertCurrencies.ItemsSource = this.selectedApi.ApiInfo.SupportedConvertCurrencies;
            comboBox_Assets.ItemsSource = this.readyApis.ElementAt(comboBox_Apis.SelectedIndex).Value;
        }

        private void FireOnAssetChanged(Asset newAsset)
        {
            this.OnAssetChanged?.Invoke(this, newAsset);
        }               

        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {
            double investedSum;
            double holdingsCount;
            Asset selectedAsset = (Asset)comboBox_Assets.SelectedValue;

            if (double.TryParse(textBox_InvestedSum.Text.Replace('.', ','), out investedSum) &&
                double.TryParse(textBox_HoldingsCount.Text.Replace('.', ','), out holdingsCount) &&
                selectedAsset != null && comboBox_ConvertCurrencies.SelectedValue != null)
            {
                
                this.assetTileData.ApiName = this.selectedApi.ApiInfo.ApiName;
                this.assetTileData.Asset.AssetId = selectedAsset.AssetId;

                this.assetTileData.AssetTileName = textBox_TileName.Text;
                this.assetTileData.InvestedSum = investedSum;
                this.assetTileData.HoldingsCount = holdingsCount;

                Asset newAsset = new Asset
                {
                    AssetId = selectedAsset.AssetId,
                    ConvertCurrency = comboBox_ConvertCurrencies.SelectedValue.ToString(),
                    Name = selectedAsset.Name,
                    Symbol = selectedAsset.Symbol
                };

                this.FireOnAssetChanged(newAsset);
                this.Close();
            }      
            else
            {
                MessageBox.Show("Ungültige Eingabe!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public class AssetTileSettingsWindowViewModel
        {
            public Dictionary<IApi, List<Asset>> ReadyApis { get; set; }
        }
    }
}

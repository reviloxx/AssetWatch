using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for PortfolioTileSettingsWindow.xaml
    /// </summary>
    public partial class PortfolioTileSettingsWindow : Window
    {
        private PortfolioTileData portfolioTileData;

        private AppData appData;

        public PortfolioTileSettingsWindow(AppData appData, PortfolioTileData portfolioTileData)
        {
            InitializeComponent();
            this.portfolioTileData = portfolioTileData;
            this.appData = appData;

            this.DataContext = this.appData;
            this.Loaded += this.PortfolioTileSettingsWindow_Loaded;
        }

        private void PortfolioTileSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<AssetTileData> assignedAssetTileDatas = this.appData.AssetTileDataSet
                .Where(ass => this.portfolioTileData.AssignedAssetTileIds.Contains(ass.AssetTileId))
                .ToList();

            this.Dispatcher.Invoke(() =>
            {
                textBox_PortfolioName.Text = portfolioTileData.PortfolioTileName;

                assignedAssetTileDatas.ForEach(ass =>
                {
                    if (this.appData.AssetTileDataSet.Any(ast => ast.HoldingsCount == ass.HoldingsCount &&
                                                                    ast.InvestedSum == ass.InvestedSum &&
                                                                    ast.Asset.AssetId == ass.Asset.AssetId &&
                                                                    ast.Asset.ConvertCurrency == ass.Asset.ConvertCurrency))
                    {
                        AssetTileData temp = this.appData.AssetTileDataSet.Find(ast => ast.HoldingsCount == ass.HoldingsCount &&
                                                                    ast.InvestedSum == ass.InvestedSum &&
                                                                    ast.Asset.AssetId == ass.Asset.AssetId &&
                                                                    ast.Asset.ConvertCurrency == ass.Asset.ConvertCurrency);


                        int i = listView_availableAssets.Items.IndexOf(temp);
                        listView_availableAssets.UpdateLayout();
                        listView_availableAssets.ScrollIntoView(listView_availableAssets.Items[i]);
                        (listView_availableAssets.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem).IsSelected = true;                        
                    }                      
                });

                listView_availableAssets.Focus();
            });
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (listView_availableAssets.SelectedItems.Count < 1)
            {
                return;
            }            

            List<AssetTileData> selectedAssetTileDataSet = listView_availableAssets.SelectedItems.Cast<AssetTileData>().ToList();
            string convert = selectedAssetTileDataSet[0].Asset.ConvertCurrency;

            foreach (var sel in selectedAssetTileDataSet)
            {
                if (sel.Asset.ConvertCurrency != convert)
                {
                    MessageBox.Show("Die Basiswährungen der Asset Kacheln müssen übereinstimmen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (sel.Asset.Symbol == null || sel.Asset.ConvertCurrency == null)
                {
                    MessageBox.Show("Ungültige Asset Kachel ausgewählt!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }


            this.portfolioTileData.AssignedAssetTileIds = new List<int>();
            selectedAssetTileDataSet.ForEach(sel => this.portfolioTileData.AssignedAssetTileIds.Add(sel.AssetTileId));

            this.portfolioTileData.PortfolioTileName = textBox_PortfolioName.Text;
            this.FireOnPortfolioTileDataChanged();
            this.Close();
        }

        private void FireOnPortfolioTileDataChanged()
        {
            this.OnPortfolioTileDataChanged?.Invoke(this, null);
        }

        public event EventHandler OnPortfolioTileDataChanged;
    }
}

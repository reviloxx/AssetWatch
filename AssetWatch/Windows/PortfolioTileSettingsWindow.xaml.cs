using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for PortfolioTileSettingsWindow.xaml
    /// </summary>
    public partial class PortfolioTileSettingsWindow : Window
    {
        /// <summary>
        /// Defines the portfolioTileData.
        /// </summary>
        private PortfolioTileData portfolioTileData;

        /// <summary>
        /// Defines the appData.
        /// </summary>
        private AppData appData;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioTileSettingsWindow"/> class.
        /// </summary>
        /// <param name="appData">The appData<see cref="AppData"/></param>
        /// <param name="portfolioTileData">The portfolioTileData<see cref="PortfolioTileData"/></param>
        public PortfolioTileSettingsWindow(AppData appData, PortfolioTileData portfolioTileData)
        {
            this.InitializeComponent();
            this.portfolioTileData = portfolioTileData;
            this.appData = appData;

            this.DataContext = this.appData;
            this.Loaded += this.PortfolioTileSettingsWindow_Loaded;
        }

        /// <summary>
        /// The PortfolioTileSettingsWindow_Loaded initializes the listview_availableAssets.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void PortfolioTileSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<AssetTileData> assignedAssetTileDatas = this.appData.AssetTileDataSet
                .Where(ass => this.portfolioTileData.AssignedAssetTileIds.Contains(ass.AssetTileId))
                .ToList();

            this.Dispatcher.Invoke(() =>
            {
                this.textBox_PortfolioName.Text = this.portfolioTileData.PortfolioTileName;

                assignedAssetTileDatas.ForEach(ass =>
                {
                    if (this.appData.AssetTileDataSet.Any(ast => ast.AssetTileId == ass.AssetTileId))
                    {
                        AssetTileData temp = this.appData.AssetTileDataSet.Find(ast => ast.AssetTileId == ass.AssetTileId);

                        int i = this.listView_availableAssets.Items.IndexOf(temp);
                        this.listView_availableAssets.UpdateLayout();
                        this.listView_availableAssets.ScrollIntoView(this.listView_availableAssets.Items[i]);
                        (this.listView_availableAssets.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem).IsSelected = true;
                    }
                });

                this.listView_availableAssets.Focus();
            });
        }

        /// <summary>
        /// The button_OK_Click checks if the current selection is valid.
        /// If so, fires the OnPortfolioTileDataChanged event and closes the settings window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.listView_availableAssets.SelectedItems.Count < 1)
            {
                return;
            }

            List<AssetTileData> selectedAssetTileDataSet = this.listView_availableAssets.SelectedItems.Cast<AssetTileData>().ToList();
            string convert = selectedAssetTileDataSet[0].Asset.ConvertCurrency;

            foreach (var sel in selectedAssetTileDataSet)
            {
                if (sel.Asset.ConvertCurrency != convert)
                {
                    MessageBox.Show("Die Basiswährungen der ausgewählten Asset Kacheln müssen übereinstimmen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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

            this.portfolioTileData.PortfolioTileName = this.textBox_PortfolioName.Text;
            this.FireOnPortfolioTileDataChanged();
            this.Close();
        }

        /// <summary>
        /// Fires the OnPortfolioTileDataChanged event.
        /// </summary>
        private void FireOnPortfolioTileDataChanged()
        {
            this.OnPortfolioTileDataChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Defines the OnPortfolioTileDataChanged event.
        /// </summary>
        public event EventHandler OnPortfolioTileDataChanged;
    }
}

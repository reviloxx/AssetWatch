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
            this.Dispatcher.Invoke(() =>
            {
                textBox_PortfolioName.Text = portfolioTileData.PortfolioTileName;

                // TODO: select items
                

                //portfolioTileData.AssignedAssetTileDataSet.ForEach(ass =>
                //{
                //    int i = listView_availableAssets.Items.IndexOf(ass);

                //    if (i > -1)
                //    {
                //        DataRowView row = (DataRowView)listView_availableAssets.Items[i];
                //        listView_availableAssets.SelectedItems.Add(row);
                //    }                   

                //    //(listView_availableAssets.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem).IsSelected = true;
                //});
            });
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (listView_availableAssets.SelectedItems == null)
            {
                return;
            }            

            List<AssetTileData> selectedAssetTileDataSet = listView_availableAssets.SelectedItems.Cast<AssetTileData>().ToList();

            this.portfolioTileData.AssignedAssetTilesDataSet = selectedAssetTileDataSet;
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

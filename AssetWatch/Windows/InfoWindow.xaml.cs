using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow(ApiInfo apiInfo, Asset asset)
        {
            InitializeComponent();
            this.Title = asset.Name;
            textblock_Symbol.Text = asset.Symbol;
            textblock_Rank.Text = asset.Rank;
            textblock_Price.Text = asset.PriceUsd;
            textblock_MarketCap.Text = asset.MarketCapUsd;
            textblock_AvailableSupply.Text = asset.SupplyAvailable;
            textblock_TotalSupply.Text = asset.SupplyTotal;
            textblock_PercentChange24h.Text = asset.PercentChange24h;
            
            hyperlink_Asset.NavigateUri = new Uri(apiInfo.AssetUrl.Replace("#NAME#", asset.Name));
            hyperlink_Asset.Inlines.Add(apiInfo.AssetUrlName);
        }

        private void hyperlink_Asset_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void hyperlink_Asset_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

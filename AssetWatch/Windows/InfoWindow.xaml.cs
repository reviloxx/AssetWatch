using System;
using System.Diagnostics;
using System.Windows;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoWindow"/> class.
        /// </summary>
        /// <param name="apiInfo">The apiInfo<see cref="ApiInfo"/></param>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public InfoWindow(ApiInfo apiInfo, Asset asset)
        {
            this.InitializeComponent();
            this.Title = asset.Name;
            this.textblock_Symbol.Text = asset.Symbol;
            this.textblock_Rank.Text = asset.Rank;

            this.label_Price.Text = asset.ConvertCurrency + " Price:";
            this.textblock_Price.Text = TileHelpers.FormatValueString(asset.PriceConvert, false);

            this.label_MarketCap.Text = asset.ConvertCurrency + " Market Cap:";
            this.textblock_MarketCap.Text = TileHelpers.FormatValueString(asset.MarketCapUsd, false);
            this.textblock_AvailableSupply.Text = TileHelpers.FormatValueString(asset.SupplyAvailable, false);
            this.textblock_TotalSupply.Text = TileHelpers.FormatValueString(asset.SupplyTotal, false);
            this.textblock_PercentChange24h.Text = TileHelpers.FormatValueString(asset.PercentChange24h, true) + "%";

            this.hyperlink_Asset.NavigateUri = new Uri(apiInfo.AssetUrl.Replace("#NAME#", asset.Name));
            this.hyperlink_Asset.Inlines.Add(apiInfo.AssetUrlName);
        }

        /// <summary>
        /// The hyperlink_Asset_RequestNavigate navigates to the clicked hyperlink.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="System.Windows.Navigation.RequestNavigateEventArgs"/></param>
        private void hyperlink_Asset_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        /// The hyperlink_Asset_Click closes the info window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void hyperlink_Asset_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

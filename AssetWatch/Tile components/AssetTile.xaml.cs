﻿using System;
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
    /// Interaction logic for AssetTile.xaml
    /// </summary>
    public partial class AssetTile : Window
    {
        private double currentWorth;

        public Asset Asset { get; set; }

        public event EventHandler<AssetTile> OnRemovingApiSubscription;
        public event EventHandler<AssetTile> OnAddingApiSubscription;

        public AssetTile()
        {
            InitializeComponent();
        }

        public AssetTileData AssetTileData
        {
            get; set;
        }

        public void SetTileStyle()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAsset(object sender, Asset asset)
        {           
            this.Asset = asset;
            this.currentWorth = double.Parse(this.Asset.PriceConvert) * this.AssetTileData.HoldingsCount;
            this.Dispatcher.Invoke(() =>
            {
                this.label_AssetPrice.Text = asset.ConvertCurrency + "/" + asset.Symbol;
                this.textBlock_AssetPrice.Text = asset.PriceConvert;
                this.label_Worth.Text = asset.ConvertCurrency;
                this.textBlock_AssetSymbol.Text = asset.Symbol;
            });
        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void button_Info_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Calc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

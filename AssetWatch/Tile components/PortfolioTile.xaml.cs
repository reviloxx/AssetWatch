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
    /// Interaction logic for PortfolioTile.xaml
    /// </summary>
    public partial class PortfolioTile : Window
    {
        private System.Collections.Generic.List<AssetTileData> assignedAssetTiles;

        public PortfolioTile()
        {
            InitializeComponent();
        }               

        public void AddAssetTile()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAssetTile()
        {
            throw new System.NotImplementedException();
        }

        public void SetTileStyle()
        {
            throw new System.NotImplementedException();
        }
    }
}

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
    /// Interaction logic for AssetTile.xaml
    /// </summary>
    public partial class AssetTile : Window
    {
        private int windowStyle;
        private int hasUniqueWindowStyle;
        private int position;

        public string FiatCurrency
        {
            get => default(string);
            set
            {
            }
        }

        public int CurrentWorth
        {
            get => default(int);
            set
            {
            }
        }

        public int InvestedSum
        {
            get => default(int);
            set
            {
            }
        }

        public int HoldingsCount
        {
            get => default(int);
            set
            {
            }
        }

        public void SetWindowStyle()
        {
            throw new System.NotImplementedException();
        }
        public Asset Asset
        {
            get => null;
            set
            {
            }
        }

        public ApiInfo Api
        {
            get => null;
            set
            {
            }
        }

        public void Refresh(object sender, Asset assetInfo)
        {
            throw new System.NotImplementedException();
        }

        public AssetTile()
        {
            InitializeComponent();
        }
    }
}

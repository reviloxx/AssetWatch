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
    /// Interaction logic for CalculatorWindow.xaml
    /// </summary>
    public partial class CalculatorWindow : Window
    {
        private Asset asset;

        public CalculatorWindow(Asset asset)
        {
            InitializeComponent();
            this.asset = asset;
            label_Asset.Content = asset.Symbol;
            label_Convert.Content = asset.ConvertCurrency;            
        }

        private void textBox_Convert_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!textBox_Convert.IsKeyboardFocused)
            {
                return;
            }

            if (textBox_Convert.Text == string.Empty)
            {
                textBox_Asset.Text = string.Empty;
            }

            textBox_Asset.Text = this.CalculateAssetSum().ToString();
        }

        private void textBox_Asset_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!textBox_Asset.IsKeyboardFocused)
            {
                return;
            }

            if (textBox_Asset.Text == string.Empty)
            {
                textBox_Convert.Text = string.Empty;
            }

            textBox_Convert.Text = this.CalculateConvertSum().ToString();
        }

        private double CalculateConvertSum()
        {
            double assetSum;
            double assetPrice;

            if (!double.TryParse(textBox_Asset.Text.Replace('.', ','), out assetSum))
            {
                return 0;
            }

            if (!double.TryParse(this.asset.PriceConvert, out assetPrice))
            {
                return 0;
            }

            return Math.Round(assetSum * assetPrice, 2);
        }

        private double CalculateAssetSum()
        {
            double convertInput;
            double assetPrice;

            if (!double.TryParse(textBox_Convert.Text.Replace('.', ','), out convertInput))
            {
                return 0;
            }

            if (!double.TryParse(this.asset.PriceConvert, out assetPrice))
            {
                return 0;
            }

            double result = Math.Round(convertInput / assetPrice, 5);

            return result;
        }
    }
}

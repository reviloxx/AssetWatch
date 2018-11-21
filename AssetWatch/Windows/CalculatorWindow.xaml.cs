using System;
using System.Windows;
using System.Windows.Controls;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for CalculatorWindow.xaml
    /// </summary>
    public partial class CalculatorWindow : Window
    {
        /// <summary>
        /// Defines the asset
        /// </summary>
        private Asset asset;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorWindow"/> class.
        /// </summary>
        /// <param name="asset">The asset<see cref="Asset"/></param>
        public CalculatorWindow(Asset asset)
        {
            this.InitializeComponent();
            this.asset = asset;
            this.label_Asset.Content = asset.Symbol;
            this.label_Convert.Content = asset.ConvertCurrency;
            this.textBox_Asset.Focus();
        }

        /// <summary>
        /// The textBox_Convert_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/></param>
        private void textBox_Convert_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.textBox_Convert.IsKeyboardFocused)
            {
                return;
            }

            if (this.textBox_Convert.Text == string.Empty)
            {
                this.textBox_Asset.Text = string.Empty;
            }

            this.textBox_Asset.Text = this.CalculateAssetSum().ToString();
        }

        /// <summary>
        /// The textBox_Asset_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/></param>
        private void textBox_Asset_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.textBox_Asset.IsKeyboardFocused)
            {
                return;
            }

            if (this.textBox_Asset.Text == string.Empty)
            {
                this.textBox_Convert.Text = string.Empty;
            }

            this.textBox_Convert.Text = this.CalculateConvertSum().ToString();
        }

        /// <summary>
        /// The CalculateConvertSum
        /// </summary>
        /// <returns>The <see cref="double"/></returns>
        private double CalculateConvertSum()
        {
            double assetSum;

            if (!double.TryParse(this.textBox_Asset.Text.Replace('.', ','), out assetSum))
            {
                return 0;
            }

            return Math.Round(assetSum * asset.Price, 2);
        }

        /// <summary>
        /// The CalculateAssetSum
        /// </summary>
        /// <returns>The <see cref="double"/></returns>
        private double CalculateAssetSum()
        {
            double convertInput;

            if (!double.TryParse(this.textBox_Convert.Text.Replace('.', ','), out convertInput))
            {
                return 0;
            }

            double result = Math.Round(convertInput / asset.Price, 5);

            return result;
        }
    }
}

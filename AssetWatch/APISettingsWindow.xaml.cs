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
    /// Interaction logic for APISettingsWindow.xaml
    /// </summary>
    public partial class APISettingsWindow : Window
    {
        private IApi api;
        public APISettingsWindow(IApi api)
        {
            InitializeComponent();
            this.api = api;
            textbox_API_key.IsEnabled = api.ApiInfo.ApiKeyRequired;
            textbox_API_key.Text = api.ApiData.ApiKey;
        }

        private void button_SaveExit_Click(object sender, RoutedEventArgs e)
        {
            this.api.ApiData.ApiKey = textbox_API_key.Text;
        }
    }
}

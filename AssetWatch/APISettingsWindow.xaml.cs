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
    /// Interaction logic for APISettingsWindow.xaml
    /// </summary>
    public partial class APISettingsWindow : Window
    {
        private IApi api;
        public APISettingsWindow(IApi api)
        {
            InitializeComponent();
            this.api = api;
            textbox_API_key.IsEnabled = api.ApiInfo.ApiKeyRequired && !api.ApiData.IsEnabled;

            if (api.ApiInfo.ApiKeyRequired && api.ApiInfo.GetApiKeyUrl != string.Empty)
            {
                hyperlink_getAPIKey.NavigateUri = new Uri(api.ApiInfo.GetApiKeyUrl);
                textBlock_getAPIKEy.Visibility = Visibility.Visible;
            }

            textBlock_updateIntervalInfo.Text = api.ApiInfo.UpdateIntervalInfoText;
            slider_UpdateInterval.Minimum = api.ApiInfo.MinUpdateInterval / 60;
            slider_UpdateInterval.Maximum = api.ApiInfo.MaxUpdateInterval / 60;
            //textbox_API_key.Text = api.ApiData.ApiKey;
        }

        private void button_SaveExit_Click(object sender, RoutedEventArgs e)
        {
            this.api.ApiData.ApiKey = textbox_API_key.Text;
            this.api.ApiData.UpdateInterval = (int)slider_UpdateInterval.Value * 60;
            this.Close();
        }

        private void hyperlink_getAPIKey_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

using System;
using System.Diagnostics;
using System.Windows;

namespace AssetWatch
{
    /// <summary>
    /// Interaction logic for APISettingsWindow.xaml
    /// </summary>
    public partial class APISettingsWindow : Window
    {
        /// <summary>
        /// Defines the api
        /// </summary>
        private IApi api;

        /// <summary>
        /// Initializes a new instance of the <see cref="APISettingsWindow"/> class.
        /// </summary>
        /// <param name="api">The api<see cref="IApi"/></param>
        public APISettingsWindow(IApi api)
        {
            this.InitializeComponent();
            this.api = api;
            this.textbox_API_key.IsEnabled = api.ApiInfo.ApiKeyRequired && !api.ApiData.IsEnabled;

            if (api.ApiInfo.ApiKeyRequired && api.ApiInfo.GetApiKeyUrl != null)
            {
                this.hyperlink_getAPIKey.NavigateUri = new Uri(api.ApiInfo.GetApiKeyUrl);
                this.textBlock_getAPIKEy.Visibility = Visibility.Visible;
            }

            this.textBlock_updateIntervalInfo.Text = api.ApiInfo.UpdateIntervalInfoText;
            this.slider_UpdateInterval.Minimum = api.ApiInfo.MinUpdateInterval / 60;
            this.slider_UpdateInterval.Maximum = api.ApiInfo.MaxUpdateInterval / 60;

            // TODO: bind slider value
            this.textbox_API_key.Text = api.ApiData.ApiKey;
        }

        /// <summary>
        /// The button_SaveExit_Click saves the settings in the API data object and closes the window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void button_SaveExit_Click(object sender, RoutedEventArgs e)
        {
            this.api.ApiData.ApiKey = this.textbox_API_key.Text;
            this.api.ApiData.UpdateInterval = (int)this.slider_UpdateInterval.Value * 60;
            this.Close();
        }

        /// <summary>
        /// The hyperlink_getAPIKey_RequestNavigate navigates to the clicked URL to get an API key.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="System.Windows.Navigation.RequestNavigateEventArgs"/></param>
        private void hyperlink_getAPIKey_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

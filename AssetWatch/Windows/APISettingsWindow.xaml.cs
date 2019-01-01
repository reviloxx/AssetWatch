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
            this.DataContext = this;
            this.api = api;
            this.UpdateInterval = (double)this.api.ApiData.UpdateInterval / 60.0;

            textbox_API_key.IsEnabled = api.ApiInfo.ApiKeyRequired && !api.ApiData.IsEnabled;

            if (api.ApiInfo.ApiKeyRequired && (api.ApiInfo.GetApiKeyUrl != null && api.ApiInfo.GetApiKeyUrl != string.Empty))
            {
                hyperlink_getAPIKey.NavigateUri = new Uri(api.ApiInfo.GetApiKeyUrl);
                textBlock_getAPIKEy.Visibility = Visibility.Visible;
            }

            textBlock_updateIntervalInfo.Text = api.ApiInfo.UpdateIntervalInfoText;
            slider_UpdateInterval.Minimum = api.ApiInfo.MinUpdateInterval / 60;
            slider_UpdateInterval.Maximum = api.ApiInfo.MaxUpdateInterval / 60;
            slider_UpdateInterval.TickFrequency = api.ApiInfo.UpdateIntervalStepSize / 60;
            textbox_API_key.Text = api.ApiInfo.ApiKeyRequired ? api.ApiData.ApiKey : "nicht erforderlich";
        }

        /// <summary>
        /// The button_SaveExit_Click saves the settings in the API data object and closes the window.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.api.ApiData.ApiKey = this.textbox_API_key.Text;
            this.api.ApiData.UpdateInterval = (int)(this.UpdateInterval * 60);
            this.Close();
        }

        /// <summary>
        /// The hyperlink_getAPIKey_RequestNavigate navigates to the clicked URL to get an API key.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="System.Windows.Navigation.RequestNavigateEventArgs"/></param>
        private void Hyperlink_getAPIKey_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        public double UpdateInterval { get; set; }        
    }
}

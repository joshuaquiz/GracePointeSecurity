using System.Net;
using System.Windows;
using GracePointeSecurity.Library;
using Newtonsoft.Json;

namespace GracePointeSecurity.ManagementApp
{
    /// <summary>
    /// Interaction logic for ProductKey.xaml
    /// </summary>
    public partial class ProductKey
    {
        public ProductKey()
        {
            InitializeComponent();
            SpinnerGrid.Visibility = Visibility.Hidden;
        }

        private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            SpinnerGrid.Visibility = Visibility.Visible;
            var responseString = await new WebClient()
                .DownloadStringTaskAsync(
                    $"https://9pzm7c9rci.execute-api.us-east-1.amazonaws.com/lol/camera-app-product-key?productKey={ProductKeyInput.Text}&orgName={OrganizationNameInput.Text}");
            var result = JsonConvert.DeserializeObject<ProductKeyResponse>(responseString);
            if (result.IsAlreadySetup)
            {
                MessageBox.Show(
                    this,
                    "The account is not configured correctly",
                    "Invalid configuration",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
                Application.Current.Shutdown();
            }
            else
            {
                State.AwsCredentials.AccessKeyId = result.AccessKeyId;
                State.AwsCredentials.SecretAccessKey = result.SecretAccessKey;
                Close();
            }
        }
    }
}
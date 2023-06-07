using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using GracePointeSecurity.Library;
using Newtonsoft.Json;

namespace GracePointeSecurity.ManagementApp;

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
        var result = await GetProductKey(
            ProductKeyInput.Text,
            OrganizationNameInput.Text);
        if (result?.IsAlreadySetup == true)
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
        else if (result == null)
        {
            MessageBox.Show(
                this,
                "The key was not found",
                "Invalid configuration",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK,
                MessageBoxOptions.DefaultDesktopOnly);
        }
        else
        {
            State.AwsCredentials.AccessKeyId = result.AccessKeyId;
            State.AwsCredentials.SecretAccessKey = result.SecretAccessKey;
            Close();
        }
    }

    public static async ValueTask<ProductKeyResponse?> GetProductKey(
        string? productionKey,
        string? organizationName)
    {
        var responseString = await new HttpClient()
            .GetStringAsync(
                $"https://9pzm7c9rci.execute-api.us-east-1.amazonaws.com/lol/camera-app-product-key?productKey={productionKey}&orgName={organizationName}");
        return JsonConvert.DeserializeObject<ProductKeyResponse?>(responseString);
    }
}
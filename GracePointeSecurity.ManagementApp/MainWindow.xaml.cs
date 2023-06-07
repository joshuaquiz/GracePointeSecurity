using System.Windows;
using GracePointeSecurity.Library;

namespace GracePointeSecurity.ManagementApp;

public partial class MainWindow
{
    public MainWindow()
    {
        while ((string.IsNullOrWhiteSpace(State.AwsCredentials.SecretAccessKey)
               || string.IsNullOrWhiteSpace(State.AwsCredentials.ProductionKey)
               || string.IsNullOrWhiteSpace(State.AwsCredentials.OriginationName))
               && ProductKey.GetProductKey(
                       State.AwsCredentials.ProductionKey,
                       State.AwsCredentials.OriginationName)
                   .GetAwaiter()
                   .GetResult()?.IsAlreadySetup != true)
        {
            new ProductKey().ShowDialog();
        }

        while (string.IsNullOrWhiteSpace(State.CurrentState.OriginalVideoFolder)
            || string.IsNullOrWhiteSpace(State.CurrentState.SortedVideoFolder))
        {
            new SettingsWindow().ShowDialog();
        }

        InitializeComponent();
        var logging = new Logging(s =>
            Logs.Dispatcher.InvokeAsync(() =>
            {
                Logs.Text = s;
                Logs.ScrollToEnd();
            }));
        BackgroundJobs.Logging = logging;
        BackgroundJobs.Startup();
    }

    private void SettingsButton_Onclick(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog();
        State.UpdateCurrentState();
        BackgroundJobs.ResetSettingsBiasedJobs();
    }

    private void LocateButton_OnClickButton_Onclick(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().ShowDialog();
    }
}
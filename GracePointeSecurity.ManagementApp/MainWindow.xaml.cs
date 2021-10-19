using System.Windows;
using GracePointeSecurity.Library;

namespace GracePointeSecurity.ManagementApp
{
	public partial class MainWindow
	{
        public MainWindow()
		{
            if (string.IsNullOrWhiteSpace(State.AwsCredentials.SecretAccessKey))
            {
                new ProductKey().ShowDialog();
			}

            if (string.IsNullOrWhiteSpace(State.CurrentState.OriginalVideoFolder)
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
    }
}
using System.Windows;
using GracePointeSecurity.Library;

namespace GracePointeSecurity.ManagementApp
{
	public partial class MainWindow
	{
        public MainWindow()
		{
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
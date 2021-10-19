using System.Windows;
using GracePointeSecurity.Library;

namespace GracePointeSecurity.ManagementApp
{
	public partial class SettingsWindow
	{
        public SettingsWindow()
		{
			InitializeComponent();
		}

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            State.AwsCredentials.AccessKeyId = string.Empty;
            State.AwsCredentials.SecretAccessKey = string.Empty;
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
using GracePointeSecurity.Library;
using Hangfire;

namespace GracePointeSecurity.ManagementApp
{
	public partial class App
	{
        private readonly BackgroundJobServer _backgroundJobServer;

		public App()
		{
			State.InitHangfire();
			_backgroundJobServer = new BackgroundJobServer();
			Exit += App_Exit;
		}

		private void App_Exit(object sender, System.Windows.ExitEventArgs e)
		{
			_backgroundJobServer.Dispose();
		}
    }
}
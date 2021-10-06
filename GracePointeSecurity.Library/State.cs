using System;
using System.IO;
using Hangfire;
using Hangfire.MemoryStorage;
using SQLite;

namespace GracePointeSecurity.Library
{
	public sealed class State
	{
        private static readonly Lazy<SQLiteConnection> Connection =
			new Lazy<SQLiteConnection>(
			InitDatabase);

		private static readonly Lazy<CameraFilesLocationsConfiguration> Configuration =
			new Lazy<CameraFilesLocationsConfiguration>(
			InitState);

		public static CameraFilesLocationsConfiguration CurrentState => Configuration.Value;

		internal static SQLiteConnection DatabaseConnection => Connection.Value;

		private static SQLiteConnection InitDatabase()
		{
			var connection = new SQLiteConnection(
				Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					"db.db3"));
			connection.CreateTable<CameraFilesLocationsConfiguration>();
			return connection;
		}

		public static void InitHangfire() =>
			GlobalConfiguration.Configuration.UseMemoryStorage();

		public static void UpdateCurrentState()
		{
			CameraFilesLocationsConfiguration = CurrentState;
		}

		private static CameraFilesLocationsConfiguration InitState()
		{
			var config = CameraFilesLocationsConfiguration;
			config.PropertyChanged += (sender, args) =>
			{
				CameraFilesLocationsConfiguration = (CameraFilesLocationsConfiguration) sender;
			};
			return config;
		}

		private static CameraFilesLocationsConfiguration CameraFilesLocationsConfiguration
		{
			get => DatabaseConnection.Table<CameraFilesLocationsConfiguration>()
				       .FirstOrDefault()
			       ?? new CameraFilesLocationsConfiguration();
			set
			{
				if (DatabaseConnection.Table<CameraFilesLocationsConfiguration>().FirstOrDefault() == null)
				{
					DatabaseConnection.Insert(value);
				}
				else
				{
					var command = new SQLiteCommand(DatabaseConnection)
					{
						CommandText =
							$@"UPDATE
	{nameof(CameraFilesLocationsConfiguration)}
SET
	{nameof(CameraFilesLocationsConfiguration.OriginalVideoFolder)} = ""{value.OriginalVideoFolder}"",
	{nameof(CameraFilesLocationsConfiguration.SortedVideoFolder)} = ""{value.SortedVideoFolder}"",
	{nameof(CameraFilesLocationsConfiguration.LastRun)} = ""{value.LastRun}"",
	{nameof(CameraFilesLocationsConfiguration.ShouldMove)} = {(value.ShouldMove ? 1 : 0)}"
					};
					command.ExecuteNonQuery();
				}
			}
		}
    }
}
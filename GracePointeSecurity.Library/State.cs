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

		private static readonly Lazy<AwsCredentials> AwsCredentialsSettings =
			new Lazy<AwsCredentials>(
			InitAwsState);

		private static readonly Lazy<CameraFilesLocationsConfiguration> Configuration =
			new Lazy<CameraFilesLocationsConfiguration>(
			InitLocationState);

		public static AwsCredentials AwsCredentials => AwsCredentialsSettings.Value;

		public static CameraFilesLocationsConfiguration CurrentState => Configuration.Value;

		internal static SQLiteConnection DatabaseConnection => Connection.Value;

		private static SQLiteConnection InitDatabase()
		{
			var connection = new SQLiteConnection(
				Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					"db.db3"));
			connection.CreateTable<AwsCredentials>();
			connection.CreateTable<CameraFilesLocationsConfiguration>();
			return connection;
		}

		public static void InitHangfire() =>
			GlobalConfiguration.Configuration.UseMemoryStorage();

		public static void UpdateCurrentState()
		{
			CameraFilesLocationsConfiguration = CurrentState;
		}

		private static AwsCredentials InitAwsState()
		{
			var config = AwsCredentialsConfiguration;
			config.PropertyChanged += (sender, args) =>
			{
                AwsCredentialsConfiguration = (AwsCredentials) sender;
			};
			return config;
		}

		private static CameraFilesLocationsConfiguration InitLocationState()
		{
			var config = CameraFilesLocationsConfiguration;
			config.PropertyChanged += (sender, args) =>
			{
				CameraFilesLocationsConfiguration = (CameraFilesLocationsConfiguration) sender;
			};
			return config;
		}

		private static AwsCredentials AwsCredentialsConfiguration
        {
            get => DatabaseConnection.Table<AwsCredentials>()
                       .FirstOrDefault()
                   ?? new AwsCredentials();
			set
			{
				if (DatabaseConnection.Table<AwsCredentials>().FirstOrDefault() == null)
				{
					DatabaseConnection.Insert(value);
				}
				else
				{
					var command = new SQLiteCommand(DatabaseConnection)
					{
						CommandText =
							$@"UPDATE
	{nameof(AwsCredentials)}
SET
	{nameof(AwsCredentials.AccessKeyId)} = ""{value.AccessKeyId}"",
	{nameof(AwsCredentials.SecretAccessKey)} = ""{value.SecretAccessKey}"""
					};
					command.ExecuteNonQuery();
				}
			}
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
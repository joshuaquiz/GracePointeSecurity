﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Hangfire;
using Hangfire.Storage;

namespace GracePointeSecurity.Library
{
    public static class BackgroundJobs
    {
        private const string MoveFileJobId = "MoveFile";
        private const string MaintenanceJobId = "Maintenance";

        public static Logging Logging { get; set; }

        public static void Startup()
        {
            RecurringJob.AddOrUpdate(
                MaintenanceJobId,
                () => MaintenanceJobAsync(),
                Cron.Daily);
            RecurringJob.Trigger(MaintenanceJobId);
            ResetSettingsBiasedJobs();
        }

        public static void ResetSettingsBiasedJobs()
        {
            RecurringJob.AddOrUpdate(
                MoveFileJobId,
                () => MoveFiles(),
                Cron.Hourly(),
                TimeZoneInfo.Local);
            RecurringJob.Trigger(MoveFileJobId);
        }

        public static async Task MaintenanceJobAsync()
        {
            var statusText = new StringBuilder();
            foreach (var d in DriveInfo.GetDrives().Where(x => x.IsReady))
            {
                statusText.AppendLine($"{d.DriveType} Drive {d.Name} \"{d.VolumeLabel}\" - {d.DriveFormat}");
                var percentComplete = (int)Math.Round((double)(100 * (d.TotalSize - d.AvailableFreeSpace)) / d.TotalSize);
                var usedSpaceString = $"Used space: {BytesToString(d.TotalSize - d.AvailableFreeSpace)}";
                var remainingSpaceString = $"Remaining space: {BytesToString(d.AvailableFreeSpace)}";
                statusText.AppendLine(usedSpaceString + new string(' ', 185 - (usedSpaceString.Length + remainingSpaceString.Length)) + remainingSpaceString);
                statusText.AppendLine("|" + new string('=', percentComplete) + new string('_', 100-percentComplete) + "|");
                statusText.AppendLine($"Total size: {BytesToString(d.TotalSize)}");
                statusText.AppendLine();
            }

            await new AmazonSimpleNotificationServiceClient(
                    new BasicAWSCredentials("AKIAXUZ34F56PWGQRY5M", "nlfnbrBpNH2II9a1Eymu7LxDD1SdtnjFAArfq781"),
                    RegionEndpoint.USEast1)
                .PublishAsync(
                    new PublishRequest
                    {
                        TopicArn = "arn:aws:sns:us-east-1:525722201980:GP-Drive-Info",
                        Subject = "GP Cameras Drive Info",
                        Message = statusText.ToString()
                    });
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 3);
            return (Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture) + suf[place];
        }

        public static void MoveFiles()
        {
            var stopwatch = Stopwatch.StartNew();
            var originalFolder = State.CurrentState.OriginalVideoFolder;
            var fileCount = 0;
            foreach (var file in FileActions.GetCurrentFilesInFolder(originalFolder))
            {
                var newFilePath =
                    $"{State.CurrentState.SortedVideoFolder}\\{FileActions.GetFileCreatedDate(file):yyyy\\\\MM\\\\dd\\\\HH}";
                var sortedDir = new DirectoryInfo(newFilePath);
                if (!sortedDir.Exists)
                {
                    sortedDir.Create();
                }

                var oldPath = $"{originalFolder}{file.Replace(originalFolder, string.Empty)}";
                var newFileName = file.Replace(originalFolder, string.Empty);
                var parts = newFileName.Split(".");
                newFileName = $"{parts[0]}.{parts[1]}.{Guid.NewGuid()}.{parts[2]}";
                var newPath = $"{newFilePath}{newFileName}";
                if (FileActions.AttemptFileMove(
                    oldPath,
                    newPath))
                {
                    Logging.AddLog($"Moved {oldPath} to {newPath} at {DateTime.Now:yyyy-MM-ddTHH:mm:ss}");
                    fileCount++;
                }
            }

            stopwatch.Stop();
            Logging.AddLog($"Last run: Moved {fileCount} in {stopwatch.Elapsed:g}");
            State.CurrentState.LastRun = LastRan();
        }

        private static DateTime? LastRan()
        {
            using var connection = JobStorage.Current.GetConnection();
            var currentJob = connection.GetRecurringJobs().FirstOrDefault(p => p.Id == MoveFileJobId);
            if (currentJob == null)
            {
                return null;
            }

            var previousJob = connection.GetJobData(currentJob.LastJobId);
            return previousJob?.CreatedAt;
        }
    }
}
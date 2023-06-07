using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GracePointeSecurity.Library;

public static class FileActions
{
    public static IEnumerable<string> GetCurrentFilesInFolder(string? folderToSearch) =>
        folderToSearch != null && new DirectoryInfo(folderToSearch).Exists
            ? Directory.EnumerateFiles(folderToSearch)
            : Enumerable.Empty<string>();

    public static bool AttemptFileMove(
        string originalFilePath,
        string newFilePath)
    {
        var file = new FileInfo(originalFilePath);
        var destFile = new FileInfo(newFilePath);
        if (State.CurrentState.ShouldMove)
        {
            if (destFile.Exists)
            {
                destFile.Delete();
            }

            file.MoveTo(destFile.FullName);
        }
        else
        {
            if (destFile.Exists
                && file.LastWriteTime <= destFile.LastWriteTime
                && file.Length == destFile.Length)
            {
                return false;
            }

            file.CopyTo(destFile.FullName, true);
        }

        return true;
    }

    public static DateTime GetFileCreatedDate(
        string filePath) =>
        File.GetCreationTime(filePath);
}
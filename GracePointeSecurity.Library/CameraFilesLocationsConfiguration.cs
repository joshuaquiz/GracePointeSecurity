using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GracePointeSecurity.Library;

public sealed class CameraFilesLocationsConfiguration : INotifyPropertyChanged
{
    private bool _shouldMove;
    private string? _originalVideoFolder;
    private string? _sortedVideoFolder;
    private DateTime? _lastRun;

    /// <summary>
    /// Whether or not to move the files.
    /// </summary>
    public bool ShouldMove
    {
        get => _shouldMove;
        set
        {
            _shouldMove = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// The folder to pull videos from.
    /// </summary>
    public string? OriginalVideoFolder
    {
        get => _originalVideoFolder;
        set
        {
            _originalVideoFolder = value?.Trim('/');
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Where to move the videos to.
    /// </summary>
    public string? SortedVideoFolder
    {
        get => _sortedVideoFolder;
        set
        {
            _sortedVideoFolder = value?.Trim('/');
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// The last time it ran.
    /// </summary>
    public DateTime? LastRun
    {
        get => _lastRun;
        set
        {
            _lastRun = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
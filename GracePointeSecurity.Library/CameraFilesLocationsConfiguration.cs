using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GracePointeSecurity.Library
{
	public sealed class CameraFilesLocationsConfiguration : INotifyPropertyChanged
	{
        private bool _shouldMove;
		private string _originalVideoFolder;
		private string _sortedVideoFolder;
		private DateTime? _lastRun;

		public bool ShouldMove
		{
			get => _shouldMove;
			set
			{
				_shouldMove = value;
				OnPropertyChanged(nameof(ShouldMove));
			}
		}

		public string OriginalVideoFolder
		{
			get => _originalVideoFolder;
			set
			{
				_originalVideoFolder = value?.Trim('/');
				OnPropertyChanged(nameof(OriginalVideoFolder));
			}
		}

		public string SortedVideoFolder
		{
			get => _sortedVideoFolder;
			set
			{
				_sortedVideoFolder = value?.Trim('/');
				OnPropertyChanged(nameof(SortedVideoFolder));
			}
		}

		public DateTime? LastRun
		{
			get => _lastRun;
			set
			{
				_lastRun = value;
				OnPropertyChanged(nameof(LastRun));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
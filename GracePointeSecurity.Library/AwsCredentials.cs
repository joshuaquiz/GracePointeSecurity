using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GracePointeSecurity.Library;

public sealed class AwsCredentials : INotifyPropertyChanged
{
    private string _accessKeyId;
    private string _secretAccessKey;

    public string AccessKeyId
    {
        get => _accessKeyId;
        set
        {
            _accessKeyId = value;
            OnPropertyChanged();
        }
    }

    public string SecretAccessKey
    {
        get => _secretAccessKey;
        set
        {
            _secretAccessKey = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
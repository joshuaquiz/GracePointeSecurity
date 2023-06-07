using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GracePointeSecurity.Library;

public sealed class AwsCredentials : INotifyPropertyChanged
{
    private string? _accessKeyId;
    private string? _secretAccessKey;
    private string? _productionKey;
    private string? _originationName;

    public string? AccessKeyId
    {
        get => _accessKeyId;
        set
        {
            _accessKeyId = value;
            OnPropertyChanged();
        }
    }

    public string? SecretAccessKey
    {
        get => _secretAccessKey;
        set
        {
            _secretAccessKey = value;
            OnPropertyChanged();
        }
    }

    public string? ProductionKey
    {
        get => _productionKey;
        set
        {
            _productionKey = value;
            OnPropertyChanged();
        }
    }

    public string? OriginationName
    {
        get => _originationName;
        set
        {
            _originationName = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
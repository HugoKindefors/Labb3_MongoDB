using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Labb3.Models;

public class Category : INotifyPropertyChanged
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    private string _name = string.Empty;

    public Category()
    {
        Name = string.Empty;
    }

    public Category(string name)
    {
        Name = name;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }
}

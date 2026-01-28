using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Labb3.Models;

public class Question : INotifyPropertyChanged
{
    private string _query = string.Empty;
    private string _correctAnswer = string.Empty;
    private string[] _incorrectAnswer = new string[3];

    public Question()
    {
        Query = string.Empty;
        CorrectAnswer = string.Empty;
        IncorrectAnswer = new string[3];
    }

    public Question(string query, string correctAnwser, string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
    {
        Query = query;
        CorrectAnswer = correctAnwser;
        IncorrectAnswer = new string[3] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Query
    {
        get => _query;
        set
        {
            if (_query != value)
            {
                _query = value;
                OnPropertyChanged();
            }
        }
    }

    public string CorrectAnswer
    {
        get => _correctAnswer;
        set
        {
            if (_correctAnswer != value)
            {
                _correctAnswer = value;
                OnPropertyChanged();
            }
        }
    }

    public string[] IncorrectAnswer
    {
        get => _incorrectAnswer;
        set
        {
            if (_incorrectAnswer != value)
            {
                _incorrectAnswer = value;
                OnPropertyChanged();
            }
        }
    }
}

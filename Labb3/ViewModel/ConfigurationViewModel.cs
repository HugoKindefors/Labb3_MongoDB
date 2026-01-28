using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Labb3.Command;
using Labb3.Models;

namespace Labb3.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;

            if (_mainWindowViewModel is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(MainWindowViewModel.ActivePack))
                    {
                        RaisePropertyChanged(nameof(Questions));
                        SelectedQuestion = null;
                        RaiseCanExecutes();
                    }
                };
            }
            
            if (_mainWindowViewModel?.ActivePack?.Questions is System.Collections.Specialized.INotifyCollectionChanged questionsCollection)
            {
                questionsCollection.CollectionChanged += (_, _) => RaiseCanExecutes();
            }

            AddQuestionCommand = new DelegateCommand(_ => AddQuestion(), _ => ActivePackExists());
            RemoveQuestionCommand = new DelegateCommand(_ => RemoveSelected(), _ => ActivePackExists() && SelectedQuestion is not null);
        }

        public ObservableCollection<Question> Questions
            => _mainWindowViewModel?.ActivePack?.Questions ?? new ObservableCollection<Question>();

        private Question? _selectedQuestion;
        
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                if (SetProperty(ref _selectedQuestion, value))
                {
                    LoadDraftsFromSelection();
                    RaiseCanExecutes();
                }
            }
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand RemoveQuestionCommand { get; }

        private bool ActivePackExists() => _mainWindowViewModel?.ActivePack is not null;

        private void AddQuestion()
        {
            if (_mainWindowViewModel?.ActivePack is null) return;

            var newQ = new Question(
                "New question",
                "Correct",
                "Wrong 1",
                "Wrong 2",
                "Wrong 3"
            );

            _mainWindowViewModel.ActivePack.Questions.Add(newQ);
            SelectedQuestion = newQ;
        }

        private void RemoveSelected()
        {
            if (_mainWindowViewModel?.ActivePack is null || SelectedQuestion is null) return;
            _mainWindowViewModel.ActivePack.Questions.Remove(SelectedQuestion);
            SelectedQuestion = null;
        }

        private void RaiseCanExecutes()
        {
            (AddQuestionCommand as DelegateCommand)?.RaiseCanExecuteChange();
            (RemoveQuestionCommand as DelegateCommand)?.RaiseCanExecuteChange();
        }

        public void RefreshCommands() => RaiseCanExecutes();

        private string _draftQuery = string.Empty;
        
        public string DraftQuery
        {
            get => _draftQuery;
            set
            {
                if (SetProperty(ref _draftQuery, value))
                {
                    if (SelectedQuestion is not null)
                    {
                        SelectedQuestion.Query = value;
                    }
                }
            }
        }

        private string _draftCorrect = string.Empty;
        
        public string DraftCorrectAnswer
        {
            get => _draftCorrect;
            set
            {
                if (SetProperty(ref _draftCorrect, value))
                {
                    if (SelectedQuestion is not null)
                    {
                        SelectedQuestion.CorrectAnswer = value;
                    }
                }
            }
        }

        private string _draftWrong1 = string.Empty;
        
        public string DraftWrong1
        {
            get => _draftWrong1;
            set
            {
                if (SetProperty(ref _draftWrong1, value))
                {
                    if (SelectedQuestion is not null)
                    {
                        EnsureIncorrectArray();
                        SelectedQuestion.IncorrectAnswer[0] = value;
                        SelectedQuestion.OnPropertyChanged(nameof(SelectedQuestion.IncorrectAnswer));
                    }
                }
            }
        }

        private string _draftWrong2 = string.Empty;
        
        public string DraftWrong2
        {
            get => _draftWrong2;
            set
            {
                if (SetProperty(ref _draftWrong2, value))
                {
                    if (SelectedQuestion is not null)
                    {
                        EnsureIncorrectArray();
                        SelectedQuestion.IncorrectAnswer[1] = value;
                        SelectedQuestion.OnPropertyChanged(nameof(SelectedQuestion.IncorrectAnswer));
                    }
                }
            }
        }

        private string _draftWrong3 = string.Empty;
        
        public string DraftWrong3
        {
            get => _draftWrong3;
            set
            {
                if (SetProperty(ref _draftWrong3, value))
                {
                    if (SelectedQuestion is not null)
                    {
                        EnsureIncorrectArray();
                        SelectedQuestion.IncorrectAnswer[2] = value;
                        SelectedQuestion.OnPropertyChanged(nameof(SelectedQuestion.IncorrectAnswer));
                    }
                }
            }
        }

        private void EnsureIncorrectArray()
        {
            if (SelectedQuestion is null) return;
            
            if (SelectedQuestion.IncorrectAnswer == null || SelectedQuestion.IncorrectAnswer.Length < 3)
            {
                var incorrect = SelectedQuestion.IncorrectAnswer ?? Array.Empty<string>();
                SelectedQuestion.IncorrectAnswer = new[]
                {
                    incorrect.Length > 0 ? incorrect[0] : string.Empty,
                    incorrect.Length > 1 ? incorrect[1] : string.Empty,
                    incorrect.Length > 2 ? incorrect[2] : string.Empty
                };
            }
        }

        private void LoadDraftsFromSelection()
        {
            if (SelectedQuestion is null)
            {
                _draftQuery = string.Empty;
                _draftCorrect = string.Empty;
                _draftWrong1 = string.Empty;
                _draftWrong2 = string.Empty;
                _draftWrong3 = string.Empty;
                RaisePropertyChanged(nameof(DraftQuery));
                RaisePropertyChanged(nameof(DraftCorrectAnswer));
                RaisePropertyChanged(nameof(DraftWrong1));
                RaisePropertyChanged(nameof(DraftWrong2));
                RaisePropertyChanged(nameof(DraftWrong3));
                return;
            }

            _draftQuery = SelectedQuestion.Query ?? string.Empty;
            _draftCorrect = SelectedQuestion.CorrectAnswer ?? string.Empty;
            
            var incorrect = SelectedQuestion.IncorrectAnswer ?? Array.Empty<string>();
            _draftWrong1 = incorrect.Length > 0 ? incorrect[0] ?? string.Empty : string.Empty;
            _draftWrong2 = incorrect.Length > 1 ? incorrect[1] ?? string.Empty : string.Empty;
            _draftWrong3 = incorrect.Length > 2 ? incorrect[2] ?? string.Empty : string.Empty;
            
            RaisePropertyChanged(nameof(DraftQuery));
            RaisePropertyChanged(nameof(DraftCorrectAnswer));
            RaisePropertyChanged(nameof(DraftWrong1));
            RaisePropertyChanged(nameof(DraftWrong2));
            RaisePropertyChanged(nameof(DraftWrong3));
        }
    }
}

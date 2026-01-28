using Labb3.Command;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Labb3.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly Random _random = new();
        private readonly DispatcherTimer _countdownTimer;

        private List<ShuffledQuestion> _shuffledQuestions = new();
        private int _currentIndex = -1;
        private int _remainingSeconds;
        private bool _isAnswerRevealed;
        private bool _lastAnswerCorrect;
        private bool _isPlaying;
        private int _correctAnswers;
        private bool _isQuizFinished;
        private int _selectedAnswerIndex = -1;

        public QuestionPackViewModel ActivePack { get => _mainWindowViewModel?.ActivePack; }

        public string? CurrentQuery => (_currentIndex >= 0 && _currentIndex < _shuffledQuestions.Count) 
            ? _shuffledQuestions[_currentIndex].Question.Query 
            : null;

        public int CurrentQuestionNumber => _currentIndex + 1;
        
        public int TotalQuestions => _shuffledQuestions.Count;

        public int RemainingSeconds
        {
            get => _remainingSeconds;
            private set => SetProperty(ref _remainingSeconds, value);
        }

        public bool IsAnswerRevealed
        {
            get => _isAnswerRevealed;
            private set => SetProperty(ref _isAnswerRevealed, value);
        }

        public bool LastAnswerCorrect
        {
            get => _lastAnswerCorrect;
            private set => SetProperty(ref _lastAnswerCorrect, value);
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                if (SetProperty(ref _isPlaying, value))
                {
                    RaiseCanExecutes();
                }
            }
        }

        public bool IsQuizFinished
        {
            get => _isQuizFinished;
            private set => SetProperty(ref _isQuizFinished, value);
        }

        public int CorrectAnswers
        {
            get => _correctAnswers;
            private set => SetProperty(ref _correctAnswers, value);
        }

        public IReadOnlyList<string> CurrentAnswers
        {
            get
            {
                if (_currentIndex >= 0 && _currentIndex < _shuffledQuestions.Count)
                {
                    return _shuffledQuestions[_currentIndex].ShuffledAnswers;
                }
                return new List<string> { string.Empty, string.Empty, string.Empty, string.Empty };
            }
        }

        public int CurrentCorrectIndex
        {
            get
            {
                if (_currentIndex >= 0 && _currentIndex < _shuffledQuestions.Count)
                {
                    return _shuffledQuestions[_currentIndex].CorrectIndex;
                }
                return -1;
            }
        }

        public int SelectedAnswerIndex
        {
            get => _selectedAnswerIndex;
            private set => SetProperty(ref _selectedAnswerIndex, value);
        }

        public ICommand StartCommand { get; }
        public ICommand AnswerCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand RestartCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _countdownTimer.Tick += OnCountdownTick;

            StartCommand = new DelegateCommand(_ => Start(), CanStart);
            
            AnswerCommand = new DelegateCommand(
                p => Answer(Convert.ToInt32(p)), 
                _ => IsPlaying && !IsAnswerRevealed && _currentIndex >= 0
            );
            
            NextCommand = new DelegateCommand(_ => Next(), _ => IsAnswerRevealed && _currentIndex < TotalQuestions);
            
            RestartCommand = new DelegateCommand(_ => ResetQuiz());

            if (_mainWindowViewModel is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(MainWindowViewModel.ActivePack))
                    {
                        RaisePropertyChanged(nameof(ActivePack));
                        
                        if (ActivePack?.Questions is INotifyCollectionChanged oldCollection)
                        {
                            oldCollection.CollectionChanged -= OnQuestionsChanged;
                        }
                        
                        if (_mainWindowViewModel?.ActivePack?.Questions is INotifyCollectionChanged newCollection)
                        {
                            newCollection.CollectionChanged += OnQuestionsChanged;
                        }
                        
                        RaiseCanExecutes();
                    }
                };
            }
            
            if (ActivePack?.Questions is INotifyCollectionChanged questionsCollection)
            {
                questionsCollection.CollectionChanged += OnQuestionsChanged;
            }
        }

        private void OnQuestionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCanExecutes();
        }

        private bool CanStart(object? _)
        {
            return !IsPlaying && ActivePack?.Questions?.Count > 0;
        }

        private void Start()
        {
            var currentPack = _mainWindowViewModel?.ActivePack;
            if (currentPack is null || currentPack.Questions.Count == 0) return;

            var questions = currentPack.Questions.ToList();
            Shuffle(questions);

            _shuffledQuestions = questions.Select(q =>
            {
                var answers = new List<string> { q.CorrectAnswer };
                answers.AddRange(q.IncorrectAnswer ?? Array.Empty<string>());
                
                Shuffle(answers);

                return new ShuffledQuestion 
                { 
                    Question = q,
                    ShuffledAnswers = answers,
                    CorrectIndex = answers.IndexOf(q.CorrectAnswer)
                };
            }).ToList();

            _currentIndex = 0;
            _correctAnswers = 0;
            IsPlaying = true;
            IsQuizFinished = false;
            IsAnswerRevealed = false;
            SelectedAnswerIndex = -1;
            RemainingSeconds = Math.Max(1, currentPack.TimeLimitinSeconds);
            _countdownTimer.Start();

            RaiseAllProperties();
            RaiseCanExecutes();
        }

        private void RaiseAllProperties()
        {
            RaisePropertyChanged(nameof(CurrentQuery));
            RaisePropertyChanged(nameof(CurrentQuestionNumber));
            RaisePropertyChanged(nameof(TotalQuestions));
            RaisePropertyChanged(nameof(CurrentAnswers));
            RaisePropertyChanged(nameof(CurrentCorrectIndex));
            RaisePropertyChanged(nameof(CorrectAnswers));
            RaisePropertyChanged(nameof(SelectedAnswerIndex));
        }

        private void OnCountdownTick(object? sender, EventArgs e)
        {
            if (RemainingSeconds > 0)
            {
                RemainingSeconds--;
            }
            else
            {
                SelectedAnswerIndex = -1;
                RevealAnswer(false);
            }
        }

        private void Answer(int answerIndex)
        {
            if (_currentIndex < 0 || _currentIndex >= _shuffledQuestions.Count) return;
            
            if (answerIndex < 0 || answerIndex >= _shuffledQuestions[_currentIndex].ShuffledAnswers.Count) return;

            SelectedAnswerIndex = answerIndex;
            
            bool isCorrect = answerIndex == _shuffledQuestions[_currentIndex].CorrectIndex;
            
            RevealAnswer(isCorrect);
        }

        private void RevealAnswer(bool isCorrect)
        {
            _countdownTimer.Stop();
            LastAnswerCorrect = isCorrect;
            
            if (isCorrect)
            {
                CorrectAnswers++;
            }
            
            IsAnswerRevealed = true;
            
            RaisePropertyChanged(nameof(CurrentCorrectIndex));
            RaisePropertyChanged(nameof(SelectedAnswerIndex));
            
            RaiseCanExecutes();
        }

        private void Next()
        {
            if (_currentIndex >= TotalQuestions - 1)
            {
                IsPlaying = false;
                IsQuizFinished = true;
                _countdownTimer.Stop();
                
                RaisePropertyChanged(nameof(IsPlaying));
                RaisePropertyChanged(nameof(IsQuizFinished));
                RaiseAllProperties();
                RaiseCanExecutes();
                return;
            }

            _currentIndex++;
            
            IsAnswerRevealed = false;
            LastAnswerCorrect = false;
            SelectedAnswerIndex = -1;
            
            var currentPack = _mainWindowViewModel?.ActivePack;
            RemainingSeconds = Math.Max(1, currentPack?.TimeLimitinSeconds ?? 30);
            
            _countdownTimer.Start();

            RaiseAllProperties();
            RaiseCanExecutes();
        }

        private void ResetQuiz()
        {
            IsQuizFinished = false;
            IsPlaying = false;
            IsAnswerRevealed = false;
            LastAnswerCorrect = false;
            _currentIndex = -1;
            _correctAnswers = 0;
            SelectedAnswerIndex = -1;
            _shuffledQuestions.Clear();
            _countdownTimer.Stop();
            RemainingSeconds = 0;
            
            RaisePropertyChanged(nameof(IsQuizFinished));
            RaisePropertyChanged(nameof(IsPlaying));
            RaisePropertyChanged(nameof(IsAnswerRevealed));
            RaisePropertyChanged(nameof(LastAnswerCorrect));
            RaisePropertyChanged(nameof(RemainingSeconds));
            RaiseAllProperties();
            RaiseCanExecutes();
            
            if (StartCommand is DelegateCommand startCmd && startCmd.CanExecute(null))
            {
                startCmd.Execute(null);
            }
        }

        private void RaiseCanExecutes()
        {
            (StartCommand as DelegateCommand)?.RaiseCanExecuteChange();
            (AnswerCommand as DelegateCommand)?.RaiseCanExecuteChange();
            (NextCommand as DelegateCommand)?.RaiseCanExecuteChange();
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private class ShuffledQuestion
        {
            public Question Question { get; set; } = null!;
            public List<string> ShuffledAnswers { get; set; } = new();
            public int CorrectIndex { get; set; }
        }
    }
}

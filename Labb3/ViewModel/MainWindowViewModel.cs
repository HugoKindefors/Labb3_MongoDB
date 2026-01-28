using Labb3.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Labb3.Command;
using Labb3.Services;

namespace Labb3.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

        private QuestionPackViewModel _activePack;

        public QuestionPackViewModel ActivePack
        {
            get => _activePack;
            set
            {
                if (_activePack?.Questions is System.Collections.Specialized.INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= OnQuestionsCollectionChanged;
                }

                _activePack = value;
                
                RaisePropertyChanged();
                
                PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
                
                if (_activePack?.Questions is System.Collections.Specialized.INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += OnQuestionsCollectionChanged;
                }
                
                (StartPlayCommand as DelegateCommand)?.RaiseCanExecuteChange();
                (DeletePackCommand as DelegateCommand)?.RaiseCanExecuteChange();
                (SelectPackCommand as DelegateCommand)?.RaiseCanExecuteChange();
                (SavePackCommand as DelegateCommand)?.RaiseCanExecuteChange();
                
                ConfigurationViewModel?.RefreshCommands();
            }
        }

        private void OnQuestionsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            (StartPlayCommand as DelegateCommand)?.RaiseCanExecuteChange();
            ConfigurationViewModel?.RefreshCommands();
        }

        public PlayerViewModel? PlayerViewModel { get; }

        public ConfigurationViewModel? ConfigurationViewModel { get; }

        private bool _isPlayMode;
        
        public bool IsPlayMode
        {
            get => _isPlayMode;
            private set
            {
                if (SetProperty(ref _isPlayMode, value))
                {
                    RaisePropertyChanged(nameof(IsConfigurationMode));
                }
            }
        }

        public bool IsConfigurationMode => !_isPlayMode;

        private readonly JsonStorageService _storage = new();
        
        public ICommand NewPackCommand { get; }
        public ICommand SelectPackCommand { get; }
        public ICommand DeletePackCommand { get; }
        public ICommand StartPlayCommand { get; }
        public ICommand ToggleFullScreenCommand { get; }
        public ICommand SwitchToConfigurationCommand { get; }
        public ICommand OpenPackOptionsCommand { get; }
        public ICommand SavePackCommand { get; }

        public MainWindowViewModel()
        {
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);

            NewPackCommand = new DelegateCommand(_ => CreateNewPack());
            
            SelectPackCommand = new DelegateCommand(
                p => SelectPack(p as QuestionPackViewModel),
                _ => Packs.Count > 0
            );
            
            DeletePackCommand = new DelegateCommand(
                _ => DeletePack(), 
                _ => ActivePack is not null
            );
            
            StartPlayCommand = new DelegateCommand(
                _ => StartPlay(), 
                CanStartPlay
            );
            
            ToggleFullScreenCommand = new DelegateCommand(_ => ToggleFullScreen());
            
            SwitchToConfigurationCommand = new DelegateCommand(_ => 
            {
                IsPlayMode = false;
            });
            
            OpenPackOptionsCommand = new DelegateCommand(_ => OpenPackOptions());
            
            SavePackCommand = new DelegateCommand(
                async _ => await SavePackToFileAsync(), 
                _ => ActivePack is not null
            );

            _ = LoadPacksOnStartAsync();
        }

        private async Task LoadPacksOnStartAsync()
        {
            await OpenAsync();
        }

        private bool CanStartPlay(object? _) => ActivePack?.Questions?.Count > 0;

        private void CreateNewPack()
        {
            var newPack = new QuestionPack("New Pack");
            ActivePack = new QuestionPackViewModel(newPack);
            Packs.Add(ActivePack);
            (SelectPackCommand as DelegateCommand)?.RaiseCanExecuteChange();
        }

        private void SelectPack(QuestionPackViewModel? pack)
        {
            if (pack is not null)
                ActivePack = pack;
        }

        private void DeletePack()
        {
            if (ActivePack is null) return;
            
            Packs.Remove(ActivePack);
            
            if (Packs.Count > 0)
            {
                ActivePack = Packs[0];
            }
            else
            {
                var defaultPack = new QuestionPack("New Pack");
                ActivePack = new QuestionPackViewModel(defaultPack);
                Packs.Add(ActivePack);
            }
            
            (SelectPackCommand as DelegateCommand)?.RaiseCanExecuteChange();
        }

        private async Task OpenAsync()
        {
            var packs = await _storage.LoadAsync();
            
            Packs.Clear();
            
            foreach (var pack in packs)
            {
                Packs.Add(new QuestionPackViewModel(pack));
            }
            
            if (Packs.Count > 0)
            {
                ActivePack = Packs[0];
            }
            else
            {
                var defaultPack = new QuestionPack("New Pack");
                ActivePack = new QuestionPackViewModel(defaultPack);
                Packs.Add(ActivePack);
            }
            
            (SelectPackCommand as DelegateCommand)?.RaiseCanExecuteChange();
        }
        
        private void StartPlay()
        {
            if (ActivePack is null || ActivePack.Questions.Count == 0)
                return;

            IsPlayMode = true;
            
            PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
            
            if (PlayerViewModel?.StartCommand is DelegateCommand startCmd)
            {
                startCmd.RaiseCanExecuteChange();
                
                if (startCmd.CanExecute(null))
                    startCmd.Execute(null);
            }
        }

        private void OpenPackOptions()
        {
            if (ActivePack is null)
            {
                var defaultPack = new QuestionPack("New Pack");
                ActivePack = new QuestionPackViewModel(defaultPack);
                Packs.Add(ActivePack);
            }

            var dialog = new Views.PackOptionsDialog(ActivePack)
            {
                Owner = Application.Current?.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                RaisePropertyChanged(nameof(ActivePack));
            }
        }

        private void ToggleFullScreen()
        {
            var window = Application.Current?.MainWindow;
            if (window is null) return;
            
            if (window.WindowStyle != WindowStyle.None)
            {
                window.WindowStyle = WindowStyle.None;
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.WindowState = WindowState.Normal;
            }
        }

        private async Task SavePackToFileAsync()
        {
            var packsToSave = Packs.Select(p => new QuestionPack(
                p.Name,
                p.Difficulty,
                p.TimeLimitinSeconds)
            {
                Questions = p.Questions.ToList()
            });

            await _storage.SaveAsync(packsToSave);
        }
    }
}

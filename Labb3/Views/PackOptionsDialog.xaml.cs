using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Labb3.Models;
using Labb3.Services;
using Labb3.ViewModel;

namespace Labb3.Views
{
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog(QuestionPackViewModel pack)
        {
            InitializeComponent();
            DataContext = new PackOptionsViewModel(pack);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PackOptionsViewModel vm)
            {
                vm.ApplyChanges();
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

    public class PackOptionsViewModel : Labb3.ViewModel.ViewModelBase
    {
        private readonly QuestionPackViewModel _originalPack;
        private readonly CategoryService _categoryService;
        private string _packName;
        private Difficulty _selectedDifficulty;
        private int _timeLimitSeconds;
        private Category? _selectedCategory;

        public List<Category> Categories { get; }

        public PackOptionsViewModel(QuestionPackViewModel pack)
        {
            _originalPack = pack;
            _packName = pack.Name;
            _selectedDifficulty = pack.Difficulty;
            _timeLimitSeconds = pack.TimeLimitinSeconds;

            _categoryService = new CategoryService(App.Mongo.Database);
            Categories = _categoryService.LoadAllAsync().GetAwaiter().GetResult().ToList();

            if (!string.IsNullOrWhiteSpace(pack.CategoryId))
            {
                _selectedCategory = Categories.FirstOrDefault(c => c.Id == pack.CategoryId);
            }
        }

        public string PackName
        {
            get => _packName;
            set => SetProperty(ref _packName, value);
        }

        public Difficulty SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => SetProperty(ref _selectedDifficulty, value);
        }

        public int TimeLimitSeconds
        {
            get => _timeLimitSeconds;
            set => SetProperty(ref _timeLimitSeconds, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public Difficulty[] Difficulties => new[] { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard };

        public QuestionPackViewModel ApplyChanges()
        {
            _originalPack.Name = PackName;
            _originalPack.Difficulty = SelectedDifficulty;
            _originalPack.TimeLimitinSeconds = TimeLimitSeconds;
            _originalPack.CategoryId = SelectedCategory?.Id;
            _originalPack.CategoryName = SelectedCategory?.Name;
            return _originalPack;
        }
    }
}


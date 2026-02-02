using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Labb3.Command;
using Labb3.Models;
using Labb3.Services;

namespace Labb3.ViewModel
{
    internal class CategoryViewModel : ViewModelBase
    {
        private readonly CategoryService _categoryService;

        public ObservableCollection<Category> Categories { get; } = new();

        private Category? _selectedCategory;

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    LoadSelectedCategoryName();
                    RaiseCanExecutes();
                }
            }
        }

        private string _newCategoryName = string.Empty;

        public string NewCategoryName
        {
            get => _newCategoryName;
            set
            {
                if (SetProperty(ref _newCategoryName, value))
                {
                    RaiseCanExecutes();
                }
            }
        }

        private string _editCategoryName = string.Empty;

        public string EditCategoryName
        {
            get => _editCategoryName;
            set
            {
                if (SetProperty(ref _editCategoryName, value))
                {
                    RaiseCanExecutes();
                }
            }
        }

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand UpdateCategoryCommand { get; }
        public ICommand RefreshCategoriesCommand { get; }

        public CategoryViewModel()
        {
            _categoryService = new CategoryService(App.Mongo.Database);

            AddCategoryCommand = new DelegateCommand(
                async _ => await AddCategoryAsync(),
                _ => !string.IsNullOrWhiteSpace(NewCategoryName)
            );

            DeleteCategoryCommand = new DelegateCommand(
                async _ => await DeleteCategoryAsync(),
                _ => SelectedCategory is not null
            );

            UpdateCategoryCommand = new DelegateCommand(
                async _ => await UpdateCategoryAsync(),
                _ => SelectedCategory is not null && !string.IsNullOrWhiteSpace(EditCategoryName)
            );

            RefreshCategoriesCommand = new DelegateCommand(
                async _ => await LoadCategoriesAsync()
            );

            _ = LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.LoadAllAsync();

            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            if (Categories.Any())
            {
                SelectedCategory = Categories[0];
            }

            RaiseCanExecutes();
        }

        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
                return;

            var exists = await _categoryService.ExistsAsync(NewCategoryName.Trim());
            if (exists)
            {
                MessageBox.Show(
                    $"En kategori med namnet '{NewCategoryName}' finns redan.",
                    "Kategori finns redan",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var category = new Category(NewCategoryName.Trim());
            await _categoryService.CreateAsync(category);

            Categories.Add(category);
            NewCategoryName = string.Empty;
            SelectedCategory = category;

            RaiseCanExecutes();
        }

        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory is null || string.IsNullOrWhiteSpace(SelectedCategory.Id))
                return;

            var result = MessageBox.Show(
                $"Är du säker på att du vill ta bort kategorin '{SelectedCategory.Name}'?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes)
                return;

            await _categoryService.DeleteAsync(SelectedCategory.Id);

            Categories.Remove(SelectedCategory);
            SelectedCategory = Categories.FirstOrDefault();

            RaiseCanExecutes();
        }

        private async Task UpdateCategoryAsync()
        {
            if (SelectedCategory is null || string.IsNullOrWhiteSpace(EditCategoryName))
                return;

            SelectedCategory.Name = EditCategoryName.Trim();
            await _categoryService.UpdateAsync(SelectedCategory);

            RaisePropertyChanged(nameof(Categories));
            RaiseCanExecutes();
        }

        private void LoadSelectedCategoryName()
        {
            _editCategoryName = SelectedCategory?.Name ?? string.Empty;
            RaisePropertyChanged(nameof(EditCategoryName));
        }

        private void RaiseCanExecutes()
        {
            (AddCategoryCommand as DelegateCommand)?.RaiseCanExecuteChange();
            (DeleteCategoryCommand as DelegateCommand)?.RaiseCanExecuteChange();
            (UpdateCategoryCommand as DelegateCommand)?.RaiseCanExecuteChange();
        }
    }
}

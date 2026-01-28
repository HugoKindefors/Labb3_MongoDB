using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Labb3.Views
{
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void QuestionsList_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListView listView) return;
            var dep = e.OriginalSource as DependencyObject;
            if (ItemsControl.ContainerFromElement(listView, dep) is null)
            {
                listView.SelectedItem = null;
            }
        }

        private void QuestionsListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && 
                (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            {
                if (DataContext is ViewModel.MainWindowViewModel mainVM && 
                    mainVM.ConfigurationViewModel?.RemoveQuestionCommand?.CanExecute(null) == true)
                {
                    mainVM.ConfigurationViewModel.RemoveQuestionCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }
}

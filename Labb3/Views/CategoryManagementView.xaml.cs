using System.Windows.Controls;
using Labb3.ViewModel;

namespace Labb3.Views
{
    public partial class CategoryManagementView : UserControl
    {
        public CategoryManagementView()
        {
            InitializeComponent();
            DataContext = new CategoryViewModel();
        }
    }
}

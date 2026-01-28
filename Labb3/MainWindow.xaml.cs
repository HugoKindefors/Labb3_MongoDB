using Labb3.ViewModel;
using System.Windows;

namespace Labb3;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
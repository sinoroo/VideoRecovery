using System.Windows;
using VideoRecovery.App.ViewModels;

namespace VideoRecovery.App.Views;

/// <summary>메인 윈도우. DataContext는 DI로 주입된 <see cref="MainViewModel"/>이다.</summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

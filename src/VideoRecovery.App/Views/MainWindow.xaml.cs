using System.Windows;
using VideoRecovery.App.ViewModels;

namespace VideoRecovery.App.Views;

/// <summary>
/// 셸 윈도우. code-behind 에는 InitializeComponent 와 순수 뷰 배선만 둔다.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

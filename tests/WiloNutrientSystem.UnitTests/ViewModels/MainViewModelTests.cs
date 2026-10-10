using CommunityToolkit.Mvvm.ComponentModel;
using FluentAssertions;
using Moq;
using WiloNutrientSystem.App.Services;
using WiloNutrientSystem.App.ViewModels;

namespace WiloNutrientSystem.UnitTests.ViewModels;

public class MainViewModelTests
{
    private sealed class FakePage : ObservableObject;

    private readonly Mock<INavigationService> _navigation = new();
    private readonly MainViewModel _sut;

    public MainViewModelTests() => _sut = new MainViewModel(_navigation.Object);

    [Fact]
    public void ShowDashboardCommand_NavigatesToDashboard()
    {
        _sut.ShowDashboardCommand.Execute(null);
        _navigation.Verify(n => n.NavigateTo<DashboardViewModel>(), Times.Once);
    }

    [Fact]
    public void ShowCommunicationCommand_NavigatesToCommunication()
    {
        _sut.ShowCommunicationCommand.Execute(null);
        _navigation.Verify(n => n.NavigateTo<CommunicationViewModel>(), Times.Once);
    }

    [Fact]
    public void ShowSettingsCommand_NavigatesToSettings()
    {
        _sut.ShowSettingsCommand.Execute(null);
        _navigation.Verify(n => n.NavigateTo<SettingsViewModel>(), Times.Once);
    }

    [Fact]
    public void CurrentChanged_UpdatesCurrentViewModel()
    {
        var page = new FakePage();

        _navigation.Raise(n => n.CurrentChanged += null, _navigation.Object, page);

        _sut.CurrentViewModel.Should().BeSameAs(page);
    }

    [Fact]
    public void Initialize_NavigatesToDashboard()
    {
        _sut.Initialize();
        _navigation.Verify(n => n.NavigateTo<DashboardViewModel>(), Times.Once);
    }

    [Fact]
    public void Title_HasDefaultValue()
        => _sut.Title.Should().Be("Wilo Nutrient System");
}

namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class AppShellViewModel
{
    public IAsyncRelayCommand ExitCommand => new AsyncRelayCommand(OnExitAsync);

    public IAsyncRelayCommand AddNewTeamCommand => new AsyncRelayCommand(OnAddNewTeamAsync);

    public IAsyncRelayCommand AddNewJudgeCommand => new AsyncRelayCommand(OnAddNewJudgeAsync);

    private async Task OnExitAsync() => Application.Current.Quit();

    private async Task OnAddNewTeamAsync()
    {
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditTeamView.Name);
    }

    private async Task OnAddNewJudgeAsync()    {
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditJudgeView.Name);
    }
}

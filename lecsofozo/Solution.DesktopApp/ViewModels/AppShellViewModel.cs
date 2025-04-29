namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class AppShellViewModel
{
    public IAsyncRelayCommand ExitCommand => new AsyncRelayCommand(OnExitAsync);

    public IAsyncRelayCommand AddNewTeamCommand => new AsyncRelayCommand(OnAddNewTeamAsync);

    public IAsyncRelayCommand ListTeamCommand => new AsyncRelayCommand(OnListTeamAsync);

    public IAsyncRelayCommand AddNewJudgeCommand => new AsyncRelayCommand(OnAddNewJudgeAsync);

    public IAsyncRelayCommand ListJudgeCommand => new AsyncRelayCommand(OnListJudgeAsync);

    public IAsyncRelayCommand AddNewRaceCommand => new AsyncRelayCommand(OnAddNewRaceAsync);

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

    private async Task OnListJudgeAsync()
    {
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(JudgeListView.Name);
    }

    private async Task OnListTeamAsync()
    {
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(TeamListView.Name);
    }

    private async Task OnAddNewRaceAsync()
    {
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditRaceView.Name);
    }
}

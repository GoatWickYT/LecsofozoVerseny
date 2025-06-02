
namespace Solution.DesktopApp;

public partial class AppShell : Shell
{
    public AppShellViewModel ViewModel => this.BindingContext as AppShellViewModel;

    public AppShell(AppShellViewModel viewModel)
    {
        this.BindingContext = viewModel;

        InitializeComponent();

        ConfigureShellNavigation();
    }

    private static void ConfigureShellNavigation()
    {
        Routing.RegisterRoute(MainView.Name, typeof(MainView));
        Routing.RegisterRoute(CreateOrEditTeamView.Name, typeof(CreateOrEditTeamView));
        Routing.RegisterRoute(TeamListView.Name, typeof(TeamListView));
        Routing.RegisterRoute(CreateOrEditJudgeView.Name, typeof(CreateOrEditJudgeView));
        Routing.RegisterRoute(JudgeListView.Name, typeof(JudgeListView));
        Routing.RegisterRoute(CreateOrEditRaceView.Name, typeof(CreateOrEditRaceView));
        Routing.RegisterRoute(RaceListView.Name, typeof(RaceListView));
    }
}

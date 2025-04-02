using Solution.Core.Models;

namespace Solution.DesktopApp.Views;

public partial class CreateOrEditTeamView : ContentPage
{
    public CreateOrEditTeamViewModel ViewModel => this.BindingContext as CreateOrEditTeamViewModel;

    public static string Name => nameof(CreateOrEditTeamView);

    public CreateOrEditTeamView(CreateOrEditTeamViewModel viewModel)
    {
        this.BindingContext = viewModel;

        InitializeComponent();
    }
}
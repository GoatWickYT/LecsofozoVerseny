namespace Solution.DesktopApp.Views;

public partial class RaceListView : ContentPage
{
    public RaceListViewModel ViewModel => this.BindingContext as RaceListViewModel;

    public static string Name => nameof(RaceListView);

    public RaceListView(RaceListViewModel viewModel)
    {
        this.BindingContext = viewModel;

        InitializeComponent();
    }
}
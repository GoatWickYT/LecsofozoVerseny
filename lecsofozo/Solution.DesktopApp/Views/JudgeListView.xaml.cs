namespace Solution.DesktopApp.Views;

public partial class JudgeListView : ContentPage
{
    public JudgeListViewModel ViewModel => this.BindingContext as JudgeListViewModel;

    public static string Name => nameof(JudgeListView);

    public JudgeListView(JudgeListViewModel viewModel)
    {
        this.BindingContext = viewModel;

        InitializeComponent();
    }
}
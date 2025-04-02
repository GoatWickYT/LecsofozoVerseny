namespace Solution.DesktopApp.Views;

public partial class CreateOrEditJudgeView : ContentPage
{
    public CreateOrEditJudgeViewModel ViewModel => this.BindingContext as CreateOrEditJudgeViewModel;

    public static string Name => nameof(CreateOrEditJudgeViewModel);

    public CreateOrEditJudgeView(CreateOrEditJudgeViewModel viewModel)
	{
		this.BindingContext = viewModel;

        InitializeComponent();
	}
}
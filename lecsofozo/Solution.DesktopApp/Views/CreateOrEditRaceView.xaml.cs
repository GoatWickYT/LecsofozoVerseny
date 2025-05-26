namespace Solution.DesktopApp.Views;

public partial class CreateOrEditRaceView : ContentPage
{
    public CreateOrEditRaceViewModel ViewModel => this.BindingContext as CreateOrEditRaceViewModel;

    public static string Name => nameof(CreateOrEditRaceViewModel);

    public CreateOrEditRaceView(CreateOrEditRaceViewModel viewModel)
	{
        this.BindingContext = viewModel;
        this.SizeChanged += OnSizeChanged;

        InitializeComponent();
	}

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        ContentPage page = sender as ContentPage;
        ViewModel.DatePickerWidth = page.Window.Width - 1000;
    }
}
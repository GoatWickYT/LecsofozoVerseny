namespace Solution.DesktopApp.Components;

public partial class JudgeListComponent : ContentView
{
    public static readonly BindableProperty JudgeProperty = BindableProperty.Create(
        propertyName: nameof(Judge),
        returnType: typeof(JudgeModel),
        declaringType: typeof(JudgeListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay
        );

    public JudgeModel Judge
    {
        get => (JudgeModel)GetValue(JudgeProperty);
        set => SetValue(JudgeProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        propertyName: nameof(CommandParameter),
        returnType: typeof(string),
        declaringType: typeof(JudgeListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay
        );

    public string CommandParameter
    {
        get => (string)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty DeleteCommandProperty = BindableProperty.Create(
        propertyName: nameof(DeleteCommand),
        returnType: typeof(IAsyncRelayCommand),
        declaringType: typeof(JudgeListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay
        );

    public IAsyncRelayCommand DeleteCommand
    {
        get => (IAsyncRelayCommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }



    public IAsyncRelayCommand EditCommand => new AsyncRelayCommand(OnEditAsync);

    public JudgeListComponent()
    {
        InitializeComponent();
    }

    private async Task OnEditAsync()
    {
        ShellNavigationQueryParameters navigationQueryParameter = new ShellNavigationQueryParameters
        {
            {"Judge", this.Judge }
        };
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditJudgeView.Name, navigationQueryParameter);
    }
}
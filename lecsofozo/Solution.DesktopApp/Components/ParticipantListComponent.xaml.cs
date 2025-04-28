namespace Solution.DesktopApp.Components;

public partial class ParticipantListComponent : ContentView
{
    public static readonly BindableProperty ParticipantProperty = BindableProperty.Create(
        propertyName: nameof(Participant),
        returnType: typeof(ParticipantModel),
        declaringType: typeof(ParticipantListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay
        );

    public ParticipantModel Participant
    {
        get => (ParticipantModel)GetValue(ParticipantProperty);
        set => SetValue(ParticipantProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        propertyName: nameof(CommandParameter),
        returnType: typeof(string),
        declaringType: typeof(ParticipantListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay
        );

    public string CommandParameter
    {
        get => (string)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public IAsyncRelayCommand EditCommand => new AsyncRelayCommand(OnEditAsync);

    public ParticipantListComponent()
    {
        InitializeComponent();
    }

    private async Task OnEditAsync()
    {
        ShellNavigationQueryParameters navigationQueryParameter = new ShellNavigationQueryParameters
        {
            {"Participant", this.Participant }
        };
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditTeamView.Name, navigationQueryParameter);
    }
}
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

    public static readonly BindableProperty ParticipantValidationCommandProperty = BindableProperty.Create(
        propertyName: nameof(ParticipantValidationCommand),
        returnType: typeof(IAsyncRelayCommand),
        declaringType: typeof(ParticipantListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay
        );

    public IAsyncRelayCommand ParticipantValidationCommand
    {
        get => (IAsyncRelayCommand)GetValue(ParticipantValidationCommandProperty);
        set => SetValue(ParticipantValidationCommandProperty, value);
    }

    public static readonly BindableProperty ImageSelectCommandProperty = BindableProperty.Create(
        propertyName: nameof(ImageSelectCommand),
        returnType: typeof(IAsyncRelayCommand),
        declaringType: typeof(ParticipantListComponent),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay
        );

    public IAsyncRelayCommand ImageSelectCommand
    {
        get => (IAsyncRelayCommand)GetValue(ImageSelectCommandProperty);
        set => SetValue(ImageSelectCommandProperty, value);
    }

    public ParticipantListComponent()
	{
		InitializeComponent();
	}
}
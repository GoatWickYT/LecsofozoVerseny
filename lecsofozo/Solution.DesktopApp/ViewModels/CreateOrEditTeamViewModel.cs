namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class CreateOrEditTeamViewModel(AppDbContext appDbContext,
                                               IGoogleDriveService googleDriveService,
                                               ITeamService teamService, IParticipantService participantService) : TeamModel(), IQueryAttributable
{
    #region life cycle commands
    public IAsyncRelayCommand AppearingCommand => new AsyncRelayCommand(OnAppearingAsync);
    public IAsyncRelayCommand DisappearingCommand => new AsyncRelayCommand(OnDisappearingAsync);
    #endregion

    #region validation commands
    public IRelayCommand TeamNameValidationCommand => new RelayCommand(() => this.Name.Validate());
    public IRelayCommand ParticipantNameValidationCommand => new RelayCommand(() => this.Participants.ToList().ForEach(x => x.Name.Validate()));
    #endregion

    #region event commands
    public IAsyncRelayCommand SubmitCommand => new AsyncRelayCommand(OnSubmitAsync);

    public IAsyncRelayCommand ImageSelectCommand => new AsyncRelayCommand(OnImageSelectAsync);
    #endregion


    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private ImageSource image;

    private FileResult selectedFile = null;

    [ObservableProperty]
    private ICollection<ParticipantModel> participants = new List<ParticipantModel>();

    private delegate Task ButtonActionDelagate();
    private ButtonActionDelagate asyncButtonAction;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await LoadParticipants();

        bool hasValue = query.TryGetValue("Team", out object result);

        if (!hasValue)
        {
            asyncButtonAction = OnSaveAsync;
            Title = "Create team";
            return;
        }

        TeamModel team = result as TeamModel;
        ParticipantModel participant = result as ParticipantModel;

        this.Id = team.Id;
        this.Name.Value = team.Name.Value;
        this.Participants = team.Participants;

        foreach (var member in Participants)
        {
            member.Id = participant.Id;
            member.WebContentLink = participant.WebContentLink;
            member.ImageId = participant.ImageId;
            member.Name.Value = participant.Name.Value;

            if (!string.IsNullOrEmpty(member.WebContentLink))
            {
                Image = new UriImageSource
                {
                    Uri = new Uri(member.WebContentLink),
                    CacheValidity = new TimeSpan(10, 0, 0, 0)
                };
            }
        }


        asyncButtonAction = OnUpdateAsync;
        Title = "Update team";
    }

    private async Task OnAppearingAsync() { }
    private async Task OnDisappearingAsync() { }

    private async Task OnSubmitAsync() => await asyncButtonAction();

    private async Task OnSaveAsync()
    {
        if (!IsFormValid())
        {
            return;
        }

        await UploadImageAsync();

        var result = await teamService.CreateAsync(this);
        var message = result.IsError ? result.FirstError.Description : "Team saved.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            ClearForm();
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private async Task OnUpdateAsync()
    {
        if (!IsFormValid())
        {
            return;
        }

        await UploadImageAsync();

        var result = await teamService.UpdateAsync(this);

        var message = result.IsError ? result.FirstError.Description : "Team updated.";
        var title = result.IsError ? "Error" : "Information";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private async Task OnImageSelectAsync()
    {
        selectedFile = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Please select the member image"
        });

        if (selectedFile is null)
        {
            return;
        }

        var stream = await selectedFile.OpenReadAsync();
        Image = ImageSource.FromStream(() => stream);
    }

    private async Task UploadImageAsync()
    {
        if (selectedFile is null)
        {
            return;
        }

        var imageUploadResult = await googleDriveService.UploadFileAsync(selectedFile);

        var message = imageUploadResult.IsError ? imageUploadResult.FirstError.Description : "Member image uploaded.";
        var title = imageUploadResult.IsError ? "Error" : "Information";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");

        this.Participants.ToList().ForEach(x => x.ImageId = imageUploadResult.IsError ? null : imageUploadResult.Value.Id);
        this.Participants.ToList().ForEach(x => x.WebContentLink = imageUploadResult.IsError ? null : imageUploadResult.Value.WebContentLink);
    }

    private async Task LoadParticipants()
    {
        Participants = await appDbContext.Participants.AsNoTracking().OrderBy(x => x.Name).Select(x => new ParticipantModel(x)).ToListAsync();
    }



    private bool IsFormValid()
    {
        this.Name.Validate();
        this.Participants.ToList().ForEach(x => x.Name.Validate());

        return this.Name.IsValid && this.Participants.All(x => x.Name.IsValid);
    }

    private void ClearForm()
    {
        this.Name.Value = null;
        this.Participants.ToList().ForEach(x => x.Name.Value = null);
    }
}
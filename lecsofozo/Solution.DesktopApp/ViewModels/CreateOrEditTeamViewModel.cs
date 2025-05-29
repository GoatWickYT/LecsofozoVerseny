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
    public IRelayCommand TeamNameValidationCommand => new RelayCommand(() => Team.Name.Validate());

    public IRelayCommand ParticipantValidationCommand => new RelayCommand<string>((publicId) => Participants.FirstOrDefault(x => x.PublicId == publicId).Name.Validate());
    #endregion

    #region event commands
    public IAsyncRelayCommand SubmitCommand => new AsyncRelayCommand(OnSubmitAsync);

    public IAsyncRelayCommand ImageSelectCommand => new AsyncRelayCommand<string>((publicId) => OnImageSelectAsync(Participants.FirstOrDefault(x => x.PublicId == publicId)));

    public IRelayCommand MemberAddingCommand => new RelayCommand(MemberAdd);

    public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<string>((publicId) => OnDeleteParticipantAsync(publicId));
    #endregion


    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private TeamModel team = new TeamModel();

    [ObservableProperty]
    private ObservableCollection<ParticipantModel> participants = new ObservableCollection<ParticipantModel>();

    private delegate Task ButtonActionDelagate();
    private ButtonActionDelagate asyncButtonAction;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {

        bool hasValue = query.TryGetValue("Team", out object result);

        if (!hasValue)
        {
            asyncButtonAction = OnSaveAsync;
            Title = "Create team";
            return;
        }

        TeamModel team = result as TeamModel;

        Team.Id = team.Id;
        Team.Name.Value = team.Name.Value;
        Team.PublicId = team.PublicId;
        var participants = await appDbContext.Participants.Where(x => x.TeamId == team.Id).ToListAsync();

        foreach (var participant in participants)
        {
            Participants.Add(new ParticipantModel
            {
                Id = participant.Id,
                PublicId = participant.PublicId,
                Name = new ValidatableObject<string> { Value = participant.Name },
                ImageId = participant.ImageId,
                WebContentLink = participant.WebContentLink,
                Image = !string.IsNullOrEmpty(participant.WebContentLink) ? 
                         new UriImageSource
                         {
                             Uri = new Uri(participant.WebContentLink),
                             CacheValidity = new TimeSpan(10, 0, 0, 0)
                         } :
                         null

            });
        };


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

        var result = await teamService.CreateAsync(Team);
        var message = result.IsError ? result.FirstError.Description : "Team saved.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            foreach(var participant in Participants)
            {
                participant.Team = result.Value;
                await UploadImageAsync(participant);
                await participantService.CreateAsync(participant);
            }

            ClearForm();
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private void MemberAdd()
    {
        if (Participants.Count < 10)
        {
            string publicId = Guid.NewGuid().ToString();

            ParticipantModel participant = new ParticipantModel{
                PublicId = publicId
            };

            Participants.Add(participant);
        }
    }

    private async Task OnDeleteParticipantAsync(string publicId)
    {
        ParticipantModel participant = Participants.FirstOrDefault(x => x.PublicId == publicId);

        if(participant.Id == 0)
        {
            Participants.Remove(participant);
        }
        else
        {
            var result = await participantService.DeleteAsync(publicId);
            var message = result.IsError ? result.FirstError.Description : "Deleted successfully!";
            var title = result.IsError ? "Error" : "Information";

            if (!result.IsError)
            {
                Participants.Remove(participant);
            }

            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }

    private async Task OnUpdateAsync()
    {
        if (!IsFormValid())
        {
            return;
        }

        var result = await teamService.UpdateAsync(Team);

        var message = result.IsError ? result.FirstError.Description : "Team updated.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            foreach(var participant in Participants)
            {
                participant.Team = Team;
                await UploadImageAsync(participant);
                await participantService.UpdateOrSaveAsync(participant);
            }
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private async Task OnImageSelectAsync(ParticipantModel participant)
    {
        participant.SelectedFile = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Please select the participant image"
        });

        if (participant.SelectedFile is null)
        {
            return;
        }

        var stream = await participant.SelectedFile.OpenReadAsync();
        participant.Image = ImageSource.FromStream(() => stream);
    }

    private async Task UploadImageAsync(ParticipantModel participant)
    {
        if (participant.SelectedFile is null)
        {
            return;
        }

        var imageUploadResult = await googleDriveService.UploadFileAsync(participant.SelectedFile);

        var message = imageUploadResult.IsError ? imageUploadResult.FirstError.Description : "Participant image uploaded.";
        var title = imageUploadResult.IsError ? "Error" : "Information";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");

        participant.ImageId = imageUploadResult.IsError ? null : imageUploadResult.Value.Id;
        participant.WebContentLink = imageUploadResult.IsError ? null : imageUploadResult.Value.WebContentLink;
    }

    private bool IsFormValid()
    {
        Team.Name.Validate();
        bool isParticipantValid = true; 

        foreach (var participant in Team.Participants)
        {
            isParticipantValid = isParticipantValid && participant.Name.Validate();
        }

        return Team.Name.IsValid && isParticipantValid;
    }

    private void ClearForm()
    {
        Team.Name.Value = null;
        Participants.Clear();
    }
}
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml.Controls;
using Solution.Core.Models;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;
using Windows.ApplicationModel.VoiceCommands;

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

    public IRelayCommand ParticipantValidationCommand => new RelayCommand(() => this.Participant.Name.Validate());
    #endregion

    #region event commands
    public IAsyncRelayCommand SubmitCommand => new AsyncRelayCommand(OnSubmitAsync);

    public IAsyncRelayCommand ImageSelectCommand => new AsyncRelayCommand(OnImageSelectAsync);

    public IAsyncRelayCommand MemberAddingCommand => new AsyncRelayCommand(OnMemberAddAsync);
    public IAsyncRelayCommand TeamAddingCommand => new AsyncRelayCommand(OnTeamAddAsync);
    #endregion


    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private ImageSource image;

    private FileResult selectedFile = null;

    [ObservableProperty]
    private ICollection<ParticipantModel> participants = new List<ParticipantModel>(10);

    [ObservableProperty]
    public ParticipantModel participant = new ParticipantModel();

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

    private async Task OnMemberAddAsync() => await MemberAddAsync();

    private async Task OnTeamAddAsync() => await TeamAddAsync();

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

    private async Task MemberAddAsync()
    {
        if (!IsMemberFormValid())
        {
            return;
        }

        await UploadImageAsync();

        var result = await participantService.CreateAsync(this.Participant);
        var message = result.IsError ? result.FirstError.Description : "Participant saved.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            ClearMemberForm();
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private async Task TeamAddAsync()
    {
        if (!IsFormValid())
        {
            return;
        }

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

    private bool IsFormValid()
    {
        this.Name.Validate();

        return this.Name.IsValid;
    }

    private void ClearMemberForm()
    {
        this.Participant.Name.Value = null;
        this.Participant.ImageId = null;
        this.Participant.WebContentLink = null;
    }

    private bool IsMemberFormValid()
    {
        this.Participant.Name.Validate();

        return this.Participant.Name.IsValid;
    }

    private void ClearForm()
    {
        this.Name.Value = null;
    }
}
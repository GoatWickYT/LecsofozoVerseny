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

    public IRelayCommand ParticipantValidationCommand => new RelayCommand(() => this.Participants.All(x=> x.Name.Validate()));
    #endregion

    #region event commands
    public IAsyncRelayCommand SubmitCommand => new AsyncRelayCommand(OnSubmitAsync);

    public IAsyncRelayCommand ImageSelectCommand => new AsyncRelayCommand(OnImageSelectAsync);

    public IRelayCommand MemberAddingCommand => new RelayCommand(MemberAdd);
    #endregion


    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private ImageSource image;

    private FileResult selectedFile = null;

    [ObservableProperty]
    private ICollection<ParticipantModel> participants = new List<ParticipantModel>(10);

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

        this.Id = team.Id;
        this.Name.Value = team.Name.Value;
        this.Participants = team.Participants;

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
        foreach(var participant in this.Participants)
        {
            participant.TeamId = result.Value.Id;
            await participantService.CreateAsync(participant);
        }

        var message = result.IsError ? result.FirstError.Description : "Team saved.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            ClearForm();
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    private void MemberAdd()
    {

        ParticipantModel participant = new ParticipantModel();
        
        Participants.Add(participant);
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
        bool isParticipantValid = true; 

        foreach (var participant in this.Participants)
        {
            isParticipantValid = isParticipantValid && participant.Name.Validate();
        }

        return this.Name.IsValid && isParticipantValid;
    }

    private void ClearForm()
    {
        this.Name.Value = null;
        Participants.Clear();
    }
}
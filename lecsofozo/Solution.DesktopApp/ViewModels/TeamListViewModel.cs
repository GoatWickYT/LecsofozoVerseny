namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class TeamListViewModel(ITeamService teamService, IParticipantService participantService) : TeamModel(), IQueryAttributable
{
    #region life cycle commands
    public IAsyncRelayCommand AppearingCommand => new AsyncRelayCommand(OnAppearingAsync);
    public IAsyncRelayCommand DisappearingCommand => new AsyncRelayCommand(OnDisappearingAsync);
    #endregion

    public IAsyncRelayCommand PreviousPageCommand => new AsyncRelayCommand(PreviousPage);
    public IAsyncRelayCommand FirstPageCommand => new AsyncRelayCommand(FirstPage);
    public IAsyncRelayCommand NextPageCommand => new AsyncRelayCommand(NextPage);
    public IAsyncRelayCommand LastPageCommand => new AsyncRelayCommand(LastPage);
    public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<string>((publicId) => OnDeleteAsync(publicId));

    public IAsyncRelayCommand EditCommand => new AsyncRelayCommand(OnEditAsync);

    [ObservableProperty]
    private static ICollection<TeamModel> teams;

    [ObservableProperty]
    private static ICollection<ParticipantModel> participants;

    [ObservableProperty]
    private TeamModel selectedTeam;

    [ObservableProperty]
    private string pageNumber = "page\n1";

    [ObservableProperty]
    private bool previousButtonEnabled = false;

    [ObservableProperty]
    private bool nextButtonEnabled = false;

    [ObservableProperty]
    private bool lastButtonEnabled = false;

    [ObservableProperty]
    private bool firstButtonEnabled = false;

    private int maxPageNumber;
    private int page = 1;

    private async Task OnAppearingAsync()
    {
        maxPageNumber = await teamService.GetMaxPageNumberAsync();
        await LoadTeamsAsync();
    }

    private async Task OnDisappearingAsync()
    { }

    #region Page Buttons
    private async Task PreviousPage()
    {
        if (page > 1)
        {
            page--;
            PageNumber = $"page\n{page}";
            await LoadTeamsAsync();
            return;
        }
    }

    private async Task FirstPage()
    {
        page = 1;
        PageNumber = $"page\n{page}";
        await LoadTeamsAsync();
    }

    private async Task NextPage()
    {
        if (maxPageNumber < page + 1)
        {
            return;
        }
        page++;
        PageNumber = $"page\n{page}";

        await LoadTeamsAsync();
    }

    private async Task LastPage()
    {
        LastButtonEnabled = false;
        page = await teamService.GetMaxPageNumberAsync();
        PageNumber = $"page\n{page}";

        await LoadTeamsAsync();
    }
    #endregion

    private async Task OnDeleteAsync(string? id)
    {
        var result = await teamService.DeleteAsync(id);

        var message = result.IsError ? result.FirstError.Description : "Team deleted";
        var title = result.IsError ? "Error" : "Success";

        if (!result.IsError)
        {
            var team = Teams.FirstOrDefault(x => x.PublicId == id);
            Teams.Remove(team);

            if(page != 0)
            {
                await PreviousPage();
            }

            await LoadTeamsAsync();
        }
        await Application.Current.MainPage.DisplayAlert(title, message, "Ok");
    }

    private async Task OnEditAsync()
    {
        ShellNavigationQueryParameters navigationQueryParameter = new ShellNavigationQueryParameters
        {
            {"Team", this.SelectedTeam }
        };
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditTeamView.Name, navigationQueryParameter);
    }

    private async Task LoadTeamsAsync()
    {
        var result = await teamService.GetAllAsync();

        if (result.IsError)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Teams not loaded!", "OK");
            return;
        }
        var message = result.IsError ? result.FirstError.Description : "Done";

        await EnableButtonsAsync();

        Teams = result.Value;
        if (Teams.Count != 0)
        {
            SelectedTeam = Teams.ToList()[page - 1];
            SelectedTeam.Participants = await participantService.GetByTeamIdAsync(SelectedTeam.Id);
            Participants = SelectedTeam.Participants;
        }
        else
        {
            SelectedTeam = null;
            await Application.Current.MainPage.DisplayAlert("Error", "No teams in DB, go to the adding page", "OK");
        }

    }

    private async Task EnableButtonsAsync()
    {
        LastButtonEnabled = page != maxPageNumber;
        FirstButtonEnabled = page != 1;
        NextButtonEnabled = page < await teamService.GetMaxPageNumberAsync();
        PreviousButtonEnabled = page > 1;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        bool hasValue = query.TryGetValue("Id", out object result);

        if (!hasValue)
        {
            return;
        }
        await teamService.DeleteAsync(result.ToString());
        await Application.Current.MainPage.DisplayAlert("Success", "Team deleted!", "Ok");
        await LoadTeamsAsync();
    }
}
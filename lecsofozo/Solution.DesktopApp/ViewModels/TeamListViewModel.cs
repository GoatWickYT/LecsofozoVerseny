
using Solution.Core.Interfaces;
using Solution.Services.Services;
using System.Collections.ObjectModel;
using Windows.Media.AppBroadcasting;

namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class TeamListViewModel(ITeamService teamService) : TeamModel(), IQueryAttributable
{
    #region life cycle commands
    public IAsyncRelayCommand AppearingCommand => new AsyncRelayCommand(OnAppearingAsync);
    public IAsyncRelayCommand DisappearingCommand => new AsyncRelayCommand(OnDisappearingAsync);
    #endregion

    public IAsyncRelayCommand PreviousPageCommand => new AsyncRelayCommand(PreviousPage);
    public IAsyncRelayCommand FirstPageCommand => new AsyncRelayCommand(FirstPage);
    public IAsyncRelayCommand NextPageCommand => new AsyncRelayCommand(NextPage);
    public IAsyncRelayCommand LastPageCommand => new AsyncRelayCommand(LastPage);
    public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<string>((id) => OnDeleteAsync(id));

    [ObservableProperty]
    private static ObservableCollection<TeamModel> teams;

    [ObservableProperty]
    private static ICollection<ParticipantModel> participants;

    [ObservableProperty]
    private string teamName = string.Empty;

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
            var team = Teams.SingleOrDefault(x => x.PublicId == id);
            Teams.Remove(team);

            if (Teams.Count == 0)
            {
                await LoadTeamsAsync();
            }
        }
        await Application.Current.MainPage.DisplayAlert(title, message, "Ok");
    }

    private async Task LoadTeamsAsync()
    {
        var result = await teamService.GetPagedAsync(page);

        if (result.IsError)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Teams not loaded!", "OK");
            return;
        }
        var message = result.IsError ? result.FirstError.Description : "Done";

        await EnableButtonsAsync();

        TeamName = result.Value[0].Name.Value;
        Participants = result.Value[0].Participants;
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
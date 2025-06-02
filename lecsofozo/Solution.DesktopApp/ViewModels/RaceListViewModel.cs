namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class RaceListViewModel(IRaceService raceService) : RaceModel(), IQueryAttributable
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
    private static ICollection<RaceModel> races;

    [ObservableProperty]
    private RaceModel selectedRace;

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
        maxPageNumber = await raceService.GetMaxPageNumberAsync();
        await LoadRacesAsync();
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
            await LoadRacesAsync();
            return;
        }
    }

    private async Task FirstPage()
    {
        page = 1;
        PageNumber = $"page\n{page}";
        await LoadRacesAsync();
    }

    private async Task NextPage()
    {
        if (maxPageNumber < page + 1)
        {
            return;
        }
        page++;
        PageNumber = $"page\n{page}";

        await LoadRacesAsync();
    }

    private async Task LastPage()
    {
        LastButtonEnabled = false;
        page = await raceService.GetMaxPageNumberAsync();
        PageNumber = $"page\n{page}";

        await LoadRacesAsync();
    }
    #endregion

    private async Task OnDeleteAsync(string? id)
    {
        var result = await raceService.DeleteAsync(id);

        var message = result.IsError ? result.FirstError.Description : "Races deleted";
        var title = result.IsError ? "Error" : "Success";

        if (!result.IsError)
        {
            var race = Races.FirstOrDefault(x => x.PublicId == id);
            Races.Remove(race);

            if(page != 0)
            {
                await PreviousPage();
            }

            await LoadRacesAsync();
        }
        await Application.Current.MainPage.DisplayAlert(title, message, "Ok");
    }

    private async Task OnEditAsync()
    {
        ShellNavigationQueryParameters navigationQueryParameter = new ShellNavigationQueryParameters
        {
            {"Races", this.SelectedRace }
        };
        Shell.Current.ClearNavigationStack();
        await Shell.Current.GoToAsync(CreateOrEditRaceView.Name, navigationQueryParameter);
    }

    private async Task LoadRacesAsync()
    {
        var result = await raceService.GetAllAsync();

        if (result.IsError)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Racess not loaded!", "OK");
            return;
        }
        var message = result.IsError ? result.FirstError.Description : "Done";

        await EnableButtonsAsync();

        Races = result.Value;
        if (Races.Count != 0)
        {
            SelectedRace = Races.ToList()[page - 1];
        }
        else
        {
            SelectedRace = null;
            await Application.Current.MainPage.DisplayAlert("Error", "No Racess in DB, go to the adding page", "OK");
        }

    }

    private async Task EnableButtonsAsync()
    {
        LastButtonEnabled = page != maxPageNumber;
        FirstButtonEnabled = page != 1;
        NextButtonEnabled = page < await raceService.GetMaxPageNumberAsync();
        PreviousButtonEnabled = page > 1;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        bool hasValue = query.TryGetValue("Id", out object result);

        if (!hasValue)
        {
            return;
        }
        await raceService.DeleteAsync(result.ToString());
        await Application.Current.MainPage.DisplayAlert("Success", "Races deleted!", "Ok");
        await LoadRacesAsync();
    }
}
using CommunityToolkit.Maui.Core.Extensions;

namespace Solution.DesktopApp.ViewModels;

[ObservableObject]
public partial class CreateOrEditRaceViewModel(AppDbContext appDbContext,
                                               IGoogleDriveService googleDriveService,
                                               IRaceService raceService) : RaceModel(), IQueryAttributable
{
    #region life cycle commands
    public IAsyncRelayCommand AppearingCommand => new AsyncRelayCommand(OnAppearingAsync);
    public IAsyncRelayCommand DisappearingCommand => new AsyncRelayCommand(OnDisappearingAsync);
    #endregion

    #region validation commands
    public IRelayCommand NameValidationCommand => new RelayCommand(() => this.Name.Validate());
    public IRelayCommand StreetValidationCommand => new RelayCommand(() => this.Location.Value.Street.Validate());
    public IRelayCommand HouseNumberValidationCommand => new RelayCommand(() => this.Location.Value.HouseNumber.Validate());
    public IRelayCommand ValueValidationCommand => new RelayCommand(() => this.PointsWithTeam.ToList().ForEach(x => x.Value.Validate()));
    #endregion
    public IRelayCommand SearchBarChanged => new RelayCommand<string>((string query) => SearchCities(query));

    public IRelayCommand TeamSelectedCommand => new RelayCommand(() => TeamSelected());

    public IRelayCommand JudgeSelectedCommand => new RelayCommand(() => JudgeSelected());

    public IRelayCommand ItemSelectedCommand => new RelayCommand(() => CitySelected());

    [ObservableProperty]
    private CityModel selectedCity = new CityModel();

    [ObservableProperty]
    private ObservableCollection<CityModel> searchedCities = new ObservableCollection<CityModel>();

    [ObservableProperty]
    private string searchQuery;

    [ObservableProperty]
    private IList<CityModel> cities = new List<CityModel>();

    [ObservableProperty]
    private IList<TeamModel> availableTeams = new List<TeamModel>();

    [ObservableProperty]
    private string title;

    private delegate Task ButtonActionDelegate();
    private ButtonActionDelegate asyncButtonAction;

    [ObservableProperty]
    private string selectedTeamNames;

    [ObservableProperty]
    private IList<JudgeModel> availableJudges = new List<JudgeModel>();

    [ObservableProperty]
    private string selectedJudgeNames;

    public ObservableCollection<object> SelectedJudges { get; set; } = [];

    public ObservableCollection<object> SelectedTeams { get; set; } = [];

    public ObservableCollection<PointModel> PointsWithTeam { get; set; } = [];

    public ObservableCollection<uint> PointValues { get; set; } = [];

    private void TeamSelected()
    {
        if(SelectedTeams.Count == 0)
        {
            PointsWithTeam.Clear();
        }

        PointsWithTeam.Clear();
        foreach (TeamModel team in SelectedTeams)
        {
            if (team is TeamModel selectedTeam)
            {
                PointsWithTeam.Add(new PointModel
                {
                    Team = new ValidatableObject<TeamModel> { Value = team },
                    Value = new ValidatableObject<uint>(),
                });
            }
        }

        SelectedTeamNames = string.Join(", ", SelectedTeams.OfType<TeamModel>().Select(t => t.Name.Value));
    }


    private void JudgeSelected()
    {
        foreach (var judge in SelectedJudges)
        {
            if (judge is JudgeModel selectedJudge)
            {
                this.Judges.Add(selectedJudge);
            }
        }

        SelectedJudgeNames = string.Join(", ", SelectedJudges.OfType<JudgeModel>().Select(j => j.Name.Value));
    }

    public DateTime MaxDateTime => DateTime.Now;

    [ObservableProperty]
    private double datePickerWidth;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await Task.Run(LoadCitiesAsync);
        await Task.Run(LoadTeamsAsync);
        await Task.Run(LoadJudgesAsync);

        bool hasValue = query.TryGetValue("Race", out object result);

        if (!hasValue)
        {
            //asyncButtonAction = OnSaveAsync;
            Title = "Create Race";
            return;
        }

        RaceModel model = result as RaceModel;

        this.Id = model.Id;
        this.PublicId = model.PublicId;
        this.Location.Value.City.Value = selectedCity;
        this.Location.Value.Street.Value = model.Location.Value.Street.Value;
        this.Location.Value.HouseNumber.Value = model.Location.Value.HouseNumber.Value;
        this.Teams = model.Teams;
        this.Judges = model.Judges;

        //asyncButtonAction = OnUpdateAsync;
        Title = "Update Race";
    }

    private void CitySelected()
    {
        SearchQuery = $"{SelectedCity.Name} ({SelectedCity.PostalCode})";
        SearchedCities.Clear();
    }


    private void SearchCities(string query)
    {
        if (query.Length > 1 && !query.IsNullOrEmpty())
        {
            query.ToLower();
            SearchedCities = new ObservableCollection<CityModel>(Cities.Where(x => (!query.IsNumeric() && x.Name.ToLower().Contains(query)) || (query.IsNumeric() && x.PostalCode.ToString().Contains(query))));
        }
        if (query.IsNullOrEmpty())
        {
            SearchedCities.Clear();
        }
    }

    private async Task OnAppearingAsync() { }
    private async Task OnDisappearingAsync() { }

    private async Task LoadCitiesAsync()
    {
        Cities = await appDbContext.Cities.AsNoTracking()
                                                     .OrderBy(c => c.Name)
                                                     .Select(x => new CityModel(x))
                                                     .ToListAsync();
    }

    private async Task LoadTeamsAsync()
    {
        AvailableTeams = await appDbContext.Teams.AsNoTracking()
                                                     .OrderBy(t => t.Name)
                                                     .Select(x => new TeamModel(x))
                                                     .ToListAsync();
    }

    private async Task LoadJudgesAsync()
    {
        AvailableJudges = await appDbContext.Judges.AsNoTracking()
                                                     .OrderBy(j => j.Name)
                                                     .Select(j => new JudgeModel(j))
                                                     .ToListAsync();
    }

    private bool IsFormValid()
    {
        this.Name.Validate();
        this.Location.Value.Street.Validate();
        this.Location.Value.HouseNumber.Validate();

        return this.Name.IsValid && this.Location.Value.Street.IsValid && this.Location.Value.HouseNumber.IsValid;
    }

    private void ClearForm()
    {
        this.Name.Value = null;
        this.Location.Value.City.Value = null;
        this.Location.Value.Street.Value = null;
        this.Location.Value.HouseNumber.Value = null;
    }
}
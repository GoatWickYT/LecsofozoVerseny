using ErrorOr;
using Solution.Services.Services;

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

    #region event commands
    public IAsyncRelayCommand SubmitCommand => new AsyncRelayCommand(OnSubmitAsync);

    public IRelayCommand SearchBarChanged => new RelayCommand<string>((string query) => SearchCities(query));

    public IRelayCommand TeamSelectedCommand => new RelayCommand(TeamSelected);

    public IRelayCommand ItemSelectedCommand => new RelayCommand(CitySelected);

    public IRelayCommand SelectedJudgeCommand => new RelayCommand(JudgeSelected);
    #endregion

    private delegate Task ButtonActionDelegate();
    private ButtonActionDelegate asyncButtonAction;

    [ObservableProperty]
    private CityModel selectedCity = new CityModel();

    [ObservableProperty]
    private ObservableCollection<CityModel> searchedCities = new ObservableCollection<CityModel>();

    [ObservableProperty]
    private JudgeModel firstJudge = new JudgeModel();

    [ObservableProperty]
    private JudgeModel secondJudge = new JudgeModel();

    [ObservableProperty]
    private JudgeModel thirdJudge = new JudgeModel();

    [ObservableProperty]
    private string searchQuery;

    [ObservableProperty]
    private IList<CityModel> cities = new List<CityModel>();

    [ObservableProperty]
    private IList<TeamModel> availableTeams = new List<TeamModel>();

    [ObservableProperty]
    private IList<JudgeModel> availableJudges = new List<JudgeModel>();

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string selectedTeamNames;

    [ObservableProperty]
    private double datePickerWidth;

    public ObservableCollection<object> SelectedTeams { get; set; } = [];

    public ObservableCollection<PointModel> PointsWithTeam { get; set; } = [];

    public ObservableCollection<uint> PointValues { get; set; } = [];

    public DateTime MaxDateTime => DateTime.Now;

    private async Task OnAppearingAsync() { }
    private async Task OnDisappearingAsync() { }

    private async Task OnSubmitAsync() => await asyncButtonAction();

    private void TeamSelected()
    {
        if(SelectedTeams.Count == 0)
        {
            PointsWithTeam.Clear();
        }

        PointsWithTeam.Clear();
        this.Teams.Clear();
        foreach (TeamModel team in SelectedTeams)
        {
            if (team is TeamModel selectedTeam)
            {
                this.Teams.Add(selectedTeam);
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
        if (FirstJudge == SecondJudge || SecondJudge == ThirdJudge || ThirdJudge == FirstJudge)
        {
            Application.Current.MainPage.DisplayAlert("Wrong Selection", "Same Judges can't be selected", "OK");
            return;
        }
        Judges = new List<JudgeModel>(){ FirstJudge, SecondJudge, ThirdJudge };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await Task.Run(LoadCitiesAsync);
        await Task.Run(LoadTeamsAsync);
        await Task.Run(LoadJudgesAsync);

        bool hasValue = query.TryGetValue("Race", out object result);

        if (!hasValue)
        {
            asyncButtonAction = OnSaveAsync;
            Title = "Create Race";
            return;
        }

        RaceModel model = result as RaceModel;

        this.Id = model.Id;
        this.PublicId = model.PublicId;
        this.Teams = model.Teams;
        this.Judges = model.Judges;
        this.Name.Value = model.Name.Value;
        this.Date.Value = model.Date.Value;
        this.Location.Value.City.Value = model.Location.Value.City.Value;
        this.Location.Value.Id = model.Location.Value.Id;
        this.Location.Value.Street.Value = model.Location.Value.Street.Value;
        this.Location.Value.HouseNumber.Value = model.Location.Value.HouseNumber.Value;
        this.SelectedCity = model.Location.Value.City.Value;
        this.SearchQuery = $"{SelectedCity.Name} ({SelectedCity.PostalCode})";
        this.PointsWithTeam = new ObservableCollection<PointModel>(model.Points.Select(p => new PointModel
        {
            Team = new ValidatableObject<TeamModel> { Value = p.Team.Value },
            Value = new ValidatableObject<uint> { Value = p.Value.Value }
        }));
        this.FirstJudge = model.Judges.ElementAtOrDefault(0) ?? new JudgeModel();
        this.SecondJudge = model.Judges.ElementAtOrDefault(1) ?? new JudgeModel();
        this.ThirdJudge = model.Judges.ElementAtOrDefault(2) ?? new JudgeModel();

        asyncButtonAction = OnUpdateAsync;
        Title = "Update Race";
    }

    private void CitySelected()
    {
        SearchQuery = $"{SelectedCity.Name} ({SelectedCity.PostalCode})";
        SearchedCities.Clear();
        this.Location = new ValidatableObject<LocationModel>
        {
            Value = new LocationModel
            {
                City = new ValidatableObject<CityModel> { Value = SelectedCity },
                Street = new ValidatableObject<string>(),
                HouseNumber = new ValidatableObject<string>()
            }
        };
    }

    private void SearchCities(string query)
    {
        if (query.Length > 1 && !query.IsNullOrEmpty())
        {
            query.ToLower();
            SearchedCities = new ObservableCollection<CityModel>(Cities.Where(x => (!query.IsNumeric() && x.Name.ToLower().Contains(query)) ||
                                                                                   (query.IsNumeric() && x.PostalCode.ToString().Contains(query))));
        }
        if (query.IsNullOrEmpty())
        {
            SearchedCities.Clear();
        }
    }

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
        this.PointsWithTeam.ToList().ForEach(x => x.Value.Validate());


        return this.Name.IsValid &&
            this.Location.Value.Street.IsValid &&
            this.Location.Value.HouseNumber.IsValid &&
            (FirstJudge != SecondJudge || SecondJudge != ThirdJudge || ThirdJudge != FirstJudge);
    }

    private void ClearForm()
    {
        this.Name.Value = null;
        this.Location.Value.City.Value = null;
        this.Location.Value.Street.Value = null;
        this.Location.Value.HouseNumber.Value = null;
        this.Teams.Clear();
        this.Judges.Clear();
        this.PointsWithTeam.Clear();
    }

    public async Task OnSaveAsync()
    {
        if (!IsFormValid())
        {
            return;
        }
        
        var result = await raceService.CreateAsync(this, PointsWithTeam.ToList());

        var message = result.IsError ? result.FirstError.Description : "Judge saved.";
        var title = result.IsError ? "Error" : "Information";

        if (!result.IsError)
        {
            ClearForm();
        }

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public async Task OnUpdateAsync()
    {
        if (!IsFormValid())
        {
            return;
        }

        var result = await raceService.UpdateAsync(this);

        var message = result.IsError ? result.FirstError.Description : "Judge updated.";
        var title = result.IsError ? "Error" : "Information";

        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}
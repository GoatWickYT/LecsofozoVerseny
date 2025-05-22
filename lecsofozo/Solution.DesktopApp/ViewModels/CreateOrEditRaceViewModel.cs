using CommunityToolkit.Common;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;

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
    public IRelayCommand StreetValidation => new RelayCommand(() => this.Location.Value.Street.Validate());
    public IRelayCommand HouseNumberValidation => new RelayCommand(() => this.Location.Value.HouseNumber.Validate());
    #endregion
    public IRelayCommand SearchBarChanged => new RelayCommand<string>((string query) => SearchCities(query));

    public IRelayCommand ItemSelectedCommand => new RelayCommand(() => CitySelected());

    [ObservableProperty]
    private CityModel selectedCity = new CityModel();

    [ObservableProperty]
    private ObservableCollection<CityModel> searchedCities = new ObservableCollection<CityModel>();

    [ObservableProperty]
    private string searchQuery;

    [ObservableProperty]
    private IList<CityModel> cities = new List<CityModel>();

    public DateTime MaxDateTime => DateTime.Now;

    [ObservableProperty]
    private double datePickerWidth;

    private void CitySelected()
    {
        SearchQuery = $"{SelectedCity.Name} ({SelectedCity.PostalCode})";
        SearchedCities.Clear();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await Task.Run(LoadCitiesAsync);
    }

    private void SearchCities(string query)
    {
        if (query.Length > 1 && !query.IsNullOrEmpty())
        {
            query.ToLower();
            SearchedCities = new ObservableCollection<CityModel>(Cities.Where(x => (!query.IsNumeric() && x.Name.ToLower().Contains(query)) || (query.IsNumeric() && x.PostalCode.ToString().Contains(query))));
        }
    }

    private async Task OnAppearingAsync() { }
    private async Task OnDisappearingAsync() { }

    private async Task LoadCitiesAsync()
    {
        Cities = await appDbContext.Cities.AsNoTracking()
                                                     .OrderBy(x => x.Name)
                                                     .Select(x => new CityModel(x))
                                                     .ToListAsync();
    }
}
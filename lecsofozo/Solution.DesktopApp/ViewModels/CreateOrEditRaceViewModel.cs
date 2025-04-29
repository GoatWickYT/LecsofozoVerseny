
using Microsoft.EntityFrameworkCore;

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
    public IRelayCommand CityIndexChangedCommand => new RelayCommand(() => this.Location.Value.City.Validate());

    #endregion

    public DateTime MaxDateTime => DateTime.Now;

    [ObservableProperty]
    private double datePickerWidth;

    [ObservableProperty]
    private IList<CityModel> cities = [];



    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await Task.Run(LoadCitiesAsync);
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
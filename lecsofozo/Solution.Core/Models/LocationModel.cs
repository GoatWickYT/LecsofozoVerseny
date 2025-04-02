namespace Solution.Core.Models;

public class LocationModel
{
    public uint Id { get; set; }

    public ValidatableObject<string> Street { get; set; }

    public ValidatableObject<string> HouseNumber { get; set; }

    public ValidatableObject<CityModel> City { get; set; }

    public LocationModel()
    {
        Street = new ValidatableObject<string>();
        HouseNumber = new ValidatableObject<string>();
        City = new ValidatableObject<CityModel>();
     
        AddValidators();
    }

    public LocationModel(LocationEntity entity)
    {
        this.Id = entity.Id;
        this.Street.Value = entity.Street;
        this.HouseNumber.Value = entity.HouseNumber;
        this.City.Value = new CityModel(entity.City);
    }

    public LocationEntity ToEntity()
    {
        return new LocationEntity
        {
            Street = this.Street.Value,
            HouseNumber = this.HouseNumber.Value,
            CityId = this.City.Value.Id
        };
    }

    public void ToEntity(LocationEntity entity)
    {
        entity.Street = this.Street.Value;
        entity.HouseNumber = this.HouseNumber.Value;
        entity.CityId = this.City.Value.Id;
    }

    private void AddValidators()
    {
        this.Street.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Street is required." });
        this.HouseNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "House number is required." });
        this.City.Validations.Add(new IsNotNullOrEmptyRule<CityModel> { ValidationMessage = "City is required." });
    }
}

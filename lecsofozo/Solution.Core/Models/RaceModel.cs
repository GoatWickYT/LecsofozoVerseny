namespace Solution.Core.Models;

public class RaceModel
{
    public uint Id { get; set; }

    public string PublicId { get; set; }

    public ValidatableObject<string> Name { get; set; }

    public ValidatableObject<DateTime> Date { get; set; }

    public ValidatableObject<LocationModel> Location { get; set; }

    public ICollection<TeamModel> Teams { get; set; }

    public ICollection<PointModel> Points { get; set; }

    public ICollection<JudgeModel> Judges { get; set; }

    public RaceModel()
    {
        Name = new ValidatableObject<string>();
        Date = new ValidatableObject<DateTime>();
        Location = new ValidatableObject<LocationModel>();

        AddValidators();
    }

    public RaceModel(RaceEntity entity)
    {
        this.Id = entity.Id;
        this.PublicId = entity.PublicId;
        this.Name.Value = entity.Name;
        this.Date.Value = entity.Date;
        this.Location.Value = new LocationModel(entity.Location);
    }

    public RaceEntity ToEntity()
    {
        return new RaceEntity
        {
            PublicId = this.PublicId,
            Name = this.Name.Value,
            Date = this.Date.Value,
            LocationId = this.Location.Value.Id
        };
    }

    public void ToEntity(RaceEntity entity)
    {
        entity.PublicId = this.PublicId;
        entity.Name = this.Name.Value;
        entity.Date = this.Date.Value;
        entity.LocationId = this.Location.Value.Id;
    }

    private void AddValidators()
    {
        this.Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Name is required." });
        this.Date.Validations.AddRange(
            new IsNotNullOrEmptyRule<DateTime> { ValidationMessage = "Date is required." },
            new MaxDateRule<DateTime>(DateTime.Now) { ValidationMessage = "Must be a valid date"}
            );
        this.Location.Validations.Add(new IsNotNullOrEmptyRule<LocationModel> { ValidationMessage = "Location is required." });
    }
}

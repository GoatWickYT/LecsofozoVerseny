namespace Solution.Core.Models;

public class PointModel
{
    public uint Id { get; set; }

    public ValidatableObject<uint> Value { get; set; }

    public ValidatableObject<TeamModel> Team { get; set; }

    public ValidatableObject<RaceModel> Race { get; set; }

    public PointModel()
    {
        Value = new ValidatableObject<uint>();
        Team = new ValidatableObject<TeamModel>();

        AddValidators();
    }

    public PointModel(PointEntity entity): this()
    { 
        this.Value.Value = entity.Value;
        this.Team.Value = new TeamModel(entity.Team);
    }

    public PointEntity ToEntity()
    {
        return new PointEntity
        {
            Value = this.Value.Value,
            TeamId = this.Team.Value.Id,
            RaceId = this.Race.Value.Id
        };
    } 
    
    public void ToEntity(PointEntity entity)
    {
        entity.Value = this.Value.Value;
        entity.RaceId = this.Race.Value.Id;
        entity.TeamId = this.Team.Value.Id;
    }

    private void AddValidators()
    {
        this.Value.Validations.Add(new NullableIntegerRule<uint> { ValidationMessage = "Point is required." });
    }
}
namespace Solution.Core.Models;

public class TeamModel : IObjectValidator<uint>
{
    public uint Id { get; set; }

    public string PublicId { get; set; }

    public ValidatableObject<string> Name { get; set; }

    public ICollection<ParticipantEntity> Participants { get; set; }

    public TeamModel()
    {
        Name = new ValidatableObject<string>();

        AddValidators();
    }

    public TeamModel(TeamEntity entity)
    {
        this.Id = entity.Id;
        this.PublicId = entity.PublicId;
        this.Name.Value = entity.Name;
    }

    public TeamEntity ToEntity()
    {
        return new TeamEntity
        {
            PublicId = this.PublicId,
            Name = this.Name.Value
        };
    }

    public void ToEntity(TeamEntity entity)
    {
        entity.PublicId = this.PublicId;
        entity.Name = this.Name.Value;
    }

    private void AddValidators()
    {
        this.Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Name is required." });
    }
}

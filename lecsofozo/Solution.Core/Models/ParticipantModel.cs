using CommunityToolkit.Mvvm.ComponentModel;

namespace Solution.Core.Models;

[ObservableObject]
public partial class ParticipantModel
{
    public uint Id { get; set; }

    public string PublicId { get; set; }

    public string ImageId { get; set; }

    public string WebContentLink { get; set; }

    public TeamModel Team { get; set; }

    public FileResult SelectedFile { get; set; }

    [ObservableProperty]
    private ImageSource image;

    public ValidatableObject<string> Name { get; set; }

    public ParticipantModel()
    {
        this.Name = new ValidatableObject<string>();

        AddValidators();
    }

    public ParticipantModel(ParticipantEntity entity): this()
    {
        this.Id = entity.Id;
        this.PublicId = entity.PublicId;
        this.ImageId = entity.ImageId;
        this.WebContentLink = entity.WebContentLink;
        this.Name.Value = entity.Name;
    }

    public ParticipantEntity ToEntity()
    {
        return new ParticipantEntity
        {
            PublicId = this.PublicId,
            ImageId = this.ImageId,
            WebContentLink = this.WebContentLink,
            Name = this.Name.Value,
            TeamId = this.Team.Id,
        };
    }

    public void ToEntity(ParticipantEntity entity)
    {
        entity.PublicId = this.PublicId;
        entity.ImageId = this.ImageId;
        entity.WebContentLink = this.WebContentLink;
        entity.Name = this.Name.Value;
        entity.TeamId = this.Team.Id;
    }

    private void AddValidators()
    {
        this.Name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Participant name is required." });
    }
}
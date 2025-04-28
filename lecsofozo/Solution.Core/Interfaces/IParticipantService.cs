namespace Solution.Core.Interfaces;

public interface IParticipantService
{
    Task<ErrorOr<ParticipantModel>> CreateAsync(ParticipantModel model);
    Task<ErrorOr<Success>> UpdateAsync(ParticipantModel model);
    Task<ErrorOr<Success>> DeleteAsync(string id);
    Task<ErrorOr<ParticipantModel>> GetByIdAsync(string id);
    Task<ErrorOr<List<ParticipantModel>>> GetAllAsync();
    Task<ErrorOr<List<ParticipantModel>>> GetPagedAsync(int page = 0);
    Task<int> GetMaxPageNumberAsync();
    Task<ErrorOr<Success>> UpdateOrSaveAsync(ParticipantModel model);
    Task<List<ParticipantModel>> GetByTeamIdAsync(string teamId);
}

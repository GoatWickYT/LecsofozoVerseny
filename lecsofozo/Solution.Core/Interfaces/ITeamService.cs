namespace Solution.Core.Interfaces;

public interface ITeamService
{
    Task<ErrorOr<TeamModel>> CreateAsync(TeamModel model);
    Task<ErrorOr<Success>> UpdateAsync(TeamModel model);
    Task<ErrorOr<Success>> DeleteAsync(string id);
    Task<ErrorOr<TeamModel>> GetByIdAsync(string id);
    Task<ErrorOr<List<TeamModel>>> GetAllAsync();
    Task<ErrorOr<List<TeamModel>>> GetPagedAsync(int page = 0);
    Task<int> GetMaxPageNumberAsync();
}

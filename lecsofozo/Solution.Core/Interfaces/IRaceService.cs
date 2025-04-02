namespace Solution.Core.Interfaces;

public interface IRaceService
{
    Task<ErrorOr<RaceModel>> CreateAsync(RaceModel model);
    Task<ErrorOr<Success>> UpdateAsync(RaceModel model);
    Task<ErrorOr<Success>> DeleteAsync(string id);
    Task<ErrorOr<RaceModel>> GetByIdAsync(string id);
    Task<ErrorOr<List<RaceModel>>> GetAllAsync();
    Task<ErrorOr<List<RaceModel>>> GetPagedAsync(int page = 0);
    Task<int> GetMaxPageNumberAsync();
}

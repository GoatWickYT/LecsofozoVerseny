namespace Solution.Core.Interfaces;

public interface IJudgeService
{
    Task<ErrorOr<JudgeModel>> CreateAsync(JudgeModel model);
    Task<ErrorOr<Success>> UpdateAsync(JudgeModel model);
    Task<ErrorOr<Success>> DeleteAsync(string id);
    Task<ErrorOr<JudgeModel>> GetByIdAsync(string id);
    Task<ErrorOr<List<JudgeModel>>> GetAllAsync();
    Task<ErrorOr<List<JudgeModel>>> GetPagedAsync(int page = 0);
    Task<int> GetMaxPageNumberAsync();
}

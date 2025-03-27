namespace Solution.Services.Services;

public class JudgeService(AppDbContext dbContext) : IJudgeService
{
    private int ROW_COUNT = 5;

    public async Task<ErrorOr<JudgeModel>> CreateAsync(JudgeModel model)
    {
        bool exists = dbContext.Judges.Any(j => j.Name.ToLower() == model.Name.Value.ToLower() &&
                                                j.PhoneNumber.ToLower() == model.PhoneNumber.Value.ToLower() &&
                                                j.Email.ToLower() == model.Email.Value.ToLower());

        if (exists)
        {
            return Error.Conflict(description: "Judge already exists.");
        }

        var entity = model.ToEntity();

        entity.PublicId = Guid.NewGuid().ToString();

        await dbContext.Judges.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return new JudgeModel(entity);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string id)
    {
        var result = await dbContext.Judges.AsNoTracking()
                                           .Where(x => x.PublicId == id)
                                           .ExecuteDeleteAsync();

        return result > 0 ? Result.Success : Error.NotFound();
    }

    public async Task<ErrorOr<List<JudgeModel>>> GetAllAsync() =>
        await dbContext.Judges.AsNoTracking()
                              .Select(j => new JudgeModel(j))
                              .ToListAsync();

    public async Task<ErrorOr<JudgeModel>> GetByIdAsync(string id)
    {
        var entity = await dbContext.Judges.AsNoTracking()
                                          .FirstOrDefaultAsync(j => j.PublicId == id);

        return entity is null ? Error.NotFound() : new JudgeModel(entity);
    }

    public Task<int> GetMaxPageNumberAsync()
    {
        return dbContext.Judges.AsNoTracking()
                               .CountAsync()
                               .ContinueWith(t => (int)Math.Ceiling(t.Result / (double)ROW_COUNT));
    }

    public async Task<ErrorOr<List<JudgeModel>>> GetPagedAsync(int page = 0)
    {
        page = page < 0 ? 0 : page - 1;

        return await dbContext.Judges.AsNoTracking()
                           .Skip(page * ROW_COUNT)
                           .Take(ROW_COUNT)
                           .Select(x => new JudgeModel(x))
                           .ToListAsync();
    }

    public async Task<ErrorOr<Success>> UpdateAsync(JudgeModel model)
    {
        var result = await dbContext.Judges.AsNoTracking()
                                           .Where(x => x.PublicId == model.PublicId)
                                           .ExecuteUpdateAsync(x => x.SetProperty(p => p.PublicId, model.PublicId)
                                                                     .SetProperty(p => p.Name, model.Name.Value)
                                                                     .SetProperty(p => p.Email, model.Email.Value)
                                                                     .SetProperty(p => p.PhoneNumber, model.PhoneNumber.Value)
                                                                     .SetProperty(p => p.WebContentLink, model.WebContentLink)
                                                                     .SetProperty(p => p.ImageId, model.ImageId));

        return result > 0 ? Result.Success : Error.NotFound();
    }
}

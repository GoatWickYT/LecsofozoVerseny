namespace Solution.Services.Services;

public class ParticipantService(AppDbContext dbContext) : IParticipantService
{
    private int ROW_COUNT = 5;

    public async Task<ErrorOr<ParticipantModel>> CreateAsync(ParticipantModel model)
    {
        bool exists = dbContext.Participants.Any(p => p.Name.ToLower() == model.Name.Value.ToLower());

        if (exists)
        {
            return Error.Conflict(description: "Participant already exists.");
        }

        var entity = model.ToEntity();

        entity.PublicId = Guid.NewGuid().ToString();

        await dbContext.Participants.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return new ParticipantModel(entity);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string id)
    {
        var result = await dbContext.Participants.AsNoTracking()
                                           .Where(p => p.PublicId == id)
                                           .ExecuteDeleteAsync();

        return result > 0 ? Result.Success : Error.NotFound();
    }

    public async Task<ErrorOr<List<ParticipantModel>>> GetAllAsync() =>
        await dbContext.Participants.AsNoTracking()
                              .Select(p => new ParticipantModel(p))
                              .ToListAsync();

    public async Task<ErrorOr<ParticipantModel>> GetByIdAsync(string id)
    {
        var entity = await dbContext.Participants.AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.PublicId == id);

        return entity is null ? Error.NotFound() : new ParticipantModel(entity);
    }

    public Task<int> GetMaxPageNumberAsync()
    {
        return dbContext.Participants.AsNoTracking()
                                     .CountAsync()
                                     .ContinueWith(t => (int)Math.Ceiling(t.Result / (double)ROW_COUNT));
    }

    public async Task<ErrorOr<List<ParticipantModel>>> GetPagedAsync(int page = 0)
    {
        page = page < 0 ? 0 : page - 1;

        return await dbContext.Participants.AsNoTracking()
                                           .Skip(page * ROW_COUNT)
                                           .Take(ROW_COUNT)
                                           .Select(x => new ParticipantModel(x))
                                           .ToListAsync();
    }

    public async Task<ErrorOr<Success>> UpdateAsync(ParticipantModel model)
    {
        var result = await dbContext.Participants.AsNoTracking()
                                                 .Where(x => x.PublicId == model.PublicId)
                                                 .ExecuteUpdateAsync(x => x.SetProperty(p => p.PublicId, model.PublicId)
                                                                           .SetProperty(p => p.Name, model.Name.Value)
                                                                           .SetProperty(p => p.WebContentLink, model.WebContentLink)
                                                                           .SetProperty(p => p.ImageId, model.ImageId));

        return result > 0 ? Result.Success : Error.NotFound();
    }
}

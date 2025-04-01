using System.Linq;

namespace Solution.Services.Services;

public class TeamService(AppDbContext dbContext) : ITeamService
{
    private int ROW_COUNT = 5;

    public async Task<ErrorOr<TeamModel>> CreateAsync(TeamModel model)
    {
        bool exists = dbContext.Teams.Any(t => t.Name.ToLower() == model.Name.Value.ToLower() &&
                                               t.Participants.Select(x => new ParticipantModel(x)).Order().SequenceEqual(model.Participants.Select(x => x).Order()));

        if (exists)
        {
            return Error.Conflict(description: "Team already exists.");
        }

        var entity = model.ToEntity();

        entity.PublicId = Guid.NewGuid().ToString();

        await dbContext.Teams.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return new TeamModel(entity);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string id)
    {
        var result = await dbContext.Teams.AsNoTracking()
                                          .Where(t => t.PublicId == id)
                                          .ExecuteDeleteAsync();

        return result > 0 ? Result.Success : Error.NotFound();
    }

    public async Task<ErrorOr<List<TeamModel>>> GetAllAsync() =>
        await dbContext.Teams.AsNoTracking()
                             .Select(t => new TeamModel(t))
                             .ToListAsync();

    public async Task<ErrorOr<TeamModel>> GetByIdAsync(string id)
    {
        var entity = await dbContext.Teams.AsNoTracking()
                                          .FirstOrDefaultAsync(t => t.PublicId == id);

        return entity is null ? Error.NotFound() : new TeamModel(entity);
    }

    public Task<int> GetMaxPageNumberAsync()
    {
        return dbContext.Teams.AsNoTracking()
                              .CountAsync()
                              .ContinueWith(t => (int)Math.Ceiling(t.Result / (double)ROW_COUNT));
    }

    public async Task<ErrorOr<List<TeamModel>>> GetPagedAsync(int page = 0)
    {
        page = page < 0 ? 0 : page - 1;

        return await dbContext.Teams.AsNoTracking()
                                    .Skip(page * ROW_COUNT)
                                    .Take(ROW_COUNT)
                                    .Select(t => new TeamModel(t))
                                    .ToListAsync();
    }

    public async Task<ErrorOr<Success>> UpdateAsync(TeamModel model)
    {
        var result = await dbContext.Teams.AsNoTracking()
                                          .Where(x => x.PublicId == model.PublicId)
                                          .ExecuteUpdateAsync(x => x.SetProperty(p => p.PublicId, model.PublicId)
                                                                    .SetProperty(p => p.Participants, model.Participants.Select(x => x.ToEntity()).ToList())
                                                                    .SetProperty(p => p.Name, model.Name.Value));

        return result > 0 ? Result.Success : Error.NotFound();
    }
}

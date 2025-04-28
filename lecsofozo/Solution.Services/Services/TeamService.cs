using Solution.ValidationLibrary;
using System.Linq;

namespace Solution.Services.Services;

public class TeamService(AppDbContext dbContext) : ITeamService
{
    public async Task<ErrorOr<TeamModel>> CreateAsync(TeamModel model)
    {
        bool exists = dbContext.Teams.Any(t => t.Name.ToLower() == model.Name.Value.ToLower());

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
                             .Include(x => x.Participants)
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
                              .CountAsync();
    }

    public async Task<ErrorOr<List<TeamModel>>> GetPagedAsync(int page = 0)
    {
        page = page < 0 ? 0 : page - 1;

        var result = await dbContext.Teams.AsNoTracking()
                                          .Skip(page)
                                          .Take(1)
                                          .Select(t => new TeamModel
                                          {
                                              Id = t.Id,
                                              PublicId = t.PublicId,
                                              Name = new ValidatableObject<string> { Value = t.Name },
                                              Participants = dbContext.Participants.AsNoTracking()
                                                    .Where(p => p.TeamId == t.Id)
                                                    .Select(p => new ParticipantModel
                                                    {
                                                        Id = p.Id,
                                                        PublicId = p.PublicId,
                                                        Name = new ValidatableObject<string> { Value = p.Name },
                                                        ImageId = p.ImageId,
                                                        WebContentLink = p.WebContentLink
                                                    }).ToList()
                                          })
                                          .ToListAsync();

        return result.Any() ? result : Error.NotFound();
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

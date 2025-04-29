using Solution.Database.Entities;
using Solution.ValidationLibrary;
using System.ComponentModel.Design;

namespace Solution.Services.Services;

public class ParticipantService(AppDbContext dbContext) : IParticipantService
{
    private int ROW_COUNT = 10;

    public async Task<ErrorOr<ParticipantModel>> CreateAsync(ParticipantModel model)
    {
        bool exists = dbContext.Participants.Any(p => p.Name.ToLower() == model.Name.Value.ToLower());

        if (exists)
        {
            return Error.Conflict(description: "Participant already exists.");
        }

        ParticipantEntity entity = model.ToEntity();

        await dbContext.Participants.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return new ParticipantModel(entity)
        {
            Team = model.Team
        };
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

    public async Task<List<ParticipantModel>> GetByTeamIdAsync(uint teamId)
    {
        var entities = await dbContext.Participants.Include(x => x.Team).AsNoTracking().Where(x => x.TeamId == teamId).ToListAsync();
        List<ParticipantModel> result = new List<ParticipantModel>();

        foreach (var participant in entities)
        {
            result.Add(new ParticipantModel(participant));
        }

        return result;
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

    public async Task<ErrorOr<Success>> UpdateOrSaveAsync(ParticipantModel model)
    {
        if (model.Id > 0)
        {
            var result = await UpdateAsync(model);
            return result;
        }
        else
        {
            var result2 = await CreateAsync(model);
            return result2.IsError ? Error.Failure() : Result.Success;
        }
    }
}
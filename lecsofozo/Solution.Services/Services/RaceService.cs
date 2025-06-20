﻿using Solution.Database.Entities;
using Solution.ValidationLibrary;

namespace Solution.Services.Services;

public class RaceService(AppDbContext dbContext) : IRaceService
{
    private int ROW_COUNT = 5;

    public async Task<ErrorOr<RaceModel>> CreateAsync(RaceModel model, List<PointModel> TeamsWithPoints)
    {
        bool exists = dbContext.Races.Any(p => p.Name.ToLower() == model.Name.Value.ToLower() && 
                                               p.Date == model.Date.Value &&
                                               p.Location == model.Location.Value.ToEntity());

        if (exists)
        {
            return Error.Conflict(description: "Race already exists.");
        }

        var entity = model.ToEntity();

        entity.PublicId = Guid.NewGuid().ToString();
        entity.Location = model.Location.Value.ToEntity();
        entity.Location.HouseNumber = model.Location.Value.HouseNumber.Value;
        entity.Location.Street = model.Location.Value.Street.Value;
        entity.Location.City = dbContext.Cities.FirstOrDefault(x => x.Id == model.Location.Value.City.Value.Id);
        entity.Points = new List<PointEntity>(TeamsWithPoints.Select(p => new PointEntity
        {
            Value = p.Value.Value,
            TeamId = dbContext.Teams.FirstOrDefault(t => t.PublicId == p.Team.Value.PublicId)?.Id ?? 0,
            RaceId = entity.Id
        }));
        entity.Teams = dbContext.Teams.Where(t => model.Teams.Select(x => x.PublicId).Contains(t.PublicId)).ToList();
        entity.Judges = dbContext.Judges.Where(j => model.Judges.Select(x => x.PublicId).Contains(j.PublicId)).ToList();

        await dbContext.Races.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return new RaceModel(entity);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(string id)
    {
        var result = await dbContext.Races.AsNoTracking()
                                           .Where(p => p.PublicId == id)
                                           .ExecuteDeleteAsync();

        return result > 0 ? Result.Success : Error.NotFound();
    }

    public async Task<ErrorOr<List<RaceModel>>> GetAllAsync() =>
        await dbContext.Races.AsNoTracking()
                             .Include(p => p.Location)
                             .Include(p => p.Points).ThenInclude(p => p.Team)
                             .Include(p => p.Teams)
                             .Include(p => p.Judges)
                             .Select(p => new RaceModel(p))
                             .ToListAsync();

    public async Task<ErrorOr<RaceModel>> GetByIdAsync(string id)
    {
        var entity = await dbContext.Races.AsNoTracking()
                                          .Include(p => p.Location)
                                          .Include(p => p.Teams)
                                          .Include(p => p.Points)
                                          .Include(p => p.Judges)
                                          .FirstOrDefaultAsync(p => p.PublicId == id);

        return entity is null ? Error.NotFound() : new RaceModel(entity);
    }

    public Task<int> GetMaxPageNumberAsync()
    {
        return dbContext.Races.AsNoTracking()
                                     .CountAsync()
                                     .ContinueWith(t => (int)Math.Ceiling(t.Result / (double)ROW_COUNT));
    }

    public async Task<ErrorOr<List<RaceModel>>> GetPagedAsync(int page = 0)
    {
        page = page < 0 ? 0 : page - 1;

        return await dbContext.Races.AsNoTracking()
                                           .Skip(page * ROW_COUNT)
                                           .Take(ROW_COUNT)
                                           .Select(x => new RaceModel(x))
                                           .ToListAsync();
    }

    public async Task<ErrorOr<Success>> UpdateAsync(RaceModel model)
    {
        var result = await dbContext.Races.AsNoTracking()
                                                 .Where(x => x.PublicId == model.PublicId)
                                                 .ExecuteUpdateAsync(x => x.SetProperty(p => p.PublicId, model.PublicId)
                                                                           .SetProperty(p => p.Name, model.Name.Value)
                                                                           .SetProperty(p => p.Date, model.Date.Value)
                                                                           .SetProperty(p => p.Location, model.Location.Value.ToEntity())
                                                                           .SetProperty(p => p.Judges, model.Judges.Select(j => j.ToEntity()).ToList())
                                                                           .SetProperty(p => p.Teams, model.Teams.Select(t => t.ToEntity()).ToList())
                                                                           .SetProperty(p => p.Points, model.Points.Select(x => x.ToEntity()).ToList()));

        return result > 0 ? Result.Success : Error.NotFound();
    }
}

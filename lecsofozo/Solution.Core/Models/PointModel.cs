﻿namespace Solution.Core.Models;

public class PointModel
{
    public uint Id { get; set; }

    public ValidatableObject<uint> Value { get; set; }

    public ValidatableObject<TeamModel> Team { get; set; }

    public ValidatableObject<RaceModel> Race { get; set; }

    public PointModel()
    {
        Value = new ValidatableObject<uint>();
        Team = new ValidatableObject<TeamModel>();
        Race = new ValidatableObject<RaceModel>();
    }

    public PointModel(PointEntity entity)
    { 
        this.Value.Value = entity.Value;
        this.Team.Value = new TeamModel(entity.Team);
        this.Race.Value = new RaceModel(entity.Race);
    }

    public PointEntity ToEntity()
    {
        return new PointEntity
        {
            Value = this.Value.Value,
            TeamId = this.Team.Value.Id,
            RaceId = this.Race.Value.Id
        };
    } 
    
    public void ToEntity(PointEntity entity)
    {
        entity.Value = this.Value.Value;
        entity.RaceId = this.Race.Value.Id;
        entity.TeamId = this.Team.Value.Id;
    }
}

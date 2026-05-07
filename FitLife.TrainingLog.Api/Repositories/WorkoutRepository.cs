using FitLife.TrainingLog.Api.Models;
using MongoDB.Driver;

namespace FitLife.TrainingLog.Api.Repositories;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly IMongoCollection<WorkoutProgram> _programs;
    private readonly IMongoCollection<WorkoutActivity> _activities;

    public WorkoutRepository(IMongoDatabase database)
    {
        _programs = database.GetCollection<WorkoutProgram>("workoutprograms");
        _activities = database.GetCollection<WorkoutActivity>("workoutactivities");
    }

    public async Task<WorkoutProgram> CreateProgramAsync(WorkoutProgram program)
    {
        await _programs.InsertOneAsync(program);
        return program;
    }

    public async Task<List<WorkoutProgram>> GetProgramsByMemberAsync(Guid memberId)
    {
        return await _programs.Find(p => p.MemberId == memberId).ToListAsync();
    }

    public async Task<WorkoutProgram?> GetProgramByIdAsync(Guid memberId, Guid programId)
    {
        return await _programs
            .Find(p => p.Id == programId && p.MemberId == memberId)
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutProgram> UpdateProgramAsync(WorkoutProgram program)
    {
        await _programs.ReplaceOneAsync(p => p.Id == program.Id, program);
        return program;
    }

    public async Task<WorkoutActivity> LogActivityAsync(WorkoutActivity activity)
    {
        await _activities.InsertOneAsync(activity);
        return activity;
    }

    public async Task<List<WorkoutActivity>> GetActivitiesByMemberAsync(Guid memberId)
    {
        return await _activities.Find(a => a.MemberId == memberId).ToListAsync();
    }
}
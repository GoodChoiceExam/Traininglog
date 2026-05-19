using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;
using FitLife.TrainingLog.Api.Repositories;
using Moq;
using FitLife.TrainingLog.Api.Services;

namespace FitLife.TrainingLog.Tests;

[TestFixture]
public class WorkoutServiceTests
{
    private Mock<IWorkoutRepository> _repoMock;
    private WorkoutService _service;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IWorkoutRepository>();
        _service = new WorkoutService(_repoMock.Object);
    }

    [Test]
    public async Task CreateProgramAsync_ReturnsResponse_WithCorrectName()
    {
        var memberId = Guid.NewGuid();
        var request = new CreateWorkoutProgramRequest("Styrke dag A");

        _repoMock.Setup(r => r.CreateProgramAsync(It.IsAny<WorkoutProgram>()))
                 .ReturnsAsync((WorkoutProgram p) => p);

        var result = await _service.CreateProgramAsync(memberId, request);

        Assert.That(result.Name, Is.EqualTo("Styrke dag A"));
    }

    [Test]
    public async Task CreateProgramAsync_SætterMemberIdFraParameter()
    {
        var memberId = Guid.NewGuid();
        WorkoutProgram? captured = null;

        _repoMock.Setup(r => r.CreateProgramAsync(It.IsAny<WorkoutProgram>()))
                 .Callback<WorkoutProgram>(p => captured = p)
                 .ReturnsAsync((WorkoutProgram p) => p);

        await _service.CreateProgramAsync(memberId, new CreateWorkoutProgramRequest("Test"));

        Assert.That(captured!.MemberId, Is.EqualTo(memberId));
    }

    [Test]
    public async Task GetProgramByIdAsync_ReturnerNull_HvisIkkeEjet()
    {
        var memberId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        _repoMock.Setup(r => r.GetProgramByIdAsync(memberId, programId))
                 .ReturnsAsync((WorkoutProgram?)null);

        var result = await _service.GetProgramByIdAsync(memberId, programId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task LogActivityAsync_SætterMemberIdFraParameter()
    {
        var memberId = Guid.NewGuid();
        var request = new CreateWorkoutActivityRequest(
            "Morgen løbetur",
            ActivityType.Gym,
            45,
            DateTime.UtcNow);

        WorkoutActivity? captured = null;

        _repoMock.Setup(r => r.LogActivityAsync(It.IsAny<WorkoutActivity>()))
                 .Callback<WorkoutActivity>(a => captured = a)
                 .ReturnsAsync((WorkoutActivity a) => a);

        await _service.LogActivityAsync(memberId, request);

        Assert.That(captured!.MemberId, Is.EqualTo(memberId));
    }
}
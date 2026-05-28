using FitLife.TrainingLog.Api.DTOs;
using FitLife.TrainingLog.Api.Models;
using FitLife.TrainingLog.Api.Repositories;
using Moq;
using FitLife.TrainingLog.Api.Services;


namespace FitLife.TrainingLog.Tests;

// Tests for WorkoutService. Repository mockes med Moq så vi tester
// service-logikken isoleret uden MongoDB.
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

    // Verificerer at programmet returneres med det korrekte navn fra requesten
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

    // Verificerer at MemberId fra parameteren sættes korrekt på det oprettede program
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

    // Verificerer at null returneres hvis programmet ikke tilhører det angivne member
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

}
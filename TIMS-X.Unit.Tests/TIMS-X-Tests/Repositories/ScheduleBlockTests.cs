using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ScheduleBlockTests : TestObjectBase<ScheduleBlock>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "New")]
    [InlineData(OperationType.Update, "New")]
    public async void ValidateAlreadyExists_ReturnsError(OperationType operationType, string newName)
    {
        // Arrange
        var scheduleBlocks = new List<ScheduleBlock>
        {
            new() { Name = "New" }
        };
        var scheduleBlockUoW = new Mock<IScheduleBlockUnitOfWork>();
        scheduleBlockUoW
            .Setup(x => x.GetScheduleBlocks(It.IsAny<Expression<Func<ScheduleBlock, bool>>>(), null))
            .Returns(Task.FromResult(scheduleBlocks));
        var validator = new ScheduleBlockValidator(scheduleBlockUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ScheduleBlock { Name = newName }),
            OperationType.Update => await validator.Update(new ScheduleBlock { Name = newName }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Name must be unique", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different")]
    [InlineData(OperationType.Update, "Different")]
    public async void ValidateScheduleBlockIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var scheduleBlockUoW = new Mock<IScheduleBlockUnitOfWork>();
        scheduleBlockUoW
            .Setup(x => x.GetScheduleBlocks(It.IsAny<Expression<Func<ScheduleBlock, bool>>>(), null))
            .Returns(Task.FromResult(new List<ScheduleBlock>()));
        var validator = new ScheduleBlockValidator(scheduleBlockUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ScheduleBlock { Name = newName }),
            OperationType.Update => await validator.Update(new ScheduleBlock { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different")]
    [InlineData(OperationType.Update, "Different")]
    public async void ValidateScheduleBlockStartDateBeforeEndDate_ReturnsError(OperationType operationType,
        string newName)
    {
        // Arrange
        var scheduleBlockUoW = new Mock<IScheduleBlockUnitOfWork>();
        scheduleBlockUoW
            .Setup(x => x.GetScheduleBlocks(It.IsAny<Expression<Func<ScheduleBlock, bool>>>(), null))
            .Returns(Task.FromResult(new List<ScheduleBlock>()));
        var validator = new ScheduleBlockValidator(scheduleBlockUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ScheduleBlock
                { Name = newName, StartDate = DateTime.MaxValue, EndDate = DateTime.Today }),
            OperationType.Update => await validator.Update(new ScheduleBlock
                { Name = newName, StartDate = DateTime.MaxValue, EndDate = DateTime.Today }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("End date must be after start date", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different")]
    [InlineData(OperationType.Update, "Different")]
    public async void ValidateScheduleBlockStartDate_ReturnsError(OperationType operationType, string newName)
    {
        // Arrange
        var scheduleBlockUoW = new Mock<IScheduleBlockUnitOfWork>();
        scheduleBlockUoW
            .Setup(x => x.GetScheduleBlocks(It.IsAny<Expression<Func<ScheduleBlock, bool>>>(), null))
            .Returns(Task.FromResult(new List<ScheduleBlock>()));
        var validator = new ScheduleBlockValidator(scheduleBlockUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ScheduleBlock
                { Name = newName, StartDate = DateTime.MinValue, EndDate = DateTime.Today }),
            OperationType.Update => await validator.Update(new ScheduleBlock
                { Name = newName, StartDate = DateTime.MinValue, EndDate = DateTime.Today }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Schedule Block is being made in the past", validationResult.Message);
        Assert.Equal(Severity.Warning, validationResult.Severity);
    }
}
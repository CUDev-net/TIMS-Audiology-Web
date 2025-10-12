using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HistoryTypeTests : TestObjectBase<HistoryType>
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
        var historyTypes = new List<HistoryType>
        {
            new() { Name = "New" }
        };
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        historyTypeUoW
            .Setup(x => x.GetHistoryTypes(It.IsAny<Expression<Func<HistoryType, bool>>>(), null,
                null)).Returns(Task.FromResult(historyTypes));
        var validator = new HistoryTypeValidator(historyTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HistoryType { Name = newName }),
            OperationType.Update => await validator.Update(new HistoryType { Name = newName }),
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
    public async void ValidateHistoryTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        historyTypeUoW
            .Setup(x => x.GetHistoryTypes(It.IsAny<Expression<Func<HistoryType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HistoryType>()));
        var validator = new HistoryTypeValidator(historyTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HistoryType { Name = newName }),
            OperationType.Update => await validator.Update(new HistoryType { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
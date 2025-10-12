using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaStockItemStatusTests : TestObjectBase<HaStockItemStatus>
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
        var haStockItemStatuses = new List<HaStockItemStatus>
        {
            new() { Name = "New" }
        };
        var haStockItemStatusUoW = new Mock<IHaStockItemStatusUnitOfWork>();
        haStockItemStatusUoW
            .Setup(x => x.GetHaStockItemStatuses(It.IsAny<Expression<Func<HaStockItemStatus, bool>>>(), null,
                null)).Returns(Task.FromResult(haStockItemStatuses));
        var validator = new HaStockItemStatusValidator(haStockItemStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaStockItemStatus { Name = newName }),
            OperationType.Update => await validator.Update(new HaStockItemStatus { Name = newName }),
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
    public async void ValidateHaStockItemStatusIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var haStockItemStatusUoW = new Mock<IHaStockItemStatusUnitOfWork>();
        haStockItemStatusUoW
            .Setup(x => x.GetHaStockItemStatuses(It.IsAny<Expression<Func<HaStockItemStatus, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HaStockItemStatus>()));
        var validator = new HaStockItemStatusValidator(haStockItemStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaStockItemStatus { Name = newName }),
            OperationType.Update => await validator.Update(new HaStockItemStatus { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
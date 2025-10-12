using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class BatterySizeTests : TestObjectBase<BatterySize>
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
        var authorizations = new List<BatterySize>
        {
            new() { Name = "New" }
        };
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        batterySizeUoW
            .Setup(x => x.GetBatterySizes(It.IsAny<Expression<Func<BatterySize, bool>>>(), null,
                null)).Returns(Task.FromResult(authorizations));
        var validator = new BatterySizeValidator(batterySizeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new BatterySize { Name = newName }),
            OperationType.Update => await validator.Update(new BatterySize { Name = newName }),
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
    public async void ValidateBatterySizeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        batterySizeUoW
            .Setup(x => x.GetBatterySizes(It.IsAny<Expression<Func<BatterySize, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<BatterySize>()));
        var validator = new BatterySizeValidator(batterySizeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new BatterySize { Name = newName }),
            OperationType.Update => await validator.Update(new BatterySize { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
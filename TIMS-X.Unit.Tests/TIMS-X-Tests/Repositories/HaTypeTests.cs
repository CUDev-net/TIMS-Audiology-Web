using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaTypeTests : TestObjectBase<HaType>
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
        var haTypes = new List<HaType>
        {
            new() { Name = "New" }
        };
        var haTypeUoW = new Mock<IHaTypeUnitOfWork>();
        haTypeUoW
            .Setup(x => x.GetHaTypes(It.IsAny<Expression<Func<HaType, bool>>>(), null,
                null)).Returns(Task.FromResult(haTypes));
        var validator = new HaTypeValidator(haTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaType { Name = newName }),
            OperationType.Update => await validator.Update(new HaType { Name = newName }),
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
    public async void ValidateHaTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var haTypeUoW = new Mock<IHaTypeUnitOfWork>();
        haTypeUoW
            .Setup(x => x.GetHaTypes(It.IsAny<Expression<Func<HaType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HaType>()));
        var validator = new HaTypeValidator(haTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaType { Name = newName }),
            OperationType.Update => await validator.Update(new HaType { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
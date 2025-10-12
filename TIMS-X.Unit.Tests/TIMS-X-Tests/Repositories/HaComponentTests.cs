using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaComponentTests : TestObjectBase<HaComponent>
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
        var haComponents = new List<HaComponent>
        {
            new() { Name = "New" }
        };
        var haComponentUoW = new Mock<IHaComponentUnitOfWork>();
        haComponentUoW
            .Setup(x => x.GetHaComponents(It.IsAny<Expression<Func<HaComponent, bool>>>(),
                null,
                null)).Returns(Task.FromResult(haComponents));
        var validator = new HaComponentValidator(haComponentUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaComponent { Name = newName }),
            OperationType.Update => await validator.Update(new HaComponent { Name = newName }),
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
    public async void ValidateHaComponentIsUnique_ReturnsNoError(OperationType operationType,
        string newName)
    {
        // Arrange
        var haComponentUoW = new Mock<IHaComponentUnitOfWork>();
        haComponentUoW
            .Setup(x => x.GetHaComponents(It.IsAny<Expression<Func<HaComponent, bool>>>(),
                null,
                null)).Returns(Task.FromResult(new List<HaComponent>()));
        var validator = new HaComponentValidator(haComponentUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaComponent { Name = newName }),
            OperationType.Update => await validator.Update(new HaComponent { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ModifierTests : TestObjectBase<Modifier>
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
        var modifiers = new List<Modifier>
        {
            new() { Name = "New" }
        };
        var modifierUoW = new Mock<IModifierUnitOfWork>();
        modifierUoW
            .Setup(x => x.GetModifiers(It.IsAny<Expression<Func<Modifier, bool>>>(), null,
                null)).Returns(Task.FromResult(modifiers));
        var validator = new ModifierValidator(modifierUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Modifier { Name = newName }),
            OperationType.Update => await validator.Update(new Modifier { Name = newName }),
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
    public async void ValidateModifierIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var modifierUoW = new Mock<IModifierUnitOfWork>();
        modifierUoW
            .Setup(x => x.GetModifiers(It.IsAny<Expression<Func<Modifier, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<Modifier>()));
        var validator = new ModifierValidator(modifierUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Modifier { Name = newName }),
            OperationType.Update => await validator.Update(new Modifier { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
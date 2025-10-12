using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaStyleTests : TestObjectBase<HaStyle>
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
        var haStyles = new List<HaStyle>
        {
            new() { Name = "New" }
        };
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        haStyleUoW
            .Setup(x => x.GetHaStyles(It.IsAny<Expression<Func<HaStyle, bool>>>(), null,
                null)).Returns(Task.FromResult(haStyles));
        var validator = new HaStyleValidator(haStyleUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaStyle { Name = newName }),
            OperationType.Update => await validator.Update(new HaStyle { Name = newName }),
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
    public async void ValidateHaStyleIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        haStyleUoW
            .Setup(x => x.GetHaStyles(It.IsAny<Expression<Func<HaStyle, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HaStyle>()));
        var validator = new HaStyleValidator(haStyleUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaStyle { Name = newName }),
            OperationType.Update => await validator.Update(new HaStyle { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
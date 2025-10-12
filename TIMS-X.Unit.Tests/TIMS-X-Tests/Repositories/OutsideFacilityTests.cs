using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class OutsideFacilityTests : TestObjectBase<OutsideFacility>
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
        var outsideFacilities = new List<OutsideFacility>
        {
            new() { Name = "New" }
        };
        var outsideFacilityUoW = new Mock<IOutsideFacilityUnitOfWork>();
        outsideFacilityUoW
            .Setup(x => x.GetOutsideFacilities(It.IsAny<Expression<Func<OutsideFacility, bool>>>(), null,
                null)).Returns(Task.FromResult(outsideFacilities));
        var validator = new OutsideFacilityValidator(outsideFacilityUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new OutsideFacility { Name = newName }),
            OperationType.Update => await validator.Update(new OutsideFacility { Name = newName }),
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
    public async void ValidateOutsideFacilityIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var outsideFacilityUoW = new Mock<IOutsideFacilityUnitOfWork>();
        outsideFacilityUoW
            .Setup(x => x.GetOutsideFacilities(It.IsAny<Expression<Func<OutsideFacility, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<OutsideFacility>()));
        var validator = new OutsideFacilityValidator(outsideFacilityUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new OutsideFacility { Name = newName }),
            OperationType.Update => await validator.Update(new OutsideFacility { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
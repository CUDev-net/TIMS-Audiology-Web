using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaWarrantyTypeTests : TestObjectBase<HaWarrantyType>
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
        var haWarrantyTypes = new List<HaWarrantyType>
        {
            new() { Name = "New" }
        };
        var haWarrantyTypeUoW = new Mock<IHaWarrantyTypeUnitOfWork>();
        haWarrantyTypeUoW
            .Setup(x => x.GetHaWarrantyTypes(It.IsAny<Expression<Func<HaWarrantyType, bool>>>(), null,
                null)).Returns(Task.FromResult(haWarrantyTypes));
        var validator = new HaWarrantyTypeValidator(haWarrantyTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaWarrantyType { Name = newName }),
            OperationType.Update => await validator.Update(new HaWarrantyType { Name = newName }),
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
    public async void ValidateHaWarrantyTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var haWarrantyTypeUoW = new Mock<IHaWarrantyTypeUnitOfWork>();
        haWarrantyTypeUoW
            .Setup(x => x.GetHaWarrantyTypes(It.IsAny<Expression<Func<HaWarrantyType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HaWarrantyType>()));
        var validator = new HaWarrantyTypeValidator(haWarrantyTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaWarrantyType { Name = newName }),
            OperationType.Update => await validator.Update(new HaWarrantyType { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
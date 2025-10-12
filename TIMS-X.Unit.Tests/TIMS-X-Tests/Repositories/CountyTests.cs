using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class CountyTests : TestObjectBase<County>
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
        var counties = new List<County>
        {
            new() { Name = "New" }
        };
        var countyUoW = new Mock<ICountyUnitOfWork>();
        countyUoW
            .Setup(x => x.GetCounties(It.IsAny<Expression<Func<County, bool>>>(),
                null,
                null)).Returns(Task.FromResult(counties));
        var validator = new CountyValidator(countyUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new County { Name = newName }),
            OperationType.Update => await validator.Update(new County { Name = newName }),
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
    public async void ValidateCountyIsUnique_ReturnsNoError(OperationType operationType,
        string newName)
    {
        // Arrange
        var countyUoW = new Mock<ICountyUnitOfWork>();
        countyUoW
            .Setup(x => x.GetCounties(It.IsAny<Expression<Func<County, bool>>>(),
                null,
                null)).Returns(Task.FromResult(new List<County>()));
        var validator = new CountyValidator(countyUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new County { Name = newName }),
            OperationType.Update => await validator.Update(new County { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
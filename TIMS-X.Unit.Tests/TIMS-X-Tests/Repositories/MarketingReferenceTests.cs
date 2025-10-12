using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class MarketingReferenceTests : TestObjectBase<MarketingReference>
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
        var marketingCategories = new List<MarketingReference>
        {
            new() { Name = "New" }
        };
        var marketingCategoryUoW = new Mock<IMarketingReferenceUnitOfWork>();
        marketingCategoryUoW
            .Setup(x => x.GetMarketingReferences(It.IsAny<Expression<Func<MarketingReference, bool>>>(), null,
                null)).Returns(Task.FromResult(marketingCategories));
        var validator = new MarketingReferenceValidator(marketingCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new MarketingReference { Name = newName }),
            OperationType.Update => await validator.Update(new MarketingReference { Name = newName }),
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
    public async void ValidateMarketingReferenceIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var marketingCategoryUoW = new Mock<IMarketingReferenceUnitOfWork>();
        marketingCategoryUoW
            .Setup(x => x.GetMarketingReferences(It.IsAny<Expression<Func<MarketingReference, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<MarketingReference>()));
        var validator = new MarketingReferenceValidator(marketingCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new MarketingReference { Name = newName }),
            OperationType.Update => await validator.Update(new MarketingReference { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}
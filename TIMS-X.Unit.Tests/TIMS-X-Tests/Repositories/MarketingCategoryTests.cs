using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class MarketingCategoryTests : TestObjectBase<MarketingReferenceCategory>
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
        var marketingReferenceCategories = new List<MarketingReferenceCategory>
        {
            new() { Name = "New" }
        };
        var marketingReferenceCategoryUoW = new Mock<IMarketingCategoryUnitOfWork>();
        marketingReferenceCategoryUoW
            .Setup(x => x.GetMarketingCategories(It.IsAny<Expression<Func<MarketingReferenceCategory, bool>>>(), null,
                null)).Returns(Task.FromResult(marketingReferenceCategories));
        var validator = new MarketingCategoryValidator(marketingReferenceCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new MarketingReferenceCategory { Name = newName }),
            OperationType.Update => await validator.Update(new MarketingReferenceCategory { Name = newName }),
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
    public async void ValidateMarketingCategoryIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var marketingReferenceCategoryUoW = new Mock<IMarketingCategoryUnitOfWork>();
        marketingReferenceCategoryUoW
            .Setup(x => x.GetMarketingCategories(It.IsAny<Expression<Func<MarketingReferenceCategory, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<MarketingReferenceCategory>()));
        var validator = new MarketingCategoryValidator(marketingReferenceCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new MarketingReferenceCategory { Name = newName }),
            OperationType.Update => await validator.Update(new MarketingReferenceCategory { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    } 
}
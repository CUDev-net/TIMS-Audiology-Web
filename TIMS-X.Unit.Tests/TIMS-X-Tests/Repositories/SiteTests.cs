using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class SiteTests : TestObjectBase<Site>
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
        var sites = new List<Site>
        {
            new() { Name = "New" }
        };
        var siteUoW = new Mock<ISiteUnitOfWork>();
        siteUoW
            .Setup(x => x.GetSites(It.IsAny<Expression<Func<Site, bool>>>(), null,
                null)).Returns(Task.FromResult(sites));
        var validator = new SiteValidator(siteUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Site { Name = newName }),
            OperationType.Update => await validator.Update(new Site { Name = newName }),
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
    public async void ValidateSiteIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var siteUoW = new Mock<ISiteUnitOfWork>();
        siteUoW
            .Setup(x => x.GetSites(It.IsAny<Expression<Func<Site, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<Site>()));
        var validator = new SiteValidator(siteUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Site { Name = newName }),
            OperationType.Update => await validator.Update(new Site { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}